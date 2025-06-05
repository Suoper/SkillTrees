// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillHyperdetonation
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillHyperdetonation : SpellSkillData
  {
    [ModOption("Depth Allowance X", "This is used by the max depth damager detector to control how far in you have to stab before the event is invoked. This value is taken away from the damager's max length, the higher this is the less you have to stab.")]
    [ModOptionCategory("Hyperdetonation", 9)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.01f, 100f, 0.005f)]
    public static float allowanceX = 0.01f;
    [ModOption("Depth Allowance Y", "This is used by the max depth damager detector to control how far out you have to remove the weapon before you can trigger the event again, the higher this value the more you will have to remove.")]
    [ModOptionCategory("Hyperdetonation", 9)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.01f, 100f, 0.005f)]
    public static float allowanceY = 0.085f;
    [ModOption("Min Stab Velocity", "The minimum velocity your hand has to be for the skill to trigger.")]
    [ModOptionCategory("Hyperdetonation", 9)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.1f, 100f, 0.05f)]
    public static float minVelocity = 0.25f;
    public List<SkillSpellPair> skillSpellPairs;

    public virtual void OnImbueLoad(SpellData spell, Imbue imbue)
    {
      base.OnImbueLoad(spell, imbue);
      foreach (SkillSpellPair skillSpellPair in this.skillSpellPairs)
      {
        if (((CatalogData) imbue.spellCastBase).id == skillSpellPair.spellId && imbue.imbueCreature.HasSkill(skillSpellPair.skillId))
        {
          foreach (Damager componentsInChild in ((Component) imbue.colliderGroup.collisionHandler.item).GetComponentsInChildren<Damager>())
          {
            if ((double) componentsInChild.penetrationDepth != 0.0 && (double) componentsInChild.penetrationLength <= 0.0)
            {
              MaxDepthDetector orAddComponent = Utils.GetOrAddComponent<MaxDepthDetector>((Component) componentsInChild);
              orAddComponent.Activate(componentsInChild, new Vector2(SkillHyperdetonation.allowanceX, SkillHyperdetonation.allowanceY));
              orAddComponent.onPenetrateMaxDepth += new MaxDepthDetector.OnPenetrateMaxDepth(this.OnPenetrateMaxDepth);
            }
          }
        }
      }
    }

    private void OnPenetrateMaxDepth(
      Damager damager,
      CollisionInstance collisionInstance,
      Vector3 velocity,
      float depth)
    {
      if ((double) velocity.magnitude <= (double) SkillHyperdetonation.minVelocity || !(collisionInstance?.targetColliderGroup?.collisionHandler?.Entity is Creature entity) || entity.isPlayer)
        return;
      BrainModuleCrystal module = entity.brain.instance.GetModule<BrainModuleCrystal>(true);
      if (module.isCrystallised)
      {
        Color color = module.lerper.currentColorType == ColorType.Solid ? Dye.GetEvaluatedColor(module.lerper.currentSpellId, module.lerper.currentSpellId) : Dye.GetEvaluatedColor(module.lerper.currentSpellId, "Crystallic");
        SkillOverchargedCore.Detonate(entity, color);
      }
    }

    public virtual void OnImbueUnload(SpellData spell, Imbue imbue)
    {
      base.OnImbueUnload(spell, imbue);
      foreach (SkillSpellPair skillSpellPair in this.skillSpellPairs)
      {
        if (((CatalogData) imbue.spellCastBase).id == skillSpellPair.spellId && imbue.imbueCreature.HasSkill(skillSpellPair.skillId))
        {
          foreach (Damager componentsInChild in ((Component) imbue.colliderGroup.collisionHandler.item).GetComponentsInChildren<Damager>())
          {
            if ((double) componentsInChild.penetrationDepth != 0.0)
            {
              MaxDepthDetector component = ((Component) componentsInChild)?.GetComponent<MaxDepthDetector>();
              if ((Object) component != (Object) null)
              {
                component.Deactivate();
                component.onPenetrateMaxDepth -= new MaxDepthDetector.OnPenetrateMaxDepth(this.OnPenetrateMaxDepth);
              }
            }
          }
        }
      }
    }
  }
}
