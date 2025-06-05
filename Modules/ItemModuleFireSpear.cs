// Decompiled with JetBrains decompiler
// Type: Arcana.Modules.ItemModuleFireSpear
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Behaviors;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Modules
{
  public class ItemModuleFireSpear : ItemModule
  {
    public virtual void OnItemLoaded(Item item)
    {
      base.OnItemLoaded(item);
      if ((bool) (Object) ((ThunderBehaviour) item).gameObject.GetComponent<FireSpear>())
        return;
      ((ThunderBehaviour) item).gameObject.AddComponent<FireSpear>();
    }
  }
}
