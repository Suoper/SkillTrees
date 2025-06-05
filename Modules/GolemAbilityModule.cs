// Decompiled with JetBrains decompiler
// Type: Crystallic.Modules.GolemAbilityModule
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Modules;

#nullable disable
namespace Crystallic.Modules
{
  public class GolemAbilityModule : GameModeModule
  {
    public List<GolemAbility> abilities;

    public virtual IEnumerator OnLoadCoroutine()
    {
      Golem.OnLocalGolemSet += new Action(this.OnLocalGolemSet);
      return ((Module) this).OnLoadCoroutine();
    }

    private void OnLocalGolemSet()
    {
      if (Utils.IsNullOrEmpty((ICollection) this.abilities))
        return;
      ((GolemController) Golem.local).abilities.AddRange((IEnumerable<GolemAbility>) this.abilities);
    }
  }
}
