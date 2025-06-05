// Decompiled with JetBrains decompiler
// Type: Crystallic.Patches.RaidStopPatch
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.Modules;
using HarmonyLib;
using ThunderRoad;
using ThunderRoad.Modules;
using UnityEngine;

#nullable disable
namespace Crystallic.Patches
{
  [HarmonyPatch(typeof (HomeTower), "RaidCompleted")]
  public class RaidStopPatch
  {
    public static void Postfix(HomeTower __instance)
    {
      if (InvasionContent.GetCurrent().invasionComplete)
        return;
      InvasionModule.invasionActive = false;
      InvasionContent.GetCurrent().invasionComplete = true;
      if (InvasionModule.loopMusicEffect != null)
        InvasionModule.loopMusicEffect.End(false, -1f);
      ((MonoBehaviour) GameManager.local).StartCoroutine(InvasionModule.FadeMusic(0.0f, 1f, 5f));
      CrystalHuntProgressionModule progressionModule;
      if (GameModeManager.instance.currentGameMode.TryGetModule<CrystalHuntProgressionModule>(ref progressionModule))
        progressionModule.SetEndGameState((CrystalHuntProgressionModule.EndGameState) 0);
    }
  }
}
