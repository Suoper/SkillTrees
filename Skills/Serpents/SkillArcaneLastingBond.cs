// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.Serpents.SkillArcaneLastingBond
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Behaviors;
using ThunderRoad;
using ThunderRoad.Skill;

#nullable disable
namespace Arcana.Skills.Serpents
{
  public class SkillArcaneLastingBond : SpellSkillData
  {
    public string serpentSkillId = "Skill_ArcaneSerpents";
    public int maxChain = 4;
    public bool resetOnAttackFinish = true;
    public bool requireUseOnMax = false;

    public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnLateSkillsLoaded(skillData, creature);
      SkillArcaneSerpents skillArcaneSerpents;
      if (!creature.TryGetSkill<SkillArcaneSerpents>(this.serpentSkillId, ref skillArcaneSerpents))
        return;
      skillArcaneSerpents.OnSerpentListChange -= new SkillArcaneSerpents.SerpentListChange(this.OnSerpentListChange);
      skillArcaneSerpents.OnSerpentListChange += new SkillArcaneSerpents.SerpentListChange(this.OnSerpentListChange);
      skillArcaneSerpents.requireUseOnMax = this.requireUseOnMax;
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      SkillArcaneSerpents skillArcaneSerpents;
      if (!creature.TryGetSkill<SkillArcaneSerpents>(this.serpentSkillId, ref skillArcaneSerpents))
        return;
      skillArcaneSerpents.OnSerpentListChange -= new SkillArcaneSerpents.SerpentListChange(this.OnSerpentListChange);
      skillArcaneSerpents.requireUseOnMax = true;
    }

    private void OnSerpentListChange(Serpent serpent, bool alive)
    {
      serpent.data.maxChain = this.maxChain;
      serpent.data.resetOnAttackFinish = this.resetOnAttackFinish;
    }
  }
}
