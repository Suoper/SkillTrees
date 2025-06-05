// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SkillArcaneResonance
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Statuses;
using ThunderRoad;
using ThunderRoad.Skill;

#nullable disable
namespace Arcana.Skills
{
  internal class SkillArcaneResonance : SpellSkillData
  {
    public virtual void OnCatalogRefresh() => ((SkillData) this).OnCatalogRefresh();

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      ArcaneStatus.allowExplosionStatusApply = true;
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      ArcaneStatus.allowExplosionStatusApply = false;
    }
  }
}
