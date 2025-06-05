// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.Barrier.SkillArcaneLockdown
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Arcana.Skills.Barrier
{
  internal class SkillArcaneLockdown : SpellSkillData
  {
    public string arcaneBarrierSkillId;
    public float duration = 5f;

    public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnLateSkillsLoaded(skillData, creature);
      SkillArcaneBarrier skillArcaneBarrier;
      if (!creature.TryGetSkill<SkillArcaneBarrier>(this.arcaneBarrierSkillId, ref skillArcaneBarrier))
        return;
      skillArcaneBarrier.OnBarrierGripEvent -= new SkillArcaneBarrier.BarrierGripEvent(this.OnBarrierGripEvent);
      skillArcaneBarrier.OnBarrierGripEvent += new SkillArcaneBarrier.BarrierGripEvent(this.OnBarrierGripEvent);
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      SkillArcaneBarrier skillArcaneBarrier;
      if (!creature.TryGetSkill<SkillArcaneBarrier>(this.arcaneBarrierSkillId, ref skillArcaneBarrier))
        return;
      skillArcaneBarrier.OnBarrierGripEvent -= new SkillArcaneBarrier.BarrierGripEvent(this.OnBarrierGripEvent);
    }

    public void OnBarrierGripEvent(SkillArcaneBarrier.Barrier barrier)
    {
      ((MonoBehaviour) GameManager.local).StartCoroutine(barrier.Despawn(this.duration));
      barrier.skill.barriers.Remove(barrier);
    }
  }
}
