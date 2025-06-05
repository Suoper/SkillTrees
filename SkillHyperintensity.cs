// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillHyperintensity
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.Skill.Spell;
using System.Collections;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillHyperintensity : SpellSkillData
  {
    public static EffectInstance overchargeLeftLoopEffect;
    public static EffectInstance overchargeRightLoopEffect;
    public static bool leftFullyCharged;
    public static bool rightFullyCharged;
    private static bool allowLeftDrain;
    private static bool allowRightDrain;
    public AnimationCurve hapticCurve = new AnimationCurve(new Keyframe[3]
    {
      new Keyframe(0.0f, 20f),
      new Keyframe(0.05f, 45f),
      new Keyframe(0.1f, 20f)
    });
    public Coroutine leftCoroutine;
    public float leftLastChargeTime;
    public EffectData overchargeLoopEffectData;
    public string overchargeLoopEffectId;
    public EffectData overchargeStartEffectData;
    public string overchargeStartEffectId;
    public Coroutine rightCoroutine;
    public float rightLastChargeTime;
    public float timeToOvercharge;

    public static event SkillHyperintensity.OnSpellOvercharge onSpellOvercharge;

    public static event SkillHyperintensity.OnSpellReleased onSpellReleased;

    public static bool isOvercharged(Side side)
    {
      return side == 1 ? SkillHyperintensity.leftFullyCharged : SkillHyperintensity.rightFullyCharged;
    }

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      // ISSUE: method pointer
      EventManager.onPossess += new EventManager.PossessEvent((object) this, __methodptr(OnPossess));
    }

    private void OnPossess(Creature creature, EventTime eventTime)
    {
      if (eventTime == 0)
        return;
      // ISSUE: method pointer
      EventManager.onPossess -= new EventManager.PossessEvent((object) this, __methodptr(OnPossess));
      EndingContent.GetCurrent().hasT4Skill = true;
    }

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      SkillHyperintensity.leftFullyCharged = false;
      SkillHyperintensity.rightFullyCharged = false;
      this.overchargeStartEffectData = Catalog.GetData<EffectData>(this.overchargeStartEffectId, true);
      this.overchargeLoopEffectData = Catalog.GetData<EffectData>(this.overchargeLoopEffectId, true);
    }

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      // ISSUE: method pointer
      spellCastCrystallic.OnSpellStopEvent += new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellStopEvent));
      // ISSUE: method pointer
      spellCastCrystallic.OnSpellThrowEvent += new SpellCastCharge.SpellThrowEvent((object) this, __methodptr(OnSpellThrowEvent));
      // ISSUE: method pointer
      spellCastCrystallic.OnSpellUpdateEvent += new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellUpdateEvent));
      EffectInstance effectInstance = spellCastCrystallic.spellCaster.side == 1 ? SkillHyperintensity.overchargeLeftLoopEffect : SkillHyperintensity.overchargeRightLoopEffect;
      SkillHyperintensity.leftFullyCharged = false;
      SkillHyperintensity.rightFullyCharged = false;
      if (effectInstance != null)
        effectInstance?.End(false, -1f);
    }

    private void OnSpellStopEvent(SpellCastCharge spell)
    {
      SpellCastCrystallic spellCastCrystallic = spell as SpellCastCrystallic;
      if (spell.spellCaster.side == 1)
      {
        SkillHyperintensity.leftFullyCharged = false;
        if (SkillHyperintensity.overchargeLeftLoopEffect != null)
        {
          SkillHyperintensity.overchargeLeftLoopEffect.End(false, -1f);
          SkillHyperintensity.overchargeLeftLoopEffect = (EffectInstance) null;
        }
        if (this.leftCoroutine != null)
        {
          ((MonoBehaviour) GameManager.local).StopCoroutine(this.OverchargeRoutine(spell as SpellCastCrystallic));
          this.leftCoroutine = (Coroutine) null;
        }
      }
      else if (spell.spellCaster.side == 0)
      {
        SkillHyperintensity.rightFullyCharged = false;
        if (SkillHyperintensity.overchargeRightLoopEffect != null)
        {
          SkillHyperintensity.overchargeRightLoopEffect.End(false, -1f);
          SkillHyperintensity.overchargeRightLoopEffect = (EffectInstance) null;
        }
        if (this.rightCoroutine != null)
        {
          ((MonoBehaviour) GameManager.local).StopCoroutine(this.OverchargeRoutine(spell as SpellCastCrystallic));
          this.rightCoroutine = (Coroutine) null;
        }
      }
      ((MonoBehaviour) GameManager.local).StartCoroutine(this.UnsubscribeRoutine(0.1f, spell as SpellCastCrystallic));
    }

    private void OnSpellThrowEvent(SpellCastCharge spell, Vector3 velocity)
    {
      SpellCastCrystallic spellCastCrystallic = spell as SpellCastCrystallic;
      if (spell.spellCaster.side == 1)
      {
        SkillHyperintensity.leftFullyCharged = false;
        if (SkillHyperintensity.overchargeLeftLoopEffect != null)
        {
          SkillHyperintensity.overchargeLeftLoopEffect.End(false, -1f);
          SkillHyperintensity.overchargeLeftLoopEffect = (EffectInstance) null;
        }
        if (this.leftCoroutine != null)
        {
          ((MonoBehaviour) GameManager.local).StopCoroutine(this.OverchargeRoutine(spell as SpellCastCrystallic));
          this.leftCoroutine = (Coroutine) null;
        }
      }
      else if (spell.spellCaster.side == 0)
      {
        SkillHyperintensity.rightFullyCharged = false;
        if (SkillHyperintensity.overchargeRightLoopEffect != null)
        {
          SkillHyperintensity.overchargeRightLoopEffect.End(false, -1f);
          SkillHyperintensity.overchargeRightLoopEffect = (EffectInstance) null;
        }
        if (this.rightCoroutine != null)
        {
          ((MonoBehaviour) GameManager.local).StopCoroutine(this.OverchargeRoutine(spell as SpellCastCrystallic));
          this.rightCoroutine = (Coroutine) null;
        }
      }
      ((MonoBehaviour) GameManager.local).StartCoroutine(this.UnsubscribeRoutine(0.1f, spell as SpellCastCrystallic));
    }

    public static void ForceInvokeOvercharged(SpellCastCrystallic spellCastCrystallic)
    {
      SkillHyperintensity.OnSpellOvercharge onSpellOvercharge = SkillHyperintensity.onSpellOvercharge;
      if (onSpellOvercharge == null)
        return;
      onSpellOvercharge(spellCastCrystallic);
    }

    public static void ForceInvokeRelease(SpellCastCrystallic spellCastCrystallic)
    {
      SkillHyperintensity.OnSpellReleased onSpellReleased = SkillHyperintensity.onSpellReleased;
      if (onSpellReleased == null)
        return;
      onSpellReleased(spellCastCrystallic);
    }

    public IEnumerator UnsubscribeRoutine(float delay, SpellCastCrystallic spellCastCrystallic)
    {
      yield return (object) new WaitForSeconds(delay);
      SkillHyperintensity.OnSpellReleased onSpellReleased = SkillHyperintensity.onSpellReleased;
      if (onSpellReleased != null)
        onSpellReleased(spellCastCrystallic);
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      // ISSUE: method pointer
      spellCastCrystallic.OnSpellStopEvent += new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellStopEvent));
      // ISSUE: method pointer
      spellCastCrystallic.OnSpellThrowEvent -= new SpellCastCharge.SpellThrowEvent((object) this, __methodptr(OnSpellThrowEvent));
      // ISSUE: method pointer
      spellCastCrystallic.OnSpellUpdateEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellUpdateEvent));
      EffectInstance effectInstance = spellCastCrystallic.spellCaster.side == 1 ? SkillHyperintensity.overchargeLeftLoopEffect : SkillHyperintensity.overchargeRightLoopEffect;
      SkillHyperintensity.leftFullyCharged = false;
      SkillHyperintensity.rightFullyCharged = false;
      if (effectInstance != null)
        effectInstance?.End(false, -1f);
    }

    public static void ToggleDrain(Side side, bool active)
    {
      Side side1 = side;
      if (side1 != null)
      {
        if (side1 != 1)
          return;
        SkillHyperintensity.allowLeftDrain = active;
      }
      else
        SkillHyperintensity.allowRightDrain = active;
    }

    private void OnSpellUpdateEvent(SpellCastCharge spell)
    {
      Side side = spell.spellCaster.side;
      if (side != null)
      {
        if (side != 1)
          return;
        if (Mathf.Approximately(spell.currentCharge, 1f) && !SkillHyperintensity.leftFullyCharged)
        {
          this.leftLastChargeTime = Time.time;
          SkillHyperintensity.leftFullyCharged = true;
          if (this.leftCoroutine != null)
          {
            ((MonoBehaviour) GameManager.local).StopCoroutine(this.leftCoroutine);
            this.leftCoroutine = (Coroutine) null;
          }
          this.leftCoroutine = ((MonoBehaviour) GameManager.local).StartCoroutine(this.OverchargeRoutine(spell as SpellCastCrystallic));
        }
        else
        {
          if ((double) spell.currentCharge >= 1.0 || !SkillHyperintensity.leftFullyCharged || !SkillHyperintensity.allowLeftDrain)
            return;
          if (SkillHyperintensity.overchargeLeftLoopEffect != null)
          {
            SkillHyperintensity.overchargeLeftLoopEffect.End(false, -1f);
            SkillHyperintensity.overchargeLeftLoopEffect = (EffectInstance) null;
          }
          SkillHyperintensity.OnSpellReleased onSpellReleased = SkillHyperintensity.onSpellReleased;
          if (onSpellReleased != null)
            onSpellReleased(spell as SpellCastCrystallic);
          SkillHyperintensity.leftFullyCharged = false;
        }
      }
      else if (Mathf.Approximately(spell.currentCharge, 1f) && !SkillHyperintensity.rightFullyCharged)
      {
        this.rightLastChargeTime = Time.time;
        SkillHyperintensity.rightFullyCharged = true;
        if (this.rightCoroutine != null)
        {
          ((MonoBehaviour) GameManager.local).StopCoroutine(this.rightCoroutine);
          this.rightCoroutine = (Coroutine) null;
        }
        this.rightCoroutine = ((MonoBehaviour) GameManager.local).StartCoroutine(this.OverchargeRoutine(spell as SpellCastCrystallic));
      }
      else
      {
        if ((double) spell.currentCharge >= 1.0 || !SkillHyperintensity.rightFullyCharged || !SkillHyperintensity.allowRightDrain)
          return;
        if (SkillHyperintensity.overchargeRightLoopEffect != null)
        {
          SkillHyperintensity.overchargeRightLoopEffect.End(false, -1f);
          SkillHyperintensity.overchargeRightLoopEffect = (EffectInstance) null;
        }
        SkillHyperintensity.OnSpellReleased onSpellReleased = SkillHyperintensity.onSpellReleased;
        if (onSpellReleased != null)
          onSpellReleased(spell as SpellCastCrystallic);
        SkillHyperintensity.rightFullyCharged = false;
      }
    }

    private IEnumerator OverchargeRoutine(SpellCastCrystallic spell)
    {
      if (spell.spellCaster.side == 1)
      {
        while ((double) Time.time - (double) this.leftLastChargeTime < (double) this.timeToOvercharge)
          yield return (object) null;
        this.Overcharge(spell);
      }
      else
      {
        while ((double) Time.time - (double) this.rightLastChargeTime < (double) this.timeToOvercharge)
          yield return (object) null;
        this.Overcharge(spell);
      }
    }

    public void Overcharge(SpellCastCrystallic spell)
    {
      if (!spell.spellCaster.isFiring || !Mathf.Approximately(spell.currentCharge, 1f))
        return;
      spell.spellCaster.ragdollHand.PlayHapticClipOver(this.hapticCurve, 0.25f);
      this.overchargeStartEffectData.Spawn(spell.spellCaster.Orb, true, (ColliderGroup) null, false).Play(0, false, false);
      if (spell.spellCaster.side == 1)
      {
        if (SkillHyperintensity.overchargeLeftLoopEffect != null)
          SkillHyperintensity.overchargeLeftLoopEffect.End(false, -1f);
        SkillHyperintensity.overchargeLeftLoopEffect = this.overchargeLoopEffectData.Spawn(spell.spellCaster.Orb, true, (ColliderGroup) null, false);
        SkillHyperintensity.overchargeLeftLoopEffect.Play(0, false, false);
        SkillHyperintensity.overchargeLeftLoopEffect.SetColorImmediate(spell.currentColor);
      }
      else
      {
        if (SkillHyperintensity.overchargeRightLoopEffect != null)
          SkillHyperintensity.overchargeRightLoopEffect.End(false, -1f);
        SkillHyperintensity.overchargeRightLoopEffect = this.overchargeLoopEffectData.Spawn(spell.spellCaster.Orb, true, (ColliderGroup) null, false);
        SkillHyperintensity.overchargeRightLoopEffect.Play(0, false, false);
        SkillHyperintensity.overchargeRightLoopEffect.SetColorImmediate(spell.currentColor);
      }
      SkillHyperintensity.OnSpellOvercharge onSpellOvercharge = SkillHyperintensity.onSpellOvercharge;
      if (onSpellOvercharge == null)
        return;
      onSpellOvercharge(spell);
    }

    public delegate void OnSpellOvercharge(SpellCastCrystallic spellCastCrystallic);

    public delegate void OnSpellReleased(SpellCastCrystallic spellCastCrystallic);
  }
}
