// Decompiled with JetBrains decompiler
// Type: Crystallic.Patches.CombinationMechanismPatch
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic.Patches
{
  [HarmonyPatch]
  public class CombinationMechanismPatch
  {
    [ModOption("Force Imbue Mechanisms", "Force imbues all the mechanisms in the level with Crystallc energy, for testing.")]
    [ModOptionCategory("Debug", 99)]
    [ModOptionButton]
    public static void ForceImbue(bool _)
    {
      if ((UnityEngine.Object) Level.current == (UnityEngine.Object) null || (UnityEngine.Object) Player.currentCreature == (UnityEngine.Object) null)
        return;
      if (((CatalogData) Level.current.data).id != "Tower")
      {
        Debug.LogWarning((object) "Cannot imbue mechanisms, you are not on the tower map!");
      }
      else
      {
        foreach (CombinationImbuedMechanism combinationImbuedMechanism in UnityEngine.Object.FindObjectsOfType<CombinationImbuedMechanism>(true))
        {
          foreach (ColliderGroup key in (combinationImbuedMechanism.GetType().GetField("combination", BindingFlags.Instance | BindingFlags.NonPublic).GetValue((object) combinationImbuedMechanism) as Dictionary<ColliderGroup, string>).Keys)
            key.imbue.Transfer(Catalog.GetData<SpellCastCharge>("Crystallic", true), 100f, (Creature) null);
        }
      }
    }

    private static MethodBase TargetMethod()
    {
      return (MethodBase) AccessTools.Method(typeof (CombinationImbuedMechanism), "Awake");
    }

    private static void Postfix(CombinationImbuedMechanism __instance)
    {
      Utils.RunAfter((MonoBehaviour) __instance, (Action) (() =>
      {
        if (EndingContent.GetCurrent().endingComplete || !EndingContent.GetCurrent().hasT4Skill)
          return;
        __instance.isOrderedConbination = false;
        FieldInfo field = __instance.GetType().GetField("combination", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field != (FieldInfo) null && field.GetValue((object) __instance) is Dictionary<ColliderGroup, string> dictionary2)
        {
          List<ColliderGroup> colliderGroupList = new List<ColliderGroup>();
          foreach (KeyValuePair<ColliderGroup, string> keyValuePair in dictionary2)
            colliderGroupList.Add(keyValuePair.Key);
          dictionary2.Clear();
          foreach (ColliderGroup key in colliderGroupList)
            dictionary2.Add(key, "Crystallic");
        }
      }), 5f, false);
    }
  }
}
