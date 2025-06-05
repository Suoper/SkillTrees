// Decompiled with JetBrains decompiler
// Type: Arcana.Golem.GolemAbilityLoader
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Modules;

#nullable disable
namespace Arcana.Golem
{
  public class GolemAbilityLoader : GameModeModule
  {
    public List<GolemAbility> abilities;

    public virtual IEnumerator OnLoadCoroutine()
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) this.\u003C\u003En__0();
      ThunderRoad.Golem.OnLocalGolemSet += new Action(this.OnLocalGolemSet);
    }

    private void OnLocalGolemSet()
    {
      if (this.abilities == null)
        return;
      ((GolemController) ThunderRoad.Golem.local).abilities.AddRange((IEnumerable<GolemAbility>) this.abilities);
    }
  }
}
