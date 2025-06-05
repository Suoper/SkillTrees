// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SpellMerge.SkillArcaneOrb
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Behaviors;
using Arcana.Spells;
using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Skills.SpellMerge
{
  public class SkillArcaneOrb : SpellArcaneMergeSkillData
  {
    public string arcaneMergeSpellId;
    public string orbEffectId;
    public string projectileId;
    public string projectileDamagerId;
    public string projectileStatusId;
    public float projectileStatusDuration;
    public float projectileStatusTransfer;
    public float projectileEffectRadius;
    public float spellDuration = 7f;
    public float projectileSpeed = 1.8f;
    public float adjustmentSpeed = 4f;
    public float orbHeight = 0.9f;
    public EffectData orbEffectData;
    public ItemData projectileData;
    public DamagerData projectileDamagerData;
    public StatusData projectileStatusData;

    public event SkillArcaneOrb.OnOrb OnOrbFormedEvent;

    public event SkillArcaneOrb.OnStatusZoneAdded OnStatusZoneAddedEvent;

    public event SkillArcaneOrb.OnOrb OnOrbEndEvent;

    public event SkillArcaneOrb.OnOrbThrow OnOrbThrowEvent;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.orbEffectData = Catalog.GetData<EffectData>(this.orbEffectId, true);
      this.projectileData = Catalog.GetData<ItemData>(this.projectileId, true);
      this.projectileDamagerData = Catalog.GetData<DamagerData>(this.projectileDamagerId, true);
      this.projectileStatusData = Catalog.GetData<StatusData>(this.projectileStatusId, true);
    }

    public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnLateSkillsLoaded(skillData, creature);
      ArcaneMerge arcaneMerge;
      if (!creature.TryGetSkill<ArcaneMerge>(this.arcaneMergeSpellId, ref arcaneMerge))
        return;
      arcaneMerge.allowThrow = true;
      if (!(arcaneMerge.defaultSkillData is SkillArcaneOrb))
      {
        arcaneMerge.OnThrowEvent -= new ArcaneMerge.OnThrow(((SpellArcaneMergeSkillData) this).OnThrow);
        arcaneMerge.OnThrowEvent += new ArcaneMerge.OnThrow(((SpellArcaneMergeSkillData) this).OnThrow);
      }
      this.OnOrbFormedEvent -= new SkillArcaneOrb.OnOrb(this.AddStatusZone);
      this.OnOrbFormedEvent += new SkillArcaneOrb.OnOrb(this.AddStatusZone);
      Debug.Log((object) "Loaded Skill - Arcane Orb");
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      if (!(skillData is ArcaneMerge arcaneMerge))
        return;
      arcaneMerge.allowThrow = false;
      if (!(arcaneMerge.defaultSkillData is SkillArcaneOrb))
        arcaneMerge.OnThrowEvent -= new ArcaneMerge.OnThrow(((SpellArcaneMergeSkillData) this).OnThrow);
      this.OnOrbFormedEvent -= new SkillArcaneOrb.OnOrb(this.AddStatusZone);
      Debug.Log((object) "Unloaded Skill - Arcane Orb");
    }

    public override void OnThrow(ArcaneMerge merge, Vector3 velocity)
    {
      SkillArcaneOrb.OnOrbThrow onOrbThrowEvent = this.OnOrbThrowEvent;
      if (onOrbThrowEvent != null)
        onOrbThrowEvent(merge, velocity);
      this.SpawnProjectile(merge, velocity, (object) this);
    }

    public void SpawnProjectile(ArcaneMerge merge, Vector3 velocity, object handler = null)
    {
      this.projectileData.SpawnAsync((Action<Item>) (item =>
      {
        item.DisallowDespawn = true;
        ItemMagicAreaProjectile component = ((Component) item).GetComponent<ItemMagicAreaProjectile>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
          return;
        ((ItemMagicProjectile) component).homingIgnoredCreature = merge.mana.creature;
        ((ItemMagicProjectile) component).destroyInWater = true;
        ((ItemMagicProjectile) component).guidance = (GuidanceMode) 0;
        ((ItemMagicProjectile) component).guidanceFunc = (Func<Vector3>) null;
        ((ItemMagicProjectile) component).guidanceAmount = 0.0f;
        ((ItemMagicProjectile) component).guidanceDelay = 0.0f;
        ((ItemMagicProjectile) component).directedHoming = false;
        ((ItemMagicProjectile) component).speed = this.projectileSpeed;
        ((ItemMagicProjectile) component).allowDeflect = false;
        Extensions.GetPhysicBody((Component) component).useGravity = false;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: method pointer
        component.OnHit += SkillArcaneOrb.\u003C\u003Ec.\u003C\u003E9__35_1 ?? (SkillArcaneOrb.\u003C\u003Ec.\u003C\u003E9__35_1 = new ItemMagicAreaProjectile.Hit((object) SkillArcaneOrb.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CSpawnProjectile\u003Eb__35_1)));
        item.lastHandler = merge.mana.creature.handRight;
        OrbMovementController movementController = Utils.GetOrAddComponent<OrbMovementController>((Component) item);
        movementController.item = item;
        movementController.adjustmentSpeed = this.adjustmentSpeed;
        movementController.orbHeight = this.orbHeight;
        ((MonoBehaviour) item).StartCoroutine(DespawnRoutine());
        ((ItemMagicProjectile) component).Fire(velocity.normalized * this.projectileSpeed, this.orbEffectData, (Item) null, Player.currentCreature.ragdoll, (HapticDevice) 3, false);
        SkillArcaneOrb.OnOrb onOrbFormedEvent = this.OnOrbFormedEvent;
        if (onOrbFormedEvent == null)
          return;
        onOrbFormedEvent(component, merge, this, (EventTime) 1, handler);

        IEnumerator DespawnRoutine()
        {
          SkillArcaneOrb.OnOrb onOrbEndEvent1 = this.OnOrbEndEvent;
          if (onOrbEndEvent1 != null)
            onOrbEndEvent1(component, merge, this, (EventTime) 0, handler);
          yield return (object) new WaitForSeconds(this.spellDuration);
          UnityEngine.Object.Destroy((UnityEngine.Object) movementController);
          SkillArcaneOrb.OnOrb onOrbEndEvent2 = this.OnOrbEndEvent;
          if (onOrbEndEvent2 != null)
            onOrbEndEvent2(component, merge, this, (EventTime) 1, handler);
          ((ItemMagicProjectile) component).End();
        }
      }), new Vector3?(merge.mana.mergePoint.position), new Quaternion?(Quaternion.identity), (Transform) null, true, (List<ContentCustomData>) null, (Item.Owner) 0);
    }

    private void AddStatusZone(
      ItemMagicAreaProjectile projectile,
      ArcaneMerge spell,
      SkillArcaneOrb skill,
      EventTime eventTime,
      object handler)
    {
      if (eventTime != 1)
        return;
      ((ItemMagicProjectile) projectile).item.ForceLayer((LayerName) 23);
      ((ItemMagicProjectile) projectile).item.mainCollisionHandler.forceAllowHitLocomotionLayer = GameManager.GetLayer((LayerName) 20);
      ((ItemMagicProjectile) projectile).despawnOnHit = false;
      projectile.doExplosion = false;
      GameObject gameObject = new GameObject();
      gameObject.transform.SetParent(((Component) projectile).transform);
      gameObject.transform.SetParent(((Component) projectile).transform);
      gameObject.transform.localPosition = Vector3.zero;
      gameObject.transform.localRotation = Quaternion.identity;
      ArcaneOrbStatusApplicator applicator = gameObject.AddComponent<ArcaneOrbStatusApplicator>();
      applicator.Form(projectile, skill);
      SkillArcaneOrb.OnStatusZoneAdded statusZoneAddedEvent = this.OnStatusZoneAddedEvent;
      if (statusZoneAddedEvent == null)
        return;
      statusZoneAddedEvent(applicator, projectile, spell, skill, handler);
    }

    public delegate void OnOrb(
      ItemMagicAreaProjectile projectile,
      ArcaneMerge spell,
      SkillArcaneOrb skill,
      EventTime eventTime,
      object handler);

    public delegate void OnStatusZoneAdded(
      ArcaneOrbStatusApplicator applicator,
      ItemMagicAreaProjectile projectile,
      ArcaneMerge spell,
      SkillArcaneOrb skill,
      object handler);

    public delegate void OnOrbThrow(ArcaneMerge merge, Vector3 velocity);
  }
}
