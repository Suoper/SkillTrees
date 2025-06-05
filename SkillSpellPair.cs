// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillSpellPair
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using ThunderRoad;

#nullable disable
namespace Crystallic.Skill
{
  [Serializable]
  public class SkillSpellPair
  {
    public string spellId;
    public string skillId;

    public virtual bool HasSkill(Creature creature, bool skipNullConditions = true)
    {
      bool flag = creature.HasSkill(this.skillId);
      return skipNullConditions ? flag || string.IsNullOrEmpty(this.skillId) : flag;
    }

    public virtual bool IsSpell(SpellData spellData, bool skipNullConditions = true)
    {
      bool flag = ((CatalogData) spellData).id == this.spellId;
      return skipNullConditions ? flag || string.IsNullOrEmpty(this.spellId) : flag;
    }

    public virtual bool IsValid(Creature creature, SpellData spellData, bool skipNullConditions = true)
    {
      return this.IsSpell(spellData, skipNullConditions) & this.HasSkill(creature, skipNullConditions);
    }
  }
}
