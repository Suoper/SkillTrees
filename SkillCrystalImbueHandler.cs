// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillCrystalImbueHandler
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillCrystalImbueHandler : SpellSkillData
  {
    public string classAddress;
    public Color colorModifier;
    public bool crystallise;
    public float crystalliseDuration;
    public string imbueEffectId;
    public string imbueHitEffectId;
    public Vector2 minMaxImpactVelocity;
    public string spellId;
    public Type type;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.type = Type.GetType(this.classAddress);
    }

    public virtual void OnImbueLoad(SpellData spell, Imbue imbue)
    {
      base.OnImbueLoad(spell, imbue);
      if (((CatalogData) spell)?.id != this.spellId || this.type == (Type) null)
        return;
      Debug.Log((object) string.Format("Adding ImbueBehaviour of type: {0} to item: {1}", (object) this.type.Name, (object) imbue.colliderGroup.collisionHandler.item));
      ((ImbueBehavior) ((ThunderBehaviour) imbue).gameObject.AddComponent(this.type))?.Activate(imbue, this);
    }

    public virtual void OnImbueUnload(SpellData spell, Imbue imbue)
    {
      base.OnImbueUnload(spell, imbue);
      if (((CatalogData) spell)?.id != this.spellId || this.type == (Type) null)
        return;
      ImbueBehavior[] components = ((ThunderBehaviour) imbue)?.gameObject?.GetComponents<ImbueBehavior>();
      if (components == null || components.Length == 0)
        return;
      foreach (ImbueBehavior imbueBehavior in components)
        imbueBehavior?.Deactivate();
    }
  }
}
