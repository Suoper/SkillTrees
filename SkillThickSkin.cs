// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillThickSkin
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.Skill.Spell;
using System;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillThickSkin : SpellSkillData
  {
    public int key;
    public float damageMultiplier = 0.0f;
    public Vector2 defaultRandomness;
    private static Vector2 randomness;
    private static Vector2 defaultStaticRandomness;
    public SpellCastCrystallic spellCastCrystallic;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.spellCastCrystallic = Catalog.GetData<SpellCastCharge>("Crystallic", true) as SpellCastCrystallic;
    }

    public static void SetRandomness(Vector2 randomness) => SkillThickSkin.randomness = randomness;

    public static void ClearRandomness()
    {
      SkillThickSkin.randomness = SkillThickSkin.defaultStaticRandomness;
    }

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      SkillThickSkin.defaultStaticRandomness = this.defaultRandomness;
      // ISSUE: method pointer
      EventManager.onCreatureHit += new EventManager.CreatureHitEvent((object) this, __methodptr(OnCreatureHit));
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      // ISSUE: method pointer
      EventManager.onCreatureHit -= new EventManager.CreatureHitEvent((object) this, __methodptr(OnCreatureHit));
    }

    private void OnCreatureHit(
      Creature creature,
      CollisionInstance collisionInstance,
      EventTime eventTime)
    {
      if (creature.isPlayer && eventTime == null && UnityEngine.Random.Range((int) SkillThickSkin.randomness.x, (int) SkillThickSkin.randomness.y) == this.key)
      {
        switch (collisionInstance.sourceColliderGroup?.collisionHandler?.Entity)
        {
          case Item obj:
            if (obj.owner != 1 && (UnityEngine.Object) obj.mainHandler != (UnityEngine.Object) null)
            {
              Hit(obj.mainHandler.creature);
              break;
            }
            break;
          case Creature pushedCreature when !creature.isPlayer:
            Hit(pushedCreature);
            break;
        }
        if (this.spellCastCrystallic != null && this.spellCastCrystallic.imbueCollisionEffectData != null && (bool) (UnityEngine.Object) collisionInstance.targetCollider && (bool) (UnityEngine.Object) collisionInstance.sourceCollider)
          this.spellCastCrystallic?.imbueCollisionEffectData?.Spawn(collisionInstance.contactPoint, Quaternion.LookRotation(collisionInstance.contactNormal, ((Component) collisionInstance.sourceCollider).transform.up), ((Component) collisionInstance?.targetCollider)?.transform, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>()).Play(0, false, false);
        creature?.SetDamageMultiplier((object) this, this.damageMultiplier);
      }
      else
      {
        if (!creature.isPlayer || eventTime != 1)
          return;
        creature.RemoveDamageMultiplier((object) this);
      }

      void Hit(Creature pushedCreature)
      {
        pushedCreature.TryPush((Creature.PushType) 3, ((ThunderBehaviour) pushedCreature).transform.position - ((ThunderBehaviour) creature).transform.position, 1, (RagdollPart.Type) 4);
      }
    }
  }
}
