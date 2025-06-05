// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillCrystallisation
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using Crystallic.Skill.Spell;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillCrystallisation : SpellSkillData
  {
    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      spellCastCrystallic.OnShardHit += new SpellCastCrystallic.ShardshotHitEvent(this.OnShardHit);
    }

    private void OnShardHit(
      SpellCastCrystallic spellCastCrystallic,
      ThunderEntity entity,
      SpellCastCrystallic.ShardshotHit hitInfo)
    {
      if (!(entity is Creature creature) || !(bool) (Object) hitInfo.hitPart || hitInfo.hitPart.hasMetalArmor || creature.isPlayer)
        return;
      BrainModuleCrystal module = creature.brain.instance.GetModule<BrainModuleCrystal>(true);
      module.Crystallise(5f, "Crystallic");
      module.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, "Crystallic"), "Crystallic");
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      spellCastCrystallic.OnShardHit -= new SpellCastCrystallic.ShardshotHitEvent(this.OnShardHit);
    }
  }
}
