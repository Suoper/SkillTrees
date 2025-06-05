// Decompiled with JetBrains decompiler
// Type: Crystallic.Modules.GolemHandlerModule
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Modules;

#nullable disable
namespace Crystallic.Modules
{
  public class GolemHandlerModule : GameModeModule
  {
    public List<string> moduleAddresses = new List<string>();

    public virtual IEnumerator OnLoadCoroutine()
    {
      Golem.OnLocalGolemSet += new Action(this.OnLocalGolemSet);
      return ((Module) this).OnLoadCoroutine();
    }

    public virtual void OnUnload()
    {
      ((Module) this).OnUnload();
      Golem.OnLocalGolemSet -= new Action(this.OnLocalGolemSet);
    }

    private void OnLocalGolemSet()
    {
      ((GolemController) Golem.local).Brain().Initialize(this.moduleAddresses);
    }
  }
}
