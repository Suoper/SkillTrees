// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillCrystalSapping
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Skill;
using ThunderRoad.Skill.Spell;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillCrystalSapping : SkillSapStatusApplier
  {
    [ModOption("Crystallise With Other Sapping")]
    [ModOptionCategory("Crystal Sapping", 13)]
    public static bool applyCrystalSapping = true;
    public string mixSpellId;

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is SpellCastLightning spellCastLightning))
        return;
      // ISSUE: method pointer
      spellCastLightning.OnChargeSappingEvent += new SpellCastLightning.ChargeSappingEvent((object) this, __methodptr(OnChargeSappingEvent));
      // ISSUE: method pointer
      spellCastLightning.OnBoltHitColliderGroupEvent += new SpellCastLightning.BoltHitColliderGroupEvent((object) this, __methodptr(OnBoltHitColliderGroupEvent));
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      ((SpellSkillData) this).OnSpellUnload(spell, caster);
      if (!(spell is SpellCastLightning spellCastLightning))
        return;
      // ISSUE: method pointer
      spellCastLightning.OnChargeSappingEvent -= new SpellCastLightning.ChargeSappingEvent((object) this, __methodptr(OnChargeSappingEvent));
      // ISSUE: method pointer
      spellCastLightning.OnBoltHitColliderGroupEvent -= new SpellCastLightning.BoltHitColliderGroupEvent((object) this, __methodptr(OnBoltHitColliderGroupEvent));
    }

    private void OnBoltHitColliderGroupEvent(
      SpellCastLightning spell,
      ColliderGroup colliderGroup,
      Vector3 position,
      Vector3 normal,
      Vector3 velocity,
      float intensity,
      ColliderGroup source,
      HashSet<ThunderEntity> seenEntities)
    {
      if (!SkillCrystalSapping.applyCrystalSapping || !(colliderGroup.collisionHandler.Entity is Creature entity) || !((Object) entity != (Object) ((SpellCastCharge) spell).spellCaster.mana.creature) || entity.factionId == ((SpellCastCharge) spell).spellCaster.mana.creature.factionId || !(this.mixSpellId != "Lightning") || this.mixSpellId == null)
        return;
      BrainModuleCrystal module = entity.brain.instance.GetModule<BrainModuleCrystal>(true);
      module.Crystallise(5f);
      module.SetColor(Dye.GetEvaluatedColor("Lightning", this.mixSpellId), this.mixSpellId);
    }

    private void OnChargeSappingEvent(
      SpellCastLightning spell,
      SkillChargeSapping skill,
      EventTime time,
      SpellCastCharge other)
    {
      if (time != 1)
        return;
      this.mixSpellId = ((CatalogData) other).id;
    }
  }
}
