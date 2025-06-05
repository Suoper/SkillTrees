// Decompiled with JetBrains decompiler
// Type: Arcana.Behaviors.SpellPunchDetector
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Behaviors
{
  public class SpellPunchDetector : ThunderBehaviour
  {
    public bool active;
    public float lastPunchTime;
    public float minSqrMagnitude;
    public float maxSqrMagnitude;
    public float punchStopWindow;
    public Vector3 lastVelocity;
    public SpellCastCharge spell;
    public RagdollHand ragdollHand;

    public event SpellPunchDetector.OnSpellPunch OnSpellPunchEvent;

    public virtual ManagedLoops EnabledManagedLoops => (ManagedLoops) 2;

    public void StartTracking(
      SpellCastCharge spell,
      float minSqrMagnitude,
      float maxSqrMagnitude,
      float punchStopWindow)
    {
      this.spell = spell;
      this.minSqrMagnitude = minSqrMagnitude;
      this.maxSqrMagnitude = maxSqrMagnitude;
      this.punchStopWindow = punchStopWindow;
      this.lastPunchTime = Time.time;
      this.lastVelocity = Vector3.zero;
      this.active = true;
      this.ragdollHand = spell.spellCaster.ragdollHand;
    }

    public void StopTracking() => this.active = false;

    protected virtual void ManagedUpdate()
    {
      base.ManagedUpdate();
      if (!this.spell.spellCaster.isFiring || !this.active || !this.spell.Ready)
      {
        this.lastPunchTime = 0.0f;
      }
      else
      {
        Vector3 vector3 = this.ragdollHand.Velocity();
        float sqrMagnitude = vector3.sqrMagnitude;
        if ((double) sqrMagnitude >= (double) this.minSqrMagnitude)
        {
          this.lastPunchTime = Time.realtimeSinceStartup;
          this.lastVelocity = vector3;
        }
        else
        {
          if ((double) sqrMagnitude > (double) this.maxSqrMagnitude || (double) Time.realtimeSinceStartup - (double) this.lastPunchTime >= (double) this.punchStopWindow)
            return;
          SpellPunchDetector.OnSpellPunch onSpellPunchEvent = this.OnSpellPunchEvent;
          if (onSpellPunchEvent != null)
            onSpellPunchEvent(this.spell, this.lastVelocity);
        }
      }
    }

    public delegate void OnSpellPunch(SpellCastCharge spell, Vector3 velocity);
  }
}
