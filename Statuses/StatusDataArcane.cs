// Decompiled with JetBrains decompiler
// Type: Arcana.Statuses.StatusDataArcane
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Statuses
{
  internal class StatusDataArcane : StatusData
  {
    public const string Instability = "Instability";
    public const string InstabilityGainMulti = "InstabilityGainMulti";
    public const string InstabilityLossMulti = "InstabilityLossMulti";
    public float chargeDuration;
    public float instabilityReductionPerSecond;
    public float instabilityReductionPerSecondKilled;
    public float instabilityReductionPerSecondPlayer;
    public float maxInstability;
    [ModOptionCategory("Status", 100)]
    [ModOptionFloatValues(0.0f, 1f, 0.05f)]
    [ModOption("Enemy Explode Instant Kill Chance", "Chance that an enemy is instantly killed and shredded when exploding", order = 1)]
    public static float enemyExplodeKillChance = 0.35f;
    [ModOptionCategory("Status", 100)]
    [ModOptionFloatValues(0.0f, 1f, 0.05f)]
    [ModOption("Enemy Explode Health % Damage", "Percentage of current health that is applied as damage when an enemy is exploded and they survive the ordeal", order = 2)]
    public static float enemyDamagePercentage = 0.7f;
    [ModOptionCategory("Status", 100)]
    [ModOptionFloatValues(0.0f, 1f, 0.05f)]
    [ModOption("Player Explode Health % Damage", "Percentage of current health that is applied as damage when the player is exploded", order = 3)]
    public static float playerDamagePercentage = 0.3f;
    public bool explodeOnCollapse = false;
    public float explodeOnCollapseDelay = 0.75f;
    public string accumulateEffectId;
    public EffectData accumulateEffectData;
    public string fullyChargedEffectId;
    public EffectData fullyChargedEffectData;
    public string explodeEffectId;
    public EffectData explodeEffectData;
    public string implodeEffectId;
    public EffectData implodeEffectData;
    public float effectForceRadius;
    public float effectForceStrength;
    public int radialMask;

    public virtual void OnCatalogRefresh()
    {
      base.OnCatalogRefresh();
      this.accumulateEffectData = Catalog.GetData<EffectData>(this.accumulateEffectId, true);
      this.fullyChargedEffectData = Catalog.GetData<EffectData>(this.fullyChargedEffectId, true);
      this.explodeEffectData = Catalog.GetData<EffectData>(this.explodeEffectId, true);
      this.implodeEffectData = Catalog.GetData<EffectData>(this.implodeEffectId, true);
      this.radialMask = StatusDataArcane.GetRadialForceMask();
    }

    public virtual bool SpawnEffect(ThunderEntity entity, out EffectInstance effect)
    {
      effect = (EffectInstance) null;
      switch (entity)
      {
        case Creature creature:
          if (this.creatureEffectData != null)
          {
            effect = this.creatureEffectData?.Spawn(creature.ragdoll.targetPart.meshBone.transform, true, (ColliderGroup) null, false);
            if (effect == null)
              return false;
            break;
          }
          break;
        case Item obj:
          if (this.itemEffectData != null)
          {
            effect = this.itemEffectData?.Spawn(((ThunderBehaviour) obj).transform, true, (ColliderGroup) null, false);
            if (effect == null)
              return false;
            for (int index = 0; index < obj.colliderGroups.Count; ++index)
            {
              if ((bool) (Object) obj.colliderGroups[index].imbueEmissionRenderer)
              {
                effect.SetRenderer(obj.colliderGroups[index].imbueEmissionRenderer, false);
                break;
              }
            }
            break;
          }
          break;
      }
      return true;
    }

    public bool SpawnEffect(ThunderEntity entity, EffectData effectData, out EffectInstance effect)
    {
      effect = (EffectInstance) null;
      if (!(entity is Creature creature))
        return false;
      EffectInstance effectInstance = effectData?.Spawn(creature.ragdoll.targetPart.meshBone.transform, true, (ColliderGroup) null, false);
      if (effectInstance != null)
        effect = effectInstance;
      return true;
    }

    public static int GetRadialForceMask()
    {
      return 0 | 1 << GameManager.GetLayer((LayerName) 9) | 1 << GameManager.GetLayer((LayerName) 8) | 1 << GameManager.GetLayer((LayerName) 21) | 1 << GameManager.GetLayer((LayerName) 1) | 1 << GameManager.GetLayer((LayerName) 10) | 1 << GameManager.GetLayer((LayerName) 24) | 1 << GameManager.GetLayer((LayerName) 11);
    }
  }
}
