// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillCompactShot
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.Skill.Spell;
using ThunderRoad;
using ThunderRoad.Skill;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillCompactShot : SpellSkillData
  {
    [ModOption("Shardshot Angle", "Controls the angle of shardshots with this skill unlocked")]
    [ModOptionCategory("Compact Shot", 4)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 100f, 0.5f)]
    public static float shardshotAngle = 15f;

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      spellCastCrystallic.OnShardshotStart += new SpellCastCrystallic.ShardshotEvent(this.OnShardshotStart);
    }

    private void OnShardshotStart(
      SpellCastCrystallic spellCastCrystallic,
      EffectInstance effectInstance)
    {
      spellCastCrystallic.SetShardshotAngle(SkillCompactShot.shardshotAngle);
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      spellCastCrystallic.SetShardshotAngle(25f);
      spellCastCrystallic.OnShardshotStart -= new SpellCastCrystallic.ShardshotEvent(this.OnShardshotStart);
    }
  }
}
