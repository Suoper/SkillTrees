// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillObsidianStinger
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillObsidianStinger : SpellSkillData
  {
    [ModOption("Dismemberment Allowance", "This value is used to decide how close the stinger has to be to a limb to dismember it.")]
    [ModOptionCategory("Obsidian Stinger", 10)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.05f, 100f, 0.01f)]
    public float dismembermentAllowance = 0.4f;

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      Stinger.onStingerSpawn -= new Stinger.StingerEvent(this.OnStingerSpawn);
      Stinger.onStingerSpawn += new Stinger.StingerEvent(this.OnStingerSpawn);
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
      if (!((UnityEngine.Object) hitCreature != (UnityEngine.Object) null))
        return;
      RagdollPart partToSlice;
      if (!hitCreature.ragdoll.GetClosestPart(collisionInstance.contactPoint, this.dismembermentAllowance, out partToSlice) || !partToSlice.sliceAllowed || partToSlice.hasMetalArmor)
        return;
      Utils.RunAfter((MonoBehaviour) partToSlice, (Action) (() =>
      {
        partToSlice.TrySlice();
        hitCreature.Kill();
      }), 0.05f, false);
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      Stinger.onStingerSpawn -= new Stinger.StingerEvent(this.OnStingerSpawn);
    }
  }
}
