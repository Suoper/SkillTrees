// Decompiled with JetBrains decompiler
// Type: Arcana.Behaviors.DragonStorm
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Misc;
using Arcana.Skills.SpellMerge;
using Arcana.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using ThunderRoad.Pools;
using UnityEngine;

#nullable disable
namespace Arcana.Behaviors
{
  public class DragonStorm : ThunderBehaviour
  {
    public int dragonCount = 2;
    public List<LightningDragon> dragons;
    private SkillThunderbond skill;
    private EffectInstance stormEffect;
    private DragonStorm.StormZone stormZone;
    public float startTime;
    private float lastStrikeTime;
    public static bool active;
    public static Vector3 lastStormPosition;
    public static float lastRadius;

    public event DragonStorm.StormEvent OnStormStartEvent;

    public event DragonStorm.StormEvent OnStormEndEvent;

    public virtual ManagedLoops EnabledManagedLoops => (ManagedLoops) 2;

    public void Form(SkillThunderbond thunderbond)
    {
      if (DragonStorm.active)
        return;
      this.skill = thunderbond;
      this.stormEffect = this.skill.stormEffectData?.Spawn(this.transform, true, (ColliderGroup) null, false);
      this.stormEffect?.Play(0, false, false);
      this.stormZone = this.transform.gameObject.AddComponent<DragonStorm.StormZone>();
      this.stormZone.Form(this.skill);
      this.dragons = new List<LightningDragon>();
      for (int index = 0; index < this.dragonCount; ++index)
        this.dragons.Add(this.CreateDragon(this.transform.position, index));
      this.startTime = Time.time;
      this.lastStrikeTime = this.startTime;
      DragonStorm.lastStormPosition = this.transform.position;
      DragonStorm.lastRadius = this.skill.stormRadius;
      DragonStorm.active = true;
      DragonStorm.StormEvent onStormStartEvent = this.OnStormStartEvent;
      if (onStormStartEvent == null)
        return;
      onStormStartEvent(this);
    }

    protected virtual void ManagedUpdate()
    {
      base.ManagedUpdate();
      if (!DragonStorm.active)
        return;
      if ((double) Time.time - (double) this.lastStrikeTime > (double) this.skill.lightningStrikePeriod)
      {
        this.StrikeLightning(this.transform.position + Vector3.up * this.skill.lightningStrikeVerticalOffset);
        this.lastStrikeTime = Time.time;
      }
      if ((double) Time.time - (double) this.startTime < (double) this.skill.stormDuration)
        return;
      foreach (LightningDragon dragon in this.dragons)
        dragon.Despawn();
      this.stormZone.End();
      this.stormEffect?.End(false, -1f);
      DragonStorm.active = false;
      DragonStorm.StormEvent onStormEndEvent = this.OnStormEndEvent;
      if (onStormEndEvent != null)
        onStormEndEvent(this);
      UnityEngine.Object.Destroy((UnityEngine.Object) this);
    }

    public LightningDragon CreateDragon(Vector3 centerpoint, int index)
    {
      GameObject leader = new GameObject(string.Format("Dragon Leader {0}", (object) index));
      RotateAroundCenter rotateAroundCenter = leader.AddComponent<RotateAroundCenter>();
      rotateAroundCenter.centerPosition = centerpoint;
      rotateAroundCenter.radius = this.skill.dragonRadius;
      rotateAroundCenter.speed = this.skill.dragonSpeed;
      rotateAroundCenter.sineWaveAmplitude = this.skill.dragonSinAmplitude;
      rotateAroundCenter.sineWaveFrequency = this.skill.dragonSinFrequency;
      rotateAroundCenter.rotationOffset = 360f / (float) this.dragonCount * (float) index;
      LightningDragon dragon = new GameObject(string.Format("Dragon {0}", (object) index)).AddComponent<LightningDragon>();
      dragon.Init(this.skill.dragonData);
      dragon.Form(leader.transform, this.skill.mana);
      dragon.OnDespawnEvent += (LightningDragon.OnDespawn) (lightningDragon =>
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) rotateAroundCenter);
        UnityEngine.Object.Destroy((UnityEngine.Object) leader);
      });
      return dragon;
    }

    public void StrikeLightning(Vector3 centerPoint)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      DragonStorm.\u003C\u003Ec__DisplayClass22_0 cDisplayClass220 = new DragonStorm.\u003C\u003Ec__DisplayClass22_0();
      Vector3 randomPointInCircle = Utilities.GetRandomPointInCircle(centerPoint, this.skill.stormRadius, this.skill.stormEyeRadius, this.skill.lightningStrikeVerticalOffsetVariance);
      RaycastHit raycastHit;
      if (!Physics.Raycast(randomPointInCircle, Vector3.down, ref raycastHit, this.skill.lightningStrikeVerticalOffset * 2f, Utilities.GetProjectileRaycastMask()))
        return;
      Vector3 point = ((RaycastHit) ref raycastHit).point;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass220.sourceTransform = ((PoolManager<Transform>) PoolUtils.GetTransformPoolManager()).Get();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass220.sourceTransform.transform.position = randomPointInCircle;
      // ISSUE: reference to a compiler-generated field
      EffectInstance effectInstance = this.skill.mainBoltEffectData?.Spawn(cDisplayClass220.sourceTransform, true, (ColliderGroup) null, false);
      effectInstance?.SetMainGradient(this.skill.defaultBoltGradient);
      // ISSUE: reference to a compiler-generated field
      effectInstance?.SetSource(cDisplayClass220.sourceTransform);
      // ISSUE: reference to a compiler-generated field
      cDisplayClass220.targetTransform = ((PoolManager<Transform>) PoolUtils.GetTransformPoolManager()).Get();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass220.targetTransform.transform.position = point;
      if (effectInstance != null)
      {
        // ISSUE: reference to a compiler-generated field
        effectInstance.SetTarget(cDisplayClass220.targetTransform);
        // ISSUE: method pointer
        effectInstance.onEffectFinished += new EffectInstance.EffectFinishEvent((object) cDisplayClass220, __methodptr(\u003CStrikeLightning\u003Eg__OnFinished\u007C0));
        effectInstance.Play(0, false, false);
      }
      // ISSUE: reference to a compiler-generated field
      this.skill.impactEffectData?.Spawn(cDisplayClass220.targetTransform, true, (ColliderGroup) null, false).Play(0, false, false);
    }

    public delegate void StormEvent(DragonStorm storm);

    public class StormZone : ThunderBehaviour
    {
      public SkillThunderbond skill;
      public Zone zone;
      public DragonStorm.StormZone.StormEye stormEye;

      public void Form(SkillThunderbond skill)
      {
        this.skill = skill;
        ((Collider) this.gameObject.AddComponent<SphereCollider>()).isTrigger = true;
        this.stormEye = Utilities.GetTransformCopy(this.gameObject.transform).gameObject.AddComponent<DragonStorm.StormZone.StormEye>();
        this.stormEye.Form(skill);
        this.zone = this.gameObject.AddComponent<Zone>();
        this.zone.SetRadius(skill.stormRadius);
      }

      public void End()
      {
        ((Behaviour) this.zone).enabled = false;
        this.stormEye.End();
        UnityEngine.Object.Destroy((UnityEngine.Object) this.zone, 0.1f);
        UnityEngine.Object.Destroy((UnityEngine.Object) this);
      }

      public void Update()
      {
        SpellStatusImbueable spellStatusImbueable1 = this.skill.stormStatuses.Find((Predicate<SpellStatusImbueable>) (status => status.isImbueStatus));
        SpellStatusImbueable spellStatusImbueable2 = this.skill.stormStatuses.Find((Predicate<SpellStatusImbueable>) (status => status.spellId == "Arcane"));
        foreach (Creature key in this.zone.creaturesInZone.Keys)
        {
          if (!key.isKilled && !key.isPlayer && !this.stormEye.zone.creaturesInZone.ContainsKey(key))
          {
            foreach (SpellStatusImbueable stormStatuse in this.skill.stormStatuses)
            {
              Creature creature = key;
              StatusData statusData = stormStatuse.statusData;
              double statusDuration = (double) stormStatuse.statusDuration;
              float? statusParameter = stormStatuse.statusParameter;
              float deltaTime = Time.deltaTime;
              // ISSUE: variable of a boxed type
              __Boxed<float?> local = (ValueType) (statusParameter.HasValue ? new float?(statusParameter.GetValueOrDefault() * deltaTime) : new float?());
              ((ThunderEntity) creature).Inflict(statusData, (object) this, (float) statusDuration, (object) local, true);
            }
            key.DamagePatched(this.skill.stormDamage * Time.deltaTime, (DamageType) 4);
            ArcaneStatus arcaneStatus;
            if (this.skill.stormTriggerInstabilityExplosion && spellStatusImbueable2 != null && ((ThunderEntity) key).TryGetStatus<ArcaneStatus>(spellStatusImbueable2.statusData, ref arcaneStatus) && spellStatusImbueable2.statusData is StatusDataArcane statusData1 && (arcaneStatus.CanExplode || key.isKilled))
            {
              ArcaneStatus.Explode(statusData1, key);
              arcaneStatus.EndCharged();
            }
          }
        }
        foreach (Item obj in this.zone.itemsInZone.Keys.Where<Item>((Func<Item, bool>) (item => !this.stormEye.zone.itemsInZone.Keys.Contains<Item>(item))))
        {
          obj.breakable?.Break();
          if (spellStatusImbueable1 != null)
          {
            foreach (ColliderGroup colliderGroup in obj.colliderGroups)
              ;
          }
        }
      }

      public class StormEye : ThunderBehaviour
      {
        public SkillThunderbond skill;
        public Zone zone;

        public void Form(SkillThunderbond skill)
        {
          this.skill = skill;
          ((Collider) this.gameObject.AddComponent<SphereCollider>()).isTrigger = true;
          this.zone = this.gameObject.AddComponent<Zone>();
          this.zone.SetRadius(skill.stormEyeRadius);
        }

        public void End()
        {
          ((Behaviour) this.zone).enabled = false;
          UnityEngine.Object.Destroy((UnityEngine.Object) this.zone, 0.1f);
          UnityEngine.Object.Destroy((UnityEngine.Object) this);
        }
      }
    }
  }
}
