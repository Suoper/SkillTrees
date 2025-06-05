// Decompiled with JetBrains decompiler
// Type: Arcana.Golem.GolemArcaneProjectileBeam
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Misc;
using Arcana.Spells;
using System;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Golem
{
  public class GolemArcaneProjectileBeam : GolemArcaneHeadCast
  {
    public int projectileSetCount = 6;
    public float projectileSetTimeBetween = 0.6f;
    public float projectileTimeBetween = 0.1f;
    public bool projectileHoming = false;
    public float projectileHomingRadius = 2f;
    public float projectileforwardOffset = 0.5f;
    public float targetRayConeMinAngle = 0.0f;
    public float targetRayConeMaxAngle = 2f;
    public string projectileEffectId;
    public string sprayEffectId;
    protected EffectData projectileEffectData;
    protected EffectData sprayEffectData;
    private EffectInstance sprayEffectInstance;
    public ProjectileManager projectileManager;
    private ArcaneBolt arcaneBolt;
    private Vector3 guidanceDirection;
    private Transform sprayStart;
    private int rayMask;
    private float lastProjectileCast;

    public override void OnBegin(GolemController golem)
    {
      base.OnBegin(golem);
      if (this.arcaneBolt == null)
        this.arcaneBolt = Catalog.GetData<ArcaneBolt>("Arcane", true);
      if (this.projectileEffectData == null || this.sprayEffectData == null)
      {
        this.projectileEffectData = Catalog.GetData<EffectData>(this.projectileEffectId, true);
        this.sprayEffectData = Catalog.GetData<EffectData>(this.sprayEffectId, true);
      }
      if (this.projectileManager == null)
        this.projectileManager = new ProjectileManager((SpellData) null, (SpellCaster) null, this.arcaneBolt.projectileData, this.projectileEffectData, this.arcaneBolt.projectileDamagerData, this.arcaneBolt.projectileStatusData, (SpellCastCharge) this.arcaneBolt)
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
          doHoming = this.projectileHoming,
          projectileHomingRadius = new float?(this.projectileHomingRadius),
          performRayTargeting = true
        };
      if ((UnityEngine.Object) this.sprayStart == (UnityEngine.Object) null)
        this.sprayStart = new GameObject("HeadSprayStart").transform;
      this.rayMask = Utilities.GetProjectileRaycastMask();
    }

    public override void OnDeployStart()
    {
      base.OnDeployStart();
      this.sprayStart.SetPositionAndRotation(this.golem.eyeTransform.position + this.golem.eyeTransform.forward * this.projectileforwardOffset, Quaternion.LookRotation(this.golem.eyeTransform.forward));
      this.sprayStart.SetParent(this.golem.eyeTransform);
      this.sprayEffectInstance = this.sprayEffectData?.Spawn(this.sprayStart, true, (ColliderGroup) null, false);
      this.sprayEffectInstance?.Play(0, false, false);
      this.lastProjectileCast = 0.0f;
    }

    public override void OnEnd()
    {
      base.OnEnd();
      this.sprayEffectInstance?.End(false, -1f);
      this.sprayEffectInstance = (EffectInstance) null;
    }

    public virtual void OnUpdate()
    {
      base.OnUpdate();
      this.guidanceDirection = this.golem.eyeTransform.forward;
      if (this.state != GolemArcaneHeadCast.State.Firing)
        return;
      this.FireProjectiles();
    }

    public void FireProjectiles()
    {
      if ((double) Time.time - (double) this.lastProjectileCast < (double) this.projectileSetTimeBetween)
        return;
      this.lastProjectileCast = Time.time;
      ((MonoBehaviour) GameManager.local).StartCoroutine(this.projectileManager.SpawnProjectilesCoroutine(this.sprayStart, this.guidanceDirection * this.arcaneBolt.projectileVelocity, (GuidanceMode) 2, new float?(0.35f), this.GetRaycastTarget(this.sprayStart, this.guidanceDirection, this.arcaneBolt.targetRaycastDistance, this.rayMask), new ProjectileManager.ProjectileSpawnEvent(this.projectileManager.ProjectileGuidanceHomingCoroutine), overrideCount: new int?(this.projectileSetCount), overrideTimeBetween: new float?(this.projectileTimeBetween), perProjectileTargetFunc: new Func<Vector3>(this.GetProjectileTarget)));
    }

    private Vector3 GetProjectileTarget()
    {
      return Utilities.GetRandomVelocityInCone(this.guidanceDirection, this.targetRayConeMaxAngle, 1f, this.targetRayConeMinAngle);
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
  }
}
