// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SkillArcaneSentience
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Skills.SpellMerge;
using Arcana.Spells;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Arcana.Skills
{
  internal class SkillArcaneSentience : SpellSkillData
  {
    public float castHomingRadius;
    public float mergeHomingRadius;

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is ArcaneBolt arcaneBolt))
        return;
      arcaneBolt.projectileManager.projectileHomingRadius = new float?(this.castHomingRadius);
    }

    public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnLateSkillsLoaded(skillData, creature);
      SkillArcaneTempest skillArcaneTempest;
      if (!creature.TryGetSkill<SkillArcaneTempest>("Skill_ArcaneTempest", ref skillArcaneTempest))
        return;
      skillArcaneTempest.projectileManager.projectileHomingRadius = new float?(this.mergeHomingRadius);
      Debug.Log((object) "Loaded Tempest Sentience");
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      if (!(spell is ArcaneBolt arcaneBolt))
        return;
      arcaneBolt.projectileManager.projectileHomingRadius = new float?(arcaneBolt.projectileHomingRadius);
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      SkillArcaneTempest skillArcaneTempest;
      if (!creature.TryGetSkill<SkillArcaneTempest>("Skill_ArcaneTempest", ref skillArcaneTempest))
        return;
      skillArcaneTempest.projectileManager.projectileHomingRadius = new float?(skillArcaneTempest.projectileHomingRadius);
    }
  }
}
