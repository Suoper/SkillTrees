// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillSeekingShards
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.Skill.Spell;
using System;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillSeekingShards : SpellSkillData
  {
    [ModOption("Seeking Radius", "Controls how large the attraction radius is for each creature.")]
    [ModOptionCategory("Seeking Shards", 16)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.5f, 100f, 0.5f)]
    public static float seekingRadius = 5f;
    [ModOption("Seeking Max Distance", "Controls how far the ray shoots, this is used to detect creatures to seek.")]
    [ModOptionCategory("Seeking Shards", 16)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.5f, 100f, 0.5f)]
    public static float seekingMaxDistance = 7f;
    [ModOption("Seeking Max Angle", "Controls how far the ray shoots, this is used to detect creatures to seek.")]
    [ModOptionCategory("Seeking Shards", 16)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.5f, 100f, 0.5f)]
    public static float seekingMaxAngle = 25f;
    public List<ParticleSystemForceField> activeFields = new List<ParticleSystemForceField>();

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      spellCastCrystallic.OnShardshotStart += new SpellCastCrystallic.ShardshotEvent(this.OnShardshotStart);
      spellCastCrystallic.OnShardshotEnd += new SpellCastCrystallic.ShardshotEvent(this.OnShardshotEnd);
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      spellCastCrystallic.OnShardshotStart -= new SpellCastCrystallic.ShardshotEvent(this.OnShardshotStart);
      spellCastCrystallic.OnShardshotEnd -= new SpellCastCrystallic.ShardshotEvent(this.OnShardshotEnd);
    }

    private void OnShardshotEnd(
      SpellCastCrystallic spellCastCrystallic,
      EffectInstance effectInstance)
    {
      foreach (UnityEngine.Object activeField in this.activeFields)
        UnityEngine.Object.Destroy(activeField);
    }

    private void OnShardshotStart(
      SpellCastCrystallic spellCastCrystallic,
      EffectInstance effectInstance)
    {
      Transform transform;
      Creature componentInParent;
      int num;
      if ((bool) (UnityEngine.Object) Creature.AimAssist(spellCastCrystallic.spellCaster.Orb.position, spellCastCrystallic.lastVelocity.normalized, SkillSeekingShards.seekingMaxDistance, SkillSeekingShards.seekingMaxAngle, ref transform, (Func<Creature, bool>) Filter.LiveCreaturesExcept(Player.currentCreature), (CreatureType) 0, (Creature) null, 0.1f))
      {
        componentInParent = transform.GetComponentInParent<Creature>();
        num = componentInParent != null ? 1 : 0;
      }
      else
        num = 0;
      if (num == 0 || !((UnityEngine.Object) componentInParent != (UnityEngine.Object) null))
        return;
      ParticleSystemForceField orAddComponent = Extensions.GetOrAddComponent<ParticleSystemForceField>(((ThunderBehaviour) componentInParent.ragdoll.targetPart).gameObject);
      this.activeFields.Add(orAddComponent);
      orAddComponent.gravity = ParticleSystem.MinMaxCurve.op_Implicit(0.45f);
      orAddComponent.endRange = SkillSeekingShards.seekingRadius;
    }
  }
}
