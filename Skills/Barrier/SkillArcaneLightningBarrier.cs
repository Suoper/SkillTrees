// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.Barrier.SkillArcaneLightningBarrier
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Arcana.Skills.Barrier
{
  internal class SkillArcaneLightningBarrier : SpellSkillData
  {
    public string arcaneBarrierSkillId;
    public string barrierLayerEffectId;
    public EffectData barrierLayerEffectData;
    private float barrierHitCooldown = 1f;
    private float lastBarrierHitTime = float.MinValue;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.barrierLayerEffectData = Catalog.GetData<EffectData>(this.barrierLayerEffectId, true);
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
      if ((double) Time.time - (double) this.lastBarrierHitTime < (double) this.barrierHitCooldown)
        return;
      Item obj = collision.sourceColliderGroup?.collisionHandler?.item;
      ColliderGroup sourceColliderGroup = collision.sourceColliderGroup;
      bool? nullable1;
      int num1;
      if (sourceColliderGroup == null)
      {
        num1 = 1;
      }
      else
      {
        nullable1 = sourceColliderGroup.collisionHandler?.isItem;
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
      Creature creature1 = collision.sourceColliderGroup?.collisionHandler?.item?.lastHandler?.creature;
      if ((Object) creature1 == (Object) null || creature1.isPlayer || (Object) creature1 == (Object) barrier.creature)
        return;
      this.lastBarrierHitTime = Time.time;
      ((ThunderEntity) creature1).Inflict("Electrocute", (object) this, 0.75f, (object) null, true);
    }
  }
}
