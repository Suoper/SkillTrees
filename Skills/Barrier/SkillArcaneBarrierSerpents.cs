// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.Barrier.SkillArcaneBarrierSerpents
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Behaviors;
using Arcana.Misc;
using Arcana.Skills.Serpents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Arcana.Skills.Barrier
{
  public class SkillArcaneBarrierSerpents : SpellSkillData
  {
    public string arcaneBarrierSkillId;
    public float serpentShootCooldown = 1f;
    public float attackRadius = 3f;
    public float radius = 0.7f;
    public float radiusVariance = 0.1f;
    public float speed = 180f;
    public float speedVariance = 45f;
    public float sineAmplitude = 0.03f;
    public float sineAmplitudeVariance = 0.02f;
    public float sineFrequency = 6f;
    public float sineFrequencyVariance = 2f;
    public bool doSineMovement = true;
    private float lastAttackTime;
    private List<Serpent> barrierSerpents;

    private List<Serpent> GetSidedSerpents(SkillArcaneBarrier.Barrier barrier)
    {
      return SkillArcaneSerpents.serpents.Where<Serpent>((Func<Serpent, bool>) (x => (x.side == barrier.side && x.orbitHandler is SkillArcaneBarrier.Barrier || x.orbitHandler == null || x.isAttacking && x.orbitHandler is SkillArcaneBarrier.Barrier) && !x.isAttacking)).ToList<Serpent>();
    }

    public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnLateSkillsLoaded(skillData, creature);
      SkillArcaneBarrier skillArcaneBarrier;
      if (!creature.TryGetSkill<SkillArcaneBarrier>(this.arcaneBarrierSkillId, ref skillArcaneBarrier))
        return;
      skillArcaneBarrier.OnBarrierStartEvent -= new SkillArcaneBarrier.BarrierEvent(this.OnBarrierStart);
      skillArcaneBarrier.OnBarrierStartEvent += new SkillArcaneBarrier.BarrierEvent(this.OnBarrierStart);
      skillArcaneBarrier.OnBarrierStopEvent -= new SkillArcaneBarrier.BarrierEvent(this.OnBarrierEnd);
      skillArcaneBarrier.OnBarrierStopEvent += new SkillArcaneBarrier.BarrierEvent(this.OnBarrierEnd);
      skillArcaneBarrier.OnBarrierUpdateEvent -= new SkillArcaneBarrier.BarrierEvent(this.OnBarrierUpdate);
      skillArcaneBarrier.OnBarrierUpdateEvent += new SkillArcaneBarrier.BarrierEvent(this.OnBarrierUpdate);
      this.barrierSerpents = new List<Serpent>();
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      SkillArcaneBarrier skillArcaneBarrier;
      if (!creature.TryGetSkill<SkillArcaneBarrier>(this.arcaneBarrierSkillId, ref skillArcaneBarrier))
        return;
      skillArcaneBarrier.OnBarrierStartEvent -= new SkillArcaneBarrier.BarrierEvent(this.OnBarrierStart);
      skillArcaneBarrier.OnBarrierStopEvent -= new SkillArcaneBarrier.BarrierEvent(this.OnBarrierEnd);
      skillArcaneBarrier.OnBarrierUpdateEvent -= new SkillArcaneBarrier.BarrierEvent(this.OnBarrierUpdate);
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      foreach (Serpent serpent in this.barrierSerpents.ToList<Serpent>())
      {
        serpent.ResetOrbit(serpent.data.teleportReturnTimeout);
        serpent.objectOrbitNormal = UnityEngine.Random.onUnitSphere;
        serpent.movementMultiplier = 1f;
        serpent.tempScale = new float?();
        serpent.ResetRotation();
        serpent.AssignHandler();
        this.barrierSerpents.Remove(serpent);
      }
    }

    private void CheckForTargets(SpellCastCharge spell, SkillArcaneBarrier.Barrier barrier)
    {
      if ((double) Time.time - (double) this.lastAttackTime < (double) this.serpentShootCooldown)
        return;
      List<Serpent> list1 = this.barrierSerpents.Where<Serpent>((Func<Serpent, bool>) (x => x.orbitHandler == barrier && (UnityEngine.Object) x.ObjectOrbitTransform == (UnityEngine.Object) ((ThunderBehaviour) barrier.item).transform && !x.IsChangingOrbit())).ToList<Serpent>();
      if (Utils.IsNullOrEmpty((ICollection) list1))
        return;
      List<Creature> list2 = ((IEnumerable<Creature>) Utilities.GetCreaturesInRadius(((ThunderBehaviour) barrier.item).transform.position, this.attackRadius, 1)).ToList<Creature>();
      if (Utils.IsNullOrEmpty((ICollection) list2))
        return;
      Serpent attackingSerpent = list1.First<Serpent>();
      attackingSerpent.SetTargets(list2, new Func<Serpent.OnAttackFinish>(OnAttackFinish));
      this.lastAttackTime = Time.time;

      Serpent.OnAttackFinish OnAttackFinish()
      {
        SkillArcaneBarrier.Barrier barrier = barrier.skill.barriers.FirstOrDefault<SkillArcaneBarrier.Barrier>((Func<SkillArcaneBarrier.Barrier, bool>) (x => x.MatchBarrier(spell)));
        if (barrier != null)
          this.AssignSerpent(attackingSerpent, spell, barrier);
        else
          this.ReleaseSerpent(attackingSerpent, spell, barrier);
        return (Serpent.OnAttackFinish) null;
      }
    }

    private void OnBarrierStart(SpellCastCharge spell, SkillArcaneBarrier.Barrier barrier)
    {
      foreach (Serpent sidedSerpent in this.GetSidedSerpents(barrier))
        this.AssignSerpent(sidedSerpent, spell, barrier);
    }

    private void OnBarrierEnd(SpellCastCharge spell, SkillArcaneBarrier.Barrier barrier)
    {
      foreach (Serpent serpent in this.barrierSerpents.Where<Serpent>((Func<Serpent, bool>) (x => x.orbitHandler == barrier && !x.isAttacking)).ToArray<Serpent>())
        this.ReleaseSerpent(serpent, spell, barrier);
    }

    private void OnBarrierUpdate(SpellCastCharge spell, SkillArcaneBarrier.Barrier barrier)
    {
      foreach (Serpent serpent in this.GetSidedSerpents(barrier).Where<Serpent>((Func<Serpent, bool>) (x => !x.orbitingObject && !x.isAttacking && !x.IsChangingOrbit())).ToList<Serpent>())
        this.AssignSerpent(serpent, spell, barrier);
      this.CheckForTargets(spell, barrier);
    }

    private void AssignSerpent(
      Serpent serpent,
      SpellCastCharge spell,
      SkillArcaneBarrier.Barrier barrier)
    {
      serpent.AssignHandler((object) barrier);
      serpent.AssignNewOrbit(((ThunderBehaviour) barrier.item).transform, false, true, serpent.data.teleportTargetTimeout, (object) barrier);
      serpent.objectOrbitNormal = ((ThunderBehaviour) barrier.item).transform.forward;
      serpent.movementMultiplier = 1.5f;
      serpent.tempScale = new float?(3f);
      serpent.LoadRotationData(new float?(this.radius), new float?(this.radiusVariance), new float?(this.speed), new float?(this.speedVariance), new float?(this.sineAmplitude), new float?(this.sineAmplitudeVariance), new float?(this.sineFrequency), new float?(this.sineFrequencyVariance), new bool?(this.doSineMovement));
      this.barrierSerpents.Add(serpent);
    }

    private void ReleaseSerpent(
      Serpent serpent,
      SpellCastCharge spell,
      SkillArcaneBarrier.Barrier barrier)
    {
      SkillArcaneSerpents skillArcaneSerpents;
      if (!barrier.creature.TryGetSkill<SkillArcaneSerpents>("Skill_ArcaneSerpents", ref skillArcaneSerpents))
        return;
      if (skillArcaneSerpents.currentHandle != null && spell.spellCaster.isFiring && !spell.spellCaster.isMerging && !spell.spellCaster.isSpraying)
        skillArcaneSerpents.SerpentsTargetHandle((object) barrier);
      else
        serpent.ResetOrbit(serpent.data.teleportReturnTimeout);
      serpent.objectOrbitNormal = UnityEngine.Random.onUnitSphere;
      serpent.movementMultiplier = 1f;
      serpent.tempScale = new float?();
      serpent.ResetRotation();
      serpent.AssignHandler();
      if (barrier.Other() != null && this.barrierSerpents.Any<Serpent>((Func<Serpent, bool>) (x => x.orbitHandler == barrier.Other())))
        this.OnBarrierStart(spell, barrier.Other());
      this.barrierSerpents.Remove(serpent);
    }
  }
}
