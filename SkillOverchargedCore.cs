// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillOverchargedCore
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using Crystallic.Skill.Spell;
using System;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillOverchargedCore : SpellSkillData
  {
    [ModOption("Detonation Radius", "Decides how far force is added to creatures in a spherical radius, this is used for all detonation skills as a generic detonate method.")]
    [ModOptionCategory("Overcharged Core", 11)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 100f, 0.1f)]
    public static float detonationRadius = 5f;
    [ModOption("Detonation Force", "Decides how strong the applied force is for all detonate skills.")]
    [ModOptionCategory("Overcharged Core", 11)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 100f, 0.1f)]
    public static float detonationForce = 30f;
    [ModOption("Detonation Force Upwards Modifier", "Decides the force multiplier when a rigidbody is pushed up.")]
    [ModOptionCategory("Overcharged Core", 11)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.1f, 100f, 0.1f)]
    public static float detonationUpdardsModifier = 0.3f;
    public static EffectData detonateEffectData;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      SkillOverchargedCore.detonateEffectData = Catalog.GetData<EffectData>("DetonateCrystallicLarge", true);
    }

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      SkillHyperintensity.onSpellOvercharge += new SkillHyperintensity.OnSpellOvercharge(this.OnSpellOvercharge);
      SkillHyperintensity.onSpellReleased += new SkillHyperintensity.OnSpellReleased(this.OnSpellReleased);
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      SkillHyperintensity.onSpellOvercharge -= new SkillHyperintensity.OnSpellOvercharge(this.OnSpellOvercharge);
      SkillHyperintensity.onSpellReleased -= new SkillHyperintensity.OnSpellReleased(this.OnSpellReleased);
    }

    public void OnSpellOvercharge(SpellCastCrystallic spellCastCrystallic)
    {
      Stinger.onStingerSpawn += new Stinger.StingerEvent(this.OnStingerSpawn);
    }

    private void OnSpellReleased(SpellCastCrystallic spellCastCrystallic)
    {
      Stinger.onStingerSpawn -= new Stinger.StingerEvent(this.OnStingerSpawn);
    }

    private void OnStingerSpawn(Stinger stinger)
    {
      stinger.onStingerStab += new Stinger.OnStingerStab(this.OnStingerStab);
    }

    private void OnStingerStab(
      Stinger stinger,
      Damager damager,
      CollisionInstance collisionInstance,
      Creature hitCreature)
    {
      stinger.onStingerStab -= new Stinger.OnStingerStab(this.OnStingerStab);
      if (!(bool) (UnityEngine.Object) hitCreature || hitCreature.isPlayer)
        return;
      BrainModuleCrystal module = hitCreature.brain.instance.GetModule<BrainModuleCrystal>(true);
      SkillOverchargedCore.Detonate(hitCreature, Dye.GetEvaluatedColor(module.lerper.currentSpellId, module.lerper.currentSpellId), stinger);
    }

    public static void Detonate(Creature creature, Color color, Stinger stinger = null)
    {
      if (creature.isPlayer)
        return;
      if (SkillOverchargedCore.detonateEffectData == null)
        SkillOverchargedCore.detonateEffectData = Catalog.GetData<EffectData>("DetonateCrystallicLarge", true);
      EffectInstance effectInstance = SkillOverchargedCore.detonateEffectData?.Spawn(((ThunderBehaviour) creature.ragdoll.targetPart).transform.position, Quaternion.identity, ((ThunderBehaviour) creature.ragdoll.targetPart).transform, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
      effectInstance?.Play(0, false, false);
      effectInstance.SetColorImmediate(color);
      creature.Shred();
      ((ThunderEntity) creature).AddExplosionForce(SkillOverchargedCore.detonationForce, ((ThunderBehaviour) creature.ragdoll.targetPart).transform.position, SkillOverchargedCore.detonationRadius, SkillOverchargedCore.detonationUpdardsModifier, (ForceMode) 1, (CollisionHandler) null);
    }
  }
}
