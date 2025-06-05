// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SkillArcaneLevitation
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Misc;
using Arcana.Spells;
using System;
using System.Collections;
using System.Reflection;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Arcana.Skills
{
  internal class SkillArcaneLevitation : SpellSkillData
  {
    public string floatingStatusId = "Floating";
    public StatusData floatingStatusData;
    public float statusRemoveDelay;
    public string floatingArmEffectId;
    public EffectData floatingArmEffectData;
    private EffectInstance leftArm;
    private EffectInstance rightArm;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.floatingStatusData = Catalog.GetData<StatusData>(this.floatingStatusId, true);
      this.floatingArmEffectData = Catalog.GetData<EffectData>(this.floatingArmEffectId, true);
    }

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      switch (spell)
      {
        case ArcaneBolt arcaneBolt:
          // ISSUE: method pointer
          arcaneBolt.OnSpellCastEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellCast));
          // ISSUE: method pointer
          arcaneBolt.OnSpellCastEvent += new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellCast));
          // ISSUE: method pointer
          arcaneBolt.OnSpellStopEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellStop));
          // ISSUE: method pointer
          arcaneBolt.OnSpellStopEvent += new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellStop));
          // ISSUE: method pointer
          arcaneBolt.OnSpellThrowEvent -= new SpellCastCharge.SpellThrowEvent((object) this, __methodptr(OnSpellThrow));
          // ISSUE: method pointer
          arcaneBolt.OnSpellThrowEvent += new SpellCastCharge.SpellThrowEvent((object) this, __methodptr(OnSpellThrow));
          if (caster?.mana?.creature != null)
          {
            // ISSUE: method pointer
            caster.mana.creature.airHelper.OnGroundEvent -= new AirHelper.AirEvent((object) this, __methodptr(OnGroundEvent));
            // ISSUE: method pointer
            caster.mana.creature.airHelper.OnGroundEvent += new AirHelper.AirEvent((object) this, __methodptr(OnGroundEvent));
          }
          if (caster?.ragdollHand != null)
          {
            // ISSUE: method pointer
            caster.ragdollHand.OnGrabEvent -= new RagdollHand.GrabEvent((object) this, __methodptr(OnGrabEvent));
            // ISSUE: method pointer
            caster.ragdollHand.OnGrabEvent += new RagdollHand.GrabEvent((object) this, __methodptr(OnGrabEvent));
            // ISSUE: method pointer
            caster.ragdollHand.OnUnGrabEvent -= new RagdollHand.UnGrabEvent((object) this, __methodptr(OnUnGrabEvent));
            // ISSUE: method pointer
            caster.ragdollHand.OnUnGrabEvent += new RagdollHand.UnGrabEvent((object) this, __methodptr(OnUnGrabEvent));
          }
          if (caster?.ragdollHand?.otherHand == null)
            break;
          // ISSUE: method pointer
          caster.ragdollHand.otherHand.OnGrabEvent -= new RagdollHand.GrabEvent((object) this, __methodptr(OnGrabEvent));
          // ISSUE: method pointer
          caster.ragdollHand.otherHand.OnGrabEvent += new RagdollHand.GrabEvent((object) this, __methodptr(OnGrabEvent));
          // ISSUE: method pointer
          caster.ragdollHand.otherHand.OnUnGrabEvent -= new RagdollHand.UnGrabEvent((object) this, __methodptr(OnUnGrabEvent));
          // ISSUE: method pointer
          caster.ragdollHand.otherHand.OnUnGrabEvent += new RagdollHand.UnGrabEvent((object) this, __methodptr(OnUnGrabEvent));
          break;
        case SpellMergeData spellMergeData:
          // ISSUE: method pointer
          spellMergeData.OnMergeStartEvent -= new SpellMergeData.MergeEvent((object) this, __methodptr(OnMergeSpellCast));
          // ISSUE: method pointer
          spellMergeData.OnMergeStartEvent += new SpellMergeData.MergeEvent((object) this, __methodptr(OnMergeSpellCast));
          // ISSUE: method pointer
          spellMergeData.OnMergeEndEvent -= new SpellMergeData.MergeEvent((object) this, __methodptr(OnMergeSpellStop));
          // ISSUE: method pointer
          spellMergeData.OnMergeEndEvent += new SpellMergeData.MergeEvent((object) this, __methodptr(OnMergeSpellStop));
          break;
      }
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      switch (spell)
      {
        case ArcaneBolt spell1:
          // ISSUE: method pointer
          spell1.OnSpellCastEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellCast));
          // ISSUE: method pointer
          spell1.OnSpellStopEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellStop));
          // ISSUE: method pointer
          spell1.OnSpellThrowEvent -= new SpellCastCharge.SpellThrowEvent((object) this, __methodptr(OnSpellThrow));
          if (caster?.mana?.creature != null && !(caster.other.spellInstance is ArcaneBolt))
          {
            // ISSUE: method pointer
            caster.mana.creature.airHelper.OnGroundEvent -= new AirHelper.AirEvent((object) this, __methodptr(OnGroundEvent));
            this.ClearLevitation(caster.ragdollHand.creature);
          }
          this.OnSpellStop((SpellCastCharge) spell1);
          break;
        case SpellMergeData spellMergeData:
          // ISSUE: method pointer
          spellMergeData.OnMergeStartEvent -= new SpellMergeData.MergeEvent((object) this, __methodptr(OnMergeSpellCast));
          // ISSUE: method pointer
          spellMergeData.OnMergeEndEvent -= new SpellMergeData.MergeEvent((object) this, __methodptr(OnMergeSpellStop));
          break;
      }
    }

    private void OnSpellCast(SpellCastCharge spell)
    {
      if (!(spell is ArcaneBolt) || spell.spellCaster.other.spellInstance is ArcaneBolt && spell.spellCaster.other.isFiring)
        return;
      ((MonoBehaviour) GameManager.local).StartCoroutine(this.ApplyStatusRoutine((object) spell, this.GetCreatureFromSpell((object) spell)));
    }

    private void OnSpellStop(SpellCastCharge spell)
    {
      if (spell.spellCaster.other.spellInstance is ArcaneBolt && spell.spellCaster.other.isFiring)
        return;
      ((MonoBehaviour) GameManager.local).StartCoroutine(this.RemoveStatusRoutine((object) spell, this.GetCreatureFromSpell((object) spell), new float?(this.statusRemoveDelay)));
    }

    private void OnSpellThrow(SpellCastCharge spell, Vector3 velocity) => this.OnSpellStop(spell);

    private void OnMergeSpellCast(SpellMergeData spell)
    {
      ((MonoBehaviour) GameManager.local).StartCoroutine(this.ApplyStatusRoutine((object) spell, this.GetCreatureFromSpell((object) spell)));
    }

    private void OnMergeSpellStop(SpellMergeData spell)
    {
      ((MonoBehaviour) GameManager.local).StartCoroutine(this.RemoveStatusRoutine((object) spell, this.GetCreatureFromSpell((object) spell), new float?(this.statusRemoveDelay)));
    }

    private void OnGroundEvent(Creature creature)
    {
      if (creature.airHelper.inAir)
        return;
      this.ClearLevitation(creature);
      if (creature.mana == null)
        return;
      if (creature.mana.casterLeft.spellInstance is ArcaneBolt spellInstance1 && creature.mana.casterLeft.isFiring)
      {
        this.OnSpellCast((SpellCastCharge) spellInstance1);
      }
      else
      {
        if (!(creature.mana.casterRight.spellInstance is ArcaneBolt spellInstance) || !creature.mana.casterRight.isFiring)
          return;
        this.OnSpellCast((SpellCastCharge) spellInstance);
      }
    }

    private void OnGrabEvent(
      Side side,
      Handle handle,
      float axisPosition,
      HandlePose orientation,
      EventTime eventTime)
    {
      if ((UnityEngine.Object) handle == (UnityEngine.Object) null || eventTime != 1 || !handle.handlers.TrueForAll((Predicate<RagdollHand>) (x => ((ThunderEntity) x.creature).HasStatus(this.floatingStatusData))) || handle == null)
        return;
      ((ThunderEntity) handle.item)?.Inflict(this.floatingStatusData, (object) this, float.PositiveInfinity, (object) new FloatingParams(0.0f, 1f, 1f, true), true);
    }

    private void OnUnGrabEvent(Side side, Handle handle, bool throwing, EventTime eventTime)
    {
      if (eventTime != 1 || handle == null)
        return;
      ((ThunderEntity) handle.item)?.ClearByHandler((object) this);
    }

    private IEnumerator ApplyStatusRoutine(object spell, Creature creature)
    {
      if (this.floatingStatusData != null && creature != null)
      {
        while (!creature.airHelper.inAir || (double) creature.locomotion.physicBody.velocity.y > 1.0)
          yield return (object) new WaitForEndOfFrame();
        if (!((ThunderEntity) creature).HasStatus(this.floatingStatusData) && this.IsCasting(spell))
        {
          ((ThunderEntity) creature).Inflict(this.floatingStatusData, (object) this, float.PositiveInfinity, (object) new FloatingParams(0.0f, 5f, 1f, true), true);
          ((ThunderEntity) creature.handLeft.grabbedHandle?.item)?.Inflict(this.floatingStatusData, (object) this, float.PositiveInfinity, (object) new FloatingParams(0.0f, 1f, 1f, true), true);
          ((ThunderEntity) creature.handRight.grabbedHandle?.item)?.Inflict(this.floatingStatusData, (object) this, float.PositiveInfinity, (object) new FloatingParams(0.0f, 1f, 1f, true), true);
          this.HandleDiveStrike(creature);
          if (this.floatingArmEffectData != null)
          {
            RagdollPart leftForearm = creature.ragdoll.GetPartByName("LeftForeArm");
            RagdollPart rightForearm = creature.ragdoll.GetPartByName("RightForeArm");
            if (this.leftArm == null)
            {
              this.leftArm = this.floatingArmEffectData?.Spawn(((ThunderBehaviour) leftForearm).transform.position, Quaternion.LookRotation(leftForearm.upDirection, leftForearm.forwardDirection), ((ThunderBehaviour) leftForearm).transform, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
              this.leftArm?.Play(0, false, false);
            }
            if (this.rightArm == null)
            {
              this.rightArm = this.floatingArmEffectData?.Spawn(((ThunderBehaviour) rightForearm).transform.position, Quaternion.LookRotation(rightForearm.upDirection, rightForearm.forwardDirection), ((ThunderBehaviour) rightForearm).transform, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
              this.rightArm?.Play(0, false, false);
            }
          }
        }
      }
    }

    private IEnumerator RemoveStatusRoutine(object spell, Creature creature, float? delay = null)
    {
      yield return (object) new WaitForSeconds((float) ((double) delay ?? (double) this.statusRemoveDelay));
      Floating floating;
      if (creature != null && ((ThunderEntity) creature).TryGetStatus<Floating>(this.floatingStatusData, ref floating) && !this.IsCasting(spell))
        this.ClearLevitation(creature);
    }

    private void ClearLevitation(Creature creature)
    {
      this.HandleDiveStrike(creature, true);
      ((ThunderEntity) creature).ClearByHandler((object) this);
      ((ThunderEntity) creature.handLeft.grabbedHandle?.item)?.ClearByHandler((object) this);
      ((ThunderEntity) creature.handRight.grabbedHandle?.item)?.ClearByHandler((object) this);
      this.leftArm?.End(false, -1f);
      this.leftArm = (EffectInstance) null;
      this.rightArm?.End(false, -1f);
      this.rightArm = (EffectInstance) null;
    }

    private Creature GetCreatureFromSpell(object spell)
    {
      if (true)
        ;
      Creature creatureFromSpell;
      switch (spell)
      {
        case SpellCastCharge spellCastCharge:
          creatureFromSpell = spellCastCharge.spellCaster.ragdollHand.creature;
          break;
        case SpellMergeData spellMergeData:
          creatureFromSpell = spellMergeData.mana.creature;
          break;
        default:
          creatureFromSpell = (Creature) null;
          break;
      }
      if (true)
        ;
      return creatureFromSpell;
    }

    private bool IsCasting(object spell)
    {
      switch (spell)
      {
        case SpellCastCharge spellCastCharge:
          SpellCaster spellCaster = spellCastCharge.spellCaster;
          SpellCaster other = spellCastCharge.spellCaster.other;
          return spellCastCharge is ArcaneBolt && spellCaster.isFiring && spellCaster.allowCasting || other.spellInstance is ArcaneBolt && other.isFiring && other.allowCasting || spellCaster.mana.mergeActive && (spellCastCharge is ArcaneBolt || other.spellInstance is ArcaneBolt);
        case SpellMergeData spellMergeData:
          SpellCaster casterLeft = spellMergeData.mana.casterLeft;
          SpellCaster casterRight = spellMergeData.mana.casterRight;
          return casterLeft.spellInstance is ArcaneBolt && casterLeft.isFiring || casterRight.spellInstance is ArcaneBolt && casterRight.isFiring || spellMergeData.mana.mergeActive && (casterLeft.spellInstance is ArcaneBolt || casterRight.spellInstance is ArcaneBolt);
        default:
          return false;
      }
    }

    private void HandleDiveStrike(Creature creature, bool resubscribe = false)
    {
      SkillDiveStrike firstArgument;
      if (!creature.TryGetSkill<SkillDiveStrike>("DiveStrike", ref firstArgument))
        return;
      EventInfo eventInfo = typeof (DiveDetector).GetEvent("onDiveDetected");
      Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, (object) firstArgument, firstArgument.GetMethod("ActivateDive"));
      eventInfo.RemoveEventHandler((object) null, handler);
      if (!resubscribe)
        return;
      eventInfo.AddEventHandler((object) null, handler);
    }
  }
}
