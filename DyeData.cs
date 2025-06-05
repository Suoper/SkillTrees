// Decompiled with JetBrains decompiler
// Type: Crystallic.DyeData
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic
{
  [Serializable]
  public class DyeData : CustomData
  {
    public string spellId;
    public Color color;
    public List<DyeMixture> dyeMixtures;
  }
}
