// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.MaxDepthDetector
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class MaxDepthDetector : MonoBehaviour
  {
    public Damager damager;
    public bool active;
    public bool hasReachedMaxDepth;
    public Vector2 allowance;

    public void Update()
    {
      if (!this.active || !((Object) this.damager != (Object) null))
        return;
      foreach (CollisionInstance collision in this.damager.collisionHandler.collisions)
      {
        if ((Object) collision.damageStruct.damager == (Object) this.damager && collision.damageStruct.penetration > 0)
        {
          if ((double) collision.damageStruct.lastDepth >= (double) this.damager.penetrationDepth - (double) this.allowance.x && !this.hasReachedMaxDepth)
          {
            this.hasReachedMaxDepth = true;
            MaxDepthDetector.OnPenetrateMaxDepth penetrateMaxDepth = this.onPenetrateMaxDepth;
            if (penetrateMaxDepth != null)
              penetrateMaxDepth(this.damager, collision, ((ThunderEntity) this.damager.collisionHandler.item).Velocity, collision.damageStruct.lastDepth);
          }
          else if (this.hasReachedMaxDepth && (double) collision.damageStruct.lastDepth + (double) this.allowance.y < (double) this.damager.penetrationDepth)
            this.hasReachedMaxDepth = false;
        }
      }
    }

    public event MaxDepthDetector.OnPenetrateMaxDepth onPenetrateMaxDepth;

    public void Activate(Damager damager, Vector2 allowance)
    {
      this.active = true;
      this.allowance = allowance;
      this.damager = damager;
    }

    public void Deactivate() => this.active = false;

    public delegate void OnPenetrateMaxDepth(
      Damager damager,
      CollisionInstance collisionInstance,
      Vector3 velocity,
      float depth);
  }
}
