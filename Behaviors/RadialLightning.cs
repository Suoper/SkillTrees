// Decompiled with JetBrains decompiler
// Type: Arcana.Behaviors.RadialLightning
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Misc;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Skill.Spell;
using UnityEngine;

#nullable disable
namespace Arcana.Behaviors
{
  public class RadialLightning : ThunderBehaviour
  {
    public float randomStartDelay = 0.5f;
    public float boltRange = 3f;
    public float boltPeriod = 0.5f;
    public float boltPeriodVariance = 0.25f;
    private SpellCastLightning spellCastLightning;
    private float nextBoltTime;

    public virtual ManagedLoops EnabledManagedLoops => (ManagedLoops) 2;

    public void Prep(
      float boltRange,
      float boltPeriod,
      float boltPeriodVariance,
      float randomStartDelay,
      SpellCastLightning spellCastLightning)
    {
      this.boltRange = boltRange;
      this.boltPeriod = boltPeriod;
      this.boltPeriodVariance = boltPeriodVariance;
      this.randomStartDelay = randomStartDelay;
      this.spellCastLightning = spellCastLightning;
      this.nextBoltTime = Time.time + Random.Range(0.0f, randomStartDelay);
    }

    protected virtual void ManagedUpdate()
    {
      base.ManagedUpdate();
      if (this.spellCastLightning == null || (double) Time.time < (double) this.nextBoltTime)
        return;
      this.nextBoltTime = Time.time + Random.Range(0.0f, this.boltPeriod + this.boltPeriodVariance);
      RadialLightning.BoltHitData hit = this.GetHit(this.transform.position, this.boltRange);
      if ((Object) hit.collider == (Object) null)
        return;
      this.spellCastLightning.Hit(hit.group, hit.closestPoint, hit.hitNormal, hit.direction, 1f, false, (ColliderGroup) null, 1f, (HashSet<ThunderEntity>) null, hit.collider);
      this.spellCastLightning.PlayBolt(this.transform.position, hit.closestPoint);
    }

    private RadialLightning.BoltHitData GetHit(Vector3 origin, float radius)
    {
      Collider[] colliderArray = Physics.OverlapSphere(origin, radius, Utilities.GetProjectileRaycastMask(), (QueryTriggerInteraction) 1);
      float num = float.MaxValue;
      RadialLightning.BoltHitData hit = new RadialLightning.BoltHitData();
      foreach (Collider collider in colliderArray)
      {
        if (!((Object) collider == (Object) null))
        {
          Vector3 centerDirection = ((Component) collider).transform.position - origin;
          RaycastHit raycastHit;
          if (Physics.Raycast(origin, Utilities.GetRandomVelocityInCone(centerDirection, 45f, 1f), ref raycastHit, radius * 2f, Utilities.GetProjectileRaycastMask(), (QueryTriggerInteraction) 1))
          {
            ColliderGroup componentInParent = ((Component) collider).GetComponentInParent<ColliderGroup>();
            if ((!((Object) componentInParent != (Object) null) || !((Object) componentInParent.collisionHandler.item == (Object) null) && !(((CatalogData) componentInParent.collisionHandler.item.data).id == "ArcaneProjectile")) && (double) ((RaycastHit) ref raycastHit).distance <= (double) num)
            {
              num = ((RaycastHit) ref raycastHit).distance;
              hit.collider = collider;
              hit.group = componentInParent;
              hit.closestPoint = ((RaycastHit) ref raycastHit).point;
              hit.hitNormal = ((RaycastHit) ref raycastHit).normal;
              hit.direction = -((RaycastHit) ref raycastHit).normal;
              if ((Object) componentInParent != (Object) null && (componentInParent.isMetal || componentInParent.collisionHandler.isBreakable || (bool) (Object) ((Component) collider).GetComponentInParent<Creature>()))
                break;
            }
          }
        }
      }
      return hit;
    }

    private struct BoltHitData
    {
      public Collider collider;
      public ColliderGroup group;
      public Vector3 closestPoint;
      public Vector3 hitNormal;
      public Vector3 direction;
    }
  }
}
