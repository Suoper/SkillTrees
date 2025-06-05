// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SkillArcaneFragments
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Misc;
using Arcana.Skills.SpellMerge;
using Arcana.Spells;
using System.Collections;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Arcana.Skills
{
  internal class SkillArcaneFragments : SpellSkillData
  {
    public string arcaneTempestId;
    public SkillData arcaneTempestSkillData;
    public bool triggerOnBaseCast = false;
    public string fragmentEffectId;
    public float fragmentSpeed;
    public float fragmentRadius;
    public int fragmentMinCount;
    public int fragmentMaxCount;
    public EffectData fragmentEffectData;
    private ProjectileManager projectileManager;
    private float homingDelay = 0.5f;
    private float spellDuration = 2f;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.fragmentEffectData = Catalog.GetData<EffectData>(this.fragmentEffectId, true);
    }

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is ArcaneBolt spell1))
        return;
      this.projectileManager = new ProjectileManager((SpellData) spell1, spell1.spellCaster, spell1.projectileData, this.fragmentEffectData, spell1.projectileDamagerData, spell1.projectileStatusData, (SpellCastCharge) spell1);
      spell1.projectileManager.OnProjectileHitEvent -= new ProjectileManager.OnProjectileHit(this.OnProjectileHit);
      spell1.projectileManager.OnProjectileHitEvent += new ProjectileManager.OnProjectileHit(this.OnProjectileHit);
    }

    public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnLateSkillsLoaded(skillData, creature);
      SkillArcaneTempest skillArcaneTempest;
      if (creature.TryGetSkill<SkillArcaneTempest>(this.arcaneTempestSkillData, ref skillArcaneTempest))
      {
        skillArcaneTempest.projectileManager.OnProjectileHitEvent -= new ProjectileManager.OnProjectileHit(this.OnProjectileHit);
        skillArcaneTempest.projectileManager.OnProjectileHitEvent += new ProjectileManager.OnProjectileHit(this.OnProjectileHit);
      }
      else
      {
        ArcaneMerge arcaneMerge;
        SkillArcaneTempest defaultSkillData;
        int num;
        if (creature.TryGetSkill<ArcaneMerge>(this.arcaneTempestSkillData is SkillArcaneTempest tempestSkillData ? tempestSkillData.arcaneMergeSpellId : (string) null, ref arcaneMerge))
        {
          defaultSkillData = arcaneMerge.defaultSkillData as SkillArcaneTempest;
          num = defaultSkillData != null ? 1 : 0;
        }
        else
          num = 0;
        if (num == 0)
          return;
        defaultSkillData.projectileManager.OnProjectileHitEvent -= new ProjectileManager.OnProjectileHit(this.OnProjectileHit);
        defaultSkillData.projectileManager.OnProjectileHitEvent += new ProjectileManager.OnProjectileHit(this.OnProjectileHit);
      }
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      if (!(spell is ArcaneBolt arcaneBolt))
        return;
      arcaneBolt.projectileManager.OnProjectileHitEvent -= new ProjectileManager.OnProjectileHit(this.OnProjectileHit);
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      SkillArcaneTempest skillArcaneTempest;
      if (creature.TryGetSkill<SkillArcaneTempest>(this.arcaneTempestSkillData, ref skillArcaneTempest))
      {
        skillArcaneTempest.projectileManager.OnProjectileHitEvent -= new ProjectileManager.OnProjectileHit(this.OnProjectileHit);
      }
      else
      {
        ArcaneMerge arcaneMerge;
        SkillArcaneTempest defaultSkillData;
        int num;
        if (creature.TryGetSkill<ArcaneMerge>(this.arcaneTempestSkillData is SkillArcaneTempest tempestSkillData ? tempestSkillData.arcaneMergeSpellId : (string) null, ref arcaneMerge))
        {
          defaultSkillData = arcaneMerge.defaultSkillData as SkillArcaneTempest;
          num = defaultSkillData != null ? 1 : 0;
        }
        else
          num = 0;
        if (num == 0)
          return;
        defaultSkillData.projectileManager.OnProjectileHitEvent -= new ProjectileManager.OnProjectileHit(this.OnProjectileHit);
      }
    }

    public void OnProjectileHit(
      SpellData spell,
      ItemMagicProjectile projectile,
      CollisionInstance collision,
      SpellCaster caster)
    {
      if (spell is ArcaneBolt && !this.triggerOnBaseCast)
        return;
      Creature componentInParent = ((Component) collision.targetCollider).GetComponentInParent<Creature>();
      if (!((Object) componentInParent != (Object) null))
        return;
      ((MonoBehaviour) caster).StartCoroutine(this.SpawnFragments(spell, Vector3.up, ((ThunderBehaviour) componentInParent.ragdoll.headPart).transform.position, 0.4f));
    }

    public IEnumerator SpawnFragments(
      SpellData spell,
      Vector3 direction,
      Vector3 spawnPoint,
      float damageMultiplier = 1f)
    {
      float coneAngle = 30f;
      float radius;
      if (spell is ArcaneBolt arcaneBolt)
      {
        double fragmentRadius = (double) this.fragmentRadius;
        Imbue imbue = arcaneBolt.imbue;
        double num = imbue != null ? (double) imbue.GetModifier().imbueEffectiveness : 1.0;
        radius = (float) (fragmentRadius * num);
      }
      else
        radius = this.fragmentRadius;
      int count = Random.Range(this.fragmentMinCount, this.fragmentMaxCount);
      Creature[] targets = Utilities.GetCreaturesInRadius(spawnPoint, radius, count);
      int targetIndex = 0;
      for (int i = 1; i < count + 1; ++i)
      {
        Vector3 vector = Utilities.GetRandomVelocityInCone(direction, coneAngle, direction.magnitude, coneAngle * 0.8f);
        ProjectileManager projectileManager = this.projectileManager;
        Vector3 position = spawnPoint + vector.normalized * 0.3f;
        Quaternion rotation = Quaternion.LookRotation(vector.normalized, Vector3.Cross((double) Vector3.Dot(vector.normalized, Vector3.up) > 0.99000000953674316 ? Vector3.right : Vector3.up, vector.normalized));
        Vector3 velocity = vector * this.fragmentSpeed;
        float num = damageMultiplier;
        Creature creature = targets.Length != 0 ? targets[targetIndex++ % targets.Length] : (Creature) null;
        // ISSUE: method pointer
        ProjectileManager.ProjectileSpawnEvent onSpawn = new ProjectileManager.ProjectileSpawnEvent((object) this, __methodptr(\u003CSpawnFragments\u003Eg__OnSpawn\u007C18_0));
        Vector3? guidanceTarget = new Vector3?();
        Creature targetCreature = creature;
        double damageMultiplier1 = (double) num;
        projectileManager.ThrowProjectile(position, rotation, velocity, onSpawn: onSpawn, guidanceTarget: guidanceTarget, delayColliders: true, targetCreature: targetCreature, damageMultiplier: (float) damageMultiplier1);
        yield return (object) 0;
        vector = new Vector3();
      }
    }

    public IEnumerator DelayHoming(ItemMagicProjectile projectile)
    {
      yield return (object) new WaitForSeconds(this.homingDelay);
      projectile.homing = true;
    }

    private IEnumerator DespawnRoutine(ItemMagicProjectile projectile)
    {
      yield return (object) new WaitForSeconds(this.spellDuration);
      projectile.End();
    }
  }
}
