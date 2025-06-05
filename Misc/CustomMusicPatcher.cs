// Decompiled with JetBrains decompiler
// Type: Arcana.Misc.CustomMusicPatcher
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Misc
{
  [HarmonyPatch(typeof (SkillTree), "LoadMusicCoroutine")]
  public class CustomMusicPatcher
  {
    public static IEnumerator GetEnumerator(SkillTree instance)
    {
      SynchronousMusicPlayer musicPlayer;
      List<AudioClip> musicClips;
      Dictionary<int, int> treeToMusicTrack;
      if (instance.TryGetPrivate<SynchronousMusicPlayer>("musicPlayer", out musicPlayer) && instance.TryGetPrivate<List<AudioClip>>("musicClips", out musicClips) && instance.TryGetPrivate<Dictionary<int, int>>("treeToMusicTrack", out treeToMusicTrack))
      {
        List<SkillTreeData> allTrees = Catalog.GetDataList<SkillTreeData>();
        int index = 0;
        for (int i = 0; i < allTrees.Count; ++i)
        {
          if (string.IsNullOrEmpty(allTrees[i].musicAddress))
          {
            Debug.LogWarning((object) ("Skill Tree " + ((CatalogData) allTrees[i]).id + " has a null or empty musicAddress!"));
          }
          else
          {
            AudioClip audioClip = (AudioClip) null;
            yield return (object) Catalog.LoadAssetCoroutine<AudioClip>(allTrees[i].musicAddress, (Action<AudioClip>) (value => audioClip = value), "SkillTree");
            if ((UnityEngine.Object) audioClip != (UnityEngine.Object) null)
            {
              if (((UnityEngine.Object) audioClip).name != null)
                Debug.Log((object) string.Format("Adding track {0} at index {1}", (object) ((UnityEngine.Object) audioClip).name, (object) index));
              else
                Debug.Log((object) "Audio clip doesnt have a name!");
              musicClips.Add(audioClip);
              treeToMusicTrack[((CatalogData) allTrees[i]).hashId] = index++;
            }
            else
              Debug.LogError((object) "Failed to load audio clip asset!");
          }
        }
        musicPlayer.LoadClips(musicClips);
        musicPlayer.Play();
      }
    }

    public static bool Prefix(SkillTree __instance, ref IEnumerator __result)
    {
      Debug.Log((object) "Patched method run!");
      __result = CustomMusicPatcher.GetEnumerator(__instance);
      return false;
    }
  }
}
