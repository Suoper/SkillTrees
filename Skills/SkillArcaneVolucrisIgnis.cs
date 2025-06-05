// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SkillArcaneVolucrisIgnis
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Behaviors;
using Arcana.Misc;
using Arcana.Spells;
using System.Collections;
using ThunderRoad;
using ThunderRoad.Skill;
using ThunderRoad.Skill.Spell;
using UnityEngine;

#nullable disable
namespace Arcana.Skills
{
  public class SkillArcaneVolucrisIgnis : SpellSkillData
  {
    public float projectileVelocity = 12f;
    public float punchStartMinMagnitude = 4f;
    public float punchStopMaxMagnitude = 0.5f;
    public float punchStopWindow = 0.1f;
    private float punchStartMinSqrMagnitude;
    private float punchStopMaxSqrMagnitude;
    public string projectileId = "DynamicProjectile";
    public string phoenixEffectId = "SpellArcaneFirePhoenix";
    public ItemData projectileData;
    public EffectData phoenixEffectData;
    private ProjectileManager projectileManager;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.projectileData = Catalog.GetData<ItemData>(this.projectileId, true);
      this.phoenixEffectData = Catalog.GetData<EffectData>(this.phoenixEffectId, true);
      this.punchStartMinSqrMagnitude = this.punchStartMinMagnitude * this.punchStartMinMagnitude;
      this.punchStopMaxSqrMagnitude = this.punchStopMaxMagnitude * this.punchStopMaxMagnitude;
    }

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is ArcaneBolt spell1) || (Object) caster == (Object) null)
        return;
      SpellPunchDetector orAddComponent = Utils.GetOrAddComponent<SpellPunchDetector>((Component) spell1.spellCaster.ragdollHand);
      if ((Object) orAddComponent == (Object) null)
        return;
      orAddComponent.StartTracking((SpellCastCharge) spell1, this.punchStartMinSqrMagnitude, this.punchStopMaxSqrMagnitude, this.punchStopWindow);
      orAddComponent.OnSpellPunchEvent -= new SpellPunchDetector.OnSpellPunch(this.OnPunch);
      orAddComponent.OnSpellPunchEvent += new SpellPunchDetector.OnSpellPunch(this.OnPunch);
      this.projectileManager = new ProjectileManager((SpellData) spell1, caster, this.projectileData, this.phoenixEffectData, spell1.projectileDamagerData, spell1.projectileStatusData, (SpellCastCharge) spell1)
      {
        thresholds = spell1.thresholds,
        damageOverTimeCurve = spell1.damageOverTimeCurve,
        projectileVelocity = this.projectileVelocity,
        projectilePlayerGuided = false,
        projectileCount = 1,
        projectileStatusDuration = new float?(spell1.projectileStatusDuration),
        projectileStatusTransfer = new float?(spell1.projectileStatusTransfer)
      };
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      if (!(spell is ArcaneBolt arcaneBolt) || (Object) caster == (Object) null)
        return;
      SpellPunchDetector orAddComponent = Utils.GetOrAddComponent<SpellPunchDetector>((Component) arcaneBolt.spellCaster.ragdollHand);
      if ((Object) orAddComponent == (Object) null)
        return;
      orAddComponent.StopTracking();
      orAddComponent.OnSpellPunchEvent -= new SpellPunchDetector.OnSpellPunch(this.OnPunch);
    }

    public void OnPunch(SpellCastCharge spell, Vector3 punchDirection)
    {
      spell.ForceRecharge();
      this.projectileManager.spellCaster = spell.spellCaster;
      this.FireSpear(spell, punchDirection);
    }

    public void FireSpear(SpellCastCharge spell, Vector3 punchDirection)
    {
      this.projectileManager.ThrowProjectile(spell.spellCaster.magicSource.transform.position + Vector3.up * 1f + punchDirection.normalized * 0.35f, Quaternion.LookRotation(punchDirection.normalized), punchDirection.normalized * this.projectileVelocity, onSpawn: new ProjectileManager.ProjectileSpawnEvent(this.OnSpawn));
    }

    private void OnSpawn(ItemMagicProjectile projectile, bool? overrideraytargeting)
    {
      ((MonoBehaviour) projectile).StartCoroutine(this.FireTrailRoutine(projectile));
    }

    private IEnumerator FireTrailRoutine(ItemMagicProjectile projectile)
    {
      float distanceThreshold = 1f;
      float accumulatedDistance = 0.0f;
      Vector3 lastPosition = ((Component) projectile).transform.position;
      while (projectile.alive)
      {
        Vector3 currentPosition = ((Component) projectile).transform.position;
        float deltaDistance = Vector3.Distance(currentPosition, lastPosition);
        accumulatedDistance += deltaDistance;
        lastPosition = currentPosition;
        if ((double) accumulatedDistance >= (double) distanceThreshold)
        {
          FlameWall.Create(((Component) projectile).transform.position, new Quaternion()).Init(Catalog.GetData<SkillTwinFlame>("TwinFlame", true));
          accumulatedDistance = 0.0f;
        }
        yield return (object) null;
        currentPosition = new Vector3();
      }
    }
  }
}
