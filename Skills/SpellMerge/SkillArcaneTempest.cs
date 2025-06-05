// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SpellMerge.SkillArcaneTempest
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Misc;
using Arcana.Spells;
using System;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Skills.SpellMerge
{
  internal class SkillArcaneTempest : SpellArcaneMergeSkillData
  {
    public string arcaneMergeSpellId;
    public int projectileSetCount;
    public float projectileSetTimeBetween;
    public float projectileTimeBetween;
    public float projectileHomingRadius;
    public float targetRayConeMinAngle = 0.0f;
    public float targetRayConeMaxAngle = 2f;
    public bool doHoming = true;
    public string projectileEffectId;
    public EffectData projectileEffectData;
    public string sprayEffectId;
    public EffectData sprayEffectData;
    private EffectInstance sprayEffectInstance;
    public AnimationCurve sprayForceCurve;
    public float sprayHandPositionSpringMultiplier;
    public float sprayHandPositionDamperMultiplier;
    public float sprayHandRotationSpringMultiplier;
    public float sprayHandRotationDamperMultiplier;
    public float sprayHandLocomotionVelocityCorrectionMultiplier = 1f;
    public float sprayCastMinHandAngle;
    public Ray targetRay;
    private Transform sprayStart;
    private float sprayStartOffset = 0.35f;
    private ArcaneBolt arcaneBolt;
    private float lastProjectileCast;
    private Vector3 guidanceDirection;
    private int rayMask;
    public ProjectileManager projectileManager;

    public bool SprayActive { get; private set; }

    public Vector3 SprayPosition(Mana mana)
    {
      return mana.mergePoint.position + this.targetRay.direction * this.sprayStartOffset;
    }

    public event SkillArcaneTempest.OnSpray OnSprayStartEvent;

    public event SkillArcaneTempest.OnSpray OnSprayEndEvent;

    public event SkillArcaneTempest.OnSpray OnSprayUpdateEvent;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.projectileEffectData = Catalog.GetData<EffectData>(this.projectileEffectId, true);
      this.sprayEffectData = Catalog.GetData<EffectData>(this.sprayEffectId, true);
      this.sprayForceCurve.postWrapMode = WrapMode.Loop;
      this.lastProjectileCast = 0.0f;
    }

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      this.arcaneBolt = Catalog.GetData<ArcaneBolt>("Arcane", true);
      this.projectileManager = new ProjectileManager((SpellData) null, creature.handRight.caster, this.arcaneBolt.projectileData, this.projectileEffectData, this.arcaneBolt.projectileDamagerData, this.arcaneBolt.projectileStatusData, (SpellCastCharge) this.arcaneBolt)
      {
        thresholds = this.arcaneBolt.thresholds,
        damageOverTimeCurve = this.arcaneBolt.damageOverTimeCurve,
        projectileVelocity = this.arcaneBolt.projectileVelocity,
        projectilePlayerGuided = this.arcaneBolt.projectilePlayerGuided,
        projectileGuidanceDelay = this.arcaneBolt.projectileGuidanceDelay,
        projectileCount = this.projectileSetCount,
        projectileTimeBetween = new float?(this.projectileSetTimeBetween),
        projectileConeMinAngle = new float?(this.arcaneBolt.projectileConeMinAngle),
        projectileConeMaxAngle = new float?(this.arcaneBolt.projectileConeMaxAngle),
        guidanceDirection = this.guidanceDirection,
        projectileStatusDuration = new float?(this.arcaneBolt.projectileStatusDuration),
        projectileStatusTransfer = new float?(this.arcaneBolt.projectileStatusTransfer),
        doHoming = this.doHoming,
        projectileHomingRadius = new float?(this.projectileHomingRadius),
        performRayTargeting = true
      };
      this.rayMask = Utilities.GetProjectileRaycastMask();
      Debug.Log((object) "Prepped Arcane Tempest");
    }

    public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnLateSkillsLoaded(skillData, creature);
      ArcaneMerge arcaneMerge;
      if (!creature.TryGetSkill<ArcaneMerge>(this.arcaneMergeSpellId, ref arcaneMerge))
        return;
      this.projectileManager.spell = (SpellData) arcaneMerge;
      if (!(arcaneMerge.defaultSkillData is SkillArcaneTempest))
      {
        arcaneMerge.OnUnloadEvent -= new ArcaneMerge.OnUnload(((SpellArcaneMergeSkillData) this).OnUnload);
        arcaneMerge.OnUnloadEvent += new ArcaneMerge.OnUnload(((SpellArcaneMergeSkillData) this).OnUnload);
        arcaneMerge.OnMergeEvent -= new ArcaneMerge.OnMerge(((SpellArcaneMergeSkillData) this).OnMerge);
        arcaneMerge.OnMergeEvent += new ArcaneMerge.OnMerge(((SpellArcaneMergeSkillData) this).OnMerge);
        arcaneMerge.OnFixedUpdateEvent -= new ArcaneMerge.OnFixedUpdate(((SpellArcaneMergeSkillData) this).OnFixedUpdate);
        arcaneMerge.OnFixedUpdateEvent += new ArcaneMerge.OnFixedUpdate(((SpellArcaneMergeSkillData) this).OnFixedUpdate);
        arcaneMerge.OnUpdateEvent -= new ArcaneMerge.OnUpdate(((SpellArcaneMergeSkillData) this).OnUpdate);
        arcaneMerge.OnUpdateEvent += new ArcaneMerge.OnUpdate(((SpellArcaneMergeSkillData) this).OnUpdate);
      }
      Debug.Log((object) "Loaded Skill - Arcane Tempest");
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      if (!(skillData is ArcaneMerge arcaneMerge))
        return;
      if (!(arcaneMerge.defaultSkillData is SkillArcaneTempest))
      {
        arcaneMerge.OnUnloadEvent -= new ArcaneMerge.OnUnload(((SpellArcaneMergeSkillData) this).OnUnload);
        arcaneMerge.OnMergeEvent -= new ArcaneMerge.OnMerge(((SpellArcaneMergeSkillData) this).OnMerge);
        arcaneMerge.OnFixedUpdateEvent -= new ArcaneMerge.OnFixedUpdate(((SpellArcaneMergeSkillData) this).OnFixedUpdate);
        arcaneMerge.OnUpdateEvent -= new ArcaneMerge.OnUpdate(((SpellArcaneMergeSkillData) this).OnUpdate);
      }
      Debug.Log((object) "Unloaded Skill - Arcane Tempest");
    }

    public override void OnUnload()
    {
      this.sprayEffectInstance?.End(false, -1f);
      this.sprayEffectInstance = (EffectInstance) null;
    }

    public override void OnMerge(ArcaneMerge merge, bool active)
    {
      this.sprayEffectInstance?.End(false, -1f);
      this.sprayEffectInstance = (EffectInstance) null;
      if ((UnityEngine.Object) this.sprayStart == (UnityEngine.Object) null)
        this.sprayStart = new GameObject("SprayTarget").transform;
      this.SprayActive = false;
      if (active)
        return;
      merge.mana.casterLeft.ragdollHand.playerHand.controlHand.StopHapticLoop((object) this);
      merge.mana.casterRight.ragdollHand.playerHand.controlHand.StopHapticLoop((object) this);
    }

    public override void OnFixedUpdate(ArcaneMerge merge, float fixedDeltaTime)
    {
      if (!this.SprayActive)
        return;
      ((RagdollPart) merge.mana.casterLeft.ragdollHand).physicBody.AddForce(-this.targetRay.direction * this.sprayForceCurve.Evaluate(Time.time), (ForceMode) 0);
      ((RagdollPart) merge.mana.casterRight.ragdollHand).physicBody.AddForce(-this.targetRay.direction * this.sprayForceCurve.Evaluate(Time.time), (ForceMode) 0);
    }

    public override void OnUpdate(ArcaneMerge merge, float deltaTime)
    {
      this.targetRay.origin = merge.mana.mergePoint.position;
      this.targetRay.direction = Vector3.Slerp(merge.mana.casterLeft.magicSource.up, merge.mana.casterRight.magicSource.up, 0.5f);
      Vector3 vector3;
      int num;
      if ((double) Vector3.SignedAngle(this.targetRay.direction, merge.mana.casterLeft.magicSource.up, Vector3.Cross(merge.mana.creature.centerEyes.position - this.targetRay.origin, merge.mana.casterLeft.magicSource.position - this.targetRay.origin).normalized) < -(double) this.sprayCastMinHandAngle)
      {
        Vector3 direction = this.targetRay.direction;
        Vector3 up = merge.mana.casterRight.magicSource.up;
        vector3 = Vector3.Cross(merge.mana.casterRight.magicSource.position - this.targetRay.origin, merge.mana.creature.centerEyes.position - this.targetRay.origin);
        Vector3 normalized = vector3.normalized;
        num = (double) Vector3.SignedAngle(direction, up, normalized) > (double) this.sprayCastMinHandAngle ? 1 : 0;
      }
      else
        num = 0;
      if (num != 0)
      {
        if (!this.SprayActive && (double) merge.currentCharge >= (double) merge.minCharge)
        {
          this.SprayActive = true;
          this.sprayEffectInstance = this.sprayEffectData.Spawn(this.sprayStart, true, (ColliderGroup) null, false);
          if (merge.mana.creature.isPlayer)
            this.sprayEffectInstance.SetHaptic((HapticDevice) 3, Catalog.gameData.haptics.telekinesisThrow);
          this.sprayEffectInstance?.Play(0, false, false);
          this.sprayStart.transform.SetPositionAndRotation(this.SprayPosition(merge.mana), Quaternion.LookRotation(this.targetRay.direction));
          SkillArcaneTempest.OnSpray onSprayStartEvent = this.OnSprayStartEvent;
          if (onSprayStartEvent != null)
            onSprayStartEvent(this, merge.mana);
        }
      }
      else if (this.SprayActive)
      {
        this.sprayEffectInstance?.End(false, -1f);
        this.sprayEffectInstance = (EffectInstance) null;
        this.SprayActive = false;
        SkillArcaneTempest.OnSpray onSprayEndEvent = this.OnSprayEndEvent;
        if (onSprayEndEvent != null)
          onSprayEndEvent(this, merge.mana);
      }
      if (!this.SprayActive || (double) merge.currentCharge < (double) merge.minCharge)
        return;
      merge.mana.casterRight.ragdollHand.playerHand.controlHand.HapticLoop((object) this, 1f, 0.01f);
      merge.mana.casterLeft.ragdollHand.playerHand.controlHand.HapticLoop((object) this, 1f, 0.01f);
      this.sprayStart.transform.SetPositionAndRotation(this.SprayPosition(merge.mana), Quaternion.Slerp(this.sprayStart.transform.rotation, Quaternion.LookRotation(this.targetRay.direction), Time.deltaTime * 3f));
      if ((double) Time.time - (double) this.lastProjectileCast >= (double) this.projectileSetTimeBetween)
      {
        // ISSUE: method pointer
        ((MonoBehaviour) merge.mana.creature).StartCoroutine(this.projectileManager.SpawnProjectilesCoroutine(this.sprayStart, this.targetRay.direction * this.arcaneBolt.projectileVelocity, (GuidanceMode) 2, new float?(0.35f), this.GetRaycastTarget(merge.mana.mergePoint, this.targetRay.direction, this.arcaneBolt.targetRaycastDistance, this.rayMask), new ProjectileManager.ProjectileSpawnEvent(this.projectileManager.ProjectileGuidanceHomingCoroutine), overrideCount: new int?(this.projectileSetCount), overrideTimeBetween: new float?(this.projectileTimeBetween), perProjectileTargetFunc: new Func<Vector3>((object) this, __methodptr(\u003COnUpdate\u003Eg__projectileTarget\u007C50_0))));
        this.lastProjectileCast = Time.time;
      }
      vector3 = this.targetRay.direction;
      this.guidanceDirection = vector3.normalized;
      SkillArcaneTempest.OnSpray sprayUpdateEvent = this.OnSprayUpdateEvent;
      if (sprayUpdateEvent == null)
        return;
      sprayUpdateEvent(this, merge.mana);
    }

    private Vector3? GetRaycastTarget(
      Transform start,
      Vector3 direction,
      float distance,
      int mask)
    {
      Vector3? raycastTarget = new Vector3?();
      RaycastHit raycastHit;
      if (Physics.Raycast(start.position, direction.normalized, ref raycastHit, distance, mask))
        raycastTarget = new Vector3?(((RaycastHit) ref raycastHit).point);
      return raycastTarget;
    }

    public delegate void OnSpray(SkillArcaneTempest skill, Mana mana);
  }
}
