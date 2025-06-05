// Decompiled with JetBrains decompiler
// Type: Crystallic.AI.GolemBrain
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace Crystallic.AI
{
  public class GolemBrain : ThunderBehaviour
  {
    private static GolemBrain _instance;
    public List<GolemBrainModule> modules = new List<GolemBrainModule>();
    public List<string> moduleAddresses = new List<string>();
    public static bool effectsActive;
    public Dictionary<Part, GolemPart> parts = new Dictionary<Part, GolemPart>();
    public List<EffectInstance> activeEffects = new List<EffectInstance>();

    public static GolemBrain Instance
    {
      get
      {
        if ((UnityEngine.Object) GolemBrain._instance == (UnityEngine.Object) null)
        {
          if ((UnityEngine.Object) Golem.local != (UnityEngine.Object) null)
            GolemBrain._instance = ((ThunderBehaviour) Golem.local).gameObject.AddComponent<GolemBrain>();
          else
            Debug.LogWarning((object) "GolemBrain: Golem is null, cannot instantiate or access GolemBrain!");
        }
        return GolemBrain._instance;
      }
    }

    public event GolemBrain.BrainModuleEvent onGolemBrainModuleLoaded;

    public event GolemBrain.BrainModuleEvent onGolemBrainModuleUnloaded;

    private void OnDestroy()
    {
      for (int index = 0; index < this.modules.Count; ++index)
      {
        this.modules[index].Unload(Golem.local);
        GolemBrain.BrainModuleEvent brainModuleUnloaded = this.onGolemBrainModuleUnloaded;
        if (brainModuleUnloaded != null)
          brainModuleUnloaded(this.modules[index]);
      }
      this.modules.Clear();
    }

    public void Initialize(List<string> moduleAddresses)
    {
      this.moduleAddresses = moduleAddresses;
      Debug.Log((object) (string.Format("Golem brain instance created with {0} modules loaded:\n - ", (object) this.moduleAddresses.Count) + string.Join("\n - ", (IEnumerable<string>) this.moduleAddresses)));
      ((GolemController) Golem.local).defeatEvent.RemoveListener(new UnityAction(this.OnDefeat));
      ((GolemController) Golem.local).defeatEvent.AddListener(new UnityAction(this.OnDefeat));
      HashSet<Type> typeSet = new HashSet<Type>((IEnumerable<Type>) this.modules.ConvertAll<Type>((Converter<GolemBrainModule, Type>) (m => ((object) m).GetType())));
      ((GolemController) Golem.local).Brain().InitialiseParts();
      for (int index = 0; index < moduleAddresses.Count; ++index)
      {
        Type type = Type.GetType(moduleAddresses[index]);
        if (type == (Type) null)
          Debug.LogError((object) ("Type not found for address: " + moduleAddresses[index]));
        else if (typeof (GolemBrainModule).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
        {
          try
          {
            if (!typeSet.Contains(type))
            {
              GolemBrainModule golemBrainModule = this.gameObject.AddComponent(type) as GolemBrainModule;
              this.modules.Add(golemBrainModule);
              typeSet.Add(type);
              golemBrainModule?.Load(Golem.local);
              GolemBrain.BrainModuleEvent brainModuleLoaded = this.onGolemBrainModuleLoaded;
              if (brainModuleLoaded != null)
                brainModuleLoaded(golemBrainModule);
            }
          }
          catch (Exception ex)
          {
            Debug.LogError((object) string.Format("Failed to load Golem Brain Module of type: {0}: {1}", (object) type, (object) ex.Message));
          }
        }
      }
    }

    private void OnDefeat()
    {
      for (int index = 0; index < this.modules.Count; ++index)
      {
        this.modules[index].Unload(Golem.local);
        GolemBrain.BrainModuleEvent brainModuleUnloaded = this.onGolemBrainModuleUnloaded;
        if (brainModuleUnloaded != null)
          brainModuleUnloaded(this.modules[index]);
      }
      UnityEngine.Object.Destroy((UnityEngine.Object) this);
    }

    public GolemBrainModule Add<T>()
    {
      bool flag = true;
      foreach (object module in this.modules)
      {
        if (module.GetType() == typeof (T))
          flag = false;
      }
      if (flag)
      {
        GolemBrainModule golemBrainModule = this.gameObject.AddComponent(typeof (T)) as GolemBrainModule;
        this.modules.Add(golemBrainModule);
        GolemBrain.BrainModuleEvent brainModuleLoaded = this.onGolemBrainModuleLoaded;
        if (brainModuleLoaded != null)
          brainModuleLoaded(golemBrainModule);
        golemBrainModule.Load(Golem.local);
        return golemBrainModule;
      }
      Debug.LogError((object) string.Format("Cannot add module of type: {0}! Module already exists.", (object) typeof (T)));
      return (GolemBrainModule) null;
    }

    public void Remove(GolemBrainModule golemBrainModule)
    {
      if ((UnityEngine.Object) Golem.local == (UnityEngine.Object) null)
        return;
      GolemBrainModule golemBrainModule1 = this.modules.Find((Predicate<GolemBrainModule>) (m => (UnityEngine.Object) m == (UnityEngine.Object) golemBrainModule));
      if (!((UnityEngine.Object) golemBrainModule1 != (UnityEngine.Object) null) || !this.modules.Contains(golemBrainModule1))
        return;
      this.modules.Remove(golemBrainModule1);
      golemBrainModule1.Unload(Golem.local);
      GolemBrain.BrainModuleEvent brainModuleUnloaded = this.onGolemBrainModuleUnloaded;
      if (brainModuleUnloaded != null)
        brainModuleUnloaded(golemBrainModule1);
      UnityEngine.Object.Destroy((UnityEngine.Object) golemBrainModule1);
    }

    public GolemBrainModule GetModule<T>(bool addIfNotFound = true)
    {
      GolemBrainModule module = this.modules.Find((Predicate<GolemBrainModule>) (m => ((object) m).GetType() == typeof (T)));
      if (addIfNotFound && !(bool) (UnityEngine.Object) module)
        module = this.Add<T>();
      return module;
    }

    public bool TryGetModule<T>(out T brainModule) where T : GolemBrainModule
    {
      brainModule = this.modules.Find((Predicate<GolemBrainModule>) (module => module is T)) as T;
      return (UnityEngine.Object) (object) brainModule != (UnityEngine.Object) null;
    }

    public GolemPart GetPartByName(string name)
    {
      return this.parts.Values.First<GolemPart>((Func<GolemPart, bool>) (p => ((UnityEngine.Object) p).name == name));
    }

    public GolemPart GetPart(Part part, bool log = false)
    {
      GolemPart part1 = (GolemPart) null;
      if ((UnityEngine.Object) Golem.local == (UnityEngine.Object) null)
        return (GolemPart) null;
      foreach (KeyValuePair<Part, GolemPart> part2 in this.parts)
      {
        if (part2.Key == part)
          part1 = part2.Value;
        if (log)
          Debug.Log((object) string.Format("{0} == {1} | {2}", (object) part2.Key, (object) part, (object) (part2.Key == part)));
      }
      return part1;
    }

    public void InitialiseParts()
    {
      Animator animator = ((GolemController) Golem.local).animator;
      foreach (Part key in Enum.GetValues(typeof (Part)))
      {
        HumanBodyBones humanBodyBones = (HumanBodyBones) key;
        Transform boneTransform = animator.GetBoneTransform(humanBodyBones);
        if (!((UnityEngine.Object) boneTransform == (UnityEngine.Object) null))
        {
          GolemPart golemPart = key == Part.Head ? (GolemPart) boneTransform.gameObject.AddComponent<HeadPart>() : boneTransform.gameObject.AddComponent<GolemPart>();
          golemPart.part = key;
          if (!this.parts.ContainsKey(key))
            this.parts.Add(key, golemPart);
        }
      }
      if (this.parts.Count <= 0)
        return;
      this.RefreshParts();
    }

    private IEnumerator EffectRoutine(
      float duration,
      EffectData upperArmLeftData,
      EffectData upperArmRightData,
      EffectData lowerArmLeftData,
      EffectData lowerArmRightData,
      EffectData torsoData,
      EffectData upperLegLeftData,
      EffectData upperLegRightData,
      EffectData lowerLegLeftData,
      EffectData lowerLegRightData,
      bool infinite)
    {
      GolemBrain.effectsActive = true;
      Transform upperLeftArm = ((ThunderBehaviour) Golem.local).transform.GetChildByNameRecursive("LeftShoulder");
      GolemPart upperRightArm = this.GetPart(Part.RightUpperArm);
      GolemPart lowerLeftArm = this.GetPart(Part.LeftLowerArm);
      GolemPart lowerRightArm = this.GetPart(Part.RightLowerArm);
      GolemPart torso = this.GetPart(Part.Chest);
      GolemPart upperLeftLeg = this.GetPart(Part.LeftUpperLeg);
      GolemPart upperRightLeg = this.GetPart(Part.RightUpperLeg);
      GolemPart lowerLeftLeg = this.GetPart(Part.LeftLowerLeg);
      GolemPart lowerRightLeg = this.GetPart(Part.RightLowerLeg);
      if (upperArmLeftData != null)
      {
        EffectInstance upperArmLeftGolemEffect = upperArmLeftData?.Spawn(upperLeftArm.transform, true, (ColliderGroup) null, false);
        this.activeEffects.Add(upperArmLeftGolemEffect);
        upperArmLeftGolemEffect = (EffectInstance) null;
      }
      if (upperArmRightData != null)
      {
        EffectInstance upperArmRightGolemEffect = upperArmRightData?.Spawn(upperRightArm.transform, true, (ColliderGroup) null, false);
        this.activeEffects.Add(upperArmRightGolemEffect);
        upperArmRightGolemEffect = (EffectInstance) null;
      }
      if (lowerArmLeftData != null)
      {
        EffectInstance lowerArmLeftGolemEffect = lowerArmLeftData?.Spawn(lowerLeftArm.transform, true, (ColliderGroup) null, false);
        this.activeEffects.Add(lowerArmLeftGolemEffect);
        lowerArmLeftGolemEffect = (EffectInstance) null;
      }
      if (lowerArmRightData != null)
      {
        EffectInstance lowerArmRightGolemEffect = lowerArmRightData?.Spawn(lowerRightArm.transform, true, (ColliderGroup) null, false);
        this.activeEffects.Add(lowerArmRightGolemEffect);
        lowerArmRightGolemEffect = (EffectInstance) null;
      }
      if (torsoData != null)
      {
        EffectInstance torsoGolemEffect = torsoData?.Spawn(torso.transform, true, (ColliderGroup) null, false);
        this.activeEffects.Add(torsoGolemEffect);
        torsoGolemEffect = (EffectInstance) null;
      }
      if (upperLegLeftData != null)
      {
        EffectInstance upperLegLeftGolemEffect = upperLegLeftData?.Spawn(upperLeftLeg.transform, true, (ColliderGroup) null, false);
        this.activeEffects.Add(upperLegLeftGolemEffect);
        upperLegLeftGolemEffect = (EffectInstance) null;
      }
      if (upperLegRightData != null)
      {
        EffectInstance upperLegRightGolemEffect = upperLegRightData?.Spawn(upperRightLeg.transform, true, (ColliderGroup) null, false);
        this.activeEffects.Add(upperLegRightGolemEffect);
        upperLegRightGolemEffect = (EffectInstance) null;
      }
      if (lowerLegLeftData != null)
      {
        EffectInstance lowerLegLeftGolemEffect = lowerLegLeftData?.Spawn(lowerLeftLeg.transform, true, (ColliderGroup) null, false);
        this.activeEffects.Add(lowerLegLeftGolemEffect);
        lowerLegLeftGolemEffect = (EffectInstance) null;
      }
      if (lowerLegRightData != null)
      {
        EffectInstance lowerLegRightGolemEffect = lowerLegRightData?.Spawn(lowerRightLeg.transform, true, (ColliderGroup) null, false);
        this.activeEffects.Add(lowerLegRightGolemEffect);
        lowerLegRightGolemEffect = (EffectInstance) null;
      }
      if (this.activeEffects.Count > 0)
      {
        int i = 0;
        while (true)
        {
          int num = i;
          int? count = this.activeEffects?.Count;
          int valueOrDefault = count.GetValueOrDefault();
          if (num < valueOrDefault & count.HasValue)
          {
            EffectInstance effectInstance = this.activeEffects[i];
            effectInstance.Play(0, false, false);
            effectInstance = (EffectInstance) null;
            ++i;
          }
          else
            break;
        }
      }
      if (!infinite)
      {
        yield return (object) new WaitForSeconds(duration);
        if (this.activeEffects.Count > 0)
        {
          int i = 0;
          while (true)
          {
            int num = i;
            int? count = this.activeEffects?.Count;
            int valueOrDefault = count.GetValueOrDefault();
            if (num < valueOrDefault & count.HasValue)
            {
              EffectInstance effectInstance = this.activeEffects[i];
              effectInstance.End(false, -1f);
              this.activeEffects?.RemoveAt(i);
              effectInstance = (EffectInstance) null;
              ++i;
            }
            else
              break;
          }
        }
        GolemBrain.effectsActive = false;
      }
      else
        yield return (object) null;
    }

    public List<EffectInstance> PlayFullBodyEffect(
      float duration,
      EffectData upperArmLeftData = null,
      EffectData upperArmRightData = null,
      EffectData lowerArmLeftData = null,
      EffectData lowerArmRightData = null,
      EffectData torsoData = null,
      EffectData upperLegLeftData = null,
      EffectData upperLegRightData = null,
      EffectData lowerLegLeftData = null,
      EffectData lowerLegRightData = null,
      bool infinite = false)
    {
      if (!GolemBrain.effectsActive)
        ((MonoBehaviour) Golem.local).StartCoroutine(this.EffectRoutine(duration, upperArmLeftData, upperArmRightData, lowerArmLeftData, lowerArmRightData, torsoData, upperLegLeftData, upperLegRightData, lowerLegLeftData, lowerLegRightData, infinite));
      return this.activeEffects;
    }

    public void RefreshParts()
    {
      foreach (KeyValuePair<Part, GolemPart> part in this.parts)
        part.Value.UpdateChildParts();
    }

    public delegate void BrainModuleEvent(GolemBrainModule golemBrainModule);
  }
}
