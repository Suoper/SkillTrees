// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SkillArcaneSapping
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using ThunderRoad.Skill.Spell;

#nullable disable
namespace Arcana.Skills
{
  internal class SkillArcaneSapping : SkillSapStatusApplier
  {
    public float instabilityTransferPerBolt = 10f;

    public virtual void ApplyStatus(SpellCastLightning spell)
    {
      spell.AddStatus((object) this, this.statusData, this.statusDuration, (object) this.instabilityTransferPerBolt);
    }
  }
}
