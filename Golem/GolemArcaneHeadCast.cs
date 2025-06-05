// Decompiled with JetBrains decompiler
// Type: Arcana.Golem.GolemArcaneHeadCast
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using System;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Golem
{
  public class GolemArcaneHeadCast : GolemAbility
  {
    public float headSweepRange = 5f;
    public bool lockSweep;
    public float beamMaxDistance = 50f;
    public float beamStartMaxAngle = 30f;
    public float beamAngleHardMax = 80f;
    public float beamCooldownDuration = 20f;
    public float beamStopDelayTargetLost = 3f;
    public Vector2 beamingMinMaxDuration = new Vector2(5f, 10f);
    public GolemArcaneHeadCast.State state;
    public bool sweepSide;
    protected LookMode sweepMode;
    protected float headSweepRangeMultiplier = 1f;
    protected float beamTargetLostDuration;

    public static float LastBeamTime
    {
      get => GolemBeam.lastBeamTime;
      set => GolemBeam.lastBeamTime = value;
    }

    public virtual bool HeadshotInterruptable => true;

    public virtual bool OverrideLook
    {
      get
      {
        switch (this.state)
        {
          case GolemArcaneHeadCast.State.Starting:
          case GolemArcaneHeadCast.State.Firing:
            if ((UnityEngine.Object) this.golem != (UnityEngine.Object) null)
            {
              LookMode lookMode = this.golem.lookMode;
              return (lookMode == 1 ? 0 : (lookMode != 2 ? 1 : 0)) == 0;
            }
            break;
        }
        return false;
      }
    }

    public virtual void LookAt()
    {
      base.LookAt();
      this.golem.headIktarget.rotation = Quaternion.LookRotation(this.golem.lookingTarget.position - ((ThunderBehaviour) this.golem).transform.position, Vector3.up);
      Vector3 target = this.golem.lookingTarget.position + (this.golem.lookMode == 2 ? this.golem.headIktarget.transform.up : this.golem.headIktarget.transform.right) * ((this.sweepSide ? this.headSweepRange : -this.headSweepRange) * this.headSweepRangeMultiplier);
      this.golem.headIktarget.transform.position = Vector3.MoveTowards(this.golem.headIktarget.transform.position, target, this.golem.headLookSpeed * this.golem.headLookSpeedMultiplier * Time.deltaTime);
      if (!this.lockSweep && Extensions.PointInRadius(this.golem.headIktarget.position, target, 0.1f))
        this.sweepSide = !this.sweepSide;
      this.golem.eyeTransform.rotation = Quaternion.LookRotation(this.golem.headIktarget.position - this.golem.eyeTransform.transform.position, Vector3.up);
    }

    public virtual bool Allow(GolemController golem)
    {
      return base.Allow(golem) && (double) Time.time - (double) GolemArcaneHeadCast.LastBeamTime > (double) this.beamCooldownDuration && golem.IsSightable(golem.attackTarget, this.beamMaxDistance, this.beamStartMaxAngle);
    }

    public virtual void Begin(GolemController golem)
    {
      base.Begin(golem);
      GolemArcaneHeadCast.LastBeamTime = Time.time;
      ((ThunderEntity) golem).weakpoints.Add(((Component) golem.headCrystalBody).transform);
      this.OnBegin(golem);
      LookMode sweepMode = this.sweepMode;
      if (sweepMode > 1)
      {
        if (sweepMode == 2)
          this.sweepSide = false;
      }
      else
        this.sweepSide = UnityEngine.Random.Range(0, 2) != 0;
      float num = UnityEngine.Random.Range(this.beamingMinMaxDuration.x, this.beamingMinMaxDuration.y);
      golem.Deploy(num, new Action(this.OnDeployStart), new Action(this.OnDeployed), new Action(this.OnDeployEnd));
    }

    public virtual void OnBegin(GolemController golem)
    {
    }

    public virtual void OnDeployStart()
    {
      this.state = GolemArcaneHeadCast.State.Starting;
      this.golem.lookMode = this.sweepMode;
      this.lockSweep = true;
      this.golem.headLookSpeedMultiplier = this.sweepMode == null ? 0.5f : 1f;
    }

    public virtual void OnDeployed()
    {
      if (this.state != 0)
        return;
      this.lockSweep = false;
      this.state = GolemArcaneHeadCast.State.Firing;
    }

    public virtual void OnDeployEnd() => this.End();

    public virtual void OnEnd()
    {
      base.OnEnd();
      GolemArcaneHeadCast.LastBeamTime = Time.time;
      this.state = GolemArcaneHeadCast.State.Finished;
      if (this.golem.isDeployed)
        this.golem.StopDeploy();
      this.golem.headLookSpeedMultiplier = 1f;
      ((ThunderEntity) this.golem).weakpoints.Remove(((Component) this.golem.headCrystalBody).transform);
    }

    public virtual void OnCycle(float delta)
    {
      base.OnCycle(delta);
      if (this.golem.IsSightable(this.golem.attackTarget, this.beamMaxDistance, this.beamAngleHardMax))
      {
        this.beamTargetLostDuration = 0.0f;
      }
      else
      {
        this.beamTargetLostDuration += delta;
        if ((double) this.beamTargetLostDuration <= (double) this.beamStopDelayTargetLost)
          return;
        this.End();
      }
    }

    public enum State
    {
      Starting,
      Firing,
      Finished,
    }
  }
}
