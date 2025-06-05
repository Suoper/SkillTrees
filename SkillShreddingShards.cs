// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillShreddingShards
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.Skill.Spell;
using System;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillShreddingShards : SpellSkillData
  {
    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      spellCastCrystallic.OnShardHit += new SpellCastCrystallic.ShardshotHitEvent(this.OnShardHit);
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      spellCastCrystallic.OnShardHit -= new SpellCastCrystallic.ShardshotHitEvent(this.OnShardHit);
    }

    private void OnShardHit(
      SpellCastCrystallic spellCastCrystallic,
      ThunderEntity entity,
      SpellCastCrystallic.ShardshotHit hitInfo)
    {
      if (!((UnityEngine.Object) hitInfo.hitPart != (UnityEngine.Object) null) || hitInfo.wasMetal || !hitInfo.hitPart.sliceAllowed || hitInfo.hitPart.ragdoll.creature.isPlayer || (double) Vector3.Distance(((Component) hitInfo.hitPart.characterJoint).transform.position, hitInfo.hitPoint) >= 0.075000002980232239)
        return;
      Utils.RunAfter((MonoBehaviour) hitInfo.hitPart, (Action) (() =>
      {
        hitInfo.hitPart.TrySlice();
        hitInfo.hitPart.ragdoll.creature.Kill();
      }), 0.05f, false);
    }
  }
}
