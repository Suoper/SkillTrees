// Decompiled with JetBrains decompiler
// Type: ThunderEntityExtensions
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using ThunderRoad;
using UnityEngine;

#nullable disable
public static class ThunderEntityExtensions
{
  public static void Despawn(this ThunderEntity entity, float time)
  {
    Utils.RunAfter((MonoBehaviour) entity, (Action) (() => entity.Despawn()), time, false);
  }
}
