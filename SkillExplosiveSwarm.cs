// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillExplosiveSwarm
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using Crystallic.Skill.Spell;
using System;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Skill.Spell;
using ThunderRoad.Skill.SpellPower;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillExplosiveSwarm : SkillSlowTimeData
  {
    private string statusId;
    private string mixId = "Fire";
    public EffectData projectileEffectData;
    public SkillRemoteDetonation remoteDetonationSkill;
    public string remoteDetonationSkillId = "RemoteDetonation";
    protected SkillRemoteDetonation skillRemoteDetonate;
    protected List<ItemMagicProjectile> activeProjectiles = new List<ItemMagicProjectile>();
    public EffectData detonateEffectData;
    public string detonateEffectId = "RemoteDetonation";

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.projectileEffectData = Catalog.GetData<EffectData>("CrystallicFireball", true);
      this.remoteDetonationSkill = Catalog.GetData<SkillRemoteDetonation>(this.remoteDetonationSkillId, true);
      this.detonateEffectData = Catalog.GetData<EffectData>(this.detonateEffectId, true);
    }

    public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnLateSkillsLoaded(skillData, creature);
      if (this.skillRemoteDetonate != null)
      {
        // ISSUE: method pointer
        this.skillRemoteDetonate.OnDetonateHitCreatureEvent -= new SkillRemoteDetonation.DetonateHitEvent((object) this, __methodptr(OnDetonateHitCreatureEvent));
        this.skillRemoteDetonate = (SkillRemoteDetonation) null;
      }
      if (!creature.TryGetSkill<SkillRemoteDetonation>((SkillData) this.remoteDetonationSkill, ref this.skillRemoteDetonate))
        return;
      // ISSUE: method pointer
      this.skillRemoteDetonate.OnDetonateHitCreatureEvent -= new SkillRemoteDetonation.DetonateHitEvent((object) this, __methodptr(OnDetonateHitCreatureEvent));
      // ISSUE: method pointer
      this.skillRemoteDetonate.OnDetonateHitCreatureEvent += new SkillRemoteDetonation.DetonateHitEvent((object) this, __methodptr(OnDetonateHitCreatureEvent));
      SkillBoltAbsorption.onBoltAbsorptionTriggered += new SkillBoltAbsorption.OnBoltAbsorptionTriggered(this.OnBoltAbsorptionTriggered);
      SkillBoltAbsorption.onEnd += new SkillBoltAbsorption.End(this.OnEnd);
    }

    public override void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      if (this.skillRemoteDetonate == null)
        return;
      // ISSUE: method pointer
      this.skillRemoteDetonate.OnDetonateHitCreatureEvent -= new SkillRemoteDetonation.DetonateHitEvent((object) this, __methodptr(OnDetonateHitCreatureEvent));
      SkillBoltAbsorption.onBoltAbsorptionTriggered -= new SkillBoltAbsorption.OnBoltAbsorptionTriggered(this.OnBoltAbsorptionTriggered);
      SkillBoltAbsorption.onEnd -= new SkillBoltAbsorption.End(this.OnEnd);
      this.skillRemoteDetonate = (SkillRemoteDetonation) null;
    }

    public override void OnSlowMotionEnter(SpellPowerSlowTime spellPowerSlowTime, float scale)
    {
      base.OnSlowMotionEnter(spellPowerSlowTime, scale);
      this.mixId = "Mind";
      this.statusId = "Slowed";
      for (int index = 0; index < this.activeProjectiles.Count; ++index)
        this.activeProjectiles[index].effectInstance.SetColorImmediate(Dye.GetEvaluatedColor("Mind", "Mind"));
    }

    public override void OnSlowMotionExit(SpellPowerSlowTime spellPowerSlowTime)
    {
      base.OnSlowMotionExit(spellPowerSlowTime);
      this.mixId = "Fire";
      this.statusId = "Burning";
      for (int index = 0; index < this.activeProjectiles.Count; ++index)
        this.activeProjectiles[index].effectInstance.SetColorImmediate(Color.white);
    }

    private void OnEnd()
    {
      this.mixId = "Fire";
      this.statusId = "Burning";
      for (int index = 0; index < this.activeProjectiles.Count; ++index)
        this.activeProjectiles[index].effectInstance.SetColorImmediate(Color.white);
    }

    private void OnDetonateHitCreatureEvent(
      ItemMagicProjectile projectile,
      SpellCastProjectile spell,
      ThunderEntity hitEntity,
      Vector3 closestPoint,
      float distance)
    {
      if (!hitEntity.IsBurning)
        return;
      for (int index = 0; index < UnityEngine.Random.Range(1, 3); ++index)
      {
        // ISSUE: method pointer
        spell.ShootFireSpark(this.projectileEffectData, closestPoint + Vector3.up * UnityEngine.Random.Range(0.1f, 0.3f), (Vector3.up * 2f + UnityEngine.Random.insideUnitSphere) * 2.5f, false, (Creature) null, 1f, new SpellCastProjectile.ProjectileSpawnEvent((object) this, __methodptr(OnSpawnEvent)));
      }
    }

    private void OnSpawnEvent(ItemMagicProjectile projectile)
    {
      this.activeProjectiles.Add(projectile);
      // ISSUE: method pointer
      projectile.OnProjectileCollisionEvent += new ItemMagicProjectile.ProjectileCollisionEvent((object) this, __methodptr(OnProjectileCollisionEvent));
      Utils.RunAfter((MonoBehaviour) projectile, (Action) (() => projectile.homing = true), 0.2f, false);
    }

    private void OnBoltAbsorptionTriggered(
      Color color,
      SpellCastCrystallic main,
      SpellCastLightning other)
    {
      this.mixId = ((CatalogData) other).id;
      this.mixId = "Electrocute";
      for (int index = 0; index < this.activeProjectiles.Count; ++index)
        this.activeProjectiles[index].effectInstance.SetColorImmediate(color);
    }

    private void OnProjectileCollisionEvent(
      ItemMagicProjectile projectile,
      CollisionInstance collisionInstance)
    {
      this.activeProjectiles.Remove(projectile);
      // ISSUE: method pointer
      projectile.OnProjectileCollisionEvent -= new ItemMagicProjectile.ProjectileCollisionEvent((object) this, __methodptr(OnProjectileCollisionEvent));
      if (!(collisionInstance?.targetColliderGroup?.collisionHandler?.Entity is Creature entity) || entity.isPlayer)
        return;
      this.detonateEffectData?.Spawn(((ThunderBehaviour) entity.ragdoll.targetPart).transform, true, (ColliderGroup) null, false).Play(0, false, false);
      BrainModuleCrystal module = entity.brain.instance.GetModule<BrainModuleCrystal>(true);
      module.Crystallise(5f);
      module.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, this.mixId), this.mixId);
      if (Player.currentCreature.HasSkill("OverchargedCore") && (double) collisionInstance.impactVelocity.magnitude > 8.0)
        SkillOverchargedCore.Detonate(entity, Dye.GetEvaluatedColor(this.mixId, this.mixId));
      ((ThunderEntity) entity).Inflict(this.statusId, (object) this, float.PositiveInfinity, (object) 100, true);
    }
  }
}
