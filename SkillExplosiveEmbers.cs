// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillExplosiveEmbers
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using ThunderRoad;
using ThunderRoad.Skill;
using ThunderRoad.Skill.Spell;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillExplosiveEmbers : SpellSkillData
  {
    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is SpellCastProjectile spellCastProjectile))
        return;
      // ISSUE: method pointer
      spellCastProjectile.OnFireballHitEvent += new SpellCastProjectile.OnFireballHit((object) this, __methodptr(OnFireballHit));
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      if (!(spell is SpellCastProjectile spellCastProjectile))
        return;
      // ISSUE: method pointer
      spellCastProjectile.OnFireballHitEvent -= new SpellCastProjectile.OnFireballHit((object) this, __methodptr(OnFireballHit));
    }

    private void OnFireballHit(
      SpellCastProjectile spell,
      ItemMagicProjectile projectile,
      CollisionInstance collision,
      SpellCaster caster)
    {
      RagdollPart ragdollPart1 = collision?.targetColliderGroup?.collisionHandler?.ragdollPart;
      if (ragdollPart1 == null)
        return;
      BrainModuleCrystal module = ragdollPart1.ragdoll.creature.brain.instance.GetModule<BrainModuleCrystal>(true);
      if (!module.isCrystallised)
        return;
      for (RagdollPart ragdollPart2 = ragdollPart1; (Object) ragdollPart2 != (Object) null; ragdollPart2 = ragdollPart2.parentPart)
      {
        if (ragdollPart2.sliceAllowed)
        {
          ragdollPart2?.TrySlice();
          ((ThunderEntity) ragdollPart1.ragdoll.creature).Inflict("Burning", (object) this, float.PositiveInfinity, (object) 40, true);
          module.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, "Fire"), "Fire");
          ragdollPart2.physicBody.AddForce((((ThunderBehaviour) ragdollPart2).transform.position - collision.contactPoint).normalized * 25f, (ForceMode) 1);
          break;
        }
      }
    }
  }
}
