// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SpellMerge.SkillThunderbond
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Behaviors;
using Arcana.Misc;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Skills.SpellMerge
{
  public class SkillThunderbond : SpellMergeData
  {
    public float spellHandSeparationMaxAngle = 45f;
    public float stormDuration = 10f;
    public float stormDamage = 10f;
    public List<SpellStatusImbueable> stormStatuses;
    public float stormRadius = 32f;
    public float stormEyeRadius = 9.5f;
    public float stormImbueAmount = 40f;
    public bool stormTriggerInstabilityExplosion = false;
    public string stormEffectId;
    public EffectData stormEffectData;
    public float dragonRadius = 23.25f;
    public float dragonSpeed = 45f;
    public float dragonSinAmplitude = 2f;
    public float dragonSinFrequency = 2f;
    public LightningDragon.DragonData dragonData;
    public float lightningStrikePeriod = 0.75f;
    public float lightningStrikeVerticalOffset = 25f;
    public float lightningStrikeVerticalOffsetVariance = 2f;
    public string mainBoltEffectId;
    public EffectData mainBoltEffectData;
    public string impactEffectId;
    public EffectData impactEffectData;
    public Gradient defaultBoltGradient;

    public event SkillThunderbond.ThunderbondEvent OnThunderbondStartEvent;

    public event SkillThunderbond.ThunderbondEvent OnThunderbondEndEvent;

    public virtual bool CanMerge() => !DragonStorm.active;

    public void OnStormStart(DragonStorm storm)
    {
      SkillThunderbond.ThunderbondEvent thunderbondStartEvent = this.OnThunderbondStartEvent;
      if (thunderbondStartEvent != null)
        thunderbondStartEvent(this, storm);
      storm.OnStormStartEvent -= new DragonStorm.StormEvent(this.OnStormStart);
    }

    public void OnStormEnd(DragonStorm storm)
    {
      SkillThunderbond.ThunderbondEvent thunderbondEndEvent = this.OnThunderbondEndEvent;
      if (thunderbondEndEvent != null)
        thunderbondEndEvent(this, storm);
      storm.OnStormEndEvent -= new DragonStorm.StormEvent(this.OnStormEnd);
    }

    public virtual void OnCatalogRefresh()
    {
      base.OnCatalogRefresh();
      this.stormEffectData = Catalog.GetData<EffectData>(this.stormEffectId, true);
      this.mainBoltEffectData = Catalog.GetData<EffectData>(this.mainBoltEffectId, true);
      this.impactEffectData = Catalog.GetData<EffectData>(this.impactEffectId, true);
      this.dragonData.LoadCatalogData();
      foreach (SpellStatus stormStatuse in this.stormStatuses)
        stormStatuse.LoadCatalogData();
    }

    public virtual void Merge(bool active)
    {
      base.Merge(active);
      PlayerHand handLeft = Player.local.handLeft;
      PlayerHand handRight = Player.local.handRight;
      Vector3 from1 = ((ThunderBehaviour) Player.local).transform.rotation * handLeft.controlHand.GetHandVelocity();
      Vector3 from2 = ((ThunderBehaviour) Player.local).transform.rotation * handRight.controlHand.GetHandVelocity();
      if (active || (double) from1.magnitude < (double) SpellCaster.throwMinHandVelocity || (double) from2.magnitude < (double) SpellCaster.throwMinHandVelocity || (double) Vector3.Angle(from1, ((ThunderBehaviour) handLeft).transform.position - this.mana.mergePoint.position) > (double) this.spellHandSeparationMaxAngle && (double) Vector3.Angle(from2, ((ThunderBehaviour) handRight).transform.position - this.mana.mergePoint.position) > (double) this.spellHandSeparationMaxAngle || (double) this.currentCharge < (double) this.minCharge || DragonStorm.active)
        return;
      DragonStorm dragonStorm = Utilities.GetTransformCopy(this.mana.mergePoint.transform).gameObject.AddComponent<DragonStorm>();
      dragonStorm.transform.SetParent((Transform) null);
      dragonStorm.OnStormStartEvent -= new DragonStorm.StormEvent(this.OnStormStart);
      dragonStorm.OnStormStartEvent += new DragonStorm.StormEvent(this.OnStormStart);
      dragonStorm.OnStormEndEvent -= new DragonStorm.StormEvent(this.OnStormEnd);
      dragonStorm.OnStormEndEvent += new DragonStorm.StormEvent(this.OnStormEnd);
      dragonStorm.Form(this);
    }

    public delegate void ThunderbondEvent(SkillThunderbond skill, DragonStorm storm);
  }
}
