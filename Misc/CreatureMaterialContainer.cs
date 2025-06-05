// Decompiled with JetBrains decompiler
// Type: Arcana.Misc.CreatureMaterialContainer
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Misc
{
  public class CreatureMaterialContainer
  {
    public Dictionary<Renderer, Material[]> renderers = new Dictionary<Renderer, Material[]>();
    public Dictionary<MaterialInstance, Material[]> materialInstances = new Dictionary<MaterialInstance, Material[]>();
    public Material bodyMaterialLOD0;
    public Material bodyMaterialLOD1;
    public Material handMaterialLOD0;
    public Material handMaterialLOD1;

    public CreatureMaterialContainer(Creature creature)
    {
      this.bodyMaterialLOD0 = creature.currentEthnicGroup.bodyMaterialLod0;
      this.bodyMaterialLOD1 = creature.currentEthnicGroup.bodyMaterialLod0;
      this.handMaterialLOD0 = creature.currentEthnicGroup.handsMaterialLod0;
      this.handMaterialLOD1 = creature.currentEthnicGroup.handsMaterialLod1;
    }
  }
}
