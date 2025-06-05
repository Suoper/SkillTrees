// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SkillArcaneBarrage
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Spells;
using ThunderRoad;
using ThunderRoad.Skill;

#nullable disable
namespace Arcana.Skills
{
  internal class SkillArcaneBarrage : SpellSkillData
  {
    public int projectileCount;
    public float projectileTimeBetween;

    public virtual void OnCatalogRefresh() => ((SkillData) this).OnCatalogRefresh();

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is ArcaneBolt arcaneBolt))
        return;
      arcaneBolt.projectileManager.projectileCount = this.projectileCount;
      arcaneBolt.projectileManager.projectileTimeBetween = new float?(this.projectileTimeBetween);
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      if (!(spell is ArcaneBolt arcaneBolt))
        return;
      arcaneBolt.projectileManager.projectileCount = arcaneBolt.projectileCount;
      arcaneBolt.projectileManager.projectileTimeBetween = new float?(arcaneBolt.projectileTimeBetween);
    }
  }
}
