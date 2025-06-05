// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SkillArcaneInstabilityBurst
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Spells;
using Arcana.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Arcana.Skills
{
  public class SkillArcaneInstabilityBurst : SpellSkillData
  {
    public string arcaneStatusId = "Instability";
    public StatusData arcaneStatusData;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.arcaneStatusData = Catalog.GetData<StatusData>(this.arcaneStatusId, true);
    }

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      // ISSUE: method pointer
      creature.OnDamageEvent -= new Creature.DamageEvent((object) this, __methodptr(OnDamageCreature));
      // ISSUE: method pointer
      creature.OnDamageEvent += new Creature.DamageEvent((object) this, __methodptr(OnDamageCreature));
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      // ISSUE: method pointer
      creature.OnDamageEvent -= new Creature.DamageEvent((object) this, __methodptr(OnDamageCreature));
    }

    private void OnDamageCreature(CollisionInstance collisioninstance, EventTime eventtime)
    {
      Creature componentInParent = ((Component) collisioninstance?.targetCollider)?.gameObject.GetComponentInParent<Creature>();
      if (!(bool) (UnityEngine.Object) componentInParent)
        return;
      if (!((IEnumerable<SpellCaster>) new SpellCaster[2]
      {
        componentInParent.mana.casterLeft,
        componentInParent.mana.casterRight
      }).Any<SpellCaster>((Func<SpellCaster, bool>) (caster =>
      {
        if (!(caster.spellInstance is ArcaneBolt))
          return false;
        return caster.isFiring || caster.isSpraying || caster.isMerging;
      })))
        return;
      ArcaneStatus.Explode(this.arcaneStatusData as StatusDataArcane, ((ThunderBehaviour) componentInParent.ragdoll.targetPart).transform.position, (object) this);
    }
  }
}
