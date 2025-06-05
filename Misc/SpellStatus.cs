// Decompiled with JetBrains decompiler
// Type: Arcana.Misc.SpellStatus
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Misc
{
  public class SpellStatus
  {
    public string spellId;
    public SpellCastData spellData;
    public string statusId;
    public StatusData statusData;
    public float statusDuration;
    public float? statusParameter;

    public virtual void LoadCatalogData()
    {
      this.spellData = Catalog.GetData<SpellCastData>(this.spellId, true);
      this.statusData = Catalog.GetData<StatusData>(this.statusId, true);
      Debug.Log((object) string.Format("Arcana - Spell Status - Loaded Spell {0} with Status {1}\n\t(Duration: {2} | Parameter: {3})", (object) this.spellId, (object) this.statusId, (object) this.statusDuration, (object) this.statusParameter));
    }
  }
}
