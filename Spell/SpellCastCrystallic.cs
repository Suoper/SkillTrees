// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.Spell.SpellCastCrystallic
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill.Spell
{
  public class SpellCastCrystallic : SpellCastCharge
  {
    [ModOption("Imbue Hit Velocity Requirement", "Controls how hard you need to hit a surface for the imbue to work")]
    [ModOptionCategory("Spell", 2)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 100f, 0.5f)]
    public static float imbueHitVelocity = 7.5f;
    [ModOption("Slam Upwards Force Multiplier", "When you slam the staff into a surface, creatures are launched up by a value of (0, 1, 0), world up. the value gets multiplied by this.")]
    [ModOptionCategory("Spell", 2)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 100f, 0.5f)]
    public static float slamUpwardsForceMult = 2.5f;
    [ModOption("Shoot Shardshot", "Controls whether throwing the spell will shoot a shardshot or not, this is for those of you who do not like the spray shot.")]
    [ModOptionCategory("Spell", 2)]
    public static bool shootShardshot = true;
    [ModOption("Shoot Stingers", "Controls whether throwing the spell will shoot a stinger or not, for those of you that dislike the stinger, but want the T4 skills.")]
    [ModOptionCategory("Spell", 2)]
    public static bool shootStinger = true;
    public bool canDrain = true;
    public string hitTransferEffectId;
    public string imbueCollisionEffectId;
    public string fingerEffectPresetId;
    public string pulseEffectId;
    public string staffSlamEffectId;
    public float defaultShardshotAngle = 25f;
    public float cooldown = 0.1f;
    public float intensityPerSkill;
    public float lastTime = 1f;
    public float hitDamage = 1.5f;
    public float particleAngle = 25f;
    public float staffSlamMaxForce = 100f;
    public float staffSlamMaxRadius = 5f;
    public float staffSlamMinForce = 60f;
    public float forceMultiplier = 1f;
    public Vector3 lastVelocity;
    public AnimationCurve forceCurve = new AnimationCurve(new Keyframe[2]
    {
      new Keyframe(0.0f, 0.05f),
      new Keyframe(1f, 10f)
    });
    public AnimationCurve pulseCurve = new AnimationCurve(new Keyframe[3]
    {
      new Keyframe(0.0f, 10f),
      new Keyframe(0.05f, 25f),
      new Keyframe(0.1f, 10f)
    });
    public AnimationCurve imbueHitCurve = new AnimationCurve(new Keyframe[3]
    {
      new Keyframe(0.0f, 0.05f),
      new Keyframe(0.05f, 5f),
      new Keyframe(0.1f, 0.05f)
    });
    private readonly List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    public List<EffectInstance> fingerEffects = new List<EffectInstance>();
    public EffectInstance shardshotEffectInstance;
    public EffectInstance transferEffectInstance;
    public EffectData staffSlamEffectData;
    public EffectData hitTransferEffectData;
    public EffectData pulseEffectData;
    public EffectData imbueCollisionEffectData;
    public ForceFieldPresetData fingerEffectPresetData;
    public Gradient defaultMainGradient;
    public Color currentColor = Color.white;
    public string spellId = "Crystallic";
    public bool speedUpByTimeScale;

    public event SpellCastCrystallic.SprayEvent onSprayStart;

    public event SpellCastCrystallic.SprayEvent onSprayLoop;

    public event SpellCastCrystallic.SprayEvent onSprayEnd;

    public event SpellCastCrystallic.ShardshotEvent OnShardshotStart;

    public event SpellCastCrystallic.ShardshotEvent OnShardshotEnd;

    public event SpellCastCrystallic.ShardshotHitEvent OnShardHit;

    public SpellCastCrystallic Clone() => ((object) this).MemberwiseClone() as SpellCastCrystallic;

    public virtual void OnCatalogRefresh()
    {
      base.OnCatalogRefresh();
      this.pulseEffectData = Catalog.GetData<EffectData>(this.pulseEffectId, true);
      this.imbueCollisionEffectData = Catalog.GetData<EffectData>(this.imbueCollisionEffectId, true);
      this.fingerEffectPresetData = Catalog.GetData<ForceFieldPresetData>(this.fingerEffectPresetId, true);
      this.hitTransferEffectData = Catalog.GetData<EffectData>(this.hitTransferEffectId, true);
      this.staffSlamEffectData = Catalog.GetData<EffectData>(this.staffSlamEffectId, true);
      new Harmony("com.silk.crystallic").PatchAll();
    }

    public virtual void UpdateGripCast(HandleRagdoll handle)
    {
      base.UpdateGripCast(handle);
      if (!this.isGripCasting || this.gripCastEffectInstance == null || !((UnityEngine.Object) handle != (UnityEngine.Object) null) || !((UnityEngine.Object) handle?.ragdollPart?.ragdoll?.creature != (UnityEngine.Object) null))
        return;
      BrainModuleCrystal module = handle.ragdollPart.ragdoll.creature.brain.instance.GetModule<BrainModuleCrystal>(true);
      module.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, "Body"), "Body");
    }

    public virtual void LoadSkillPassives(int skillCount)
    {
      base.LoadSkillPassives(skillCount);
      ((SpellData) this).AddModifier((object) this, (Modifier) 2, (float) (1.0 + (double) this.intensityPerSkill * (double) skillCount));
    }

    public virtual void Load(Imbue imbue)
    {
      base.Load(imbue);
      // ISSUE: method pointer
      this.spellCaster.mana.OnImbueUnloadEvent += new Mana.ImbueLoadEvent((object) this, __methodptr(Unload));
    }

    private void Unload(SpellCastCharge spellData, Imbue unloadedImbue)
    {
      if ((UnityEngine.Object) this.imbue == (UnityEngine.Object) unloadedImbue)
        this.imbueEffect.ForceStop((ParticleSystemStopBehavior) 0);
      // ISSUE: method pointer
      this.spellCaster.mana.OnImbueUnloadEvent -= new Mana.ImbueLoadEvent((object) this, __methodptr(Unload));
    }

    public void TryDrainCharge(float drainDurationMult)
    {
      if (!this.canDrain)
        return;
      this.currentCharge -= 1f * Time.deltaTime * drainDurationMult;
    }

    public void SetShardshotAngle(float angle) => this.particleAngle = angle;

    public void SetForceMultiplier(float multiplier)
    {
      this.forceMultiplier = Mathf.Clamp(this.forceCurve.Evaluate(multiplier / 0.1f), 0.0f, 1f);
    }

    public void ResetForceMultiplier() => this.forceMultiplier = 1f;

    public void SetColor(Color color, string spellId, float time = 0.5f)
    {
      List<EffectInstance> effectInstanceList = new List<EffectInstance>();
      effectInstanceList.Add(this.chargeEffectInstance);
      effectInstanceList.AddRange((IEnumerable<EffectInstance>) this.fingerEffects);
      effectInstanceList.Add(SkillHyperintensity.overchargeLeftLoopEffect);
      effectInstanceList.Add(SkillHyperintensity.overchargeRightLoopEffect);
      effectInstanceList.Add(SkillCrystallicQuasar.beamLeftEffectInstance);
      effectInstanceList.Add(SkillCrystallicQuasar.beamRightEffectInstance);
      effectInstanceList.Add(SkillCrystallicQuasar.beamLeftImpactEffectInstance);
      effectInstanceList.Add(SkillCrystallicQuasar.beamRightImpactEffectInstance);
      this.currentColor = color;
      this.spellId = spellId;
      for (int index = 0; index < effectInstanceList.Count; ++index)
        effectInstanceList[index].SetColorImmediate(this.currentColor);
    }

    public virtual void Fire(bool active)
    {
      base.Fire(active);
      SkillHyperintensity.ToggleDrain(this.spellCaster.side, true);
      this.SetColor(Dye.GetEvaluatedColor(this.spellId, this.spellId), this.spellId, 0.01f);
      this.allowCharge = true;
      this.allowSpray = false;
      if (!active)
      {
        this.DisableFingerEffects();
        this.spellCaster.AllowSpellWheel((object) this);
      }
      else
      {
        this.spellCaster.DisableSpellWheel((object) this);
        this.EnableFingerEffects();
        EventManager.InvokeSpellUsed("Crystallic", this.spellCaster.ragdollHand.creature, this.spellCaster.side);
      }
    }

    public virtual void Throw(Vector3 velocity)
    {
      base.Throw(velocity);
      this.DisableFingerEffects();
      this.spellCaster.ragdollHand.PlayHapticClipOver(this.pulseCurve, 1f);
      if (!SpellCastCrystallic.shootShardshot)
        return;
      this.Shoot(velocity);
    }

    public void Shoot(Vector3 velocity)
    {
      this.lastVelocity = velocity;
      EffectInstance effectInstance = this.pulseEffectData?.Spawn(this.spellCaster.magicSource.position + velocity.normalized * 0.1f, Quaternion.LookRotation(velocity), (Transform) null, (CollisionInstance) null, true, (ColliderGroup) null, this.spellCaster.mana.creature.isPlayer, 1f, 1f, Array.Empty<Type>());
      if (effectInstance == null)
        return;
      this.shardshotEffectInstance = effectInstance;
      effectInstance.SetColorImmediate(this.currentColor);
      if (this.spellCaster.mana.creature.isPlayer)
        effectInstance.SetHaptic(this.spellCaster.side, Catalog.gameData.haptics.telekinesisThrow);
      if (SpellCastCrystallic.shootShardshot)
      {
        SpellCastCrystallic.ShardshotEvent onShardshotStart = this.OnShardshotStart;
        if (onShardshotStart != null)
          onShardshotStart(this, effectInstance);
        effectInstance.SetConeAngle(this.particleAngle, "Beam");
        effectInstance?.Play(0, false, false);
        if (this.speedUpByTimeScale && SkillSlowTimeData.timeSlowed)
          effectInstance.SetSpeed(8f, "Beam");
        float speed = 1f * ((SpellData) this).GetModifier((Modifier) 2) * this.forceMultiplier;
        if (this.spellCaster.mana.creature.isPlayer)
          Player.local.AddForce(-this.spellCaster.magicSource.transform.forward, speed);
        // ISSUE: method pointer
        effectInstance.OnParticleCollisionEvent += new EffectInstance.ParticleCollisionEvent((object) this, __methodptr(ShardHit));
        // ISSUE: method pointer
        effectInstance.onEffectFinished += new EffectInstance.EffectFinishEvent((object) this, __methodptr(\u003CShoot\u003Eg__OnEffectFinished\u007C71_0));
      }
    }

    public void ShardHit(GameObject other)
    {
      EffectInstance shardshotEffectInstance = this.shardshotEffectInstance;
      int? nullable1;
      if (shardshotEffectInstance == null)
      {
        nullable1 = new int?();
      }
      else
      {
        ParticleSystem particleSystem = shardshotEffectInstance.GetParticleSystem("Beam");
        nullable1 = particleSystem != null ? new int?(ParticlePhysicsExtensions.GetCollisionEvents(particleSystem, other, this.collisionEvents)) : new int?();
      }
      int? nullable2 = nullable1;
      int num1 = 0;
      ThunderEntity component;
      if (!(nullable2.GetValueOrDefault() > num1 & nullable2.HasValue) || !other.TryGetComponentInParent<ThunderEntity>(out component))
        return;
      ParticleCollisionEvent collisionEvent = this.collisionEvents[0];
      RagdollPart ragdollPart;
      RagdollPart hitPart = !(component is Creature creature1) || !creature1.ragdoll.GetClosestPart(((ParticleCollisionEvent) ref collisionEvent).intersection, 0.15f, out ragdollPart) ? (RagdollPart) null : ragdollPart;
      SpellCastCrystallic.ShardshotHit hitInfo = new SpellCastCrystallic.ShardshotHit(this.shardshotEffectInstance, ((ParticleCollisionEvent) ref collisionEvent).colliderComponent is Collider ? ((ParticleCollisionEvent) ref collisionEvent).colliderComponent as Collider : (Collider) null, ((ParticleCollisionEvent) ref collisionEvent).intersection, ((ParticleCollisionEvent) ref collisionEvent).normal, ((ParticleCollisionEvent) ref collisionEvent).velocity, collisionEvent, other, component, hitPart, this.hitDamage, (UnityEngine.Object) hitPart != (UnityEngine.Object) null && hitPart.hasMetalArmor);
      SpellCastCrystallic.ShardshotHitEvent onShardHit = this.OnShardHit;
      if (onShardHit != null)
        onShardHit(this, component, hitInfo);
      switch (component)
      {
        case Creature creature2:
          if ((UnityEngine.Object) creature2 != (UnityEngine.Object) this.spellCaster.mana.creature && creature2.factionId != this.spellCaster.mana.creature.factionId)
          {
            if (!creature2.isPlayer)
              creature2.TryPush((Creature.PushType) 2, ((ThunderBehaviour) creature2.ragdoll.targetPart).transform.position - this.spellCaster.magicSource.transform.position, 1, (RagdollPart.Type) 0);
            creature2.DamagePatched(this.hitDamage, (DamageType) 4);
            this.spellCaster.ragdollHand.HapticTick(10f, false);
            if (!this.spellCaster.mana.creature.isPlayer)
            {
              BrainModuleCrystal module = creature2.brain.instance.GetModule<BrainModuleCrystal>(true);
              module.Crystallise(5f, "Crystallic");
              module.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, "Crystallic"), "Crystallic");
              break;
            }
            break;
          }
          break;
        case Item obj:
          Vector3 vector3_1 = ((ThunderBehaviour) obj).transform.position - hitInfo.hitPoint;
          float magnitude = vector3_1.magnitude;
          if ((double) magnitude > 0.0)
          {
            vector3_1.Normalize();
            float num2 = Mathf.Lerp(this.staffSlamMinForce, this.staffSlamMaxForce, 1f - Mathf.Clamp01(magnitude / this.staffSlamMaxRadius));
            ((ThunderEntity) obj).AddForce(vector3_1 * num2, (ForceMode) 1, (CollisionHandler) null);
            foreach (ColliderGroup colliderGroup in obj.colliderGroups)
            {
              if ((UnityEngine.Object) colliderGroup != (UnityEngine.Object) null && colliderGroup.allowImbueEffect && (UnityEngine.Object) colliderGroup.imbue != (UnityEngine.Object) null && colliderGroup != null)
                colliderGroup.imbue?.Transfer((SpellCastCharge) this, num2, this.spellCaster.mana.creature);
            }
          }
          Breakable breakable = obj.breakable;
          if (breakable != null)
          {
            breakable.Explode(30f, hitInfo.hitPoint, this.staffSlamMaxRadius, 0.25f, (ForceMode) 1);
            for (int index = 0; index < breakable.subBrokenItems.Count; ++index)
            {
              PhysicBody physicBody = breakable.subBrokenItems[index].physicBody;
              if (PhysicBody.op_Implicit(physicBody))
              {
                Vector3 vector3_2 = physicBody.transform.position - hitInfo.hitPoint;
                vector3_2.Normalize();
                physicBody.AddForceAtPosition(vector3_2 * 10f * ((SpellData) this).GetModifier((Modifier) 2), physicBody.transform.position, (ForceMode) 1);
              }
            }
            for (int index = 0; index < breakable.subBrokenBodies.Count; ++index)
            {
              PhysicBody subBrokenBody = breakable.subBrokenBodies[index];
              if (PhysicBody.op_Implicit(subBrokenBody))
              {
                Vector3 vector3_3 = subBrokenBody.transform.position - hitInfo.hitPoint;
                vector3_3.Normalize();
                subBrokenBody.AddForceAtPosition(vector3_3 * 10f * ((SpellData) this).GetModifier((Modifier) 2), subBrokenBody.transform.position, (ForceMode) 1);
              }
            }
            break;
          }
          break;
      }
    }

    public virtual bool OnImbueCollisionStart(CollisionInstance collisionInstance)
    {
      if ((double) Time.time - (double) this.lastTime > (double) this.cooldown && (double) collisionInstance.impactVelocity.magnitude > (double) SpellCastCrystallic.imbueHitVelocity)
      {
        this.lastTime = Time.time;
        ThunderEntity entity1 = collisionInstance?.targetColliderGroup?.collisionHandler?.Entity;
        Item entity2 = collisionInstance?.sourceColliderGroup?.collisionHandler?.Entity as Item;
        Item entity3 = collisionInstance.targetColliderGroup?.collisionHandler?.Entity as Item;
        if ((bool) (UnityEngine.Object) entity3 && (bool) (UnityEngine.Object) entity2)
        {
          this.hitTransferEffectData?.Spawn((entity2.GetLocalBounds().center + entity3.GetLocalBounds().center) / 2f, Quaternion.identity, (Transform) null, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>()).Play(0, false, false);
          foreach (ColliderGroup colliderGroup in entity2.colliderGroups)
          {
            if ((UnityEngine.Object) colliderGroup != (UnityEngine.Object) null && colliderGroup.allowImbueEffect && colliderGroup != null)
              colliderGroup.imbue?.Transfer((SpellCastCharge) this, collisionInstance.impactVelocity.magnitude, this.spellCaster.mana.creature);
          }
        }
        if ((bool) (UnityEngine.Object) entity2)
          entity2.PlayHapticClip(this.imbueHitCurve, 0.25f);
        EffectInstance effectInstance = this.imbueCollisionEffectData?.Spawn(collisionInstance.contactPoint, Quaternion.LookRotation(collisionInstance.contactNormal, ((Component) collisionInstance.sourceCollider).transform.up), ((Component) collisionInstance.targetCollider).transform, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
        effectInstance?.Play(0, false, false);
        effectInstance.SetColorImmediate(this.currentColor);
        if (entity1 is Creature creature && (UnityEngine.Object) creature != (UnityEngine.Object) null && (UnityEngine.Object) creature != (UnityEngine.Object) this.spellCaster.mana.creature && !collisionInstance.targetMaterial.isMetal)
        {
          BrainModuleCrystal module = creature?.brain?.instance?.GetModule<BrainModuleCrystal>(true);
          module?.Crystallise(5f);
          module?.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, this.spellId), this.spellId);
        }
      }
      return base.OnImbueCollisionStart(collisionInstance);
    }

    public virtual bool OnCrystalSlam(CollisionInstance collisionInstance)
    {
      base.OnCrystalSlam(collisionInstance);
      EffectInstance effectInstance = this.staffSlamEffectData?.Spawn(collisionInstance.contactPoint, Quaternion.LookRotation(collisionInstance.contactNormal), (Transform) null, collisionInstance, true, (ColliderGroup) null, false, collisionInstance.intensity, 0.0f, Array.Empty<Type>());
      effectInstance?.Play(0, false, false);
      if (effectInstance != null)
        effectInstance.SetColorImmediate(this.currentColor);
      Creature creature1 = this.imbue?.colliderGroup?.collisionHandler?.item?.mainHandler?.creature;
      foreach (ThunderEntity inRadiu in ThunderEntity.InRadius(collisionInstance.contactPoint, this.staffSlamMaxRadius, (Func<ThunderEntity, bool>) null, (List<ThunderEntity>) null))
      {
        if (!(inRadiu is Creature creature2))
        {
          if (inRadiu is Item obj && (UnityEngine.Object) obj != (UnityEngine.Object) this.imbue.colliderGroup.collisionHandler.item)
          {
            Vector3 vector3_1 = ((ThunderBehaviour) obj).transform.position - collisionInstance.contactPoint;
            float magnitude = vector3_1.magnitude;
            if ((double) magnitude > 0.0)
            {
              vector3_1.Normalize();
              float num = Mathf.Lerp(this.staffSlamMinForce, this.staffSlamMaxForce, 1f - Mathf.Clamp01(magnitude / this.staffSlamMaxRadius));
              ((ThunderEntity) obj).AddForce(vector3_1 * num, (ForceMode) 1, (CollisionHandler) null);
              foreach (ColliderGroup colliderGroup in obj.colliderGroups)
              {
                if ((UnityEngine.Object) colliderGroup != (UnityEngine.Object) null && colliderGroup.allowImbueEffect && (UnityEngine.Object) colliderGroup.imbue != (UnityEngine.Object) null && (UnityEngine.Object) colliderGroup.collisionHandler.item.holder == (UnityEngine.Object) null && colliderGroup != null)
                  colliderGroup.imbue?.Transfer((SpellCastCharge) this, num, creature1);
              }
            }
            Breakable breakable = obj.breakable;
            if (breakable != null)
            {
              breakable.Explode(80f, collisionInstance.contactPoint, this.staffSlamMaxRadius, 0.25f, (ForceMode) 1);
              for (int index1 = 0; index1 < breakable.subBrokenItems.Count; ++index1)
              {
                Item subBrokenItem = breakable.subBrokenItems[index1];
                PhysicBody physicBody1 = subBrokenItem.physicBody;
                if (subBrokenItem.breakable != null)
                {
                  for (int index2 = 0; index2 < breakable.subBrokenItems.Count; ++index2)
                  {
                    PhysicBody physicBody2 = breakable.subBrokenItems[index2].physicBody;
                    if (PhysicBody.op_Implicit(physicBody2))
                    {
                      Vector3 vector3_2 = physicBody2.transform.position - collisionInstance.contactPoint;
                      vector3_2.Normalize();
                      physicBody2.AddForceAtPosition(vector3_2 * 10f * ((SpellData) this).GetModifier((Modifier) 2), physicBody2.transform.position, (ForceMode) 1);
                    }
                  }
                }
                if (PhysicBody.op_Implicit(physicBody1))
                {
                  Vector3 vector3_3 = physicBody1.transform.position - collisionInstance.contactPoint;
                  vector3_3.Normalize();
                  physicBody1.AddForceAtPosition(vector3_3 * 10f * ((SpellData) this).GetModifier((Modifier) 2), physicBody1.transform.position, (ForceMode) 1);
                }
              }
              for (int index = 0; index < breakable.subBrokenBodies.Count; ++index)
              {
                PhysicBody subBrokenBody = breakable.subBrokenBodies[index];
                if (PhysicBody.op_Implicit(subBrokenBody))
                {
                  Vector3 vector3_4 = subBrokenBody.transform.position - collisionInstance.contactPoint;
                  vector3_4.Normalize();
                  subBrokenBody.AddForceAtPosition(vector3_4 * 10f * ((SpellData) this).GetModifier((Modifier) 2), subBrokenBody.transform.position, (ForceMode) 1);
                }
              }
            }
          }
        }
        else if (((UnityEngine.Object) creature1 == (UnityEngine.Object) null || (UnityEngine.Object) creature2 != (UnityEngine.Object) creature1) && creature2.factionId != creature1.factionId)
        {
          creature2.ragdoll.SetState((Ragdoll.State) 1);
          Vector3 vector3 = ((ThunderBehaviour) creature2).transform.position - collisionInstance.contactPoint;
          float magnitude = vector3.magnitude;
          if ((double) magnitude > 0.0)
          {
            vector3.Normalize();
            float num = Mathf.Lerp(this.staffSlamMinForce, this.staffSlamMaxForce, 1f - Mathf.Clamp01(magnitude / this.staffSlamMaxRadius));
            creature2.AddForce(vector3 * num, (ForceMode) 1, 1f, (CollisionHandler) null);
            creature2.AddForce(Vector3.up * SpellCastCrystallic.slamUpwardsForceMult, (ForceMode) 1, 1f, (CollisionHandler) null);
          }
          BrainModuleCrystal module = creature2.brain.instance.GetModule<BrainModuleCrystal>(true);
          module.Crystallise(5f);
          module.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, this.spellId), this.spellId);
        }
      }
      return true;
    }

    public void EnableFingerEffects()
    {
      foreach (RagdollHand.Finger finger in this.spellCaster.ragdollHand.fingers)
      {
        EffectInstance effectInstance = this.fingerEffectData?.Spawn(finger.tip.transform, true, (ColliderGroup) null, false);
        if (effectInstance != null)
        {
          this.fingerEffects.Add(effectInstance);
          effectInstance.Play(0, false, false);
          if (effectInstance != null)
            effectInstance.SetParticleTarget(this.fingerEffectPresetData, this.spellCaster.Orb, triggerRadius: 0.04f);
        }
      }
    }

    public void DisableFingerEffects()
    {
      foreach (EffectInstance fingerEffect in this.fingerEffects)
      {
        fingerEffect.End(false, -1f);
        fingerEffect.ClearParticleTarget();
        fingerEffect.ForceStop((ParticleSystemStopBehavior) 0);
      }
    }

    public virtual void UpdateSpray()
    {
      bool flag = this.isSpraying && (double) this.currentCharge < (double) this.sprayStopMinCharge;
      base.UpdateSpray();
      if (!flag)
        return;
      this.spellCaster.Fire(false);
    }

    public virtual void OnSprayStart()
    {
      base.OnSprayStart();
      this.allowCharge = false;
      ((ValueHandler<float>) Player.local.physicRangeModifier).Add((object) this, 10f);
      SkillHyperintensity.ToggleDrain(this.spellCaster.side, false);
      SpellCastCrystallic.SprayEvent onSprayStart = this.onSprayStart;
      if (onSprayStart != null)
        onSprayStart(this);
      this.isSpraying = true;
    }

    public virtual void OnSprayLoop()
    {
      base.OnSprayLoop();
      SpellCastCrystallic.SprayEvent onSprayLoop = this.onSprayLoop;
      if (onSprayLoop == null)
        return;
      onSprayLoop(this);
    }

    public virtual void OnSprayStop()
    {
      base.OnSprayStop();
      ((ValueHandler<float>) Player.local.physicRangeModifier).Remove((object) this);
      SkillHyperintensity.ToggleDrain(this.spellCaster.side, true);
      SpellCastCrystallic.SprayEvent onSprayEnd = this.onSprayEnd;
      if (onSprayEnd != null)
        onSprayEnd(this);
      this.isSpraying = false;
    }

    public delegate void ShardshotEvent(
      SpellCastCrystallic spellCastCrystallic,
      EffectInstance effectInstance);

    public delegate void ShardshotHitEvent(
      SpellCastCrystallic spellCastCrystallic,
      ThunderEntity entity,
      SpellCastCrystallic.ShardshotHit hitInfo);

    public delegate void SprayEvent(SpellCastCrystallic spellCastCrystallic);

    public struct ShardshotHit(
      EffectInstance effectInstance,
      Collider hitCollider,
      Vector3 hitPoint,
      Vector3 hitNormal,
      Vector3 velocity,
      ParticleCollisionEvent baseCollision,
      GameObject hitObject,
      ThunderEntity hitEntity,
      RagdollPart hitPart,
      float hitDamage,
      bool wasMetal)
    {
      public EffectInstance effectInstance = effectInstance;
      public Collider hitCollider = hitCollider;
      public Vector3 hitPoint = hitPoint;
      public Vector3 hitNormal = hitNormal;
      public Vector3 velocity = velocity;
      public ParticleCollisionEvent baseCollision = baseCollision;
      public GameObject hitObject = hitObject;
      public ThunderEntity hitEntity = hitEntity;
      public RagdollPart hitPart = hitPart;
      public float hitDamage = hitDamage;
      public bool wasMetal = wasMetal;
    }
  }
}
