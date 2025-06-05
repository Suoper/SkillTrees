// Decompiled with JetBrains decompiler
// Type: Crystallic.Crystallised
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic
{
  public class Crystallised : Status
  {
    public virtual void Apply()
    {
      base.Apply();
      if (!(this.entity is Creature entity) || !(bool) (Object) entity)
        return;
      entity.brain.instance.GetModule<BrainModuleCrystal>(true).Crystallise(5f, "Lightning");
    }
  }
}
