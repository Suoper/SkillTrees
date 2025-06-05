// Decompiled with JetBrains decompiler
// Type: Crystallic.LockMovement
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using ThunderRoad;

#nullable disable
namespace Crystallic
{
  public class LockMovement : Status
  {
    public virtual void Apply()
    {
      base.Apply();
      if (!(this.entity is Creature entity) || !entity.isPlayer)
        return;
      ((ValueHandler<float>) entity.currentLocomotion.globalMoveSpeedMultiplier).Add((object) this, 0.0f);
      ((ValueHandler<float>) entity.mana.chargeSpeedMult).Add((object) this, 0.1f);
    }

    public virtual void Remove()
    {
      base.Remove();
      if (!(this.entity is Creature entity) || !entity.isPlayer)
        return;
      ((ValueHandler<float>) entity.currentLocomotion.globalMoveSpeedMultiplier).Remove((object) this);
      ((ValueHandler<float>) entity.mana.chargeSpeedMult).Remove((object) this);
    }
  }
}
