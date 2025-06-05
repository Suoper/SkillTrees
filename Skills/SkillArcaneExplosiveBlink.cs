// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SkillArcaneExplosiveBlink
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Statuses;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Arcana.Skills
{
  public class SkillArcaneExplosiveBlink : SpellSkillData
  {
    public string blinkSkillId;
    public string arcaneStatusId;
    public StatusData arcaneStatusData;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.arcaneStatusData = Catalog.GetData<StatusData>(this.arcaneStatusId, true);
    }

    public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnLateSkillsLoaded(skillData, creature);
      SkillArcaneBlink skillArcaneBlink;
      if (!creature.TryGetSkill<SkillArcaneBlink>(this.blinkSkillId, ref skillArcaneBlink))
        return;
      skillArcaneBlink.OnTeleportEvent -= new SkillArcaneBlink.OnTeleport(this.SkillArcaneBlinkOnOnTeleportEvent);
      skillArcaneBlink.OnTeleportEvent += new SkillArcaneBlink.OnTeleport(this.SkillArcaneBlinkOnOnTeleportEvent);
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      SkillArcaneBlink skillArcaneBlink;
      if (!creature.TryGetSkill<SkillArcaneBlink>(this.blinkSkillId, ref skillArcaneBlink))
        return;
      skillArcaneBlink.OnTeleportEvent -= new SkillArcaneBlink.OnTeleport(this.SkillArcaneBlinkOnOnTeleportEvent);
    }

    private void SkillArcaneBlinkOnOnTeleportEvent(Vector3 originalposition, Vector3 newposition)
    {
      ArcaneStatus.Explode(this.arcaneStatusData as StatusDataArcane, newposition, (object) this);
      ArcaneStatus.Collapse(this.arcaneStatusData as StatusDataArcane, originalposition, (object) this);
    }
  }
}
