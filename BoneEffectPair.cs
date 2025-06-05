// Decompiled with JetBrains decompiler
// Type: Crystallic.AI.BoneEffectPair
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using System.Collections.Generic;
using ThunderRoad;

#nullable disable
namespace Crystallic.AI
{
  [Serializable]
  public class BoneEffectPair
  {
    public List<string> creatureIds;
    public Dictionary<string, EffectData> boneDataPairs = new Dictionary<string, EffectData>();
    public Dictionary<string, string> boneEffectPairs = new Dictionary<string, string>()
    {
      {
        "LeftArm",
        "UpperArmCrystallic"
      },
      {
        "RightArm",
        "UpperArmCrystallic"
      },
      {
        "LeftForeArm",
        "LowerArmCrystallic"
      },
      {
        "RightForeArm",
        "LowerArmCrystallic"
      },
      {
        "LeftUpLeg",
        "UpperLegCrystallic"
      },
      {
        "RightUpLeg",
        "UpperLegCrystallic"
      },
      {
        "LeftLeg",
        "LowerLegCrystallic"
      },
      {
        "RightLeg",
        "LowerLegCrystallic"
      },
      {
        "Spine1",
        "TorsoCrystallic"
      }
    };

    public Dictionary<string, EffectData> Load(Creature creature)
    {
      foreach (KeyValuePair<string, string> boneEffectPair in this.boneEffectPairs)
      {
        if (!string.IsNullOrEmpty(boneEffectPair.Key))
          this.boneDataPairs.Add(boneEffectPair.Key, Catalog.GetData<EffectData>(boneEffectPair.Value, true));
      }
      return this.boneDataPairs;
    }
  }
}
