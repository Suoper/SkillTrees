// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.Barrier.SkillArcaneFireBarrier
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Misc;
using System;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Skill;
using ThunderRoad.Skill.Spell;
using UnityEngine;

#nullable disable
namespace Arcana.Skills.Barrier
{
  internal class SkillArcaneFireBarrier : SpellSkillData
  {
    public string arcaneBarrierSkillId;
    public string barrierLayerEffectId;
    public EffectData barrierLayerEffectData;
    private SkillHeatwave heatWaveSkill;
    private float barrierHitCooldown = 1f;
    private float lastBarrierHitTime = float.MinValue;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.barrierLayerEffectData = Catalog.GetData<EffectData>(this.barrierLayerEffectId, true);
      this.heatWaveSkill = Catalog.GetData<SkillHeatwave>("Heatwave", true);
    }

    public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnLateSkillsLoaded(skillData, creature);
      SkillArcaneBarrier skillArcaneBarrier;
      if (!creature.TryGetSkill<SkillArcaneBarrier>(this.arcaneBarrierSkillId, ref skillArcaneBarrier))
        return;
      skillArcaneBarrier.OnBarrierStartEvent -= new SkillArcaneBarrier.BarrierEvent(this.OnBarrierStartEvent);
      skillArcaneBarrier.OnBarrierStartEvent += new SkillArcaneBarrier.BarrierEvent(this.OnBarrierStartEvent);
      skillArcaneBarrier.OnBarrierHitEvent -= new SkillArcaneBarrier.BarrierCollisionEvent(this.OnBarrierHitEvent);
      skillArcaneBarrier.OnBarrierHitEvent += new SkillArcaneBarrier.BarrierCollisionEvent(this.OnBarrierHitEvent);
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      SkillArcaneBarrier skillArcaneBarrier;
      if (!creature.TryGetSkill<SkillArcaneBarrier>(this.arcaneBarrierSkillId, ref skillArcaneBarrier))
        return;
      skillArcaneBarrier.OnBarrierStartEvent -= new SkillArcaneBarrier.BarrierEvent(this.OnBarrierStartEvent);
      skillArcaneBarrier.OnBarrierHitEvent -= new SkillArcaneBarrier.BarrierCollisionEvent(this.OnBarrierHitEvent);
    }

    public void OnBarrierStartEvent(SpellCastCharge spell, SkillArcaneBarrier.Barrier barrier)
    {
      barrier.TryStartEffect(this.barrierLayerEffectData);
    }

    public void OnBarrierHitEvent(
      SpellCastCharge spell,
      SkillArcaneBarrier.Barrier barrier,
      CollisionInstance collision)
    {
      if ((double) Time.time - (double) this.lastBarrierHitTime < (double) this.barrierHitCooldown || this.heatWaveSkill == null)
        return;
      Item obj = collision.sourceColliderGroup?.collisionHandler?.item;
      ColliderGroup sourceColliderGroup1 = collision.sourceColliderGroup;
      bool? nullable1;
      int num1;
      if (sourceColliderGroup1 == null)
      {
        num1 = 1;
      }
      else
      {
        nullable1 = sourceColliderGroup1.collisionHandler?.isItem;
        num1 = !nullable1.GetValueOrDefault() ? 1 : 0;
      }
      int num2;
      if (num1 == 0 && (double) collision.impactVelocity.magnitude >= 3.0)
      {
        if (obj == null)
        {
          num2 = 0;
        }
        else
        {
          RagdollHand lastHandler = obj.lastHandler;
          bool? nullable2;
          if (lastHandler == null)
          {
            nullable1 = new bool?();
            nullable2 = nullable1;
          }
          else
          {
            Creature creature = lastHandler.creature;
            if (creature == null)
            {
              nullable1 = new bool?();
              nullable2 = nullable1;
            }
            else
              nullable2 = new bool?(creature.isPlayer);
          }
          nullable1 = nullable2;
          num2 = nullable1.GetValueOrDefault() ? 1 : 0;
        }
      }
      else
        num2 = 1;
      if (num2 != 0 | (((CatalogData) obj?.data).id == "DynamicProjectile" || ((CatalogData) obj?.data).id == "ArcaneProjectile" || obj?.data.slot == "Arrow"))
        return;
      this.lastBarrierHitTime = Time.time;
      Vector3 contactPoint = collision.contactPoint;
      ColliderGroup sourceColliderGroup2 = collision.sourceColliderGroup;
      Vector3? nullable3;
      if (sourceColliderGroup2 == null)
      {
        nullable3 = new Vector3?();
      }
      else
      {
        Vector3? position = ((ThunderBehaviour) sourceColliderGroup2.collisionHandler?.item?.mainHandler?.creature?.ragdoll.targetPart).transform.position;
        Vector3 vector3 = contactPoint;
        nullable3 = position.HasValue ? new Vector3?(position.GetValueOrDefault() - vector3) : new Vector3?();
      }
      Vector3 forward = nullable3 ?? collision.contactNormal;
      this.heatWaveSkill.effectData?.Spawn(contactPoint, Quaternion.LookRotation(forward), (Transform) null, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>())?.Play(0, false, false);
      foreach (ThunderEntity inRadiu in ThunderEntity.InRadius(contactPoint + forward.normalized * this.heatWaveSkill.radius, this.heatWaveSkill.radius, Filter.AllBut((ThunderEntity) barrier.creature), (List<ThunderEntity>) null))
      {
        if (inRadiu is Creature creature)
        {
          creature.DamagePatched(this.heatWaveSkill.damage, (DamageType) 4);
          if (!((ThunderEntity) creature).IsBurning)
            creature.TryPush((Creature.PushType) 0, forward, this.heatWaveSkill.pushLevel, (RagdollPart.Type) 4);
          ((ThunderEntity) creature).Inflict(this.heatWaveSkill.status, (object) this, float.PositiveInfinity, (object) this.heatWaveSkill.heat, true);
        }
        inRadiu.AddForce(forward.normalized * (Mathf.InverseLerp(this.heatWaveSkill.radius * 2f, 0.0f, (inRadiu.Center - contactPoint).magnitude) * this.heatWaveSkill.force), (ForceMode) 2, (CollisionHandler) null);
      }
    }
  }
}
