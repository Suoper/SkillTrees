// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillStingshot
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using System.Collections;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillStingshot : SpellSkillData
  {
    public StatusData statusData;
    public EffectData tetherEffectData;
    public string tetherEffectId = "Stingshot";

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.tetherEffectData = Catalog.GetData<EffectData>(this.tetherEffectId, true);
      this.statusData = Catalog.GetData<StatusData>("Floating", true);
    }

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      Stinger.onStingerSpawn += new Stinger.StingerEvent(this.OnStingerSpawn);
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      Stinger.onStingerSpawn -= new Stinger.StingerEvent(this.OnStingerSpawn);
    }

    private void OnStingerSpawn(Stinger stinger)
    {
      stinger.onStingerStab += new Stinger.OnStingerStab(this.OnStingerStab);
    }

    private void OnStingerStab(
      Stinger stinger,
      Damager damager,
      CollisionInstance collisionInstance,
      Creature hitCreature)
    {
      stinger.onStingerStab -= new Stinger.OnStingerStab(this.OnStingerStab);
      Utils.RunAfter((MonoBehaviour) stinger, (Action) (() => ((MonoBehaviour) stinger).StartCoroutine(this.ThrowRoutine(stinger))), 0.065f, false);
    }

    public IEnumerator ThrowRoutine(Stinger stinger)
    {
      if (stinger.spellCastCrystallic != null)
      {
        EffectInstance effectInstance = (EffectInstance) null;
        float startTime = Time.time;
        bool gripped = false;
        RagdollHand hand = stinger.spellCastCrystallic.spellCaster.ragdollHand;
        yield return (object) new WaitForSeconds(0.1f);
        stinger?.spellCastCrystallic.spellCaster.telekinesis.Disable((object) this);
        while ((double) Time.time - (double) startTime < 0.5)
        {
          if (hand.playerHand.controlHand.gripPressed && (UnityEngine.Object) hand.grabbedHandle == (UnityEngine.Object) null)
          {
            gripped = true;
            break;
          }
          yield return (object) null;
        }
        if (gripped)
        {
          ((ThunderEntity) Player.currentCreature).Inflict(this.statusData, (object) this, float.PositiveInfinity, (object) new FloatingParams(1f, 2f, 1f, true), true);
          if (this.tetherEffectData != null)
          {
            effectInstance = this.tetherEffectData.Spawn(stinger.spellCastCrystallic.spellCaster.magicSource, true, (ColliderGroup) null, false);
            effectInstance.SetSource(stinger.spellCastCrystallic.spellCaster.magicSource);
            effectInstance.SetTarget(stinger.transform);
            effectInstance.Play(0, false, false);
          }
          while (hand.playerHand.controlHand.gripPressed)
            yield return (object) null;
          Vector3 forward = stinger.spellCastCrystallic.spellCaster.ragdollHand.Velocity();
          effectInstance?.End(false, -1f);
          ((ThunderEntity) Player.currentCreature).Remove(this.statusData, (object) this);
          if ((double) forward.sqrMagnitude >= (double) SpellCaster.throwMinHandVelocity * (double) SpellCaster.throwMinHandVelocity)
            Player.local.AddForce(-forward, forward.magnitude * 2f);
          forward = new Vector3();
        }
        stinger.spellCastCrystallic.spellCaster.telekinesis.Enable((object) this);
        ((ThunderEntity) Player.currentCreature).Remove(this.statusData, (object) this);
      }
    }
  }
}
