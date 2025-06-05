// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SkillArcaneGravitonCoil
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Behaviors;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Skills
{
  public class SkillArcaneGravitonCoil : SpellSkillArcaneChromaticProjectile
  {
    public string linkEffectId = "GravityTether";
    public EffectData linkEffectData;
    public string linkStatusId = "Floating";
    public StatusData linkStatusData;
    public int linkMax = 3;
    public float linkRadius = 4f;
    public float linkDuration = 5f;
    public static HashSet<Creature> linkedCreatures;

    public override void OnCatalogRefresh()
    {
      base.OnCatalogRefresh();
      this.linkEffectData = Catalog.GetData<EffectData>(this.linkEffectId, true);
      this.linkStatusData = Catalog.GetData<StatusData>(this.linkStatusId, true);
    }

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      SkillArcaneGravitonCoil.linkedCreatures = new HashSet<Creature>();
    }

    protected override void OnProjectileHit(
      SpellCastCharge spell,
      ItemMagicProjectile projectile,
      CollisionInstance collision,
      SpellCaster caster)
    {
      if (!this.projectileLookup.ContainsKey(projectile))
      {
        base.OnProjectileHit(spell, projectile, collision, caster);
      }
      else
      {
        Creature componentInParent = ((Component) collision.targetCollider).GetComponentInParent<Creature>();
        if ((Object) componentInParent != (Object) null)
          ((ThunderBehaviour) componentInParent).gameObject.AddComponent<GravitonLink>().Link(this, this.linkMax, this.linkRadius, this.linkDuration);
        base.OnProjectileHit(spell, projectile, collision, caster);
      }
    }
  }
}
