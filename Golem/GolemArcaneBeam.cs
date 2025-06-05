// Decompiled with JetBrains decompiler
// Type: Arcana.Golem.GolemArcaneBeam
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Misc;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Golem
{
  public class GolemArcaneBeam : GolemArcaneHeadCast
  {
    public BeamManager.BeamData beamData;
    public int beamCount = 5;
    public float beamSpreadRadius = 1f;
    public float beamSpreadAngle = 200f;
    public float forwardOffset = 0.5f;
    public BeamManager.BeamOrigin beamType = BeamManager.BeamOrigin.FloatingLooseConverge;
    public BeamManager[] beamManagers;

    public override void OnBegin(GolemController golem)
    {
      base.OnBegin(golem);
      if (this.beamManagers == null || this.beamManagers.Length != this.beamCount)
        this.beamManagers = new BeamManager[this.beamCount];
      for (int index = 0; index < this.beamCount; ++index)
      {
        this.beamManagers[index] = new BeamManager((Mana) null, this.beamData, this.beamType, index == 0);
        this.beamManagers[index].Activate();
      }
    }

    public override void OnDeployStart()
    {
      base.OnDeployStart();
      foreach (BeamManager beamManager in this.beamManagers)
        beamManager.StartBeamOriginEffect();
    }

    public override void OnEnd()
    {
      base.OnEnd();
      foreach (BeamManager beamManager in this.beamManagers)
        beamManager.Deactivate();
      this.UpdateBeams(false);
    }

    public virtual void OnUpdate()
    {
      base.OnUpdate();
      switch (this.state)
      {
        case GolemArcaneHeadCast.State.Starting:
        case GolemArcaneHeadCast.State.Finished:
          this.UpdateBeams(false);
          break;
        case GolemArcaneHeadCast.State.Firing:
          this.UpdateBeams(true);
          break;
      }
    }

    public void UpdateBeams(bool firing)
    {
      Vector3 forward = this.golem.eyeTransform.forward;
      for (int index = 0; index < this.beamManagers.Length; ++index)
      {
        if (this.beamCount > 1)
          this.beamManagers[index].beamOrigin = new Vector3?(Utilities.GetPointOnArc(this.golem.eyeTransform.position, forward, index, this.beamManagers.Length, this.beamSpreadRadius, this.beamSpreadAngle) + forward.normalized * this.forwardOffset);
        this.beamManagers[index].castOriginOverride = new Vector3?(this.golem.eyeTransform.position);
        this.beamManagers[index].castDirectionOverride = new Vector3?(forward);
        this.beamManagers[index].UpdateBeam(firing, 1f);
      }
    }
  }
}
