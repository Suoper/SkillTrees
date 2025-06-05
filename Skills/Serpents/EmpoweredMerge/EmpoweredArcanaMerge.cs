// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.Serpents.EmpoweredMerge.EmpoweredArcanaMerge
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Behaviors;
using Arcana.Misc;
using Arcana.Skills.SpellMerge;
using Arcana.Spells;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Skills.Serpents.EmpoweredMerge
{
  public class EmpoweredArcanaMerge : EmpoweredMergeData
  {
    public string arcaneOrbSkillId = "Skill_ArcaneOrb";
    private SkillArcaneOrb skillArcaneOrb;
    public float orbSplitAngle = 45f;
    public float splitOrbScale = 0.5f;
    public string arcaneTempestSkillId = "Skill_ArcaneTempest";
    private SkillArcaneTempest skillArcaneTempest;
    public string sprayOriginEffectId = "SpellArcaneBeamOrigin";
    public EffectData sprayOriginEffect;
    public string sprayProjectileEffectId = "SpellArcaneShard";
    public EffectData sprayProjectileEffect;
    private ArcaneBolt arcaneBolt;
    private List<EmpoweredArcanaMerge.SprayOrigin> sprayOrigins;

    public override void OnCatalogRefresh()
    {
      base.OnCatalogRefresh();
      this.sprayOriginEffect = Catalog.GetData<EffectData>(this.sprayOriginEffectId, true);
      this.sprayProjectileEffect = Catalog.GetData<EffectData>(this.sprayProjectileEffectId, true);
    }

    public override void OnLoad(SkillArcaneEmpoweringBond skill, SpellMergeData spell)
    {
      base.OnLoad(skill, spell);
      if (!(spell is ArcaneMerge arcaneMerge))
        return;
      if (arcaneMerge.defaultSkillData is SkillArcaneOrb defaultSkillData1)
        LoadOrbEmpower(defaultSkillData1);
      else if (Player.local.creature.TryGetSkill<SkillArcaneOrb>(this.arcaneOrbSkillId, ref defaultSkillData1))
        LoadOrbEmpower(defaultSkillData1);
      if (arcaneMerge.defaultSkillData is SkillArcaneTempest defaultSkillData2)
        LoadTempestEmpower(defaultSkillData2);
      else if (Player.local.creature.TryGetSkill<SkillArcaneTempest>(this.arcaneTempestSkillId, ref defaultSkillData2))
        LoadTempestEmpower(defaultSkillData2);
      Debug.Log((object) "Loaded Empowered Merge");

      void LoadOrbEmpower(SkillArcaneOrb mergeSkill)
      {
        this.skillArcaneOrb = mergeSkill;
        mergeSkill.OnOrbThrowEvent -= new SkillArcaneOrb.OnOrbThrow(this.OnThrow);
        mergeSkill.OnOrbThrowEvent += new SkillArcaneOrb.OnOrbThrow(this.OnThrow);
        mergeSkill.OnStatusZoneAddedEvent -= new SkillArcaneOrb.OnStatusZoneAdded(this.OnStatusZoneAdded);
        mergeSkill.OnStatusZoneAddedEvent += new SkillArcaneOrb.OnStatusZoneAdded(this.OnStatusZoneAdded);
        mergeSkill.OnOrbEndEvent -= new SkillArcaneOrb.OnOrb(this.OnOrbEnd);
        mergeSkill.OnOrbEndEvent += new SkillArcaneOrb.OnOrb(this.OnOrbEnd);
        Debug.Log((object) "Loaded Empowered Skill - Arcane Orb");
      }

      void LoadTempestEmpower(SkillArcaneTempest mergeSkill)
      {
        this.skillArcaneTempest = mergeSkill;
        this.arcaneBolt = Catalog.GetData<ArcaneBolt>("Arcane", true);
        mergeSkill.OnSprayStartEvent -= new SkillArcaneTempest.OnSpray(this.OnSprayStart);
        mergeSkill.OnSprayStartEvent += new SkillArcaneTempest.OnSpray(this.OnSprayStart);
        mergeSkill.OnSprayEndEvent -= new SkillArcaneTempest.OnSpray(this.OnSprayEnd);
        mergeSkill.OnSprayEndEvent += new SkillArcaneTempest.OnSpray(this.OnSprayEnd);
        mergeSkill.OnSprayUpdateEvent -= new SkillArcaneTempest.OnSpray(this.OnSprayUpdate);
        mergeSkill.OnSprayUpdateEvent += new SkillArcaneTempest.OnSpray(this.OnSprayUpdate);
        Debug.Log((object) "Loaded Empowered Skill - Arcane Tempest");
      }
    }

    public override void OnUnload(SpellMergeData spell)
    {
      base.OnUnload(spell);
      if (!(this.mergeData is ArcaneMerge))
        return;
      if (this.skillArcaneOrb != null)
      {
        this.skillArcaneOrb.OnOrbThrowEvent -= new SkillArcaneOrb.OnOrbThrow(this.OnThrow);
        this.skillArcaneOrb.OnStatusZoneAddedEvent -= new SkillArcaneOrb.OnStatusZoneAdded(this.OnStatusZoneAdded);
        this.skillArcaneOrb.OnOrbEndEvent -= new SkillArcaneOrb.OnOrb(this.OnOrbEnd);
        this.skillArcaneOrb = (SkillArcaneOrb) null;
      }
      if (this.skillArcaneTempest != null)
      {
        this.skillArcaneTempest.OnSprayStartEvent -= new SkillArcaneTempest.OnSpray(this.OnSprayStart);
        this.skillArcaneTempest.OnSprayEndEvent -= new SkillArcaneTempest.OnSpray(this.OnSprayEnd);
        this.skillArcaneTempest.OnSprayUpdateEvent -= new SkillArcaneTempest.OnSpray(this.OnSprayUpdate);
        this.skillArcaneTempest = (SkillArcaneTempest) null;
      }
      if (this.sprayOrigins == null)
        return;
      foreach (EmpoweredArcanaMerge.SprayOrigin sprayOrigin in this.sprayOrigins.ToList<EmpoweredArcanaMerge.SprayOrigin>())
      {
        this.skillArcaneEmpoweringBond.AssignMergeSerpent(sprayOrigin.serpent);
        sprayOrigin.Destroy();
        this.sprayOrigins.Remove(sprayOrigin);
      }
      Debug.Log((object) "Unloaded Empowered Merge");
    }

    private void OnThrow(ArcaneMerge merge, Vector3 velocity)
    {
      if (!SkillArcaneEmpoweringBond.AllowEmpoweredMerge)
        return;
      this.skillArcaneOrb.SpawnProjectile(merge, Quaternion.AngleAxis(this.orbSplitAngle, Vector3.up) * velocity, (object) this);
      this.skillArcaneOrb.SpawnProjectile(merge, Quaternion.AngleAxis(-this.orbSplitAngle, Vector3.up) * velocity, (object) this);
    }

    private void OnStatusZoneAdded(
      ArcaneOrbStatusApplicator applicator,
      ItemMagicAreaProjectile projectile,
      ArcaneMerge spell,
      SkillArcaneOrb skill,
      object handler)
    {
      if (!(handler is EmpoweredArcanaMerge))
        return;
      foreach (EffectParticle effectParticle in ((ItemMagicProjectile) projectile).effectInstance.effects.OfType<EffectParticle>())
        ((Component) effectParticle.rootParticleSystem).transform.localScale *= this.splitOrbScale;
      applicator.zone.SetRadius(skill.projectileEffectRadius * this.splitOrbScale);
    }

    private void OnOrbEnd(
      ItemMagicAreaProjectile projectile,
      ArcaneMerge spell,
      SkillArcaneOrb skill,
      EventTime eventTime,
      object handler)
    {
    }

    private void OnSprayStart(SkillArcaneTempest merge, Mana mana)
    {
      Debug.Log((object) "Empowered Spray Started!");
      ProjectileManager projectileManager = new ProjectileManager((SpellData) mana.casterRight.spellInstance, mana.casterRight, this.arcaneBolt.projectileData, this.sprayProjectileEffect, this.arcaneBolt.projectileDamagerData, this.arcaneBolt.projectileStatusData, (SpellCastCharge) this.arcaneBolt)
      {
        thresholds = this.arcaneBolt.thresholds,
        damageOverTimeCurve = this.arcaneBolt.damageOverTimeCurve,
        projectileVelocity = this.arcaneBolt.projectileVelocity,
        projectilePlayerGuided = this.arcaneBolt.projectilePlayerGuided,
        projectileGuidanceDelay = this.arcaneBolt.projectileGuidanceDelay,
        projectileCount = 1,
        projectileTimeBetween = new float?(0.5f),
        projectileConeMinAngle = new float?(1f),
        projectileConeMaxAngle = new float?(5f),
        guidanceDirection = merge.targetRay.direction.normalized,
        projectileStatusDuration = new float?(this.arcaneBolt.projectileStatusDuration),
        projectileStatusTransfer = new float?(this.arcaneBolt.projectileStatusTransfer),
        doHoming = false,
        projectileHomingRadius = new float?(0.0f),
        performRayTargeting = true
      };
      this.sprayOrigins = new List<EmpoweredArcanaMerge.SprayOrigin>();
      for (int index = 0; index < SkillArcaneEmpoweringBond.MergeSerpents.Count; ++index)
        this.sprayOrigins.Add(new EmpoweredArcanaMerge.SprayOrigin(merge, mana, SkillArcaneEmpoweringBond.MergeSerpents[index], projectileManager, this.sprayOriginEffect, (float) ((double) index / (double) SkillArcaneEmpoweringBond.MergeSerpents.Count * 360.0)));
    }

    private void OnSprayEnd(SkillArcaneTempest merge, Mana mana)
    {
      Debug.Log((object) "Empowered Spray Ended!");
      foreach (EmpoweredArcanaMerge.SprayOrigin sprayOrigin in this.sprayOrigins.ToList<EmpoweredArcanaMerge.SprayOrigin>())
      {
        this.skillArcaneEmpoweringBond.AssignMergeSerpent(sprayOrigin.serpent);
        sprayOrigin.Destroy();
        this.sprayOrigins.Remove(sprayOrigin);
      }
    }

    private void OnSprayUpdate(SkillArcaneTempest merge, Mana mana)
    {
      if (!merge.SprayActive)
        return;
      foreach (EmpoweredArcanaMerge.SprayOrigin sprayOrigin in this.sprayOrigins)
      {
        sprayOrigin.UpdateRotations();
        sprayOrigin.Shoot();
      }
    }

    private class SprayOrigin
    {
      public RotateAroundCenter rotateAroundCenter;
      public RotateAroundCenter serpentTarget;
      public SkillArcaneTempest merge;
      public Mana mana;
      public Serpent serpent;
      public ProjectileManager projectileManager;
      private EffectInstance effectInstance;
      private float lastShootTime;
      private float shootDelay = 0.3f;

      public SprayOrigin(
        SkillArcaneTempest merge,
        Mana mana,
        Serpent serpent,
        ProjectileManager projectileManager,
        EffectData originEffectData,
        float offset)
      {
        this.merge = merge;
        this.mana = mana;
        this.serpent = serpent;
        this.projectileManager = projectileManager;
        this.rotateAroundCenter = new GameObject().AddComponent<RotateAroundCenter>();
        this.rotateAroundCenter.transform.SetPositionAndRotation(merge.SprayPosition(mana), Quaternion.LookRotation(merge.targetRay.direction));
        this.rotateAroundCenter.radius = 0.55f;
        this.rotateAroundCenter.speed = 30f;
        this.rotateAroundCenter.moveInSineWave = false;
        this.rotateAroundCenter.rotationOffset = offset;
        this.rotateAroundCenter.rotateTowardsDirection = false;
        this.rotateAroundCenter.UpdatePosition(merge.SprayPosition(mana), merge.targetRay.direction);
        this.serpentTarget = new GameObject().AddComponent<RotateAroundCenter>();
        this.serpentTarget.transform.SetPositionAndRotation(merge.SprayPosition(mana), Quaternion.LookRotation(merge.targetRay.direction));
        this.serpentTarget.radius = 0.09f;
        this.serpentTarget.speed = 90f;
        this.serpentTarget.moveInSineWave = false;
        this.serpentTarget.rotationOffset = offset;
        this.serpentTarget.rotateTowardsDirection = false;
        this.serpentTarget.UpdatePosition(merge.SprayPosition(mana), merge.targetRay.direction);
        this.effectInstance = originEffectData?.Spawn(this.rotateAroundCenter.transform, true, (ColliderGroup) null, false);
        this.effectInstance?.Play(0, false, false);
        serpent.LoadRotationData(new float?(0.03f), new float?(0.05f), new float?(135f), new float?(30f));
        serpent.movementMultiplier = 1.2f;
        serpent.tempScale = new float?(1f);
        serpent.objectOrbitNormal = merge.targetRay.direction;
        serpent.AssignNewOrbit(this.rotateAroundCenter.transform, false, timeLimit: serpent.data.teleportTargetTimeout);
      }

      public void UpdateRotations()
      {
        this.rotateAroundCenter.UpdatePosition(this.merge.SprayPosition(this.mana), this.merge.targetRay.direction);
        this.rotateAroundCenter.transform.rotation = Quaternion.Slerp(this.rotateAroundCenter.transform.rotation, Quaternion.LookRotation(this.merge.targetRay.direction), Time.deltaTime * 3f);
        this.serpentTarget.UpdatePosition(this.rotateAroundCenter.transform.position, this.rotateAroundCenter.transform.forward);
        this.serpentTarget.transform.rotation = this.serpent.transform.rotation;
      }

      public void Shoot()
      {
        if ((double) Time.time - (double) this.lastShootTime <= (double) this.shootDelay)
          return;
        Debug.Log((object) "Attempting Shoot");
        GameManager local = GameManager.local;
        ProjectileManager projectileManager = this.projectileManager;
        Transform transform = this.rotateAroundCenter.transform;
        Vector3 velocity = this.merge.targetRay.direction * 10f;
        ProjectileManager.ProjectileSpawnEvent projectileSpawnEvent = new ProjectileManager.ProjectileSpawnEvent(this.projectileManager.ProjectileGuidanceHomingCoroutine);
        float? startRadius = new float?();
        Vector3? rayTarget = new Vector3?();
        ProjectileManager.ProjectileSpawnEvent onSpawn = projectileSpawnEvent;
        int? overrideCount = new int?();
        float? overrideTimeBetween = new float?();
        IEnumerator routine = projectileManager.SpawnProjectilesCoroutine(transform, velocity, (GuidanceMode) 2, startRadius, rayTarget, onSpawn, overrideCount: overrideCount, overrideTimeBetween: overrideTimeBetween);
        ((MonoBehaviour) local).StartCoroutine(routine);
        this.lastShootTime = Time.time + Random.Range(0.02f, 0.1f);
      }

      public void Destroy()
      {
        // ISSUE: method pointer
        this.effectInstance.onEffectFinished += new EffectInstance.EffectFinishEvent((object) this, __methodptr(\u003CDestroy\u003Eb__12_0));
        this.effectInstance.End(false, -1f);
      }
    }
  }
}
