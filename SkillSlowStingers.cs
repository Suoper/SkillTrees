// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillSlowStingers
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Skill.Spell;
using ThunderRoad.Skill.SpellPower;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillSlowStingers : SkillSlowTimeData
  {
    public List<Stinger> slowed = new List<Stinger>();

    public override void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      Stinger.onStingerSpawn -= new Stinger.StingerEvent(this.OnStingerSpawn);
      Stinger.onStingerSpawn += new Stinger.StingerEvent(this.OnStingerSpawn);
    }

    public override void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      Stinger.onStingerSpawn -= new Stinger.StingerEvent(this.OnStingerSpawn);
    }

    public override void OnSlowMotionEnter(SpellPowerSlowTime spellPowerSlowTime, float scale)
    {
      base.OnSlowMotionEnter(spellPowerSlowTime, scale);
      foreach (Stinger stinger in Stinger.all)
      {
        Utils.GetOrAddComponent<VelocityStorer>((Component) stinger.item)?.Activate(2f);
        ((ThunderEntity) stinger.item).Inflict("Slowed", (object) this, float.PositiveInfinity, (object) null, true);
      }
    }

    private void OnStingerSpawn(Stinger stinger)
    {
      if (!SkillSlowTimeData.timeSlowed)
        return;
      ((ThunderEntity) stinger.item).Inflict("Slowed", (object) this, float.PositiveInfinity, (object) null, true);
      Utils.GetOrAddComponent<VelocityStorer>((Component) stinger.item)?.Activate(2f);
    }

    public override void OnSlowMotionExit(SpellPowerSlowTime spellPowerSlowTime)
    {
      base.OnSlowMotionExit(spellPowerSlowTime);
      foreach (Stinger stinger in this.slowed)
      {
        ((ThunderEntity) stinger.item).Remove("Slowed", (object) this);
        ((Component) stinger.item).GetComponent<VelocityStorer>()?.Deactivate();
      }
    }
  }
}
