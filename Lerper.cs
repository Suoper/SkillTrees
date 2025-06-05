// Decompiled with JetBrains decompiler
// Type: Crystallic.Lerper
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System.Collections;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic
{
  public class Lerper
  {
    public ColorType currentColorType = ColorType.Solid;
    public Color currentColor = Color.white;
    public bool isLerping;
    public string currentSpellId = "Crystallic";

    public void SetColor(
      Color target,
      ParticleSystem[] particleSystems,
      string spellId,
      float time = 1f)
    {
      if (this.isLerping)
        return;
      this.currentColorType = Dye.GetColorType(this.currentSpellId, spellId);
      ((MonoBehaviour) GameManager.local).StartCoroutine(this.SetColorRoutine(particleSystems, target, spellId, time));
    }

    public IEnumerator SetColorRoutine(
      ParticleSystem[] particles,
      Color target,
      string spellId,
      float tts = 1f)
    {
      this.isLerping = true;
      float timeElapsed = 0.0f;
      Color[] originalColors = new Color[particles.Length];
      for (int i = 0; i < particles.Length; ++i)
      {
        ParticleSystem.ColorOverLifetimeModule lt = particles[i].colorOverLifetime;
        ((ParticleSystem.ColorOverLifetimeModule) ref lt).enabled = true;
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particles[i].colorOverLifetime;
        Color[] colorArray = originalColors;
        int index = i;
        ParticleSystem.MinMaxGradient color1 = ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime).color;
        Color color2 = ((ParticleSystem.MinMaxGradient) ref color1).color;
        colorArray[index] = color2;
        lt = new ParticleSystem.ColorOverLifetimeModule();
        colorOverLifetime = new ParticleSystem.ColorOverLifetimeModule();
      }
      while ((double) timeElapsed < (double) tts)
      {
        float lerpFactor = timeElapsed / tts;
        for (int i = 0; i < particles.Length; ++i)
        {
          ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particles[i].colorOverLifetime;
          Color currentColor = originalColors[i];
          this.currentColor = Color.Lerp(currentColor, target, lerpFactor);
          ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime).color = ParticleSystem.MinMaxGradient.op_Implicit(Color.Lerp(currentColor, target, lerpFactor));
          colorOverLifetime = new ParticleSystem.ColorOverLifetimeModule();
          currentColor = new Color();
        }
        timeElapsed += Time.deltaTime;
        yield return (object) null;
      }
      for (int i = 0; i < particles.Length; ++i)
      {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particles[i].colorOverLifetime;
        ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime).color = ParticleSystem.MinMaxGradient.op_Implicit(target);
        colorOverLifetime = new ParticleSystem.ColorOverLifetimeModule();
      }
      this.isLerping = false;
      this.currentSpellId = spellId;
    }
  }
}
