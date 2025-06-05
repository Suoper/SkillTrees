// Decompiled with JetBrains decompiler
// Type: Arcana.Misc.ParticleLifetimeScaler
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Arcana.Misc
{
  public class ParticleLifetimeScaler : MonoBehaviour
  {
    public float baseVelocity = 1.4f;
    public float baseLifeTime = 2f;
    public float baseScale = 1f;
    public float targetLifetime;
    public bool directControl = false;
    public bool apply = false;
    private float velocityMagnitude;
    private Vector3 velocity;
    private Vector3 lastPosition;
    private List<ParticleSystem> particleSystems;

    private void Start()
    {
      this.particleSystems = new List<ParticleSystem>()
      {
        this.GetComponent<ParticleSystem>()
      };
      foreach (ParticleSystem componentsInChild in this.GetComponentsInChildren<ParticleSystem>())
        this.particleSystems.Add(componentsInChild);
      this.lastPosition = this.transform.position;
    }

    private void Update()
    {
      this.velocity = (this.lastPosition - this.transform.position) / Time.deltaTime;
      this.lastPosition = this.transform.position;
      this.velocityMagnitude = this.velocity.magnitude;
      foreach (ParticleSystem particleSystem in this.particleSystems)
      {
        if (!((Object) particleSystem == (Object) null))
        {
          ParticleSystem.TrailModule trails = particleSystem.trails;
          if (((ParticleSystem.TrailModule) ref trails).enabled)
          {
            ParticleSystem.MainModule main = particleSystem.main;
            if (!this.directControl)
              this.targetLifetime = Mathf.Clamp((float) ((double) this.baseLifeTime * ((double) this.baseVelocity / (double) this.velocityMagnitude) / 10.0), 0.0f, 4f) * this.gameObject.transform.localScale.x;
            if (this.apply)
            {
              ((ParticleSystem.MainModule) ref main).startLifetime = ParticleSystem.MinMaxCurve.op_Implicit(this.targetLifetime);
              ParticleSystem.Particle[] particleArray = new ParticleSystem.Particle[particleSystem.particleCount];
              int particles = particleSystem.GetParticles(particleArray);
              for (int index = 0; index < particles; ++index)
                ((ParticleSystem.Particle) ref particleArray[index]).startLifetime = this.targetLifetime;
              particleSystem.SetParticles(particleArray, particles);
            }
          }
        }
      }
    }
  }
}
