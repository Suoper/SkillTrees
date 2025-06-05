// Decompiled with JetBrains decompiler
// Type: Crystallic.Modules.ObeliskModule
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Modules;
using UnityEngine;

#nullable disable
namespace Crystallic.Modules
{
  public class ObeliskModule : GameModeModule
  {
    [ModOption("Play Obelisk Vfx", "Controls whether placing a crystal in the obelisk plays custom Vfx or not.")]
    [ModOptionCategory("Obelisk", 1)]
    public static bool playObeliskVfx = true;
    protected EffectData coreEffectData;
    public string coreEffectId;
    protected EffectData effectData;
    public string effectId;
    protected List<EffectInstance> effectInstances = new List<EffectInstance>();
    public string levelId;
    protected List<SkillTreeReceptacle> skillTreeReceptacles = new List<SkillTreeReceptacle>();
    public List<string> triggerItemIds = new List<string>();

    public virtual IEnumerator OnLoadCoroutine()
    {
      // ISSUE: method pointer
      EventManager.onLevelLoad += new EventManager.LevelLoadEvent((object) this, __methodptr(OnLevelLoad));
      // ISSUE: method pointer
      EventManager.onLevelUnload += new EventManager.LevelLoadEvent((object) this, __methodptr(OnLevelUnload));
      this.effectData = Catalog.GetData<EffectData>(this.effectId, true);
      this.coreEffectData = Catalog.GetData<EffectData>(this.coreEffectId, true);
      return ((Module) this).OnLoadCoroutine();
    }

    private void OnLevelLoad(LevelData levelData, LevelData.Mode mode, EventTime eventTime)
    {
      if (eventTime != 1 || !(((CatalogData) levelData).id == this.levelId))
        return;
      foreach (SkillTreeReceptacle receptacle in SkillTree.instance.receptacles)
      {
        // ISSUE: method pointer
        receptacle.itemMagnet.OnItemCatchEvent += new ItemMagnet.ItemEvent((object) this, __methodptr(OnItemCatchEvent));
        // ISSUE: method pointer
        receptacle.itemMagnet.OnItemReleaseEvent += new ItemMagnet.ItemEvent((object) this, __methodptr(OnItemReleaseEvent));
        this.skillTreeReceptacles.Add(receptacle);
      }
    }

    private void OnLevelUnload(LevelData levelData, LevelData.Mode mode, EventTime eventTime)
    {
      if (eventTime == 1 && ((CatalogData) levelData).id == this.levelId)
      {
        foreach (SkillTreeReceptacle skillTreeReceptacle in this.skillTreeReceptacles)
        {
          // ISSUE: method pointer
          skillTreeReceptacle.itemMagnet.OnItemCatchEvent -= new ItemMagnet.ItemEvent((object) this, __methodptr(OnItemCatchEvent));
          // ISSUE: method pointer
          skillTreeReceptacle.itemMagnet.OnItemReleaseEvent -= new ItemMagnet.ItemEvent((object) this, __methodptr(OnItemReleaseEvent));
        }
      }
      this.skillTreeReceptacles.Clear();
    }

    private void OnItemCatchEvent(Item item, EventTime time)
    {
      if (!this.triggerItemIds.Contains(((CatalogData) item.data).id) || time == null || !ObeliskModule.playObeliskVfx)
        return;
      foreach (SkillTreeReceptacle receptacle in SkillTree.instance.receptacles)
      {
        if (!item.magnets.Contains(receptacle.itemMagnet))
        {
          EffectInstance effectInstance1 = this.coreEffectData.Spawn(((ThunderBehaviour) SkillTree.instance.shardReceptacle).transform, true, (ColliderGroup) null, false);
          this.effectInstances.Add(effectInstance1);
          effectInstance1.Play(0, false, false);
          EffectInstance effectInstance2 = this.effectData.Spawn(((Component) receptacle.itemMagnet).transform, true, (ColliderGroup) null, false);
          this.effectInstances.Add(effectInstance2);
          effectInstance2.Play(0, false, false);
        }
      }
      if (time != 1)
        return;
      foreach (SkillTreeReceptacle receptacle in SkillTree.instance.receptacles)
      {
        foreach (SkillTree.SkillTreeTierNode mainTierNode in receptacle.mainTierNodes)
        {
          if ((Object) mainTierNode.skillOrbTierBlocker != (Object) null && mainTierNode.skillOrbTierBlocker.skillData.isTierBlocker)
          {
            CapsuleCollider mainCollider = mainTierNode?.skillOrbTierBlocker?.mainCollider as CapsuleCollider;
            if ((Object) mainCollider != (Object) null)
            {
              mainCollider.radius = 0.025f;
              ((Interactable) mainTierNode.skillOrbTierBlocker.handle).touchRadius = 0.3f;
            }
          }
        }
      }
    }

    private void OnItemReleaseEvent(Item item, EventTime time)
    {
      if (!this.triggerItemIds.Contains(((CatalogData) item.data).id) || time != 1)
        return;
      foreach (EffectInstance effectInstance in this.effectInstances)
        effectInstance.End(false, -1f);
    }
  }
}
