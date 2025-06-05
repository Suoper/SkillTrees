// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillStatusPair
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using ThunderRoad;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillStatusPair : SkillSpellPair
  {
    public string statusId;
    public float statusParameter;
    public float statusDuration;
    public bool playEffects;

    public virtual void Inflict(Creature creature)
    {
      ((ThunderEntity) creature).Inflict(this.statusId, (object) this, this.statusDuration, (object) this.statusParameter, this.playEffects);
    }
  }
}
