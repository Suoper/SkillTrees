// Decompiled with JetBrains decompiler
// Type: Crystallic.LorePack
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ThunderRoad;

#nullable disable
namespace Crystallic
{
  public class LorePack
  {
    public string contentAddress;
    public string groupId;
    public string itemId;
    public LorePackCondition.LoreLevelOptionCondition[] levelOptionConditions;
    public List<LoreScriptableObject.LoreData> lore;
    public string packId;
    public List<string> prerequisites;
    public LoreScriptableObject.LoreType type;
    public List<LorePackCondition.Visibility> visibilityConditions;

    public LoreScriptableObject.LorePack ToLorePack()
    {
      if (this.lore == null)
        return (LoreScriptableObject.LorePack) null;
      for (int index = 0; index < this.lore.Count; ++index)
      {
        this.lore[index].groupId = this.groupId;
        this.lore[index].displayGraphicsInJournal = false;
        LoreScriptableObject.LoreData loreData = this.lore[index];
        if (loreData.itemId == null)
          loreData.itemId = this.itemId;
        this.lore[index].loreType = this.type;
        this.lore[index].contentAddress = this.contentAddress;
      }
      LorePackCondition uninitializedObject = (LorePackCondition) FormatterServices.GetUninitializedObject(typeof (LorePackCondition));
      uninitializedObject.visibilityRequired = this.visibilityConditions;
      uninitializedObject.levelOptions = this.levelOptionConditions;
      uninitializedObject.requiredParameters = Array.Empty<string>();
      List<int> intList = new List<int>();
      List<string> prerequisites = this.prerequisites;
      if (prerequisites != null && prerequisites.Count > 0)
      {
        for (int index = 0; index < this.prerequisites.Count; ++index)
          intList.Add(LoreScriptableObject.GetLoreHashId(prerequisites[index]));
      }
      return new LoreScriptableObject.LorePack()
      {
        groupId = this.groupId,
        nameId = this.packId,
        hashId = LoreScriptableObject.GetLoreHashId(this.packId),
        loreData = this.lore,
        lorePackCondition = uninitializedObject,
        loreRequirement = intList,
        spawnPackAsOneItem = this.lore.Count > 1
      };
    }
  }
}
