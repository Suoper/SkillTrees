// Decompiled with JetBrains decompiler
// Type: Arcana.Misc.ProjectileManager
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Statuses;
using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Misc
{
  public class ProjectileManager
  {
    public float projectileVelocity = 12f;
    public bool projectilePlayerGuided = false;
    public float projectileGuidanceDelay = 0.5f;
    public int projectileCount = 1;
    public float? projectileTimeBetween = new float?();
    public float? projectileConeMinAngle = new float?();
    public float? projectileConeMaxAngle = new float?();
    public Vector3 guidanceDirection;
    public bool destroyInWater = true;
    public float? projectileStatusDuration = new float?();
    public float? projectileStatusTransfer = new float?();
    public bool doHoming = false;
    public float? projectileHomingRadius = new float?(2f);
    public bool performRayTargeting = false;
    public List<Threshold> thresholds = (List<Threshold>) null;
    public AnimationCurve damageOverTimeCurve = (AnimationCurve) null;
    public ItemData projectileData;
    public EffectData projectileEffectData;
    public DamagerData projectileDamagerData;
    public StatusData projectileStatusData;
    public ItemMagicProjectile lastThrownProjectile;
    public SpellData spell;
    private SpellCastCharge spellCastCharge;
    public SpellCaster spellCaster;
    private Dictionary<ItemMagicProjectile, ItemMagicProjectile.ProjectileCollisionEvent> collisionEventHandlers;

    public event ProjectileManager.OnProjectileSpawn OnProjectilePreSpawnEvent;

    public event ProjectileManager.OnProjectileSpawn OnProjectileSpawnEvent;

    public event ProjectileManager.OnProjectileDespawn OnProjectileDespawnEvent;

    public event ProjectileManager.OnProjectileEffectEnd OnProjectileEffectEndEvent;

    public event ProjectileManager.OnProjectileHit OnProjectileHitEvent;

    public ProjectileManager(
      SpellData spell,
      SpellCaster spellCaster,
      ItemData projectileData,
      EffectData projectileEffectData,
      DamagerData projectileDamagerData,
      StatusData projectileStatusData,
      SpellCastCharge spellCastCharge)
    {
      this.spell = spell;
      this.spellCaster = spellCaster;
      this.projectileData = projectileData;
      this.projectileEffectData = projectileEffectData;
      this.projectileDamagerData = projectileDamagerData;
      this.projectileStatusData = projectileStatusData;
      this.spellCastCharge = spellCastCharge;
      this.collisionEventHandlers = new Dictionary<ItemMagicProjectile, ItemMagicProjectile.ProjectileCollisionEvent>();
    }

    public float GuidanceTime() => this.projectileGuidanceDelay * 0.96f;

    public Vector3 GuidanceFunc() => this.guidanceDirection;

    public ProjectileManager Clone() => this.MemberwiseClone() as ProjectileManager;

    public IEnumerator SpawnProjectilesCoroutine(
      Vector3 position,
      Quaternion rotation,
      Vector3 velocity,
      GuidanceMode guidance = 0,
      float? startRadius = null,
      Vector3? rayTarget = null,
      ProjectileManager.ProjectileSpawnEvent onSpawn = null,
      bool triggerCollisionEvents = true,
      Item ignoredItem = null,
      HashSet<Item> projectiles = null,
      int? overrideCount = null,
      float? overrideTimeBetween = null,
      bool delayColliders = false,
      Func<Vector3> perProjectileTargetFunc = null)
    {
      int? nullable = overrideCount;
      Vector3[] positions = new Vector3[nullable ?? this.projectileCount];
      if (startRadius.HasValue)
      {
        for (int i = 0; i < positions.Length; ++i)
        {
          float randAngle = UnityEngine.Random.Range(0.0f, 6.28318548f);
          float randRadius = Mathf.Sqrt(UnityEngine.Random.Range(0.0f, 1f)) * startRadius.Value;
          float x = Mathf.Cos(randAngle) * randRadius;
          float y = Mathf.Sin(randAngle) * randRadius;
          positions[i] = position + new Vector3(x, y, 0.0f);
        }
      }
      int projectileNum = 0;
      while (true)
      {
        int num1 = projectileNum;
        nullable = overrideCount;
        int num2 = nullable ?? this.projectileCount;
        if (num1 < num2)
        {
          Vector3 angledVelocity = GetNewVelocity(startRadius.HasValue ? positions : (Vector3[]) null, projectileNum);
          this.ThrowProjectile(startRadius.HasValue ? positions[projectileNum] : position, rotation, angledVelocity, guidance, onSpawn, triggerCollisionEvents, ignoredItem, rayTarget, delayColliders, fallbackGuidanceFunc: new Func<Vector3>(fallbackGuidanceFunc), projectiles: projectiles);
          yield return (object) new WaitForSecondsRealtime((float) ((double) overrideTimeBetween ?? (double) this.projectileTimeBetween.GetValueOrDefault(0.5f)));
          angledVelocity = new Vector3();
          ++projectileNum;
        }
        else
          break;
      }
      yield return (object) null;

      Vector3 fallbackGuidanceFunc() => velocity.normalized;

      Vector3 GetNewVelocity(Vector3[] positions = null, int index = 0)
      {
        if (positions != null && this.projectileConeMaxAngle.HasValue && this.projectileConeMinAngle.HasValue)
          return Utilities.GetRandomVelocityBetweenVectors(velocity, positions[index] - position, velocity.magnitude, this.projectileConeMaxAngle.Value, this.projectileConeMinAngle.Value);
        return this.projectileConeMaxAngle.HasValue && this.projectileConeMinAngle.HasValue ? Utilities.GetRandomVelocityInCone(velocity, this.projectileConeMaxAngle.Value, velocity.magnitude, this.projectileConeMinAngle.Value) : velocity;
      }
    }

    public IEnumerator SpawnProjectilesCoroutine(
      Transform start,
      Vector3 velocity,
      GuidanceMode guidance = 0,
      float? startRadius = null,
      Vector3? rayTarget = null,
      ProjectileManager.ProjectileSpawnEvent onSpawn = null,
      bool triggerCollisionEvents = true,
      Item ignoredItem = null,
      HashSet<Item> projectiles = null,
      int? overrideCount = null,
      float? overrideTimeBetween = null,
      bool delayColliders = false,
      Func<Vector3> perProjectileTargetFunc = null)
    {
      yield return (object) this.SpawnProjectilesCoroutine(start.position, start.rotation, velocity, guidance, startRadius, rayTarget, onSpawn, triggerCollisionEvents, ignoredItem, projectiles, overrideCount, overrideTimeBetween, delayColliders, perProjectileTargetFunc);
    }

    public void ThrowProjectile(
      Vector3 position,
      Quaternion rotation,
      Vector3 velocity,
      GuidanceMode guidance = 0,
      ProjectileManager.ProjectileSpawnEvent onSpawn = null,
      bool triggerCollisionEvents = true,
      Item ignoredItem = null,
      Vector3? guidanceTarget = null,
      bool delayColliders = false,
      bool homing = false,
      Creature targetCreature = null,
      float damageMultiplier = 1f,
      Func<Vector3> fallbackGuidanceFunc = null,
      HashSet<Item> projectiles = null)
    {
      ItemMagicProjectile.ProjectileCollisionEvent projectileCollisionEvent;
      this.projectileData?.SpawnAsync((Action<Item>) (projectile =>
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        ProjectileManager.\u003C\u003Ec__DisplayClass51_1 cDisplayClass511 = new ProjectileManager.\u003C\u003Ec__DisplayClass51_1();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass511.CS\u0024\u003C\u003E8__locals1 = this;
        // ISSUE: reference to a compiler-generated field
        cDisplayClass511.projectile = projectile;
        if (delayColliders)
        {
          // ISSUE: reference to a compiler-generated field
          cDisplayClass511.projectile.SetColliders(false, false);
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          ((MonoBehaviour) cDisplayClass511.projectile).StartCoroutine(this.DelayEnableColliders(cDisplayClass511.projectile, 0.5f));
        }
        // ISSUE: reference to a compiler-generated field
        projectiles?.Add(cDisplayClass511.projectile);
        // ISSUE: reference to a compiler-generated field
        ((ThunderBehaviour) cDisplayClass511.projectile).transform.SetPositionAndRotation(position, rotation);
        // ISSUE: reference to a compiler-generated field
        cDisplayClass511.projectile.ResetColliderCollision();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass511.projectile.ResetRagdollCollision();
        if (ignoredItem != null)
        {
          // ISSUE: reference to a compiler-generated field
          cDisplayClass511.projectile.IgnoreItemCollision(ignoredItem, true);
        }
        Item obj = this.spellCastCharge.imbue?.colliderGroup.collisionHandler.item;
        if (obj != null)
        {
          // ISSUE: reference to a compiler-generated field
          cDisplayClass511.projectile.IgnoreItemCollision(obj, true);
        }
        Ragdoll ragdoll = this.spellCastCharge.imbue?.colliderGroup.collisionHandler.ragdollPart?.ragdoll ?? this.spellCaster?.mana?.creature.ragdoll ?? ((RagdollPart) this.spellCaster?.ragdollHand).ragdoll ?? (Ragdoll) null;
        if (ragdoll != null)
        {
          // ISSUE: reference to a compiler-generated field
          cDisplayClass511.projectile.IgnoreRagdollCollision(ragdoll);
        }
        FloatHandler floatHandler = new FloatHandler();
        if (this.spell != null)
          ((ValueHandler<float>) floatHandler).Add((object) this, this.spell.GetModifier((Modifier) 2));
        ((ValueHandler<float>) floatHandler).Add((object) this, damageMultiplier);
        // ISSUE: reference to a compiler-generated field
        foreach (CollisionHandler collisionHandler in cDisplayClass511.projectile.collisionHandlers)
        {
          collisionHandler.SetPhysicModifier((object) this, new float?(0.0f), 1f, -1f, -1f, -1f, (EffectData) null);
          foreach (Damager damager in collisionHandler.damagers)
          {
            damager.Load(this.projectileDamagerData, collisionHandler);
            damager.skillDamageMultiplierHandler = floatHandler;
          }
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        cDisplayClass511.itemProjectile = ((Component) cDisplayClass511.projectile).GetComponent<ItemMagicProjectile>();
        // ISSUE: reference to a compiler-generated field
        if ((UnityEngine.Object) cDisplayClass511.itemProjectile != (UnityEngine.Object) null)
        {
          // ISSUE: reference to a compiler-generated field
          cDisplayClass511.itemProjectile.speed = this.projectileVelocity;
          // ISSUE: reference to a compiler-generated field
          cDisplayClass511.itemProjectile.imbueSpellCastCharge = this.spellCastCharge;
          // ISSUE: reference to a compiler-generated field
          cDisplayClass511.itemProjectile.destroyInWater = this.destroyInWater;
          // ISSUE: reference to a compiler-generated field
          cDisplayClass511.itemProjectile.item.lastHandler = this.spellCastCharge.imbue?.colliderGroup.collisionHandler.item.lastHandler ?? this.spellCaster?.ragdollHand;
          // ISSUE: reference to a compiler-generated field
          cDisplayClass511.itemProjectile.guidance = guidance;
          // ISSUE: reference to a compiler-generated field
          // ISSUE: method pointer
          cDisplayClass511.itemProjectile.guidanceFunc = guidanceTarget.HasValue ? new Func<Vector3>((object) cDisplayClass511, __methodptr(\u003CThrowProjectile\u003Eg__TargetedGuidance\u007C4)) : fallbackGuidanceFunc ?? new Func<Vector3>(this.GuidanceFunc);
          // ISSUE: reference to a compiler-generated field
          cDisplayClass511.itemProjectile.guidanceDelay = this.projectileGuidanceDelay;
          // ISSUE: reference to a compiler-generated field
          cDisplayClass511.itemProjectile.triggerCollisionEvents = triggerCollisionEvents;
          // ISSUE: reference to a compiler-generated field
          // ISSUE: method pointer
          this.collisionEventHandlers[cDisplayClass511.itemProjectile] = projectileCollisionEvent ?? (projectileCollisionEvent = new ItemMagicProjectile.ProjectileCollisionEvent((object) this, __methodptr(\u003CThrowProjectile\u003Eb__1)));
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          cDisplayClass511.itemProjectile.OnProjectileCollisionEvent -= this.collisionEventHandlers[cDisplayClass511.itemProjectile];
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          cDisplayClass511.itemProjectile.OnProjectileCollisionEvent += this.collisionEventHandlers[cDisplayClass511.itemProjectile];
          // ISSUE: reference to a compiler-generated field
          // ISSUE: method pointer
          cDisplayClass511.itemProjectile.item.OnDespawnEvent -= new Item.SpawnEvent((object) cDisplayClass511, __methodptr(\u003CThrowProjectile\u003Eg__OnDespawn\u007C3));
          // ISSUE: reference to a compiler-generated field
          // ISSUE: method pointer
          cDisplayClass511.itemProjectile.item.OnDespawnEvent += new Item.SpawnEvent((object) cDisplayClass511, __methodptr(\u003CThrowProjectile\u003Eg__OnDespawn\u007C3));
          // ISSUE: reference to a compiler-generated field
          cDisplayClass511.itemProjectile.targetCreature = targetCreature;
          if (this.spellCaster?.mana?.creature.isPlayer.GetValueOrDefault())
          {
            // ISSUE: reference to a compiler-generated field
            cDisplayClass511.itemProjectile.damageMultHandler = floatHandler;
            // ISSUE: reference to a compiler-generated field
            cDisplayClass511.itemProjectile.damageCurve = this.damageOverTimeCurve;
            if (this.thresholds != null)
            {
              // ISSUE: reference to a compiler-generated field
              cDisplayClass511.itemProjectile.thresholds = new List<Threshold>((IEnumerable<Threshold>) this.thresholds);
            }
          }
          // ISSUE: reference to a compiler-generated field
          cDisplayClass511.itemProjectile.Fire(velocity, this.projectileEffectData, (Item) null, (Ragdoll) null, (HapticDevice) 0, homing);
          // ISSUE: reference to a compiler-generated field
          // ISSUE: method pointer
          cDisplayClass511.itemProjectile.effectInstance.onEffectFinished += new EffectInstance.EffectFinishEvent((object) cDisplayClass511, __methodptr(\u003CThrowProjectile\u003Eb__2));
          ProjectileManager.OnProjectileSpawn projectilePreSpawnEvent = this.OnProjectilePreSpawnEvent;
          if (projectilePreSpawnEvent != null)
          {
            // ISSUE: reference to a compiler-generated field
            projectilePreSpawnEvent(this.spellCastCharge, cDisplayClass511.itemProjectile, this.spellCaster);
          }
          ProjectileManager.OnProjectileSpawn projectileSpawnEvent1 = this.OnProjectileSpawnEvent;
          if (projectileSpawnEvent1 != null)
          {
            // ISSUE: reference to a compiler-generated field
            projectileSpawnEvent1(this.spellCastCharge, cDisplayClass511.itemProjectile, this.spellCaster);
          }
          ProjectileManager.ProjectileSpawnEvent projectileSpawnEvent2 = onSpawn;
          if (projectileSpawnEvent2 == null)
            return;
          // ISSUE: reference to a compiler-generated field
          projectileSpawnEvent2(cDisplayClass511.itemProjectile, new bool?(this.performRayTargeting));
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          cDisplayClass511.projectile.physicBody.AddForce(velocity, (ForceMode) 1);
          // ISSUE: reference to a compiler-generated field
          cDisplayClass511.projectile.Throw(1f, (Item.FlyDetection) 2);
        }
      }), new Vector3?(), new Quaternion?(), (Transform) null, true, (List<ContentCustomData>) null, (Item.Owner) 0);
    }

    public void ThrowProjectile(
      Transform start,
      Vector3 velocity,
      GuidanceMode guidance = 0,
      ProjectileManager.ProjectileSpawnEvent onSpawn = null,
      bool triggerCollisionEvents = true,
      Item ignoredItem = null,
      Vector3? guidanceTarget = null,
      bool delayColliders = false,
      bool homing = false,
      Creature targetCreature = null,
      float damageMultiplier = 1f,
      Func<Vector3> fallbackGuidanceFunc = null,
      HashSet<Item> projectiles = null)
    {
      this.ThrowProjectile(start.position, start.rotation, velocity, guidance, onSpawn, triggerCollisionEvents, ignoredItem, guidanceTarget, delayColliders, homing, targetCreature, damageMultiplier, fallbackGuidanceFunc, projectiles);
    }

    public void ProjectileGuidanceHomingCoroutine(
      ItemMagicProjectile projectile,
      bool? overrideRayTargeting = null)
    {
      this.lastThrownProjectile = projectile;
      if (((int) overrideRayTargeting ?? (this.performRayTargeting ? 1 : 0)) != 0)
        ((MonoBehaviour) projectile).StartCoroutine(this.ProjectileGuidanceTimer(projectile));
      ((MonoBehaviour) projectile).StartCoroutine(this.ProjectileHoming(projectile));
    }

    public IEnumerator ProjectileGuidanceTimer(ItemMagicProjectile projectile)
    {
      yield return (object) new WaitForSecondsRealtime(this.GuidanceTime());
      projectile.guidance = (GuidanceMode) 0;
      yield return (object) null;
    }

    public IEnumerator ProjectileHoming(ItemMagicProjectile projectile)
    {
      float lastHomingCheck = 0.0f;
      while (this.doHoming && (bool) (UnityEngine.Object) projectile.item.lastHandler)
      {
        if ((double) Time.time - (double) lastHomingCheck > 0.20000000298023224 && (!(bool) (UnityEngine.Object) projectile.targetCreature || projectile.targetCreature.isKilled || projectile.targetCreature.isCulled))
        {
          Creature closestCreature = projectile.item.lastHandler.creature.isPlayer ? projectile.ClosestCreature() : Player.currentCreature;
          float? projectileHomingRadius;
          int num1;
          if ((UnityEngine.Object) closestCreature != (UnityEngine.Object) null)
          {
            projectileHomingRadius = this.projectileHomingRadius;
            float num2 = 0.0f;
            num1 = (double) projectileHomingRadius.GetValueOrDefault() > (double) num2 & projectileHomingRadius.HasValue ? 1 : 0;
          }
          else
            num1 = 0;
          if (num1 != 0)
          {
            ItemMagicProjectile itemMagicProjectile = projectile;
            double magnitude = (double) (((Component) projectile).transform.position - ((ThunderBehaviour) closestCreature.ragdoll.targetPart).transform.position).magnitude;
            projectileHomingRadius = this.projectileHomingRadius;
            double valueOrDefault = (double) projectileHomingRadius.GetValueOrDefault();
            Creature creature = magnitude <= valueOrDefault & projectileHomingRadius.HasValue ? closestCreature : (Creature) null;
            itemMagicProjectile.targetCreature = creature;
          }
          lastHomingCheck = Time.time;
          closestCreature = (Creature) null;
        }
        if ((UnityEngine.Object) projectile.targetCreature != (UnityEngine.Object) null)
          projectile.item.physicBody.velocity = Quaternion.Slerp(Quaternion.identity, Quaternion.FromToRotation(projectile.item.physicBody.velocity, ((ThunderBehaviour) projectile.targetCreature.ragdoll.targetPart).transform.position - ((ThunderBehaviour) projectile.item).transform.position), Time.deltaTime * 30f * Mathf.Clamp01((float) (((double) Time.time - (double) projectile.throwTime) / 0.5))) * projectile.item.physicBody.velocity.normalized * projectile.speed;
        yield return (object) new WaitForEndOfFrame();
      }
    }

    protected virtual void OnProjectileCollision(
      ItemMagicProjectile projectile,
      CollisionInstance collisionInstance,
      HashSet<Item> projectiles)
    {
      if (this.projectileStatusData != null && this.projectileStatusDuration.HasValue)
      {
        ThunderEntity entity = collisionInstance.targetColliderGroup?.collisionHandler?.Entity;
        if (this.projectileStatusData is StatusDataArcane && this.projectileStatusTransfer.HasValue)
          entity?.Inflict(this.projectileStatusData, (object) projectile, this.projectileStatusDuration.Value, (object) this.projectileStatusTransfer, true);
        else
          entity?.Inflict(((CatalogData) this.projectileStatusData).id, (object) projectile, this.projectileStatusDuration.Value, (object) null, true);
      }
      if (projectile.triggerCollisionEvents)
      {
        ProjectileManager.OnProjectileHit projectileHitEvent = this.OnProjectileHitEvent;
        if (projectileHitEvent != null)
          projectileHitEvent(this.spellCastCharge, projectile, collisionInstance, this.spellCaster);
      }
      if ((UnityEngine.Object) this.lastThrownProjectile == (UnityEngine.Object) projectile)
        this.lastThrownProjectile = (ItemMagicProjectile) null;
      projectile.guidance = (GuidanceMode) 0;
      ((MonoBehaviour) projectile).StopCoroutine(this.ProjectileGuidanceTimer(projectile));
      ((MonoBehaviour) projectile).StopCoroutine(this.ProjectileHoming(projectile));
      projectiles?.Remove(projectile.item);
    }

    public IEnumerator DelayEnableColliders(Item item, float delay)
    {
      yield return (object) new WaitForSeconds(delay);
      item.SetColliders(true, false);
    }

    public delegate void OnProjectileSpawn(
      SpellCastCharge spell,
      ItemMagicProjectile projectile,
      SpellCaster caster);

    public delegate void OnProjectileDespawn(
      SpellCastCharge spell,
      ItemMagicProjectile projectile,
      SpellCaster caster);

    public delegate void OnProjectileEffectEnd(
      ItemMagicProjectile projectile,
      EffectInstance effectInstance);

    public delegate void OnProjectileHit(
      SpellCastCharge spell,
      ItemMagicProjectile projectile,
      CollisionInstance collision,
      SpellCaster caster);

    public delegate void ProjectileSpawnEvent(
      ItemMagicProjectile projectile,
      bool? overrideRayTargeting);
  }
}
