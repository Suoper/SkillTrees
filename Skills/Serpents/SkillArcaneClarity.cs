// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.Serpents.SkillArcaneClarity
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Behaviors;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Arcana.Skills.Serpents
{
  public class SkillArcaneClarity : SpellSkillData
  {
    public string serpentSkillId = "Skill_ArcaneSerpents";
    public float focusGainPercentPerHit = 0.1f;

    public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnLateSkillsLoaded(skillData, creature);
      SkillArcaneSerpents skillArcaneSerpents;
      if (!creature.TryGetSkill<SkillArcaneSerpents>(this.serpentSkillId, ref skillArcaneSerpents))
        return;
      skillArcaneSerpents.OnSerpentListChange -= new SkillArcaneSerpents.SerpentListChange(this.OnSerpentListChange);
      skillArcaneSerpents.OnSerpentListChange += new SkillArcaneSerpents.SerpentListChange(this.OnSerpentListChange);
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      SkillArcaneSerpents skillArcaneSerpents;
      if (!creature.TryGetSkill<SkillArcaneSerpents>(this.serpentSkillId, ref skillArcaneSerpents))
        return;
      skillArcaneSerpents.OnSerpentListChange -= new SkillArcaneSerpents.SerpentListChange(this.OnSerpentListChange);
    }

    private void OnCreatureHit(Serpent serpent, Creature creature)
    {
      if ((Object) serpent.ignoredCreature != (Object) Player.local.creature)
        return;
      Mana mana = Player.local.creature.mana;
      mana.currentFocus = Mathf.Clamp(mana.currentFocus + this.focusGainPercentPerHit * (mana.MaxFocus - mana.minFocus), mana.minFocus, mana.MaxFocus);
    }

    private void OnSerpentListChange(Serpent serpent, bool alive)
    {
      serpent.OnCreatureHitEvent -= new Serpent.OnCreatureHit(this.OnCreatureHit);
      serpent.OnCreatureHitEvent += new Serpent.OnCreatureHit(this.OnCreatureHit);
    }
  }
}
