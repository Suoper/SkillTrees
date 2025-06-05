// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SkillArcaneChargedArcana
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Behaviors;
using ThunderRoad;
using ThunderRoad.Skill.Spell;
using UnityEngine;

#nullable disable
namespace Arcana.Skills
{
  public class SkillArcaneChargedArcana : SpellSkillArcaneChromaticProjectile
  {
    public float randomStartDelay = 0.5f;
    public float boltRange = 3f;
    public float boltPeriod = 0.5f;
    public float boltPeriodVariance = 0.25f;
    public string spellCastLightningId = "Lightning";
    private SpellCastLightning spellCastLightning;

    public override void OnCatalogRefresh()
    {
      base.OnCatalogRefresh();
      this.spellCastLightning = Catalog.GetData<SpellCastLightning>(this.spellCastLightningId, true);
    }

    protected override void OnProjectileSpawn(
      SpellCastCharge spell,
      ItemMagicProjectile projectile,
      SpellCaster caster)
    {
      this.RemoveComponent(projectile);
      base.OnProjectileSpawn(spell, projectile, caster);
    }

    protected override void ApplyProjectileEffect(
      SpellCastCharge spell,
      ItemMagicProjectile projectile,
      SpellCaster caster)
    {
      base.ApplyProjectileEffect(spell, projectile, caster);
      if (((Component) projectile).gameObject.TryGetComponent<RadialLightning>(out RadialLightning _))
        return;
      ((Component) projectile).gameObject.AddComponent<RadialLightning>().Prep(this.boltRange, this.boltPeriod, this.boltPeriodVariance, this.randomStartDelay, this.spellCastLightning);
    }

    protected override void OnProjectileDespawn(
      SpellCastCharge spell,
      ItemMagicProjectile projectile,
      SpellCaster caster)
    {
      base.OnProjectileDespawn(spell, projectile, caster);
      this.RemoveComponent(projectile);
    }

    protected override void OnProjectileHit(
      SpellCastCharge spell,
      ItemMagicProjectile projectile,
      CollisionInstance collision,
      SpellCaster caster)
    {
      base.OnProjectileHit(spell, projectile, collision, caster);
      this.RemoveComponent(projectile);
    }

    private void RemoveComponent(ItemMagicProjectile projectile)
    {
      RadialLightning component;
      if (!((Component) projectile).gameObject.TryGetComponent<RadialLightning>(out component))
        return;
      Object.Destroy((Object) component);
    }
  }
}
