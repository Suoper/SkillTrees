// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillCrystalStinger
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
  public class SkillCrystalStinger : SpellSkillData
  {
    [ModOption("Creature Throw Velocity Mult", "This is used for NPCs since they move their hands very slowly when throwing spells. The throw velocity is multiplied by this value before spawning.")]
    [ModOptionCategory("Crystal Stinger", 8)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.1f, 100f, 0.5f)]
    public static float creatureVelocityMultiplier = 4f;
    private EffectData projectileCollisionEffectData;
    public string projectileCollisionEffectId;
    private EffectData projectileEffectData;
    public string projectileEffectId;
    private EffectData projectileTrailEffectData;
    public string projectileTrailEffectId;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.projectileCollisionEffectData = Catalog.GetData<EffectData>(this.projectileCollisionEffectId, true);
      this.projectileEffectData = Catalog.GetData<EffectData>(this.projectileEffectId, true);
      this.projectileTrailEffectData = Catalog.GetData<EffectData>(this.projectileTrailEffectId, true);
    }

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      // ISSUE: method pointer
      spellCastCrystallic.OnSpellThrowEvent += new SpellCastCharge.SpellThrowEvent((object) this, __methodptr(OnSpellThrowEvent));
    }

    private void OnSpellThrowEvent(SpellCastCharge spell, Vector3 velocity)
    {
      if (!SpellCastCrystallic.shootStinger)
        return;
      float num = spell.spellCaster.mana.creature.isPlayer ? 1f : SkillCrystalStinger.creatureVelocityMultiplier;
      Stinger.SpawnStinger(this.projectileEffectData, this.projectileTrailEffectData, this.projectileCollisionEffectData, spell.spellCaster.magicSource.transform.position + spell.spellCaster.magicSource.transform.forward * 0.15f, Quaternion.LookRotation(velocity), velocity * num, 10f, spell as SpellCastCrystallic, spell.spellCaster.mana.creature);
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      // ISSUE: method pointer
      spellCastCrystallic.OnSpellThrowEvent -= new SpellCastCharge.SpellThrowEvent((object) this, __methodptr(OnSpellThrowEvent));
    }
  }
}
