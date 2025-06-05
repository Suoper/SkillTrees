// Decompiled with JetBrains decompiler
// Type: EffectInstanceExtensions
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;
using UnityEngine.VFX;

#nullable disable
public static class EffectInstanceExtensions
{
  public static void SetParticleTarget(
    this EffectInstance effectInstance,
    ForceFieldPresetData presetData,
    Transform transform,
    bool useTrigger = true,
    float triggerRadius = 0.05f)
  {
    ParticleSystemForceField systemForceField = presetData.Create(effectInstance, transform);
    SphereCollider sphereCollider = useTrigger ? ((Component) systemForceField).gameObject.AddComponent<SphereCollider>() : (SphereCollider) null;
    if ((Object) sphereCollider != (Object) null)
    {
      sphereCollider.radius = triggerRadius;
      ((Collider) sphereCollider).isTrigger = true;
    }
    foreach (ParticleSystem particleSystem in effectInstance.GetParticleSystems())
    {
      ParticleSystem.TriggerModule trigger = particleSystem.trigger;
      ParticleSystem.ExternalForcesModule externalForces = particleSystem.externalForces;
      if ((Object) sphereCollider != (Object) null)
        ((ParticleSystem.TriggerModule) ref trigger).AddCollider((Component) sphereCollider);
      if ((Object) systemForceField != (Object) null)
        ((ParticleSystem.ExternalForcesModule) ref externalForces).AddInfluence(systemForceField);
    }
  }

  public static void ClearParticleTarget(this EffectInstance effectInstance)
  {
    GameObject gameObject = effectInstance != null ? ((Component) effectInstance.GetRootParticleSystem())?.gameObject : (GameObject) null;
    ParticleSystemForceField component1 = gameObject?.GetComponent<ParticleSystemForceField>();
    SphereCollider component2 = gameObject?.gameObject.GetComponent<SphereCollider>();
    foreach (ParticleSystem particleSystem in effectInstance.GetParticleSystems())
    {
      ParticleSystem.TriggerModule trigger = particleSystem.trigger;
      ParticleSystem.ExternalForcesModule externalForces = particleSystem.externalForces;
      if ((Object) component2 != (Object) null)
        ((ParticleSystem.TriggerModule) ref trigger).RemoveCollider((Component) component2);
      if ((Object) component1 != (Object) null)
        ((ParticleSystem.ExternalForcesModule) ref externalForces).RemoveInfluence(component1);
      Object.Destroy((Object) component1);
      Object.Destroy((Object) component2);
    }
  }

  public static void SetColorImmediate(this EffectInstance effectInstance, Color color)
  {
    foreach (ParticleSystem particleSystem in effectInstance.GetParticleSystems())
    {
      ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
      ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime).enabled = true;
      ParticleSystem.MinMaxGradient color1 = ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime).color;
      ((ParticleSystem.MinMaxGradient) ref color1).color = color;
      ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime).color = ParticleSystem.MinMaxGradient.op_Implicit(color);
    }
  }

  public static VisualEffect GetVfxGraph(this EffectInstance instance)
  {
    VisualEffect vfxGraph = (VisualEffect) null;
    if (instance != null && instance.effects != null && instance.effects.Count > 0)
    {
      foreach (EffectVfx effectVfx in instance.effects.OfType<EffectVfx>())
        vfxGraph = effectVfx.vfx;
    }
    return vfxGraph;
  }

  public static void LerpIntensity(
    this EffectInstance effectInstance,
    float a,
    float b,
    float duration,
    bool endOnFinish)
  {
    ((MonoBehaviour) GameManager.local).StartCoroutine(EffectInstanceExtensions.FadeIntensity(effectInstance, a, b, duration, endOnFinish));
  }

  private static IEnumerator FadeIntensity(
    EffectInstance effectInstance,
    float a,
    float b,
    float duration,
    bool endOnFinished)
  {
    float elapsed = 0.0f;
    while ((double) elapsed < (double) duration)
    {
      float intensity = Mathf.Lerp(a, b, elapsed / duration);
      effectInstance.SetIntensity(intensity);
      elapsed += Time.deltaTime;
      yield return (object) null;
    }
    effectInstance.SetIntensity(b);
    if (endOnFinished)
      effectInstance.End(false, -1f);
  }

  public static ParticleSystem[] GetParticleSystems(this EffectInstance instance)
  {
    List<ParticleSystem> particleSystemList = new List<ParticleSystem>();
    if (instance != null && instance.effects != null && instance.effects.Count > 0)
    {
      foreach (EffectParticle effectParticle in instance.effects.OfType<EffectParticle>())
      {
        if ((Object) ((Component) effectParticle?.rootParticleSystem)?.gameObject != (Object) null)
          particleSystemList.AddRange((IEnumerable<ParticleSystem>) ((Component) effectParticle.rootParticleSystem).gameObject.GetComponentsInChildren<ParticleSystem>());
      }
    }
    return particleSystemList.ToArray();
  }

  public static ParticleSystem[] GetParticleSystems(this List<EffectInstance> instances)
  {
    List<ParticleSystem> particleSystemList = new List<ParticleSystem>();
    if (instances != null && instances.Count > 0)
    {
      for (int index = 0; index < instances.Count; ++index)
      {
        EffectInstance instance = instances[index];
        if (instance != null && instance.effects != null && instance.effects.Count > 0)
        {
          foreach (EffectParticle effectParticle in instance.effects.OfType<EffectParticle>())
          {
            if ((Object) ((Component) effectParticle?.rootParticleSystem)?.gameObject != (Object) null)
              particleSystemList.AddRange((IEnumerable<ParticleSystem>) ((Component) effectParticle.rootParticleSystem).gameObject.GetComponentsInChildren<ParticleSystem>());
          }
        }
      }
    }
    return particleSystemList.ToArray();
  }

  public static ParticleSystem GetRootParticleSystem(this EffectInstance instance)
  {
    if (instance != null && instance.effects != null && instance.effects.Count > 0)
    {
      foreach (EffectParticle effectParticle in instance.effects.OfType<EffectParticle>())
      {
        if ((Object) ((Component) effectParticle?.rootParticleSystem)?.gameObject != (Object) null)
          return effectParticle.rootParticleSystem;
      }
    }
    return (ParticleSystem) null;
  }

  public static ParticleSystem GetParticleSystem(this EffectInstance instance, string name)
  {
    foreach (ParticleSystem particleSystem in instance.GetParticleSystems())
    {
      if (((Object) particleSystem).name == name)
        return particleSystem;
    }
    return (ParticleSystem) null;
  }

  public static void SetMaxParticles(this List<EffectInstance> effectInstances, int max)
  {
    ParticleSystem[] particleSystems = effectInstances.GetParticleSystems();
    if (particleSystems == null || particleSystems.Length == 0)
      return;
    foreach (ParticleSystem particleSystem in particleSystems)
    {
      ParticleSystem.MainModule main = particleSystem.main;
      ((ParticleSystem.MainModule) ref main).maxParticles = 45;
    }
  }

  public static void SetMaxParticles(this EffectInstance effectInstance, int max)
  {
    ParticleSystem[] particleSystems = effectInstance.GetParticleSystems();
    if (particleSystems == null || particleSystems.Length == 0)
      return;
    foreach (ParticleSystem particleSystem in particleSystems)
    {
      ParticleSystem.MainModule main = particleSystem.main;
      ((ParticleSystem.MainModule) ref main).maxParticles = 45;
    }
  }

  public static bool isEmitting(this EffectInstance effectInstance)
  {
    bool flag = false;
    foreach (ParticleSystem particleSystem in effectInstance.GetParticleSystems())
    {
      if ((Object) particleSystem != (Object) null && particleSystem.isEmitting)
        flag = true;
    }
    return flag;
  }

  public static void SetSpeed(this EffectInstance effectInstance, float value, string name)
  {
    foreach (ParticleSystem particleSystem in effectInstance.GetParticleSystems())
    {
      if ((Object) particleSystem != (Object) null && ((Component) particleSystem).gameObject.name == name)
      {
        ParticleSystem.MainModule main = particleSystem.main;
        ParticleSystem.Particle[] particleArray = new ParticleSystem.Particle[((ParticleSystem.MainModule) ref main).maxParticles];
        int particles = particleSystem.GetParticles(particleArray);
        for (int index = 0; index < particles; ++index)
        {
          ParticleSystem.Particle particle = particleArray[index];
          ((ParticleSystem.Particle) ref particle).velocity = ((ParticleSystem.Particle) ref particle).velocity.normalized * value;
          particleArray[index] = particle;
        }
        particleSystem.SetParticles(particleArray, particles);
      }
    }
  }

  public static void SetLifetime(
    this EffectInstance effectInstance,
    float lifetimeValue,
    string name)
  {
    foreach (ParticleSystem particleSystem in effectInstance.GetParticleSystems())
    {
      if ((Object) particleSystem != (Object) null && ((Component) particleSystem).gameObject.name == name)
      {
        ParticleSystem.MainModule main = particleSystem.main;
        ParticleSystem.Particle[] particleArray = new ParticleSystem.Particle[((ParticleSystem.MainModule) ref main).maxParticles];
        int particles = particleSystem.GetParticles(particleArray);
        for (int index = 0; index < particles; ++index)
        {
          ParticleSystem.Particle particle = particleArray[index];
          ((ParticleSystem.Particle) ref particle).remainingLifetime = lifetimeValue;
          particleArray[index] = particle;
        }
        particleSystem.SetParticles(particleArray, particles);
      }
    }
  }

  public static void SetConeAngle(
    this EffectInstance effectInstance,
    float coneAngleValue,
    string name)
  {
    foreach (ParticleSystem particleSystem in effectInstance.GetParticleSystems())
    {
      if ((Object) particleSystem != (Object) null && ((Component) particleSystem).gameObject.name == name)
      {
        ParticleSystem.ShapeModule shape = particleSystem.shape;
        ((ParticleSystem.ShapeModule) ref shape).angle = coneAngleValue;
      }
    }
  }

  public static void ForceStop(
    this EffectInstance effectInstance,
    ParticleSystemStopBehavior stopBehavior)
  {
    foreach (ParticleSystem particleSystem in effectInstance.GetParticleSystems())
      particleSystem.Stop(true, stopBehavior);
  }

  public static void ForceStop(
    this List<EffectInstance> effectInstances,
    ParticleSystemStopBehavior stopBehavior)
  {
    foreach (ParticleSystem particleSystem in effectInstances.GetParticleSystems())
      particleSystem.Stop(true, stopBehavior);
  }
}
