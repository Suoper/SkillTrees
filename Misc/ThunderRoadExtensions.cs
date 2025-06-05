// Decompiled with JetBrains decompiler
// Type: Arcana.Misc.ThunderRoadExtensions
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using ThunderRoad;

#nullable disable
namespace Arcana.Misc
{
  internal static class ThunderRoadExtensions
  {
    public static Side Other(this Side? side)
    {
      return side.GetValueOrDefault() == 1 ? (Side) 0 : (Side) 1;
    }
  }
}
