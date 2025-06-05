// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillCrystalFracture
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using ThunderRoad;
using ThunderRoad.Skill;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillCrystalFracture : SpellSkillData
  {
    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      BrainModuleCrystal.SetBreakForce(true);
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      BrainModuleCrystal.SetBreakForce(false);
    }
  }
}
