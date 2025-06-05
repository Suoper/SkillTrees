// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SkillArcaneSpellPunch
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using ThunderRoad;
using ThunderRoad.Skill.Spell;
using UnityEngine;

#nullable disable
namespace Arcana.Skills
{
  public class SkillArcaneSpellPunch : SkillSpellPunch
  {
    public virtual void OnFist(PlayerHand hand, bool gripping)
    {
      SpellCaster caster = hand?.ragdollHand?.caster;
      SpellCastCharge spellInstance;
      int num;
      if (!((Object) caster == (Object) null))
      {
        spellInstance = caster?.spellInstance as SpellCastCharge;
        num = spellInstance == null ? 1 : 0;
      }
      else
        num = 1;
      if (num != 0)
        return;
      if (!spellInstance.Ready)
      {
        base.OnFist(hand, gripping);
      }
      else
      {
        if (gripping)
          return;
        base.OnFist(hand, gripping);
      }
    }
  }
}
