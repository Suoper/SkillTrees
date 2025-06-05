// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillShardDilation
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.Skill.Spell;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillShardDilation : SpellSkillData
  {
    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      spellCastCrystallic.speedUpByTimeScale = true;
      spellCastCrystallic.OnShardHit += new SpellCastCrystallic.ShardshotHitEvent(this.OnShardHit);
    }

    private void OnShardHit(
      SpellCastCrystallic spellCastCrystallic,
      ThunderEntity entity,
      SpellCastCrystallic.ShardshotHit hitInfo)
    {
      if (!((Object) hitInfo.hitEntity != (Object) null) || !(hitInfo.hitEntity is Creature hitEntity) || !((Object) hitEntity != (Object) spellCastCrystallic.spellCaster.mana.creature) || !SkillSlowTimeData.timeSlowed)
        return;
      ((ThunderEntity) hitEntity).Inflict("Slowed", (object) this, 5f, (object) null, true);
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      spellCastCrystallic.speedUpByTimeScale = false;
      spellCastCrystallic.OnShardHit -= new SpellCastCrystallic.ShardshotHitEvent(this.OnShardHit);
    }
  }
}
