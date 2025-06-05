// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SpellSkillArcaneChromaticProjectile
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Misc;
using Arcana.Spells;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Arcana.Skills
{
  public class SpellSkillArcaneChromaticProjectile : SpellSkillData
  {
    public string chromaticCaptureEffectId;
    public EffectData chromaticCaptureEffectData;
    public SpellStatus spellStatus;
    protected Dictionary<ItemMagicProjectile, EffectInstance> projectileLookup;
    protected Dictionary<SpellCastData, EffectInstance> spellCastLookup;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.chromaticCaptureEffectData = Catalog.GetData<EffectData>(this.chromaticCaptureEffectId, true);
      this.spellStatus?.LoadCatalogData();
      this.projectileLookup = new Dictionary<ItemMagicProjectile, EffectInstance>();
      this.spellCastLookup = new Dictionary<SpellCastData, EffectInstance>();
    }

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (spell is ArcaneBolt arcaneBolt)
      {
        arcaneBolt.projectileManager.OnProjectileSpawnEvent -= new ProjectileManager.OnProjectileSpawn(this.OnProjectileSpawn);
        arcaneBolt.projectileManager.OnProjectileSpawnEvent += new ProjectileManager.OnProjectileSpawn(this.OnProjectileSpawn);
        arcaneBolt.projectileManager.OnProjectileDespawnEvent -= new ProjectileManager.OnProjectileDespawn(this.OnProjectileDespawn);
        arcaneBolt.projectileManager.OnProjectileDespawnEvent += new ProjectileManager.OnProjectileDespawn(this.OnProjectileDespawn);
        arcaneBolt.projectileManager.OnProjectileHitEvent -= new ProjectileManager.OnProjectileHit(this.OnProjectileHit);
        arcaneBolt.projectileManager.OnProjectileHitEvent += new ProjectileManager.OnProjectileHit(this.OnProjectileHit);
      }
      if (!(spell is SpellCastCharge spellCastCharge))
        return;
      // ISSUE: method pointer
      spellCastCharge.OnSpellCastEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellCast));
      // ISSUE: method pointer
      spellCastCharge.OnSpellCastEvent += new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellCast));
      // ISSUE: method pointer
      spellCastCharge.OnSpellStopEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellStop));
      // ISSUE: method pointer
      spellCastCharge.OnSpellStopEvent += new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellStop));
      // ISSUE: method pointer
      spellCastCharge.OnSpellThrowEvent -= new SpellCastCharge.SpellThrowEvent((object) this, __methodptr(\u003COnSpellLoad\u003Eb__6_0));
      // ISSUE: method pointer
      spellCastCharge.OnSpellThrowEvent += new SpellCastCharge.SpellThrowEvent((object) this, __methodptr(\u003COnSpellLoad\u003Eb__6_1));
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (spell is ArcaneBolt arcaneBolt)
      {
        arcaneBolt.projectileManager.OnProjectileSpawnEvent -= new ProjectileManager.OnProjectileSpawn(this.OnProjectileSpawn);
        arcaneBolt.projectileManager.OnProjectileDespawnEvent -= new ProjectileManager.OnProjectileDespawn(this.OnProjectileDespawn);
        arcaneBolt.projectileManager.OnProjectileHitEvent -= new ProjectileManager.OnProjectileHit(this.OnProjectileHit);
      }
      if (!(spell is SpellCastCharge spellCastCharge))
        return;
      // ISSUE: method pointer
      spellCastCharge.OnSpellCastEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellCast));
      // ISSUE: method pointer
      spellCastCharge.OnSpellStopEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellStop));
      // ISSUE: method pointer
      spellCastCharge.OnSpellThrowEvent -= new SpellCastCharge.SpellThrowEvent((object) this, __methodptr(\u003COnSpellUnload\u003Eb__7_0));
    }

    private void OnSpellCast(SpellCastCharge spell)
    {
      SpellCastData chromaticData;
      SpellCaster caster;
      if (!ChromaticParticles.TryGetChromaticData(this.spellStatus, spell, typeof (ArcaneBolt), out SpellCastData _, out chromaticData, out caster) || !spell.spellCaster.isFiring || !spell.spellCaster.other.isFiring)
        return;
      EffectInstance effectInstance = this.chromaticCaptureEffectData?.Spawn(caster.Orb, true, (ColliderGroup) null, false);
      if (effectInstance == null)
        return;
      this.spellCastLookup.Add(chromaticData, effectInstance);
      effectInstance.Play(0, false, false);
    }

    private void OnSpellStop(SpellCastCharge spell)
    {
      EffectInstance effectInstance1;
      if (this.spellCastLookup.TryGetValue((SpellCastData) spell, out effectInstance1))
      {
        this.spellCastLookup.Remove((SpellCastData) spell);
        effectInstance1?.End(false, -1f);
      }
      else
      {
        EffectInstance effectInstance2;
        if (spell.spellCaster?.other?.spellInstance == null || !this.spellCastLookup.TryGetValue(spell.spellCaster.other.spellInstance, out effectInstance2))
          return;
        this.spellCastLookup.Remove(spell.spellCaster.other.spellInstance);
        effectInstance2?.End(false, -1f);
      }
    }

    protected virtual void OnProjectileSpawn(
      SpellCastCharge spell,
      ItemMagicProjectile projectile,
      SpellCaster caster)
    {
      if (this.projectileLookup.ContainsKey(projectile))
        this.projectileLookup.Remove(projectile);
      SpellCaster other = caster.other;
      if (!other.isFiring || this.spellStatus.spellId != ((CatalogData) other.spellInstance).id)
        return;
      this.ApplyProjectileEffect(spell, projectile, caster);
    }

    protected virtual void ApplyProjectileEffect(
      SpellCastCharge spell,
      ItemMagicProjectile projectile,
      SpellCaster caster)
    {
      if (this.spellStatus == null)
        return;
      projectile.effectInstance.MixColorInEffectGradient(new Color?(((SkillData) this.spellStatus.spellData).primarySkillTree.color));
      EffectInstance effectInstance = (EffectInstance) null;
      if (this.spellStatus.spellData is SpellCastCharge spellData && (Object) ((Component) projectile.item).GetComponentInChildren<MeshRenderer>() != (Object) null)
      {
        effectInstance = spellData.imbueBladeEffectData?.Spawn(((Component) projectile).transform, true, projectile.item.colliderGroups[0], false);
        effectInstance?.Play(0, false, false);
      }
      this.projectileLookup.Add(projectile, effectInstance);
    }

    protected virtual void OnProjectileDespawn(
      SpellCastCharge spell,
      ItemMagicProjectile projectile,
      SpellCaster caster)
    {
      EffectInstance effectInstance;
      if (!this.projectileLookup.TryGetValue(projectile, out effectInstance))
        return;
      this.projectileLookup.Remove(projectile);
      effectInstance?.End(false, -1f);
    }

    protected virtual void OnProjectileHit(
      SpellCastCharge spell,
      ItemMagicProjectile projectile,
      CollisionInstance collision,
      SpellCaster caster)
    {
      if (!this.projectileLookup.ContainsKey(projectile))
        return;
      if (this.spellStatus.statusData != null)
      {
        ThunderEntity entity = collision.targetColliderGroup?.collisionHandler?.Entity;
        if (this.spellStatus.statusParameter.HasValue)
          entity?.Inflict(this.spellStatus.statusData, (object) this, this.spellStatus.statusDuration, (object) this.spellStatus.statusParameter.Value, true);
        else
          entity?.Inflict(this.spellStatus.statusData, (object) this, this.spellStatus.statusDuration, (object) null, true);
      }
      this.OnProjectileDespawn(spell, projectile, caster);
    }

    public void InvokeProjectileEffect(
      SpellCastCharge spell,
      ItemMagicProjectile projectile,
      SpellCaster caster)
    {
      this.ApplyProjectileEffect(spell, projectile, caster);
    }

    public void InvokeProjectileDespawn(
      SpellCastCharge spell,
      ItemMagicProjectile projectile,
      SpellCaster caster)
    {
      this.OnProjectileDespawn(spell, projectile, caster);
    }

    public void InvokeProjectileHit(
      SpellCastCharge spell,
      ItemMagicProjectile projectile,
      CollisionInstance collision,
      SpellCaster caster)
    {
      this.OnProjectileHit(spell, projectile, collision, caster);
    }
  }
}
