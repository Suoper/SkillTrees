// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillCrystalImbuement
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.Skill.Spell;
using ThunderRoad;
using ThunderRoad.Skill;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillCrystalImbuement : SpellSkillData
  {
    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      spellCastCrystallic.imbueEnabled = true;
      spellCastCrystallic.spellCaster.imbueTrigger.SetRadius(0.2f);
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      spellCastCrystallic.imbueEnabled = false;
      spellCastCrystallic.spellCaster.imbueTrigger.SetRadius(0.2f);
    }
  }
}
