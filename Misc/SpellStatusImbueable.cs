// Decompiled with JetBrains decompiler
// Type: Arcana.Misc.SpellStatusImbueable
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using ThunderRoad;

#nullable disable
namespace Arcana.Misc
{
  public class SpellStatusImbueable : SpellStatus
  {
    public SpellCastCharge imbueSpell;
    public bool isImbueStatus;

    public override void LoadCatalogData()
    {
      base.LoadCatalogData();
      this.imbueSpell = Catalog.GetData<SpellCastCharge>(this.spellId, true);
      this.statusData = Catalog.GetData<StatusData>(this.statusId, true);
    }
  }
}
