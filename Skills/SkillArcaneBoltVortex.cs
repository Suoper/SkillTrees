// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SkillArcaneBoltVortex
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Misc;
using Arcana.Skills.SpellMerge;
using Arcana.Spells;
using System;
using System.Collections;
using System.Linq;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Arcana.Skills
{
  internal class SkillArcaneBoltVortex : SpellSkillData
  {
    public string arcaneOrbId;
    public SkillData arcaneOrbSkillData;
    public string fragmentEffectId;
    public float fragmentSpeed;
    public float fragmentRadius;
    public int fragmentMinCount;
    public int fragmentMaxCount;
    public float projectilePulseTime;
    public EffectData fragmentEffectData;
    private ProjectileManager projectileManager;
    private float homingDelay = 0.5f;
    private float projectileDuration = 2.5f;
    private bool firing = false;
    private Coroutine orbRoutine;
    private Coroutine fireRoutine;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.fragmentEffectData = Catalog.GetData<EffectData>(this.fragmentEffectId, true);
      this.arcaneOrbSkillData = Catalog.GetData<SkillData>(this.arcaneOrbId, true);
    }

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is ArcaneBolt spell1))
        return;
      this.projectileManager = new ProjectileManager((SpellData) spell1, spell1.spellCaster, spell1.projectileData, this.fragmentEffectData, spell1.projectileDamagerData, spell1.projectileStatusData, (SpellCastCharge) spell1);
    }

    public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnLateSkillsLoaded(skillData, creature);
      SkillArcaneOrb skillArcaneOrb;
      if (creature.TryGetSkill<SkillArcaneOrb>(this.arcaneOrbSkillData, ref skillArcaneOrb))
      {
        skillArcaneOrb.OnOrbEndEvent -= new SkillArcaneOrb.OnOrb(this.OnOrbEnd);
        skillArcaneOrb.OnOrbEndEvent += new SkillArcaneOrb.OnOrb(this.OnOrbEnd);
        skillArcaneOrb.OnOrbFormedEvent -= new SkillArcaneOrb.OnOrb(this.OnOrbForm);
        skillArcaneOrb.OnOrbFormedEvent += new SkillArcaneOrb.OnOrb(this.OnOrbForm);
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
        defaultSkillData.OnOrbFormedEvent -= new SkillArcaneOrb.OnOrb(this.OnOrbForm);
        defaultSkillData.OnOrbFormedEvent += new SkillArcaneOrb.OnOrb(this.OnOrbForm);
      }
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      SkillArcaneOrb skillArcaneOrb;
      if (creature.TryGetSkill<SkillArcaneOrb>(this.arcaneOrbSkillData, ref skillArcaneOrb))
      {
        skillArcaneOrb.OnOrbEndEvent -= new SkillArcaneOrb.OnOrb(this.OnOrbEnd);
        skillArcaneOrb.OnOrbFormedEvent -= new SkillArcaneOrb.OnOrb(this.OnOrbForm);
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
        defaultSkillData.OnOrbFormedEvent -= new SkillArcaneOrb.OnOrb(this.OnOrbForm);
      }
    }

    public IEnumerator FireLoop(
      ArcaneMerge spell,
      SkillArcaneOrb skill,
      ItemMagicAreaProjectile projectile)
    {
      while (this.firing)
      {
        yield return (object) new WaitForSeconds(this.projectilePulseTime);
        this.fireRoutine = ((MonoBehaviour) projectile).StartCoroutine(this.SpawnFragments(spell, skill, ((Component) projectile).transform.position));
      }
      yield return (object) 0;
    }

    public IEnumerator SpawnFragments(
      ArcaneMerge spell,
      SkillArcaneOrb skill,
      Vector3 spawnPoint,
      float damageMultipler = 1f)
    {
      float radius;
      if (spell.mana.casterLeft.spellInstance is ArcaneBolt arcaneBolt)
      {
        double fragmentRadius = (double) this.fragmentRadius;
        Imbue imbue = arcaneBolt.imbue;
        double num = imbue != null ? (double) imbue.GetModifier().imbueEffectiveness : 1.0;
        radius = (float) (fragmentRadius * num);
      }
      else
        radius = this.fragmentRadius;
      int count = UnityEngine.Random.Range(this.fragmentMinCount, this.fragmentMaxCount);
      Creature[] targets = Creature.allActive.Where<Creature>((Func<Creature, bool>) (creature => !creature.isKilled && !creature.isPlayer && !creature.isCulled && (double) (((ThunderBehaviour) creature.ragdoll.targetPart).transform.position - spawnPoint).sqrMagnitude < (double) radius * (double) radius)).Take<Creature>(count).ToArray<Creature>();
      int targetIndex = 0;
      for (int i = 0; i < count + 1; ++i)
      {
        Vector3 vector = Utilities.GetRandomDirectionInCircle(spawnPoint, 1f, 0.1f);
        ProjectileManager projectileManager = this.projectileManager;
        Vector3 position = spawnPoint + vector.normalized * 0.3f;
        Quaternion rotation = Quaternion.LookRotation(vector.normalized, Vector3.Cross((double) Vector3.Dot(vector.normalized, Vector3.up) > 0.99000000953674316 ? Vector3.right : Vector3.up, vector.normalized));
        Vector3 velocity = vector * this.fragmentSpeed;
        float num = damageMultipler;
        Creature creature = targets.Length != 0 ? targets[targetIndex++ % targets.Length] : (Creature) null;
        ProjectileManager.ProjectileSpawnEvent onSpawn = new ProjectileManager.ProjectileSpawnEvent(this.OnSpawn);
        Vector3? guidanceTarget = new Vector3?();
        Creature targetCreature = creature;
        double damageMultiplier = (double) num;
        projectileManager.ThrowProjectile(position, rotation, velocity, onSpawn: onSpawn, guidanceTarget: guidanceTarget, delayColliders: true, targetCreature: targetCreature, damageMultiplier: (float) damageMultiplier);
        yield return (object) 0;
        vector = new Vector3();
      }
    }

    public void OnOrbForm(
      ItemMagicAreaProjectile projectile,
      ArcaneMerge spell,
      SkillArcaneOrb skill,
      EventTime eventTime,
      object handler)
    {
      if (eventTime != 1)
        return;
      this.projectileManager.spellCaster = ((ItemMagicProjectile) projectile).item.lastHandler?.caster;
      this.firing = true;
      this.orbRoutine = ((MonoBehaviour) projectile).StartCoroutine(this.FireLoop(spell, skill, projectile));
    }

    public void OnOrbEnd(
      ItemMagicAreaProjectile projectile,
      ArcaneMerge spell,
      SkillArcaneOrb skill,
      EventTime eventTime,
      object handler)
    {
      if (eventTime > 0)
        return;
      this.firing = false;
      if (this.orbRoutine != null)
        ((MonoBehaviour) projectile).StopCoroutine(this.orbRoutine);
      if (this.fireRoutine == null)
        return;
      ((MonoBehaviour) projectile).StopCoroutine(this.fireRoutine);
    }

    private void OnSpawn(ItemMagicProjectile projectile, bool? overrideRayTargeting)
    {
      ((MonoBehaviour) projectile).StartCoroutine(this.DelayHoming(projectile));
      ((MonoBehaviour) projectile).StartCoroutine(this.DespawnRoutine(projectile));
    }

    public IEnumerator DelayHoming(ItemMagicProjectile projectile)
    {
      yield return (object) new WaitForSeconds(this.homingDelay);
      projectile.homing = true;
    }

    private IEnumerator DespawnRoutine(ItemMagicProjectile projectile)
    {
      yield return (object) new WaitForSeconds(this.projectileDuration);
      projectile.End();
    }
  }
}
