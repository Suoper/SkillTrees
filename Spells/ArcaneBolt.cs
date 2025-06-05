// Decompiled with JetBrains decompiler
// Type: Arcana.Spells.ArcaneBolt
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Misc;
using Arcana.Statuses;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Spells
{
  public class ArcaneBolt : SpellCastCharge
  {
    public static bool harmonyPatchApplied;
    public float projectileVelocity = 14f;
    public bool projectilePlayerGuided = true;
    public float projectileGuidanceDelay = 0.5f;
    public int projectileCount = 3;
    public float projectileTimeBetween = 0.2f;
    public float projectileConeMinAngle = 25f;
    public float projectileConeMaxAngle = 30f;
    public bool projectileHoming = true;
    public bool performRayTargeting = true;
    public float targetRaycastDistance = 100f;
    public string projectileId;
    public string projectileEffectId;
    public string projectileDamagerId;
    public string projectileStatusId;
    public float projectileStatusDuration;
    public float projectileStatusTransfer;
    public float projectileHomingRadius;
    public bool explodeOnSlam = false;
    public float staffSlamDetectionRadius;
    public int staffSlamDetectionCount = 3;
    public float gripCastStatusTransfer;
    public string imbueHitStatusEffectId;
    protected StatusData imbueHitStatusEffectData;
    public float imbueHitStatusDuration = float.PositiveInfinity;
    public float imbueHitTransfer;
    public float imbuePenetrateTransferPerSecond;
    public float intensityPerSkill = 0.1f;
    public float durationPerSkill = 0.1f;
    public bool destroyInWater = true;
    [Range(0.0f, 1f)]
    public float throwHeadBias = 0.5f;
    public readonly AnimationCurve damageOverTimeCurve = AnimationCurve.Linear(1f, 1f, 1f, 1f);
    public List<CatalogThreshold> editorThresholds;
    public List<Threshold> thresholds;
    public Gradient defaultPrimaryGradient;
    public Gradient defaultSecondaryGradient;
    public List<string> particleTransformsDefaultToSecondary;
    public ItemData projectileData;
    public EffectData projectileEffectData;
    public DamagerData projectileDamagerData;
    public StatusData projectileStatusData;
    public ItemMagicProjectile lastThrownProjectile;
    public Vector3 guidanceDirection;
    public int raycastMask;
    public ProjectileManager projectileManager;
    private float? gripCanExplodeStart;
    private float gripCanExplodeDelay = 0.75f;

    public event ArcaneBolt.OnArcaneBoltThrow OnArcaneBoltThrowEvent;

    public event ArcaneBolt.SprayEvent OnSprayStartEvent;

    public event ArcaneBolt.SprayEvent OnSprayLoopEvent;

    public event ArcaneBolt.SprayEvent OnSprayStopEvent;

    public float GuidanceTime() => this.projectileGuidanceDelay * 0.96f;

    public Vector3 GuidanceFunc() => this.guidanceDirection;

    protected void UpdateDamageCurve()
    {
      if (this.editorThresholds == null)
        return;
      Keyframe[] keyframeArray = new Keyframe[this.editorThresholds.Count];
      for (int index = 0; index < this.editorThresholds.Count; ++index)
      {
        CatalogThreshold editorThreshold = this.editorThresholds[index];
        keyframeArray[index] = new Keyframe(editorThreshold.time, editorThreshold.value, editorThreshold.inCurve, editorThreshold.outCurve);
      }
      this.damageOverTimeCurve.keys = keyframeArray;
    }

    public virtual void OnCatalogRefresh()
    {
      base.OnCatalogRefresh();
      if (!ArcaneBolt.harmonyPatchApplied)
      {
        new Harmony("com.matrixphantom.arcana").PatchAll();
        ArcaneBolt.harmonyPatchApplied = true;
        Debug.Log((object) "Applied Arcana Harmony Patches!");
      }
      this.projectileData = Catalog.GetData<ItemData>(this.projectileId, true);
      this.projectileEffectData = Catalog.GetData<EffectData>(this.projectileEffectId, true);
      this.projectileDamagerData = Catalog.GetData<DamagerData>(this.projectileDamagerId, true);
      this.projectileStatusData = Catalog.GetData<StatusData>(this.projectileStatusId, true);
      this.imbueHitStatusEffectData = Catalog.GetData<StatusData>(this.imbueHitStatusEffectId, true);
      this.thresholds = new List<Threshold>();
      if (this.editorThresholds != null)
      {
        for (int index = 0; index < this.editorThresholds.Count; ++index)
          this.thresholds.Add(CatalogThreshold.op_Implicit(this.editorThresholds[index]));
      }
      this.UpdateDamageCurve();
      this.raycastMask = Utilities.GetProjectileRaycastMask();
    }

    public virtual void Load(SpellCaster spellCaster)
    {
      base.Load(spellCaster);
      this.projectileManager = this.CreateProjectileManager(spellCaster, this.projectileEffectData);
    }

    public ProjectileManager CreateProjectileManager(
      SpellCaster spellCaster,
      EffectData projectileEffectData)
    {
      ProjectileManager projectileManager = new ProjectileManager((SpellData) this, spellCaster, this.projectileData, projectileEffectData, this.projectileDamagerData, this.projectileStatusData, (SpellCastCharge) this)
      {
        thresholds = this.thresholds,
        damageOverTimeCurve = this.damageOverTimeCurve,
        projectileVelocity = this.projectileVelocity,
        projectilePlayerGuided = this.projectilePlayerGuided,
        projectileGuidanceDelay = this.projectileGuidanceDelay,
        projectileCount = this.projectileCount,
        projectileTimeBetween = new float?(this.projectileTimeBetween),
        projectileConeMinAngle = new float?(this.projectileConeMinAngle),
        projectileConeMaxAngle = new float?(this.projectileConeMaxAngle),
        guidanceDirection = this.guidanceDirection,
        projectileStatusDuration = new float?(this.projectileStatusDuration),
        projectileStatusTransfer = new float?(this.projectileStatusTransfer),
        doHoming = this.projectileHoming,
        projectileHomingRadius = new float?(this.projectileHomingRadius),
        performRayTargeting = this.performRayTargeting
      };
      projectileManager.OnProjectilePreSpawnEvent -= new ProjectileManager.OnProjectileSpawn(this.OnProjectileSpawn);
      projectileManager.OnProjectilePreSpawnEvent += new ProjectileManager.OnProjectileSpawn(this.OnProjectileSpawn);
      return projectileManager;
    }

    public virtual void LoadSkillPassives(int skillCount)
    {
      base.LoadSkillPassives(skillCount);
      ((SpellData) this).AddModifier((object) this, (Modifier) 1, (float) (1.0 + (double) this.durationPerSkill * (double) skillCount));
      ((SpellData) this).AddModifier((object) this, (Modifier) 2, (float) (1.0 + (double) this.intensityPerSkill * (double) skillCount));
    }

    public virtual void Throw(Vector3 velocity)
    {
      base.Throw(velocity);
      if (((RagdollPart) this.spellCaster.ragdollHand).ragdoll.creature.isPlayer)
        velocity = Vector3.Slerp(velocity.normalized, ((Component) Player.local.head).transform.forward, this.throwHeadBias) * velocity.magnitude;
      Transform orb = this.spellCaster.Orb;
      Vector3 vector3 = velocity * this.projectileVelocity;
      GuidanceMode guidanceMode = !((UnityEngine.Object) this.spellCaster == (UnityEngine.Object) null) ? (((RagdollPart) this.spellCaster.ragdollHand).ragdoll.creature.isPlayer ? (this.projectilePlayerGuided ? (GuidanceMode) 2 : (GuidanceMode) 0) : (GuidanceMode) 2) : (GuidanceMode) 0;
      if (this.throwEffectData != null)
      {
        EffectInstance effectInstance = this.throwEffectData.Spawn(this.spellCaster.magicSource.position, Quaternion.LookRotation(vector3.normalized), (Transform) null, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
        if (this.spellCaster.mana.creature.isPlayer)
          effectInstance.SetHaptic(this.spellCaster.side, Catalog.gameData.haptics.telekinesisThrow);
        effectInstance.Play(0, false, false);
      }
      Vector3? nullable1 = new Vector3?();
      if (!((RagdollPart) this.spellCaster.ragdollHand).ragdoll.creature.isPlayer)
      {
        nullable1 = new Vector3?(((ThunderBehaviour) Player.currentCreature.ragdoll.targetPart).transform.position);
      }
      else
      {
        RaycastHit raycastHit;
        if (this.performRayTargeting && Physics.Raycast(orb.position, velocity.normalized, ref raycastHit, this.targetRaycastDistance, this.raycastMask))
          nullable1 = new Vector3?(((RaycastHit) ref raycastHit).point);
      }
      HashSet<Item> projectiles1 = new HashSet<Item>();
      if ((UnityEngine.Object) this.projectileManager.spellCaster == (UnityEngine.Object) null)
        this.projectileManager.spellCaster = this.spellCaster;
      GameManager local = GameManager.local;
      ProjectileManager projectileManager = this.projectileManager;
      Transform transformCopy = Utilities.GetTransformCopy(orb);
      Vector3 velocity1 = vector3;
      GuidanceMode guidance = guidanceMode;
      Vector3? nullable2 = nullable1;
      ProjectileManager.ProjectileSpawnEvent projectileSpawnEvent = new ProjectileManager.ProjectileSpawnEvent(this.projectileManager.ProjectileGuidanceHomingCoroutine);
      HashSet<Item> objSet = projectiles1;
      float? startRadius = new float?();
      Vector3? rayTarget = nullable2;
      ProjectileManager.ProjectileSpawnEvent onSpawn = projectileSpawnEvent;
      HashSet<Item> projectiles2 = objSet;
      int? overrideCount = new int?();
      float? overrideTimeBetween = new float?();
      IEnumerator routine = projectileManager.SpawnProjectilesCoroutine(transformCopy, velocity1, guidance, startRadius, rayTarget, onSpawn, projectiles: projectiles2, overrideCount: overrideCount, overrideTimeBetween: overrideTimeBetween);
      ((MonoBehaviour) local).StartCoroutine(routine);
      ArcaneBolt.OnArcaneBoltThrow arcaneBoltThrowEvent = this.OnArcaneBoltThrowEvent;
      if (arcaneBoltThrowEvent == null)
        return;
      arcaneBoltThrowEvent(this, velocity, projectiles1);
    }

    private void OnProjectileSpawn(
      SpellCastCharge spell,
      ItemMagicProjectile projectile,
      SpellCaster caster)
    {
      projectile.effectInstance.MixColorInEffectGradient(new Color?(), true, this.defaultPrimaryGradient, this.defaultSecondaryGradient, particleTransformsDefaultToSecondary: this.particleTransformsDefaultToSecondary);
    }

    public virtual void Fire(bool active)
    {
      base.Fire(active);
      if (!active)
        this.currentCharge = 0.0f;
      else
        this.lastThrownProjectile = (ItemMagicProjectile) null;
    }

    public virtual void UpdateCaster()
    {
      base.UpdateCaster();
      this.guidanceDirection = (this.spellCaster.rayDir.position - (((ThunderBehaviour) this.spellCaster.ragdollHand.upperArmPart).transform.position + new Vector3(0.0f, this.spellCaster.ragdollHand.creature.morphology.armsToEyesHeight * ((ThunderBehaviour) this.spellCaster.ragdollHand.creature).transform.localScale.y, 0.0f))).normalized;
    }

    public virtual bool OnImbueCollisionStart(CollisionInstance collision)
    {
      collision.targetColliderGroup?.collisionHandler?.Entity?.Inflict(this.imbueHitStatusEffectData, (object) this, this.imbueHitStatusDuration, (object) (float) ((double) this.imbueHitTransfer * (double) collision.intensity * (double) this.imbue.EnergyRatio), true);
      return base.OnImbueCollisionStart(collision);
    }

    public virtual void UpdateImbue(float speedRatio)
    {
      base.UpdateImbue(speedRatio);
      int num;
      if ((UnityEngine.Object) this.imbue == (UnityEngine.Object) null)
      {
        num = 1;
      }
      else
      {
        bool? isPenetrating = this.imbue.colliderGroup?.collisionHandler?.item?.isPenetrating;
        bool flag = true;
        num = !(isPenetrating.GetValueOrDefault() == flag & isPenetrating.HasValue) ? 1 : 0;
      }
      if (num != 0)
        return;
      for (int index = 0; index < this.imbue.colliderGroup.collisionHandler.collisions.Length; ++index)
      {
        CollisionInstance collision = this.imbue.colliderGroup.collisionHandler.collisions[index];
        if (collision.damageStruct.penetration != null && !((UnityEngine.Object) collision.sourceColliderGroup != (UnityEngine.Object) this.imbue.colliderGroup))
          collision.targetColliderGroup?.collisionHandler?.Entity.Inflict(this.imbueHitStatusEffectData, (object) this, this.imbueHitStatusDuration, (object) (float) ((double) this.imbuePenetrateTransferPerSecond * (double) Time.deltaTime), true);
      }
    }

    public virtual void UpdateGripCast(HandleRagdoll handle)
    {
      if ((double) this.gripCastDamageAmount != 0.0 && (double) Time.unscaledTime - (double) this.lastGripCastTime > (double) this.gripCastDamageInterval)
      {
        this.lastGripCastTime = Time.unscaledTime;
        handle.ragdollPart.ragdoll.creature.DamagePatched(this.gripCastDamageAmount, (DamageType) 4);
      }
      if (handle.ragdollPart.isSliced || this.gripCastStatusEffect == null)
        return;
      ((ThunderEntity) handle.ragdollPart.ragdoll.creature).Inflict(this.gripCastStatusEffect, (object) this, this.gripCastStatusDuration, (object) (this.gripCastStatusTransfer * Time.unscaledDeltaTime), true);
      ArcaneStatus arcaneStatus;
      if (!((ThunderEntity) handle.ragdollPart.ragdoll.creature).TryGetStatus<ArcaneStatus>(this.gripCastStatusEffect, ref arcaneStatus) || !arcaneStatus.CanExplode)
        return;
      if (!this.gripCanExplodeStart.HasValue)
        this.gripCanExplodeStart = new float?(Time.time);
      int num;
      if (this.gripCanExplodeStart.HasValue)
      {
        float time = Time.time;
        float? gripCanExplodeStart = this.gripCanExplodeStart;
        float? nullable = gripCanExplodeStart.HasValue ? new float?(time - gripCanExplodeStart.GetValueOrDefault()) : new float?();
        float gripCanExplodeDelay = this.gripCanExplodeDelay;
        if ((double) nullable.GetValueOrDefault() >= (double) gripCanExplodeDelay & nullable.HasValue)
        {
          num = !handle.ragdollPart.ragdoll.creature.isKilled ? 1 : 0;
          goto label_11;
        }
      }
      num = 0;
label_11:
      if (num != 0)
      {
        ArcaneStatus.Explode(((Status) arcaneStatus).data as StatusDataArcane, handle.ragdollPart.ragdoll.creature);
        this.gripCanExplodeStart = new float?();
      }
    }

    public virtual bool OnCrystalSlam(CollisionInstance collisionInstance)
    {
      base.OnCrystalSlam(collisionInstance);
      float radius = this.staffSlamDetectionRadius * this.imbue.GetModifier().imbueEffectiveness;
      int slamDetectionCount = this.staffSlamDetectionCount;
      ((MonoBehaviour) GameManager.local).StartCoroutine(this.ExplodeOnSlamCoroutine(this.CreateProjectileManager(this.spellCaster, this.projectileEffectData), Utilities.GetCreaturesInRadius(this.imbue.colliderGroup.imbueShoot.position, radius, slamDetectionCount)));
      return true;
    }

    private IEnumerator ExplodeOnSlamCoroutine(ProjectileManager manager, Creature[] targets)
    {
      int count = targets.Length != 0 ? targets.Length : 5;
      for (int index = 0; index < count; ++index)
      {
        Creature target = targets.Length != 0 ? targets[index % targets.Length] : (Creature) null;
        ProjectileManager projectileManager = manager;
        Vector3 position = this.imbue.colliderGroup.imbueShoot.position + this.imbue.colliderGroup.imbueShoot.forward * 0.05f;
        Quaternion rotation = this.imbue.colliderGroup.imbueShoot.rotation;
        Vector3 randomVelocityInCone = Utilities.GetRandomVelocityInCone(this.imbue.colliderGroup.imbueShoot.forward, 120f, this.projectileVelocity, 120f);
        Creature creature = target;
        Item ignoredItem = this.imbue.colliderGroup.collisionHandler.item;
        Vector3? guidanceTarget = new Vector3?();
        Creature targetCreature = creature;
        projectileManager.ThrowProjectile(position, rotation, randomVelocityInCone, ignoredItem: ignoredItem, guidanceTarget: guidanceTarget, homing: true, targetCreature: targetCreature);
        yield return (object) new WaitForSeconds(0.1f);
        target = (Creature) null;
      }
    }

    public virtual void UpdateSpray()
    {
      int num = !this.isSpraying ? 0 : ((double) this.currentCharge < (double) this.sprayStopMinCharge ? 1 : 0);
      if ((this.spellCaster.other.isFiring || this.spellCaster.other.isSpraying) && (double) (((Component) this.spellCaster).transform.position - ((Component) this.spellCaster.other).transform.position).magnitude < 0.5)
      {
        if (!this.isSpraying)
          return;
        this.spellCaster.ragdollHand.poser.SetDefaultPose(this.openHandPoseData);
        this.spellCaster.SetMagicOffset(Vector3.zero, false);
        this.isSpraying = false;
        base.OnSprayStop();
      }
      else
      {
        base.UpdateSpray();
        if (num == 0)
          return;
        this.spellCaster.Fire(false);
      }
    }

    public virtual void OnSprayStart()
    {
      base.OnSprayStart();
      ArcaneBolt.SprayEvent onSprayStartEvent = this.OnSprayStartEvent;
      if (onSprayStartEvent == null)
        return;
      onSprayStartEvent(this);
    }

    public virtual void OnSprayLoop()
    {
      base.OnSprayLoop();
      ArcaneBolt.SprayEvent onSprayLoopEvent = this.OnSprayLoopEvent;
      if (onSprayLoopEvent == null)
        return;
      onSprayLoopEvent(this);
    }

    public virtual void OnSprayStop()
    {
      base.OnSprayStop();
      ArcaneBolt.SprayEvent onSprayStopEvent = this.OnSprayStopEvent;
      if (onSprayStopEvent == null)
        return;
      onSprayStopEvent(this);
    }

    public delegate void OnArcaneBoltThrow(
      ArcaneBolt spell,
      Vector3 direction,
      HashSet<Item> projectiles);

    public delegate void SprayEvent(ArcaneBolt spell);
  }
}
