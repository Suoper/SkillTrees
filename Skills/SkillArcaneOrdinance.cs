// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SkillArcaneOrdinance
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Misc;
using Arcana.Spells;
using System.Collections;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Arcana.Skills
{
  internal class SkillArcaneOrdinance : SpellSkillData
  {
    public int beamCount = 5;
    public float forwardOffset = 0.5f;
    public float imbueConsumption = 5f;
    public float castCooldown = 0.5f;
    private bool beamActive = false;
    private float lastCastTime;
    public Imbue currentImbue;
    public BeamManager.BeamOrigin beamType = BeamManager.BeamOrigin.FloatingLooseConverge;
    public BeamManager.BeamData beamData;
    public BeamManager[] beamManagers;

    public event SkillArcaneOrdinance.OnCast OnCastEvent;

    public bool isFiring => this.beamActive;

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is ArcaneBolt) || this.beamManagers != null && this.beamManagers.Length == this.beamCount)
        return;
      this.beamManagers = new BeamManager[this.beamCount];
      for (int index = 0; index < this.beamCount; ++index)
        this.beamManagers[index] = new BeamManager((Mana) null, this.beamData, this.beamType, index == 0);
    }

    public virtual void OnImbueLoad(SpellData spell, Imbue imbue)
    {
      base.OnImbueLoad(spell, imbue);
      if (!(spell is ArcaneBolt arcaneBolt) || imbue.colliderGroup.modifier.imbueType != 3)
        return;
      // ISSUE: method pointer
      arcaneBolt.OnCrystalUseEvent -= new SpellCastCharge.CrystalUseEvent((object) this, __methodptr(OnCrystalUse));
      // ISSUE: method pointer
      arcaneBolt.OnCrystalUseEvent += new SpellCastCharge.CrystalUseEvent((object) this, __methodptr(OnCrystalUse));
      this.currentImbue = imbue;
    }

    public virtual void OnImbueUnload(SpellData spell, Imbue imbue)
    {
      base.OnImbueUnload(spell, imbue);
      if (!(spell is ArcaneBolt arcaneBolt) || imbue.colliderGroup.modifier.imbueType != 3)
        return;
      // ISSUE: method pointer
      arcaneBolt.OnCrystalUseEvent -= new SpellCastCharge.CrystalUseEvent((object) this, __methodptr(OnCrystalUse));
    }

    private void OnCrystalUse(SpellCastCharge spell, Imbue imbue, RagdollHand hand, bool active)
    {
      if (active && imbue.CanConsume(this.imbueConsumption))
      {
        foreach (BeamManager beamManager in this.beamManagers)
          beamManager.Activate();
        this.beamActive = true;
        ((MonoBehaviour) imbue).StartCoroutine(this.UpdateRoutine(imbue, true));
        SkillArcaneOrdinance.OnCast onCastEvent = this.OnCastEvent;
        if (onCastEvent == null)
          return;
        onCastEvent(spell, imbue, true);
      }
      else
      {
        this.beamActive = false;
        ((MonoBehaviour) imbue).StopCoroutine(this.UpdateRoutine(imbue, true));
        foreach (BeamManager beamManager in this.beamManagers)
          beamManager.Deactivate();
        this.UpdateBeams(imbue, false);
        this.lastCastTime = Time.time;
        SkillArcaneOrdinance.OnCast onCastEvent = this.OnCastEvent;
        if (onCastEvent != null)
          onCastEvent(spell, imbue, false);
      }
    }

    public IEnumerator UpdateRoutine(Imbue imbue, bool firing)
    {
      while (this.beamActive)
      {
        this.UpdateBeams(imbue, firing);
        yield return (object) new WaitForEndOfFrame();
      }
      yield return (object) new WaitForEndOfFrame();
    }

    public void UpdateBeams(Imbue imbue, bool firing)
    {
      Vector3 forward = imbue.colliderGroup.imbueShoot.forward;
      for (int index = 0; index < this.beamManagers.Length; ++index)
      {
        this.beamManagers[index].beamOrigin = this.beamCount <= 1 ? new Vector3?(imbue.colliderGroup.imbueShoot.position + forward.normalized * this.forwardOffset) : new Vector3?(Utilities.GetPointOnCircle(imbue.colliderGroup.imbueShoot.position, forward, index, this.beamManagers.Length, 0.25f) + forward.normalized * this.forwardOffset);
        this.beamManagers[index].castOriginOverride = new Vector3?(imbue.colliderGroup.imbueShoot.position);
        this.beamManagers[index].castDirectionOverride = new Vector3?(forward);
        this.beamManagers[index].UpdateBeam(firing, 1f);
      }
    }

    public delegate void OnCast(SpellCastCharge spell, Imbue imbue, bool active);
  }
}
