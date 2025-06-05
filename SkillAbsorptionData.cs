// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillAbsorptionData
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using Crystallic.Skill.Spell;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillAbsorptionData : SpellSkillData
  {
    [ModOption("Min Absorption Charge", "The minimum charge a spell has to be to start an absorption.")]
    [ModOptionCategory("Absorption", 21)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.1f, 1f, 0.1f)]
    public static float absorptionMinCharge = 1f;
    public Dictionary<SpellCastCharge, Coroutine> chargingSpells = new Dictionary<SpellCastCharge, Coroutine>();
    public string spellId;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.chargingSpells.Clear();
    }

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is SpellCastCharge spellCastCharge1))
        return;
      SpellCastCharge spellCastCharge2 = spellCastCharge1;
      SkillAbsorptionData skillAbsorptionData1 = this;
      // ISSUE: virtual method pointer
      SpellCastCharge.SpellEvent spellEvent1 = new SpellCastCharge.SpellEvent((object) skillAbsorptionData1, __vmethodptr(skillAbsorptionData1, OnSpellCastEvent));
      spellCastCharge2.OnSpellCastEvent += spellEvent1;
      SpellCastCharge spellCastCharge3 = spellCastCharge1;
      SkillAbsorptionData skillAbsorptionData2 = this;
      // ISSUE: virtual method pointer
      SpellCastCharge.SpellEvent spellEvent2 = new SpellCastCharge.SpellEvent((object) skillAbsorptionData2, __vmethodptr(skillAbsorptionData2, OnSpellStopEvent));
      spellCastCharge3.OnSpellStopEvent += spellEvent2;
    }

    protected virtual void OnSprayStartWhileAbsorbing(SpellCastCrystallic spellCastCrystallic)
    {
    }

    protected virtual void OnSprayLoopWhileAbsorbing(SpellCastCrystallic spellCastCrystallic)
    {
    }

    private void OnSprayEndWhileAbsorbing(SpellCastCrystallic spellCastCrystallic)
    {
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      if (!(spell is SpellCastCharge spellCastCharge1))
        return;
      SpellCastCharge spellCastCharge2 = spellCastCharge1;
      SkillAbsorptionData skillAbsorptionData1 = this;
      // ISSUE: virtual method pointer
      SpellCastCharge.SpellEvent spellEvent1 = new SpellCastCharge.SpellEvent((object) skillAbsorptionData1, __vmethodptr(skillAbsorptionData1, OnSpellCastEvent));
      spellCastCharge2.OnSpellCastEvent -= spellEvent1;
      SpellCastCharge spellCastCharge3 = spellCastCharge1;
      SkillAbsorptionData skillAbsorptionData2 = this;
      // ISSUE: virtual method pointer
      SpellCastCharge.SpellEvent spellEvent2 = new SpellCastCharge.SpellEvent((object) skillAbsorptionData2, __vmethodptr(skillAbsorptionData2, OnSpellStopEvent));
      spellCastCharge3.OnSpellStopEvent -= spellEvent2;
    }

    protected virtual void OnShardshotStartWhileAbsorbing(
      SpellCastCrystallic spellCastCrystallic,
      EffectInstance effectInstance)
    {
      spellCastCrystallic.OnShardHit += new SpellCastCrystallic.ShardshotHitEvent(this.OnShardshotHitWhileAbsorbing);
    }

    protected virtual void OnShardshotHitWhileAbsorbing(
      SpellCastCrystallic spellCastCrystallic,
      ThunderEntity entity,
      SpellCastCrystallic.ShardshotHit hitInfo)
    {
    }

    protected virtual void OnShardshotEndWhileAbsorbing(
      SpellCastCrystallic spellCastCrystallic,
      EffectInstance effectInstance)
    {
      spellCastCrystallic.OnShardHit -= new SpellCastCrystallic.ShardshotHitEvent(this.OnShardshotHitWhileAbsorbing);
    }

    public IEnumerator ChargeRoutine(SpellCastCharge spellCastCharge)
    {
      while (!Mathf.Approximately(spellCastCharge.currentCharge, SkillAbsorptionData.absorptionMinCharge))
        yield return (object) null;
      this.Absorb(spellCastCharge);
    }

    public void Absorb(SpellCastCharge spell)
    {
      if (spell == null || !((Object) spell.spellCaster != (Object) null) || !spell.spellCaster.isFiring || ((CatalogData) spell)?.id == "Crystallic" && (Object) spell?.spellCaster?.other != (Object) null && !string.IsNullOrEmpty(((CatalogData) spell?.spellCaster?.other?.spellInstance)?.id) && ((CatalogData) spell.spellCaster?.other?.spellInstance)?.id == "Crystallic")
        return;
      this.PlayAbsorbEffect(spell);
      this.chargingSpells.Remove(spell);
      if (((CatalogData) spell).id == this.spellId)
      {
        Stinger.onStingerSpawn += new Stinger.StingerEvent(this.OnStingerSpawnWithSpell);
        if (spell.spellCaster.other.isFiring && ((CatalogData) spell.spellCaster.other.spellInstance).id == "Crystallic")
          this.DyeSpell(spell.spellCaster.other.spellInstance as SpellCastCrystallic, true);
      }
      else if (((CatalogData) spell).id == "Crystallic" && spell.spellCaster.other.isFiring && ((CatalogData) spell.spellCaster.other.spellInstance).id == this.spellId)
        this.DyeSpell(spell as SpellCastCrystallic, true);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      spellCastCrystallic.onSprayStart += new SpellCastCrystallic.SprayEvent(this.OnSprayStartWhileAbsorbing);
      spellCastCrystallic.onSprayLoop += new SpellCastCrystallic.SprayEvent(this.OnSprayLoopWhileAbsorbing);
      spellCastCrystallic.onSprayEnd += new SpellCastCrystallic.SprayEvent(this.OnSprayEndWhileAbsorbing);
      spellCastCrystallic.OnShardshotStart += new SpellCastCrystallic.ShardshotEvent(this.OnShardshotStartWhileAbsorbing);
      spellCastCrystallic.OnShardshotEnd += new SpellCastCrystallic.ShardshotEvent(this.OnShardshotEndWhileAbsorbing);
    }

    protected virtual void OnSpellCastEvent(SpellCastCharge spell)
    {
      if (this.chargingSpells.ContainsKey(spell))
        return;
      this.chargingSpells.Add(spell, ((MonoBehaviour) GameManager.local).StartCoroutine(this.ChargeRoutine(spell)));
    }

    protected virtual void OnSpellStopEvent(SpellCastCharge spell)
    {
      Coroutine routine;
      if (this.chargingSpells.TryGetValue(spell, out routine) && routine != null)
      {
        ((MonoBehaviour) GameManager.local).StopCoroutine(routine);
        this.chargingSpells.Remove(spell);
      }
      if (((CatalogData) spell).id == this.spellId)
      {
        Stinger.onStingerSpawn -= new Stinger.StingerEvent(this.OnStingerSpawnWithSpell);
        if (spell.spellCaster.other.isFiring && ((CatalogData) spell.spellCaster.other.spellInstance).id == "Crystallic")
          this.DyeSpell(spell.spellCaster.other.spellInstance as SpellCastCrystallic, false);
      }
      else if (((CatalogData) spell).id == "Crystallic")
        this.DyeSpell(spell as SpellCastCrystallic, false);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      spellCastCrystallic.onSprayStart -= new SpellCastCrystallic.SprayEvent(this.OnSprayStartWhileAbsorbing);
      spellCastCrystallic.onSprayLoop -= new SpellCastCrystallic.SprayEvent(this.OnSprayLoopWhileAbsorbing);
      spellCastCrystallic.onSprayEnd -= new SpellCastCrystallic.SprayEvent(this.OnSprayEndWhileAbsorbing);
      spellCastCrystallic.OnShardshotStart -= new SpellCastCrystallic.ShardshotEvent(this.OnShardshotStartWhileAbsorbing);
      spellCastCrystallic.OnShardshotEnd -= new SpellCastCrystallic.ShardshotEvent(this.OnShardshotEndWhileAbsorbing);
    }

    protected virtual void OnStingerSpawnWithSpell(Stinger stinger)
    {
      stinger.SetColor(Dye.GetEvaluatedColor(this.spellId, this.spellId), this.spellId, 0.01f);
      stinger.onStingerStab += new Stinger.OnStingerStab(this.OnStingerStab);
    }

    protected virtual void OnStingerStab(
      Stinger stinger,
      Damager damager,
      CollisionInstance collisionInstance,
      Creature hitCreature)
    {
      stinger.onStingerStab -= new Stinger.OnStingerStab(this.OnStingerStab);
      if (!(bool) (Object) hitCreature)
        return;
      BrainModuleCrystal module = hitCreature.brain.instance.GetModule<BrainModuleCrystal>(true);
      module.Crystallise(5f);
      module.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, this.spellId), this.spellId);
    }

    public virtual void PlayAbsorbEffect(SpellCastCharge main)
    {
      if (!main.spellCaster.isFiring || !main.spellCaster.other.isFiring)
        return;
      if (((CatalogData) main).id == "Crystallic")
      {
        if (main.spellCaster.other.spellInstance is SpellCastCharge spellInstance1)
          spellInstance1.readyEffectData.Spawn(main.spellCaster.Orb, true, (ColliderGroup) null, false).Play(0, false, false);
      }
      else if (((CatalogData) main.spellCaster.other.spellInstance).id == "Crystallic")
      {
        SpellCastCharge spellInstance2 = main.spellCaster.other.spellInstance as SpellCastCharge;
        main.readyEffectData.Spawn(spellInstance2?.spellCaster.Orb, true, (ColliderGroup) null, false).Play(0, false, false);
      }
    }

    public virtual void DyeSpell(SpellCastCrystallic spellCastCrystallic, bool mix)
    {
      if (spellCastCrystallic == null)
        return;
      if (mix)
        spellCastCrystallic?.SetColor(Dye.GetEvaluatedColor(this.spellId, this.spellId), this.spellId, 0.05f);
      else
        spellCastCrystallic.SetColor(Dye.GetEvaluatedColor("Crystallic", "Crystallic"), "Crystallic", 0.05f);
    }
  }
}
