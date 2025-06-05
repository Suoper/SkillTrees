// Decompiled with JetBrains decompiler
// Type: Crystallic.NoiseController
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic
{
  public class NoiseController : MonoBehaviour
  {
    private AnimationCurve _curve;
    private EffectInstance _effectInstance;
    private bool _isRunning;
    private float _lastUpdateTime;
    private NoiseMode _mode;
    private Rigidbody _rigidbody;
    private float _updateInterval;

    private void Update()
    {
      if (!this._isRunning || (double) Time.time - (double) this._lastUpdateTime < (double) this._updateInterval)
        return;
      this._lastUpdateTime = Time.time;
      this.ApplyNoiseBasedOnVelocity();
    }

    public void Initialize(
      Rigidbody rigidbody,
      EffectInstance effectInstance,
      AnimationCurve curve,
      NoiseMode mode,
      float updateInterval)
    {
      this._rigidbody = rigidbody;
      this._effectInstance = effectInstance;
      this._curve = curve;
      this._mode = mode;
      this._updateInterval = updateInterval;
      this._lastUpdateTime = Time.time;
      this._isRunning = true;
    }

    private void ApplyNoiseBasedOnVelocity()
    {
      if ((Object) this._rigidbody == (Object) null || this._effectInstance == null)
        return;
      float num = this._curve.Evaluate(this._rigidbody.velocity.magnitude);
      foreach (ParticleSystem particleSystem in this._effectInstance.GetParticleSystems())
      {
        ParticleSystem.NoiseModule noise = particleSystem.noise;
        if (((ParticleSystem.NoiseModule) ref noise).enabled)
        {
          switch (this._mode)
          {
            case NoiseMode.Strength:
              ((ParticleSystem.NoiseModule) ref noise).strength = ParticleSystem.MinMaxCurve.op_Implicit(num);
              break;
            case NoiseMode.Frequency:
              ((ParticleSystem.NoiseModule) ref noise).frequency = num;
              break;
            case NoiseMode.StrengthAndFrequency:
              ((ParticleSystem.NoiseModule) ref noise).strength = ParticleSystem.MinMaxCurve.op_Implicit(num);
              ((ParticleSystem.NoiseModule) ref noise).frequency = num;
              break;
          }
        }
      }
    }

    public void Stop() => this._isRunning = false;
  }
}
