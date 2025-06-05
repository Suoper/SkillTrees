// Decompiled with JetBrains decompiler
// Type: Arcana.Misc.BeamManager
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using System;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Misc
{
  public class BeamManager
  {
    public static List<BeamManager> all;
    public Mana mana;
    public BeamManager.BeamData data;
    public bool allowTargeting = true;
    public bool doRotationSlerp = true;
    public Creature target = (Creature) null;
    private EffectInstance beamEffect;
    private EffectInstance beamImpactEffect;
    private EffectInstance beamOriginEffect;
    private Transform beamHitPoint;
    public bool beamActive;
    protected float lastDamageTick = float.MinValue;
    public Ray beamRay;
    public bool isPrimary;
    public Vector3? beamOrigin = new Vector3?();
    public BeamManager.BeamOrigin beamOriginType;
    public Vector3? castOriginOverride;
    public Vector3? castDirectionOverride;
    public bool overrideBeamControl = false;
    public float? overrideBeamRotation = new float?();
    private double chargeRequirement = 0.800000011920929;

    public Transform BeamStart { get; private set; }

    public event BeamManager.OnBeam OnBeamStartEvent;

    public event BeamManager.OnBeam OnBeamUpdateEvent;

    public event BeamManager.OnBeam OnBeamEndEvent;

    public BeamManager(
      Mana mana,
      BeamManager.BeamData data,
      BeamManager.BeamOrigin beamOriginType,
      bool isPrimary)
    {
      this.mana = mana;
      this.data = data;
      this.beamOriginType = beamOriginType;
      this.isPrimary = isPrimary;
      data.LoadCatalogData();
      if (BeamManager.all == null)
        BeamManager.all = new List<BeamManager>();
      BeamManager.all.Add(this);
    }

    public void Activate()
    {
      this.beamEffect?.End(false, -1f);
      this.beamEffect = (EffectInstance) null;
      this.beamOriginEffect?.End(false, -1f);
      this.beamOriginEffect = (EffectInstance) null;
      if ((UnityEngine.Object) this.BeamStart == (UnityEngine.Object) null)
        this.BeamStart = new GameObject("Beam Target").transform;
      if ((UnityEngine.Object) this.beamHitPoint == (UnityEngine.Object) null)
        this.beamHitPoint = new GameObject("Beam Hit").transform;
      BeamManager.all.Add(this);
    }

    public void Deactivate()
    {
      this.beamEffect?.End(false, -1f);
      this.beamEffect = (EffectInstance) null;
      this.beamOriginEffect?.End(false, -1f);
      this.beamOriginEffect = (EffectInstance) null;
      if (this.beamImpactEffect == null)
        return;
      this.beamImpactEffect?.End(false, -1f);
      this.beamImpactEffect = (EffectInstance) null;
      BeamManager.all.Remove(this);
    }

    public void UpdatePlayerModifications(bool casting, float currentCharge)
    {
      if (!this.isPrimary || (UnityEngine.Object) this.mana == (UnityEngine.Object) null)
        return;
      if (casting)
      {
        if (this.beamActive || (double) currentCharge < this.chargeRequirement)
          return;
        this.beamActive = true;
        this.mana.creature.locomotion.SetAllSpeedModifiers((object) this, this.data.movementSpeedMult);
        if (this.mana.creature.isPlayer)
          this.beamEffect.SetHaptic((HapticDevice) 3, Catalog.gameData.haptics.telekinesisThrow);
        this.mana.casterLeft.ragdollHand.playerHand.link.SetJointModifier((object) this, this.data.beamHandPositionSpringMultiplier, this.data.beamHandPositionDamperMultiplier, this.data.beamHandRotationSpringMultiplier, this.data.beamHandRotationDamperMultiplier, this.data.beamHandLocomotionVelocityCorrectionMultiplier);
        this.mana.casterRight.ragdollHand.playerHand.link.SetJointModifier((object) this, this.data.beamHandPositionSpringMultiplier, this.data.beamHandPositionDamperMultiplier, this.data.beamHandRotationSpringMultiplier, this.data.beamHandRotationDamperMultiplier, this.data.beamHandLocomotionVelocityCorrectionMultiplier);
        this.mana.casterLeft.ragdollHand.playerHand.controlHand.HapticLoop((object) this, 1f, 0.01f);
        this.mana.casterRight.ragdollHand.playerHand.controlHand.HapticLoop((object) this, 1f, 0.01f);
      }
      else
      {
        if (!this.beamActive)
          return;
        this.mana.casterLeft.ragdollHand.playerHand.link.RemoveJointModifier((object) this);
        this.mana.casterRight.ragdollHand.playerHand.link.RemoveJointModifier((object) this);
        this.mana.creature.locomotion.RemoveSpeedModifier((object) this);
      }
    }

    public void StartBeamOriginEffect()
    {
      this.beamOriginEffect = this.data.beamOriginEffectData?.Spawn(this.BeamStart, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
      this.beamOriginEffect?.Play(0, false, false);
    }

    public void UpdateBeam(bool casting, float currentCharge)
    {
      this.beamRay.origin = this.GetBeamOrigin();
      if (!this.overrideBeamControl)
        this.beamRay.direction = this.GetBeamDirection();
      if (casting)
      {
        if (!this.beamActive && (double) currentCharge >= this.chargeRequirement)
        {
          this.beamActive = true;
          this.beamEffect = this.data.beamEffectData.Spawn(this.BeamStart, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
          this.beamEffect?.Play(0, false, false);
          if (this.beamOriginEffect == null)
            this.StartBeamOriginEffect();
          if (this.beamEffect != null)
          {
            foreach (EffectParticle effectParticle in this.beamEffect.effects.OfType<EffectParticle>())
            {
              ParticleSystem.CollisionModule collision1 = effectParticle.rootParticleSystem.collision;
              ((ParticleSystem.CollisionModule) ref collision1).collidesWith = this.data.beamMask;
              foreach (EffectParticleChild child in effectParticle.childs)
              {
                ParticleSystem.CollisionModule collision2 = child.particleSystem.collision;
                ((ParticleSystem.CollisionModule) ref collision2).collidesWith = this.data.beamMask;
              }
            }
          }
          this.BeamStart.transform.SetPositionAndRotation(this.GetBeamOrigin(), Quaternion.LookRotation(this.beamRay.direction));
          BeamManager.OnBeam onBeamStartEvent = this.OnBeamStartEvent;
          if (onBeamStartEvent != null)
            onBeamStartEvent(this);
        }
      }
      else if (this.beamActive)
      {
        this.beamEffect?.End(false, -1f);
        this.beamEffect = (EffectInstance) null;
        this.beamOriginEffect?.End(false, -1f);
        this.beamOriginEffect = (EffectInstance) null;
        this.beamActive = false;
        if (this.beamImpactEffect != null)
        {
          this.beamImpactEffect?.End(false, -1f);
          this.beamImpactEffect = (EffectInstance) null;
        }
        BeamManager.OnBeam onBeamEndEvent = this.OnBeamEndEvent;
        if (onBeamEndEvent != null)
          onBeamEndEvent(this);
      }
      if ((UnityEngine.Object) this.BeamStart != (UnityEngine.Object) null)
      {
        float valueOrDefault = this.overrideBeamRotation.GetValueOrDefault(3f);
        this.BeamStart.transform.SetPositionAndRotation(this.GetBeamOrigin(), Quaternion.Slerp(this.BeamStart.transform.rotation, Quaternion.LookRotation(this.beamRay.direction), Time.deltaTime * valueOrDefault));
        this.beamRay.direction = this.BeamStart.transform.forward;
      }
      if (!this.beamActive)
        return;
      this.DetectBeamCollision();
      BeamManager.OnBeam onBeamUpdateEvent = this.OnBeamUpdateEvent;
      if (onBeamUpdateEvent == null)
        return;
      onBeamUpdateEvent(this);
    }

    public Vector3 GetBeamOrigin()
    {
      return this.beamOrigin ?? this.castOriginOverride ?? this.mana.mergePoint.position;
    }

    public Vector3 GetBeamDirection()
    {
      Vector3? nullable = this.castOriginOverride;
      Vector3 vector3 = nullable ?? this.mana.mergePoint.position;
      nullable = this.castDirectionOverride;
      Vector3 beamDirection = nullable ?? Vector3.Slerp(this.mana.casterLeft.magicSource.up, this.mana.casterRight.magicSource.up, 0.5f);
      if ((UnityEngine.Object) this.target != (UnityEngine.Object) null && this.allowTargeting)
        return Utilities.GetRandomVelocityInCone(((ThunderBehaviour) this.target.ragdoll.targetPart).transform.position - this.GetBeamOrigin(), 5f, 1f, 1f);
      switch (this.beamOriginType)
      {
        case BeamManager.BeamOrigin.FloatingTrueConverge:
          RaycastHit raycastHit1;
          if (Physics.Raycast(vector3, beamDirection.normalized, ref raycastHit1, this.data.beamDistance, (int) this.data.beamMask))
            return ((RaycastHit) ref raycastHit1).point - this.GetBeamOrigin();
          break;
        case BeamManager.BeamOrigin.FloatingLooseConverge:
          RaycastHit raycastHit2;
          if (Physics.Raycast(vector3, beamDirection.normalized, ref raycastHit2, this.data.beamDistance, (int) this.data.beamMask))
            return Utilities.GetRandomVelocityInCone(((RaycastHit) ref raycastHit2).point - this.GetBeamOrigin(), 5f, 1f);
          break;
      }
      return beamDirection;
    }

    public void DetectBeamCollision()
    {
      RaycastHit raycastHit;
      if (!Physics.SphereCast(this.beamRay, 0.1f, ref raycastHit, this.data.beamDistance, (int) this.data.beamMask, (QueryTriggerInteraction) 1))
      {
        this.beamHitPoint.SetPositionAndRotation(this.beamRay.GetPoint(this.data.beamDistance), Quaternion.LookRotation(-this.beamRay.direction));
        if (this.beamImpactEffect == null)
          return;
        this.beamImpactEffect.End(false, -1f);
        this.beamImpactEffect = (EffectInstance) null;
      }
      else
      {
        this.beamHitPoint.SetPositionAndRotation(((RaycastHit) ref raycastHit).point + ((RaycastHit) ref raycastHit).normal.normalized * 0.05f, (double) Vector3.Angle(-this.beamRay.direction, ((RaycastHit) ref raycastHit).normal) < 10.0 ? Quaternion.LookRotation(((RaycastHit) ref raycastHit).normal) : Quaternion.LookRotation(-this.beamRay.direction));
        if (this.data.beamImpactEffectData != null && this.beamImpactEffect == null)
        {
          this.beamImpactEffect = this.data.beamImpactEffectData.Spawn(this.beamHitPoint, true, (ColliderGroup) null, false);
          this.beamImpactEffect.Play(0, false, false);
        }
        if ((UnityEngine.Object) ((RaycastHit) ref raycastHit).rigidbody == (UnityEngine.Object) null)
          return;
        ((Component) ((RaycastHit) ref raycastHit).rigidbody)?.GetComponentInParent<SimpleBreakable>()?.Break();
        CollisionHandler component = ((Component) ((RaycastHit) ref raycastHit).rigidbody)?.GetComponent<CollisionHandler>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
          return;
        if (component.isBreakable && !component.breakable.contactBreakOnly)
        {
          component.breakable.Break();
          foreach (Item subBrokenItem in component.breakable.subBrokenItems)
          {
            PhysicBody physicBody = subBrokenItem.physicBody;
            if (PhysicBody.op_Implicit(physicBody))
              physicBody.AddForceAtPosition(this.beamRay.direction * this.data.beamForce, ((RaycastHit) ref raycastHit).point, (ForceMode) 2);
          }
          foreach (PhysicBody subBrokenBody in component.breakable.subBrokenBodies)
          {
            if (PhysicBody.op_Implicit(subBrokenBody))
              subBrokenBody.AddForceAtPosition(this.beamRay.direction * this.data.beamForce, ((RaycastHit) ref raycastHit).point, (ForceMode) 2);
          }
        }
        component.physicBody.AddForceAtPosition(this.beamRay.direction * this.data.beamForce, ((RaycastHit) ref raycastHit).point, (ForceMode) 2);
        if (component.isItem)
        {
          ColliderGroup componentInParent = ((Component) ((RaycastHit) ref raycastHit).collider).GetComponentInParent<ColliderGroup>();
          if ((UnityEngine.Object) componentInParent == (UnityEngine.Object) null || !(bool) (UnityEngine.Object) componentInParent.imbue)
            return;
          SpellStatusImbueable spellStatusImbueable = this.data.spellStatuses.Find((Predicate<SpellStatusImbueable>) (status => status.isImbueStatus));
          if (spellStatusImbueable != null)
            componentInParent.imbue.Transfer(spellStatusImbueable.imbueSpell, this.data.imbueAmount * Time.deltaTime, this.mana?.creature);
        }
        else
        {
          RagdollPart ragdollPart = component.ragdollPart;
          if ((UnityEngine.Object) ragdollPart == (UnityEngine.Object) null || (UnityEngine.Object) ragdollPart.ragdoll.creature == (UnityEngine.Object) this.mana?.creature || ragdollPart.isSliced)
            return;
          Creature creature1 = ragdollPart.ragdoll.creature;
          if ((double) Time.time - (double) this.lastDamageTick > (double) this.data.damageDelay)
          {
            this.lastDamageTick = Time.time;
            Creature creature2 = creature1;
            DamageStruct damageStruct;
            // ISSUE: explicit constructor call
            ((DamageStruct) ref damageStruct).\u002Ector((DamageType) 4, this.data.damageAmount);
            damageStruct.pushLevel = 2;
            creature2.Damage(new CollisionInstance(damageStruct, (MaterialData) null, (MaterialData) null)
            {
              casterHand = this.mana?.casterRight,
              contactPoint = ((RaycastHit) ref raycastHit).point,
              contactNormal = ((RaycastHit) ref raycastHit).normal,
              targetColliderGroup = ((Component) ((RaycastHit) ref raycastHit).collider).GetComponentInParent<ColliderGroup>()
            });
            foreach (SpellStatusImbueable spellStatuse in this.data.spellStatuses)
            {
              if (spellStatuse.statusParameter.HasValue)
                ((ThunderEntity) creature1)?.Inflict(spellStatuse.statusData, (object) this, spellStatuse.statusDuration, (object) spellStatuse.statusParameter.Value, true);
              else
                ((ThunderEntity) creature1)?.Inflict(spellStatuse.statusData, (object) this, spellStatuse.statusDuration, (object) null, true);
            }
          }
        }
      }
    }

    public delegate void OnBeam(BeamManager beamManager);

    public enum BeamOrigin
    {
      MergePoint,
      FloatingTrueConverge,
      FloatingLooseConverge,
    }

    public class BeamData : ThunderBehaviour
    {
      public string beamEffectId;
      public string beamImpactEffectId;
      public string beamOriginEffectId;
      public LayerMask beamMask = (LayerMask) 144849921;
      public float movementSpeedMult = 0.8f;
      public float beamHandPositionSpringMultiplier = 1f;
      public float beamHandPositionDamperMultiplier = 1f;
      public float beamHandRotationSpringMultiplier = 0.2f;
      public float beamHandRotationDamperMultiplier = 0.6f;
      public float beamHandLocomotionVelocityCorrectionMultiplier = 1f;
      public float beamDistance = 20f;
      public float beamForce = 3f;
      public float imbueAmount = 10f;
      public float damageDelay = 0.5f;
      public float damageAmount = 10f;
      public List<SpellStatusImbueable> spellStatuses;
      public EffectData beamEffectData;
      public EffectData beamImpactEffectData;
      public EffectData beamOriginEffectData;
      public bool isLoaded;

      public void LoadCatalogData()
      {
        if (this.isLoaded)
          return;
        this.beamEffectData = Catalog.GetData<EffectData>(this.beamEffectId, true);
        this.beamImpactEffectData = Catalog.GetData<EffectData>(this.beamImpactEffectId, true);
        this.beamOriginEffectData = Catalog.GetData<EffectData>(this.beamOriginEffectId, true);
        foreach (SpellStatus spellStatuse in this.spellStatuses)
          spellStatuse.LoadCatalogData();
        this.isLoaded = true;
      }
    }
  }
}
