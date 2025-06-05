// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillSlowTimeData
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using ThunderRoad;
using ThunderRoad.Skill;
using ThunderRoad.Skill.SpellPower;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillSlowTimeData : SpellSkillData
  {
    public static bool timeSlowed;

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      // ISSUE: method pointer
      SpellPowerSlowTime.OnTimeScaleChangeEvent += new SpellPowerSlowTime.TimeScaleChangeEvent((object) this, __methodptr(OnTimeScaleChangeEvent));
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      // ISSUE: method pointer
      SpellPowerSlowTime.OnTimeScaleChangeEvent -= new SpellPowerSlowTime.TimeScaleChangeEvent((object) this, __methodptr(OnTimeScaleChangeEvent));
    }

    private void OnTimeScaleChangeEvent(SpellPowerSlowTime spell, float scale)
    {
      if (TimeManager.slowMotionState == 1)
      {
        this.OnSlowMotionEnter(spell, scale);
      }
      else
      {
        if (TimeManager.slowMotionState != 3)
          return;
        this.OnSlowMotionExit(spell);
      }
    }

    public virtual void OnSlowMotionEnter(SpellPowerSlowTime spellPowerSlowTime, float scale)
    {
      SkillSlowTimeData.timeSlowed = true;
    }

    public virtual void OnSlowMotionExit(SpellPowerSlowTime spellPowerSlowTime)
    {
      SkillSlowTimeData.timeSlowed = false;
    }
  }
}
