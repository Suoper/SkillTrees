// Decompiled with JetBrains decompiler
// Type: Crystallic.AI.BrainModuleCrystal
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Crystallic.AI
{
  public class BrainModuleCrystal : BrainData.Module
  {
    [ModOption("Allow Player Crystallisation", "Allows you to disable being crystallised. For those of you who don't like it.")]
    [ModOptionCategory("Crystallisation", 0)]
    public static bool allowPlayerCrystallisation = true;
    [ModOption("Crystallisation Quality", "Controls the max particles active for crystallisation Vfx Per limb, higher values will cause severe performance issues on lower end machines.")]
    [ModOptionCategory("Crystallisation", 0)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 100f, 1f)]
    public static float crystallisationParticleQuality = 40f;
    [ModOption("Crystallisation Break Force Multiplier", "Controls how easy it is to dismember enemies when crystallised, higher is more difficult.")]
    [ModOptionCategory("Crystallisation", 0)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 50f, 0.1f)]
    public static float breakForceMultiplier = 2.5f;
    [ModOption("Crystallisation Damage Multiplier", "Controls the damage multiplier applied to crystallised creatures. The higher this value is, the more damage they'll take.")]
    [ModOptionCategory("Crystallisation", 0)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 50f, 0.1f)]
    public static float creatureDamageMultiplier = 1.5f;
    protected static bool allowBreakForce;
    public List<BoneEffectPair> boneEffectPairs = new List<BoneEffectPair>();
    public List<EffectInstance> instances = new List<EffectInstance>();
    public bool isCrystallised;
    public Lerper lerper;
    public BoneEffectPair selected;
    public EffectData endEffectData;

    public event BrainModuleCrystal.OnCrystalliseStart onCrystalliseStart;

    public event BrainModuleCrystal.OnCrystalliseStop onCrystalliseStop;

    public virtual void Load(Creature creature)
    {
      base.Load(creature);
      this.lerper = new Lerper();
      foreach (BoneEffectPair boneEffectPair in this.boneEffectPairs)
      {
        if (boneEffectPair.creatureIds.Contains(creature.creatureId))
        {
          this.selected = boneEffectPair;
          this.selected.Load(creature);
        }
      }
      this.endEffectData = Catalog.GetData<EffectData>("EndRagdollCrystallic", true);
      // ISSUE: method pointer
      creature.OnDespawnEvent += new Creature.DespawnEvent((object) this, __methodptr(OnDespawnEvent));
    }

    private void OnDespawnEvent(EventTime eventTime)
    {
      if (eventTime == 1)
        return;
      this.SetColor(Color.white, "Crystallic", 0.01f);
      ((ValueHandler<float>) this.creature.currentLocomotion.globalMoveSpeedMultiplier).Remove((object) this);
      this.creature.locomotion.allowMove = true;
      this.creature.locomotion.allowTurn = true;
      this.creature.RemoveDamageMultiplier((object) this);
      this.creature.ragdoll.DisableCharJointBreakForce();
    }

    public static void SetBreakForce(bool active) => BrainModuleCrystal.allowBreakForce = active;

    public bool Crystallise(float duration, string spellId = null)
    {
      if (!this.isCrystallised)
      {
        this.isCrystallised = true;
        ((MonoBehaviour) this.creature).StartCoroutine(this.CrystalliseRoutine(duration));
        if (!string.IsNullOrEmpty(spellId))
          this.SetColor(Dye.GetEvaluatedColor(this.lerper.currentSpellId, spellId), spellId);
      }
      return !this.isCrystallised;
    }

    public void SetColor(Color target, string spellId, float time = 1f)
    {
      ParticleSystem[] particleSystems = this.instances.GetParticleSystems();
      this.lerper.SetColor(target, particleSystems, spellId, time);
    }

    public void SetEffects(bool active)
    {
      if (active && this.selected != null && this.selected.boneDataPairs != null && this.selected.boneDataPairs.Count > 0)
      {
        foreach (KeyValuePair<string, EffectData> boneDataPair in this.selected.boneDataPairs)
        {
          if (boneDataPair.Value != null && !string.IsNullOrEmpty(boneDataPair.Key))
          {
            RagdollPart partByName = this.creature?.ragdoll?.GetPartByName(boneDataPair.Key);
            if ((UnityEngine.Object) partByName != (UnityEngine.Object) null)
            {
              EffectInstance effectInstance = boneDataPair.Value.Spawn(((ThunderBehaviour) partByName).transform.position, Quaternion.LookRotation(partByName.upDirection, partByName.forwardDirection), ((ThunderBehaviour) partByName).transform, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
              effectInstance?.Play(0, false, false);
              this.instances.Add(effectInstance);
            }
          }
        }
        this.instances.SetMaxParticles(Mathf.RoundToInt(BrainModuleCrystal.crystallisationParticleQuality));
      }
      else
      {
        for (int index = this.instances.Count - 1; index >= 0; --index)
        {
          this.instances[index]?.End(false, -1f);
          this.instances.RemoveAt(index);
        }
      }
    }

    private IEnumerator CrystalliseRoutine(float duration)
    {
      if (!this.creature.isPlayer || BrainModuleCrystal.allowPlayerCrystallisation)
      {
        this.SetEffects(true);
        yield return (object) new WaitForSeconds(0.25f);
        ((ThunderEntity) this.creature).Inflict("LockMovement", (object) this, duration, (object) null, true);
        BrainModuleCrystal.OnCrystalliseStart crystalliseStart = this.onCrystalliseStart;
        if (crystalliseStart != null)
          crystalliseStart(this.creature);
        this.creature.locomotion.allowMove = false;
        this.creature.locomotion.allowTurn = false;
        if (this.creature.isPlayer)
          ((ValueHandler<float>) this.creature.currentLocomotion.globalMoveSpeedMultiplier).Add((object) this, 0.0f);
        this.creature.SetDamageMultiplier((object) this, BrainModuleCrystal.creatureDamageMultiplier);
        if (!this.creature.isPlayer)
        {
          this.creature.brain.Stop();
          if (SkillDiscombobulate.CreatureStunned(this.creature))
            SkillDiscombobulate.BrainToggle(this.creature, true, false);
          this.creature.ragdoll.SetState((Ragdoll.State) 1);
          this.creature.ragdoll.SetState((Ragdoll.State) 2);
          if (BrainModuleCrystal.allowBreakForce)
            this.creature.ragdoll.EnableCharJointBreakForce(BrainModuleCrystal.breakForceMultiplier);
          BrainModuleFear module1 = this.creature.brain.instance.GetModule<BrainModuleFear>(true);
          if (module1 != null && module1.isCowering)
            module1.StopPanic();
          if (GameManager.CheckContentActive((BuildSettings.ContentFlag) 512, (BuildSettings.ContentFlagBehaviour) 0) && !this.creature.isKilled)
          {
            BrainModuleSpeak module2 = this.creature.brain.instance.GetModule<BrainModuleSpeak>(false);
            if (module2 != null)
            {
              module2.StopSpeak(true);
              module2.Play(BrainModuleSpeak.hashFalling, true, true, -1);
            }
            module2 = (BrainModuleSpeak) null;
          }
          module1 = (BrainModuleFear) null;
        }
        yield return (object) new WaitForSeconds(duration);
        if (this.creature.isPlayer)
          ((ValueHandler<float>) this.creature.currentLocomotion.globalMoveSpeedMultiplier).Remove((object) this);
        this.creature.locomotion.allowMove = true;
        this.creature.locomotion.allowTurn = true;
        this.creature.RemoveDamageMultiplier((object) this);
        if (!this.creature.isPlayer)
        {
          this.creature.brain.Load(((CatalogData) this.creature.brain.instance).id);
          this.creature.ragdoll.SetState((Ragdoll.State) 1);
          this.creature.ragdoll.DisableCharJointBreakForce();
        }
        this.SetEffects(false);
        this.isCrystallised = false;
        EffectInstance instance = this.endEffectData.Spawn(((ThunderBehaviour) this.creature.ragdoll.targetPart).transform, true, (ColliderGroup) null, false);
        instance.Play(0, false, false);
        instance.SetColorImmediate(this.lerper.currentColor);
        BrainModuleCrystal.OnCrystalliseStop onCrystalliseStop = this.onCrystalliseStop;
        if (onCrystalliseStop != null)
          onCrystalliseStop(this.creature);
      }
    }

    public delegate void OnCrystalliseStart(Creature callback);

    public delegate void OnCrystalliseStop(Creature callback);
  }
}
