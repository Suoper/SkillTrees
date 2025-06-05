// Decompiled with JetBrains decompiler
// Type: Crystallic.Patches.SkillTreeOrbPatch
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using HarmonyLib;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic.Patches
{
  public class SkillTreeOrbPatch
  {
    [HarmonyPatch(typeof (SkillTreeOrb), "Init")]
    [HarmonyPostfix]
    public static void SkillOrbInitPostfix(SkillTreeOrb __instance)
    {
      __instance.distanceForceRelease = Mathf.Max((float) (((double) __instance.skillTree.maxTierInTree - 3.0) * 0.60000002384185791 + 3.0), 3f);
    }
  }
}
