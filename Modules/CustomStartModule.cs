// Decompiled with JetBrains decompiler
// Type: Crystallic.Modules.CustomStartModule
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using ThunderRoad.Modules;
using UnityEngine;

#nullable disable
namespace Crystallic.Modules
{
  public class CustomStartModule : GameModeModule
  {
    public virtual IEnumerator OnLoadCoroutine()
    {
      // ISSUE: method pointer
      EventManager.onPossess += new EventManager.PossessEvent((object) this, __methodptr(OnPossess));
      return ((Module) this).OnLoadCoroutine();
    }

    public virtual void OnUnload()
    {
      ((Module) this).OnUnload();
      // ISSUE: method pointer
      EventManager.onPossess -= new EventManager.PossessEvent((object) this, __methodptr(OnPossess));
    }

    private void OnPossess(Creature creature, EventTime eventTime)
    {
      if (eventTime == null || StartContent.GetCurrent().loreFound)
        return;
      Utils.RunAfter((MonoBehaviour) creature, (Action) (() =>
      {
        LoreSpawner component = GameObject.Find("Raein Journal 3").GetComponent<LoreSpawner>();
        foreach (Item obj in Item.allActive.ToList<Item>())
        {
          if (((CatalogData) obj.data).id == "CrystalCrystallicT1")
          {
            foreach (Item inRadiu in ThunderEntity.InRadius(((ThunderBehaviour) obj).transform.position, 1f, (Func<ThunderEntity, bool>) null, (List<ThunderEntity>) null))
            {
              if (((CatalogData) inRadiu.data).id != "CrystalCrystallicT1" && ((CatalogData) inRadiu.data).id != "DaggerCommon")
                ((ThunderEntity) inRadiu).Despawn();
            }
          }
        }
        LoaderModule loaderModule;
        if (GameModeManager.instance.currentGameMode.TryGetModule<LoaderModule>(ref loaderModule) && loaderModule.SpawnLore("CrystallicStart", ((Component) component).transform.position, ((Component) component).transform.rotation))
          UnityEngine.Object.Destroy((UnityEngine.Object) component);
        else
          Debug.LogError((object) "No lore loader module found in current game mode or spawning of lore failed!");
      }), 1f, false);
    }
  }
}
