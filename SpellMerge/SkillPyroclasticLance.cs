// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SpellMerge.SkillPyroclasticLance
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Misc;
using System;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Skills.SpellMerge
{
  public class SkillPyroclasticLance : SpellMergeData
  {
    public BeamManager.BeamData beamData;
    public int beamCount = 5;
    public float beamSpreadRadius = 1f;
    public float beamSpreadAngle = 200f;
    public float forwardOffset = 0.5f;
    public BeamManager.BeamOrigin beamType = BeamManager.BeamOrigin.FloatingLooseConverge;
    public float beamCastMinHandAngle = 20f;
    public AnimationCurve beamForceCurve = new AnimationCurve(new Keyframe[3]
    {
      new Keyframe(0.0f, 10f),
      new Keyframe(0.05f, 25f),
      new Keyframe(0.1f, 10f)
    });
    public bool beamActive;
    public Ray castRay;
    public BeamManager[] beamManagers;

    public event SkillPyroclasticLance.OnBeam OnBeamStartEvent;

    public event SkillPyroclasticLance.OnBeam OnBeamEndEvent;

    public event SkillPyroclasticLance.OnBeam OnBeamUpdateEvent;

    public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnLateSkillsLoaded(skillData, creature);
      if (this.beamManagers != null)
        return;
      this.beamManagers = new BeamManager[this.beamCount];
      for (int index = 0; index < this.beamCount; ++index)
      {
        this.beamManagers[index] = new BeamManager(creature.mana, this.beamData, this.beamType, index == 0);
        this.beamManagers[index].OnBeamStartEvent += new BeamManager.OnBeam(this.OnBeamManagerStart);
        this.beamManagers[index].OnBeamUpdateEvent += new BeamManager.OnBeam(this.OnBeamManagerUpdate);
        this.beamManagers[index].OnBeamEndEvent += new BeamManager.OnBeam(this.OnBeamManagerEnd);
      }
    }

    private void OnBeamManagerStart(BeamManager beamManager)
    {
      SkillPyroclasticLance.OnBeam onBeamStartEvent = this.OnBeamStartEvent;
      if (onBeamStartEvent == null)
        return;
      onBeamStartEvent(this, beamManager, beamManager.mana);
    }

    private void OnBeamManagerUpdate(BeamManager beamManager)
    {
      SkillPyroclasticLance.OnBeam onBeamUpdateEvent = this.OnBeamUpdateEvent;
      if (onBeamUpdateEvent == null)
        return;
      onBeamUpdateEvent(this, beamManager, beamManager.mana);
    }

    private void OnBeamManagerEnd(BeamManager beamManager)
    {
      SkillPyroclasticLance.OnBeam onBeamEndEvent = this.OnBeamEndEvent;
      if (onBeamEndEvent == null)
        return;
      onBeamEndEvent(this, beamManager, beamManager.mana);
    }

    public virtual void Merge(bool active)
    {
      base.Merge(active);
      if (active)
      {
        this.currentCharge = 0.35f;
        foreach (BeamManager beamManager in this.beamManagers)
          beamManager.Activate();
      }
      else
      {
        this.mana.creature.locomotion.RemoveSpeedModifier((object) this);
        foreach (BeamManager beamManager in this.beamManagers)
        {
          beamManager.Deactivate();
          beamManager.UpdateBeam(false, this.currentCharge);
          beamManager.UpdatePlayerModifications(false, this.currentCharge);
        }
        this.currentCharge = 0.0f;
      }
      this.beamActive = false;
    }

    public virtual void Load(Mana mana)
    {
      base.Load(mana);
      foreach (BeamManager beamManager in this.beamManagers)
        beamManager.mana = mana;
    }

    public virtual void FixedUpdate()
    {
      base.FixedUpdate();
      if (!this.beamActive)
        return;
      ((RagdollPart) this.mana.casterLeft.ragdollHand).physicBody.AddForce(-this.castRay.direction * this.beamForceCurve.Evaluate(Time.time), (ForceMode) 0);
      ((RagdollPart) this.mana.casterRight.ragdollHand).physicBody.AddForce(-this.castRay.direction * this.beamForceCurve.Evaluate(Time.time), (ForceMode) 0);
    }

    public virtual void Update()
    {
      base.Update();
      this.castRay.origin = this.mana.mergePoint.position;
      this.castRay.direction = Vector3.Slerp(this.mana.casterLeft.magicSource.up, this.mana.casterRight.magicSource.up, 0.5f);
      this.beamActive = (double) Vector3.SignedAngle(this.castRay.direction, this.mana.casterLeft.magicSource.up, Vector3.Cross(this.mana.creature.centerEyes.position - this.castRay.origin, this.mana.casterLeft.magicSource.position - this.castRay.origin).normalized) < -(double) this.beamCastMinHandAngle && (double) Vector3.SignedAngle(this.castRay.direction, this.mana.casterRight.magicSource.up, Vector3.Cross(this.mana.casterRight.magicSource.position - this.castRay.origin, this.mana.creature.centerEyes.position - this.castRay.origin).normalized) > (double) this.beamCastMinHandAngle;
      Creature[] creaturesInCone = Utilities.GetCreaturesInCone(this.mana.mergePoint.position, this.castRay.direction, 20.1f, 90f, this.beamManagers.Length, true);
      for (int index = 0; index < this.beamManagers.Length; ++index)
      {
        if (!this.beamManagers[index].overrideBeamControl)
        {
          this.beamManagers[index].target = !Utils.IsNullOrEmpty((Array) creaturesInCone) ? creaturesInCone[index % creaturesInCone.Length] : (Creature) null;
          Vector3 normal = Vector3.Slerp(this.mana.casterLeft.magicSource.up, this.mana.casterRight.magicSource.up, 0.5f);
          this.beamManagers[index].beamOrigin = new Vector3?(Utilities.GetPointOnArc(this.mana.mergePoint.position, normal, index, this.beamManagers.Length, this.beamSpreadRadius, this.beamSpreadAngle) + normal.normalized * this.forwardOffset);
        }
        this.beamManagers[index].UpdateBeam(this.beamActive, this.currentCharge);
        this.beamManagers[index].UpdatePlayerModifications(this.beamActive, this.currentCharge);
      }
    }

    public delegate void OnBeam(SkillPyroclasticLance skill, BeamManager beam, Mana mana);
  }
}
