// Decompiled with JetBrains decompiler
// Type: Crystallic.Patches.AnnihilationEndingPatch
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using HarmonyLib;
using ThunderRoad;

#nullable disable
namespace Crystallic.Patches
{
  [HarmonyPatch(typeof (Tower), "StartAnnihilationEnding")]
  public class AnnihilationEndingPatch
  {
    public static bool Prefix(Tower __instance)
    {
      if (EndingContent.GetCurrent().endingComplete || !EndingContent.GetCurrent().hasT4Skill)
        return true;
      Ending.StartCrystallicEnding(__instance);
      return false;
    }
  }
}
