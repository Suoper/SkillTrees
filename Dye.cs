// Decompiled with JetBrains decompiler
// Type: Crystallic.Dye
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic
{
  public class Dye : ThunderScript
  {
    public static List<DyeData> dyeData = new List<DyeData>();

    public virtual void ScriptEnable()
    {
      base.ScriptEnable();
      // ISSUE: method pointer
      EventManager.onLevelLoad += new EventManager.LevelLoadEvent((object) this, __methodptr(OnLevelLoad));
    }

    private void OnLevelLoad(LevelData levelData, LevelData.Mode mode, EventTime eventTime)
    {
      if (eventTime != 1)
        return;
      Dye.Load();
    }

    public static void Load()
    {
      Dye.dyeData.Clear();
      List<string> values1 = new List<string>();
      Dye.dyeData = Catalog.GetDataList<DyeData>();
      for (int index = 0; index < Dye.dyeData.Count; ++index)
      {
        DyeData dyeData = Dye.dyeData[index];
        if (Settings.debug)
        {
          List<string> values2 = new List<string>();
          foreach (DyeMixture dyeMixture in dyeData.dyeMixtures)
            values2.Add(string.Join(", ", dyeMixture.mixSpellId) + " + " + string.Join(", ", (object) dyeMixture.mixColor));
          values1.Add(string.Format("{0}  \n    - SpellId: {1}  \n    - Color: {2}  \n    - Mixes:\n       - {3}", (object) ((CatalogData) dyeData).id, (object) dyeData.spellId, (object) dyeData.color, (object) string.Join("   \n       - ", (IEnumerable<string>) values2)));
        }
        else
          values1.Add(((CatalogData) dyeData).id);
      }
      Debug.Log((object) ((!Settings.debug ? "" : "Stack Trace: \n" + Environment.StackTrace + "\n") + "Loaded Dye Data:\n - " + string.Join("\n - ", (IEnumerable<string>) values1)));
    }

    public static bool TryGetDye(string id, out DyeData dyeData)
    {
      dyeData = Dye.FindDyeDataById(id);
      return dyeData != null;
    }

    public static DyeData GetDye(string id)
    {
      DyeData dyeDataById = Dye.FindDyeDataById(id);
      if (dyeDataById == null)
        Debug.LogError((object) ("Dye with ID " + id + " not found."));
      return dyeDataById;
    }

    public static bool TryGetDyeDataFromColorAndSpell(
      Color color,
      string spellId,
      out DyeData dyeData)
    {
      foreach (DyeData dyeData1 in Dye.dyeData)
      {
        if (dyeData1.dyeMixtures.FirstOrDefault<DyeMixture>((Func<DyeMixture, bool>) (m => m.mixColor == color && m.mixSpellId == spellId)) != null)
        {
          dyeData = dyeData1;
          return true;
        }
      }
      dyeData = (DyeData) null;
      return false;
    }

    public static Color GetEvaluatedColor(string originSpellId, string hitSpellId)
    {
      Color evaluatedColor = new Color(1f, 1f, 1f, 1f);
      foreach (DyeData dyeData in Dye.dyeData)
      {
        if (dyeData.spellId == originSpellId)
        {
          if (dyeData.spellId == hitSpellId)
            return dyeData.color;
          foreach (DyeMixture dyeMixture in dyeData.dyeMixtures)
          {
            if (dyeMixture.mixSpellId == hitSpellId)
            {
              evaluatedColor = dyeMixture.mixColor;
              if (Settings.debug)
                Debug.Log((object) ("Stack Trace: \n" + Environment.StackTrace + "\nMatch found:\n" + string.Format("- Color: {0}\n", (object) evaluatedColor) + "- Json: " + ((CatalogData) dyeData).id + "\n- Spell: " + dyeData.spellId + "\n- Mix: " + dyeMixture.mixSpellId));
            }
          }
        }
      }
      return evaluatedColor;
    }

    public static ColorType GetColorType(string originSpellId, string hitSpellId)
    {
      foreach (DyeData dyeData in Dye.dyeData)
      {
        if (dyeData.spellId == originSpellId)
        {
          if (dyeData.spellId == hitSpellId)
            return ColorType.Solid;
          foreach (DyeMixture dyeMixture in dyeData.dyeMixtures)
          {
            if (dyeMixture.mixSpellId == hitSpellId)
              return ColorType.Mix;
          }
        }
      }
      return ColorType.Solid;
    }

    private static DyeData FindDyeDataById(string id)
    {
      return Dye.dyeData.FirstOrDefault<DyeData>((Func<DyeData, bool>) (d => ((CatalogData) d).id == id));
    }
  }
}
