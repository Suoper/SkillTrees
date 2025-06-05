// Decompiled with JetBrains decompiler
// Type: Crystallic.Modules.LoaderModule
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ThunderRoad;
using ThunderRoad.Modules;
using UnityEngine;

#nullable disable
namespace Crystallic.Modules
{
  public class LoaderModule : GameModeModule
  {
    public bool loaded;
    public List<LorePack> lorePacks;

    public void LogAllLore()
    {
      int num = 0;
      Debug.Log((object) ("Custom lore loader module located in instance: " + ((CatalogData) GameModeManager.instance.currentGameMode).id + " Implementing lore packs: "));
      for (int index = 0; index < this.lorePacks.Count; ++index)
      {
        num = index;
        Debug.Log((object) ("Lore pack: " + this.lorePacks[index].packId + " found for lore group: " + this.lorePacks[index].groupId));
      }
      Debug.Log((object) ("Total lore pack load count: " + (num + 1).ToString() + ". Expected count: " + this.lorePacks.Count.ToString()));
    }

    public bool SpawnLore(string lorePackId, Vector3 position, Quaternion rotation)
    {
      for (int index = 0; index < this.lorePacks.Count; ++index)
      {
        LorePack pack = this.lorePacks[index];
        if (pack.packId == lorePackId)
        {
          ItemData itemData;
          if (!Catalog.TryGetData<ItemData>(pack.lore[0].itemId, ref itemData, true))
            return false;
          itemData.SpawnAsync((Action<Item>) (item => this.ItemSpawn(item, this.gameMode.GetModule<LoreModule>(), pack)), new Vector3?(position), new Quaternion?(rotation), (Transform) null, true, (List<ContentCustomData>) null, (Item.Owner) 0);
          return true;
        }
      }
      return false;
    }

    private void ItemSpawn(Item item, LoreModule module, LorePack pack)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      LoaderModule.\u003C\u003Ec__DisplayClass4_0 cDisplayClass40 = new LoaderModule.\u003C\u003Ec__DisplayClass4_0()
      {
        item = item,
        pack = pack,
        module = module
      };
      // ISSUE: reference to a compiler-generated field
      cDisplayClass40.item.DisallowDespawn = true;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ((ThunderBehaviour) cDisplayClass40.item).transform.GetComponentInChildren<ILoreDisplay>().SetMultipleLore(cDisplayClass40.module, (LoreSpawner) null, cDisplayClass40.pack.lore);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      cDisplayClass40.item.OnGrabEvent += new Item.GrabDelegate((object) cDisplayClass40, __methodptr(\u003CItemSpawn\u003Eg__MarkAsRead\u007C0));
    }

    public virtual IEnumerator OnLoadCoroutine()
    {
      this.LogAllLore();
      if (!this.loaded && this.lorePacks != null)
      {
        this.loaded = true;
        LoreModule module;
        if (this.TryGetLore(out module))
        {
          Dictionary<string, LoreScriptableObject> currentLoreDict = this.GetCurrentLoreDict();
          foreach (LorePack lorePack1 in this.lorePacks)
          {
            LoreScriptableObject scriptableObject;
            LorePack lorePack2;
            if (currentLoreDict.TryGetValue(lorePack1.groupId, out scriptableObject))
            {
              LoreScriptableObject.LorePack lorePack3 = lorePack1.ToLorePack();
              LoreScriptableObject.LorePack lorePack4;
              if (lorePack3 != null)
              {
                List<LoreScriptableObject.LorePack> lorePackList1 = new List<LoreScriptableObject.LorePack>((IEnumerable<LoreScriptableObject.LorePack>) scriptableObject.allLorePacks)
                {
                  lorePack3
                };
                scriptableObject.rootLoreHashIds.Add(lorePack3.hashId);
                typeof (LoreScriptableObject).GetField("_hashIdToLorePack", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue((object) scriptableObject, (object) null);
                scriptableObject.allLorePacks = lorePackList1.ToArray();
                lorePack4 = (LoreScriptableObject.LorePack) null;
                List<LoreScriptableObject.LorePack> lorePackList2 = (List<LoreScriptableObject.LorePack>) null;
                lorePack2 = (LorePack) null;
                lorePackList2 = (List<LoreScriptableObject.LorePack>) null;
              }
              lorePack4 = (LoreScriptableObject.LorePack) null;
            }
            lorePack2 = (LorePack) null;
            scriptableObject = (LoreScriptableObject) null;
          }
          module.InitLoreState();
        }
      }
      return ((Module) this).OnLoadCoroutine();
    }

    public bool TryGetLore(out LoreModule module)
    {
      return GameModeManager.instance.currentGameMode.TryGetModule<LoreModule>(ref module);
    }

    public List<int> GetAvailableLore()
    {
      LoreModule module;
      return this.TryGetLore(out module) ? module.availableLore : (List<int>) null;
    }

    public List<LoreScriptableObject> GetAllLoreScriptableObjects()
    {
      LoreModule module;
      return this.TryGetLore(out module) ? module.allLoreSO : (List<LoreScriptableObject>) null;
    }

    public Dictionary<string, LoreScriptableObject> GetCurrentLoreDict()
    {
      List<LoreScriptableObject> scriptableObjects = this.GetAllLoreScriptableObjects();
      if (scriptableObjects == null)
        return (Dictionary<string, LoreScriptableObject>) null;
      Dictionary<string, LoreScriptableObject> currentLoreDict = new Dictionary<string, LoreScriptableObject>();
      for (int index = 0; index < scriptableObjects.Count; ++index)
      {
        LoreScriptableObject.LorePack[] allLorePacks = scriptableObjects[index].allLorePacks;
        string key = (string) null;
        int num;
        if (allLorePacks != null && allLorePacks.Length != 0)
        {
          key = allLorePacks[0].groupId;
          num = key == null ? 1 : 0;
        }
        else
          num = 1;
        if (num == 0)
          currentLoreDict[key] = scriptableObjects[index];
      }
      return currentLoreDict;
    }
  }
}
