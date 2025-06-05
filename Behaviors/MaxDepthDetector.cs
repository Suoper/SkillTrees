// Decompiled with JetBrains decompiler
// Type: Arcana.Behaviors.MaxDepthDetector
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Behaviors
{
  public class MaxDepthDetector : ThunderBehaviour
  {
    public Damager damager;
    public bool active;
    public bool hasReachedMaxDepth;
    public float depthRequirementRatio;
    public float eventResetRatio;
    public bool requireUnpenetrateToReset;

    public event MaxDepthDetector.OnPenetrateMaxDepth OnPenetrateMaxDepthEvent;

    public virtual ManagedLoops EnabledManagedLoops => (ManagedLoops) 2;

    public void Activate(
      Damager damager,
      float depthRequirementRatio = 0.9f,
      float eventResetRatio = 0.2f,
      bool requireUnpenetrateToReset = false)
    {
      this.damager = damager;
      this.depthRequirementRatio = depthRequirementRatio;
      this.eventResetRatio = eventResetRatio;
      this.requireUnpenetrateToReset = requireUnpenetrateToReset;
      // ISSUE: method pointer
      damager.OnUnpenetrateEvent -= new Damager.DepenetrationEvent((object) this, __methodptr(OnUnpenetrateEvent));
      // ISSUE: method pointer
      damager.OnUnpenetrateEvent += new Damager.DepenetrationEvent((object) this, __methodptr(OnUnpenetrateEvent));
      this.active = true;
    }

    public void Deactivate()
    {
      // ISSUE: method pointer
      this.damager.OnUnpenetrateEvent -= new Damager.DepenetrationEvent((object) this, __methodptr(OnUnpenetrateEvent));
      this.active = false;
    }

    protected virtual float GetTriggerThreshold(float maxDepth)
    {
      return maxDepth * this.depthRequirementRatio;
    }

    protected virtual float GetResetThreshold(float maxDepth)
    {
      return this.GetTriggerThreshold(maxDepth) * this.eventResetRatio;
    }

    protected virtual void ManagedUpdate()
    {
      base.ManagedUpdate();
      if (!this.active || (Object) this.damager == (Object) null)
        return;
      foreach (CollisionInstance collision in this.damager.collisionHandler.collisions)
      {
        if ((Object) collision.damageStruct.damager == (Object) this.damager && collision.damageStruct.penetration > 0)
        {
          if (!this.hasReachedMaxDepth)
          {
            if ((double) collision.damageStruct.lastDepth >= (double) this.GetTriggerThreshold(this.damager.penetrationDepth))
            {
              this.hasReachedMaxDepth = true;
              MaxDepthDetector.OnPenetrateMaxDepth penetrateMaxDepthEvent = this.OnPenetrateMaxDepthEvent;
              if (penetrateMaxDepthEvent != null)
                penetrateMaxDepthEvent(this.damager, collision, ((ThunderEntity) this.damager.collisionHandler.item).Velocity, collision.damageStruct.lastDepth);
            }
          }
          else if (!this.requireUnpenetrateToReset && (double) collision.damageStruct.lastDepth < (double) this.GetResetThreshold(this.damager.penetrationDepth))
            this.hasReachedMaxDepth = false;
        }
      }
    }

    private void OnUnpenetrateEvent(
      Damager damager,
      CollisionInstance collision,
      bool wentthrough,
      EventTime time)
    {
      this.hasReachedMaxDepth = false;
    }

    public delegate void OnPenetrateMaxDepth(
      Damager damager,
      CollisionInstance collision,
      Vector3 velocity,
      float depth);
  }
}
