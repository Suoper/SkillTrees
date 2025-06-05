// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SkillArcaneOrbDissipation
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Skills.SpellMerge;
using Arcana.Spells;
using Arcana.Statuses;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Arcana.Skills
{
  internal class SkillArcaneOrbDissipation : SpellSkillData
  {
    public string arcaneOrbId;
    public SkillData arcaneOrbSkillData;
    public string statusId;
    public StatusDataArcane statusData;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.statusData = Catalog.GetData<StatusData>(this.statusId, true) as StatusDataArcane;
      this.arcaneOrbSkillData = Catalog.GetData<SkillData>(this.arcaneOrbId, true);
    }

    public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnLateSkillsLoaded(skillData, creature);
      SkillArcaneOrb skillArcaneOrb;
      if (creature.TryGetSkill<SkillArcaneOrb>(this.arcaneOrbSkillData, ref skillArcaneOrb))
      {
        skillArcaneOrb.OnOrbEndEvent -= new SkillArcaneOrb.OnOrb(this.OnOrbEnd);
        skillArcaneOrb.OnOrbEndEvent += new SkillArcaneOrb.OnOrb(this.OnOrbEnd);
      }
      else
      {
        ArcaneMerge arcaneMerge;
        SkillArcaneOrb defaultSkillData;
        int num;
        if (creature.TryGetSkill<ArcaneMerge>(this.arcaneOrbSkillData is SkillArcaneOrb arcaneOrbSkillData ? arcaneOrbSkillData.arcaneMergeSpellId : (string) null, ref arcaneMerge))
        {
          defaultSkillData = arcaneMerge.defaultSkillData as SkillArcaneOrb;
          num = defaultSkillData != null ? 1 : 0;
        }
        else
          num = 0;
        if (num == 0)
          return;
        defaultSkillData.OnOrbEndEvent -= new SkillArcaneOrb.OnOrb(this.OnOrbEnd);
        defaultSkillData.OnOrbEndEvent += new SkillArcaneOrb.OnOrb(this.OnOrbEnd);
      }
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      SkillArcaneOrb skillArcaneOrb;
      if (creature.TryGetSkill<SkillArcaneOrb>(this.arcaneOrbSkillData, ref skillArcaneOrb))
      {
        skillArcaneOrb.OnOrbEndEvent -= new SkillArcaneOrb.OnOrb(this.OnOrbEnd);
      }
      else
      {
        ArcaneMerge arcaneMerge;
        SkillArcaneOrb defaultSkillData;
        int num;
        if (creature.TryGetSkill<ArcaneMerge>(this.arcaneOrbSkillData is SkillArcaneOrb arcaneOrbSkillData ? arcaneOrbSkillData.arcaneMergeSpellId : (string) null, ref arcaneMerge))
        {
          defaultSkillData = arcaneMerge.defaultSkillData as SkillArcaneOrb;
          num = defaultSkillData != null ? 1 : 0;
        }
        else
          num = 0;
        if (num == 0)
          return;
        defaultSkillData.OnOrbEndEvent -= new SkillArcaneOrb.OnOrb(this.OnOrbEnd);
      }
    }

    public void OnOrbEnd(
      ItemMagicAreaProjectile projectile,
      ArcaneMerge spell,
      SkillArcaneOrb skill,
      EventTime eventTime,
      object handler)
    {
      if (eventTime != 1)
        return;
      ArcaneStatus.Explode(this.statusData, ((Component) projectile).transform.position, (object) spell, true);
    }
  }
}
