// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.Serpents.EmpoweredMerge.EmpoweredMergeData
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using System;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Skills.Serpents.EmpoweredMerge
{
  public class EmpoweredMergeData : CustomData
  {
    public string mergeSkillId;
    private Type mergeDataType;
    public SpellMergeData mergeData;
    public SpellMergeData catalogData;
    protected SkillArcaneEmpoweringBond skillArcaneEmpoweringBond;

    public virtual void OnCatalogRefresh()
    {
      this.catalogData = Catalog.GetData<SpellMergeData>(this.mergeSkillId, true);
      this.mergeDataType = this.catalogData.GetType();
      Debug.Log((object) ("Loaded Empowered Merge for Merge " + this.mergeSkillId + " with Type " + this.mergeDataType.Name + "!"));
    }

    public virtual void OnLoad(SkillArcaneEmpoweringBond skill, SpellMergeData spell)
    {
      this.skillArcaneEmpoweringBond = skill;
      if (spell == null || !(spell.GetType() == this.mergeDataType))
        return;
      this.mergeData = spell;
    }

    public virtual void OnUnload(SpellMergeData spell)
    {
    }

    public virtual void OnUpdate(SpellMergeData mergeData)
    {
    }

    public virtual void OnFixedUpdate(SpellMergeData mergeData)
    {
    }
  }
}
