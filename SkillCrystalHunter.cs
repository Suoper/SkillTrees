// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillCrystalHunter
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillCrystalHunter : SpellSkillData
  {
    [ModOption("Cluster Thickness", "Controls the thickness of the cluster trigger.")]
    [ModOptionCategory("Crystal Hunter", 5)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 100f, 0.5f)]
    public static float thickness = 1.3f;
    [ModOption("Cluster Height", "Controls the height of the cluster trigger.")]
    [ModOptionCategory("Crystal Hunter", 5)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 100f, 0.5f)]
    public static float height = 1.5f;
    [ModOption("Cluster Lifetime", "Controls the lifetime of each cluster.")]
    [ModOptionCategory("Crystal Hunter", 5)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 100f, 0.5f)]
    public static float duration = 5f;
    private EffectData clusterEffectData;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.clusterEffectData = Catalog.GetData<EffectData>("CrystalCluster", true);
    }

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      Stinger.onStingerSpawn += new Stinger.StingerEvent(this.OnStingerSpawn);
    }

    private void OnStingerSpawn(Stinger stinger)
    {
      stinger.onStingerStab += new Stinger.OnStingerStab(this.OnStingerStab);
    }

    private void OnStingerStab(
      Stinger stinger,
      Damager damager,
      CollisionInstance collision,
      Creature hitCreature)
    {
      stinger.onStingerStab -= new Stinger.OnStingerStab(this.OnStingerStab);
      if ((Object) hitCreature != (Object) null)
        return;
      CrystalCluster.Create(collision.contactPoint, Quaternion.LookRotation(collision.contactNormal, ((ThunderBehaviour) collision.sourceColliderGroup).transform.up)).Init(stinger.lerper.currentSpellId, (EffectData) null, this.clusterEffectData, SkillCrystalHunter.thickness, SkillCrystalHunter.height, 1f, 0.0f, SkillCrystalHunter.duration, drop: false);
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      Stinger.onStingerSpawn -= new Stinger.StingerEvent(this.OnStingerSpawn);
    }
  }
}
