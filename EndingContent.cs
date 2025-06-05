// Decompiled with JetBrains decompiler
// Type: Crystallic.EndingContent
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using ThunderRoad;

#nullable disable
namespace Crystallic
{
  public class EndingContent : ContainerContent
  {
    public bool endingComplete;
    public bool hasT4Skill;

    public EndingContent()
    {
    }

    public EndingContent(EndingContent endingContent)
    {
      this.endingComplete = endingContent.endingComplete;
      this.hasT4Skill = endingContent.hasT4Skill;
    }

    public virtual CatalogData catalogData => new CatalogData();

    public static EndingContent GetCurrent()
    {
      EndingContent current = (EndingContent) Player.local.creature.container.contents.Find((Predicate<ContainerContent>) (c => c is EndingContent));
      if (current == null)
      {
        current = new EndingContent();
        Player.local.creature.container.contents.Add((ContainerContent) current);
      }
      return current;
    }

    public virtual ContainerContent Clone() => (ContainerContent) new EndingContent(this);

    public virtual List<ValueDropdownItem<string>> DropdownOptions()
    {
      return new List<ValueDropdownItem<string>>();
    }

    public virtual string GetTypeString() => ((object) this).GetType().Name;

    public virtual bool OnCatalogRefresh() => true;
  }
}
