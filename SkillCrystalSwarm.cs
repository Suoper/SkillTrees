// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillCrystalSwarm
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.Skill.Spell;
using System;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillCrystalSwarm : SpellSkillData
  {
    public EffectData projectileCollisionEffectData;
    public string projectileCollisionEffectId;
    public EffectData projectileEffectData;
    public string projectileEffectId;
    public EffectData projectileTrailEffectData;
    public string projectileTrailEffectId;
    public EffectData pulseEffectData;
    public string pulseEffectId;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.projectileCollisionEffectData = Catalog.GetData<EffectData>(this.projectileCollisionEffectId, true);
      this.projectileEffectData = Catalog.GetData<EffectData>(this.projectileEffectId, true);
      this.projectileTrailEffectData = Catalog.GetData<EffectData>(this.projectileTrailEffectId, true);
      this.pulseEffectData = Catalog.GetData<EffectData>(this.pulseEffectId, true);
    }

    public virtual void OnImbueLoad(SpellData spell, Imbue imbue)
    {
      base.OnImbueLoad(spell, imbue);
      if (!(spell is SpellCastCrystallic spellCastCrystallic) || imbue.colliderGroup.modifier.imbueType != 3)
        return;
      ((ThunderEntity) imbue.colliderGroup.collisionHandler.item).SetVariable<bool>("CanShoot", true);
      // ISSUE: method pointer
      spellCastCrystallic.OnCrystalUseEvent -= new SpellCastCharge.CrystalUseEvent((object) this, __methodptr(OnCrystalUse));
      // ISSUE: method pointer
      spellCastCrystallic.OnCrystalUseEvent += new SpellCastCharge.CrystalUseEvent((object) this, __methodptr(OnCrystalUse));
    }

    public virtual void OnImbueUnload(SpellData spell, Imbue imbue)
    {
      base.OnImbueUnload(spell, imbue);
      if (!(spell is SpellCastCrystallic spellCastCrystallic) || imbue.colliderGroup.modifier.imbueType != 3)
        return;
      ((ThunderEntity) imbue.colliderGroup.collisionHandler.item).SetVariable<bool>("CanShoot", true);
      // ISSUE: method pointer
      spellCastCrystallic.OnCrystalUseEvent -= new SpellCastCharge.CrystalUseEvent((object) this, __methodptr(OnCrystalUse));
    }

    public void OnCrystalUse(SpellCastCharge spell, Imbue imbue, RagdollHand hand, bool active)
    {
      bool variable = ((ThunderEntity) imbue.colliderGroup.collisionHandler.item).GetVariable<bool>("CanShoot");
      if (!(spell is SpellCastCrystallic) || !active || !variable)
        return;
      ((ThunderEntity) imbue.colliderGroup.collisionHandler.item).SetVariable<bool>("CanShoot", false);
      Utils.RunAfter((MonoBehaviour) imbue, (Action) (() => ((ThunderEntity) imbue.colliderGroup.collisionHandler.item).SetVariable<bool>("CanShoot", true)), 0.55f, false);
      bool forceReleaseOnSpawn = Player.currentCreature.HasSkill("OverchargedCore");
      SpellCastCrystallic spellCastCrystallic = (SpellCastCrystallic) spell;
      if (forceReleaseOnSpawn)
        SkillHyperintensity.ForceInvokeOvercharged(spellCastCrystallic);
      hand.PlayHapticClipOver(spellCastCrystallic.pulseCurve, 0.25f);
      this.pulseEffectData.Spawn(imbue.colliderGroup.imbueShoot.transform.position + imbue.colliderGroup.imbueShoot.transform.forward * 0.15f, Quaternion.LookRotation(imbue.colliderGroup.imbueShoot.transform.forward), (Transform) null, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>()).Play(0, false, false);
      Stinger.SpawnStinger(this.projectileEffectData, this.projectileTrailEffectData, this.projectileCollisionEffectData, imbue.colliderGroup.imbueShoot.transform.position + imbue.colliderGroup.imbueShoot.transform.forward * 0.15f, Quaternion.LookRotation(imbue.colliderGroup.imbueShoot.transform.forward), imbue.colliderGroup.imbueShoot.transform.forward * 4.25f, 10f, spell as SpellCastCrystallic, forceReleaseOnSpawn: forceReleaseOnSpawn);
    }
  }
}
