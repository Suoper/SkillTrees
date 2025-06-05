// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillLithowebs
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Skill;
using ThunderRoad.Skill.Spell;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillLithowebs : SpellSkillData
  {
    [ModOption("Min Lithoweb Velocity", "Min velocity required to create a lithoweb")]
    [ModOptionCategory("Lithoweb", 20)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.1f, 100f, 0.1f)]
    public static float velocity = 0.1f;
    public List<Imbue> gravImbues = new List<Imbue>();

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      // ISSUE: method pointer
      EventManager.onRagdollSliced -= new EventManager.RagdollSlicedEvent((object) this, __methodptr(OnRagdollSliced));
      // ISSUE: method pointer
      EventManager.onRagdollSliced += new EventManager.RagdollSlicedEvent((object) this, __methodptr(OnRagdollSliced));
    }

    private void OnRagdollSliced(RagdollPart part, EventTime eventTime)
    {
      if (eventTime == 0)
        return;
      for (int index = 0; index < this.gravImbues.Count; ++index)
      {
        foreach (CollisionInstance collision in this.gravImbues[index].colliderGroup.collisionHandler.item.mainCollisionHandler.collisions)
        {
          int num;
          if ((Object) collision?.targetColliderGroup?.collisionHandler?.ragdollPart == (Object) part)
          {
            float? magnitude = collision?.impactVelocity.magnitude;
            float velocity = SkillLithowebs.velocity;
            num = (double) magnitude.GetValueOrDefault() > (double) velocity & magnitude.HasValue ? 1 : 0;
          }
          else
            num = 0;
          if (num != 0)
          {
            Item slicer = collision?.sourceColliderGroup?.collisionHandler?.item;
            if (slicer != null && this.IsImbueInList(slicer) && (double) Vector3.Distance(((ThunderBehaviour) slicer).transform.position, ((ThunderBehaviour) part).transform.position) < 2.5 && (Object) part.parentPart != (Object) null)
              ((ThunderBehaviour) part).gameObject.AddComponent<Lithoweb>().Init(slicer, part, part.parentPart);
          }
        }
      }
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      // ISSUE: method pointer
      EventManager.onRagdollSliced -= new EventManager.RagdollSlicedEvent((object) this, __methodptr(OnRagdollSliced));
    }

    public virtual void OnImbueLoad(SpellData spell, Imbue imbue)
    {
      base.OnImbueLoad(spell, imbue);
      if (!(imbue.spellCastBase is SpellCastGravity) || this.gravImbues.Contains(imbue))
        return;
      this.gravImbues.Add(imbue);
    }

    public virtual void OnImbueUnload(SpellData spell, Imbue imbue)
    {
      base.OnImbueUnload(spell, imbue);
      if (!(imbue.spellCastBase is SpellCastGravity) || !this.gravImbues.Contains(imbue))
        return;
      this.gravImbues.Remove(imbue);
    }

    public bool IsImbueInList(Item item)
    {
      for (int index = 0; index < item.imbues.Count; ++index)
      {
        if (this.gravImbues.Contains(item.imbues[index]))
          return true;
      }
      return false;
    }
  }
}
