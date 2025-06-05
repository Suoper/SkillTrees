// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillCrystalDilation
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using ThunderRoad;
using ThunderRoad.Skill.SpellPower;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillCrystalDilation : SkillSlowTimeData
  {
    public override void OnSlowMotionEnter(SpellPowerSlowTime spellPowerSlowTime, float scale)
    {
      base.OnSlowMotionEnter(spellPowerSlowTime, scale);
      foreach (Creature creature in Creature.allActive)
      {
        BrainModuleCrystal module = creature.brain.instance.GetModule<BrainModuleCrystal>(true);
        if (module.isCrystallised)
        {
          module.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, "Mind"), "Mind");
          ((ThunderEntity) creature).Inflict("Slowed", (object) this, 2.5f, (object) null, true);
        }
      }
    }
  }
}
