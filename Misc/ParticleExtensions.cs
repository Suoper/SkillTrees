// Decompiled with JetBrains decompiler
// Type: Arcana.Misc.ParticleExtensions
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using System;
using System.Linq;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Misc
{
  internal static class ParticleExtensions
  {
    public static void ForceStop(
      this EffectInstance effectInstance,
      ParticleSystemStopBehavior stopBehavior = 0)
    {
      foreach (ParticleSystem particleSystem in effectInstance.effects.OfType<EffectParticle>().Select<EffectParticle, ParticleSystem>((Func<EffectParticle, ParticleSystem>) (effect => effect.rootParticleSystem)))
        particleSystem.Stop(true, stopBehavior);
    }
  }
}
