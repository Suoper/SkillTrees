// Decompiled with JetBrains decompiler
// Type: Arcana.Misc.ChromaticParticles
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using System;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Misc
{
  internal static class ChromaticParticles
  {
    public static void MixColorInEffectGradient(
      this EffectInstance effectInstance,
      Color? color,
      bool reset = false,
      Gradient defaultPrimaryGradient = null,
      Gradient defaultSecondaryGradient = null,
      GradientAlphaKey[] gradientAlphaKeysOverride = null,
      List<string> particleTransformsDefaultToSecondary = null)
    {
      foreach (EffectParticle effectParticle in effectInstance.effects.FindAll((Predicate<Effect>) (effect => effect is EffectParticle)))
      {
        foreach (EffectParticleChild child in effectParticle.childs)
          UpdateParticleSystemColorOverLifetimeGradients(child.particleSystem, color);
        UpdateParticleSystemColorOverLifetimeGradients(effectParticle.rootParticleSystem, color);
      }

      void UpdateParticleSystemColorOverLifetimeGradients(
        ParticleSystem particleSystem,
        Color? newGradientColor)
      {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime1 = particleSystem.colorOverLifetime;
        if (!((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime1).enabled)
          return;
        Gradient gradient1 = new Gradient();
        Gradient gradient2 = new Gradient();
        if (gradientAlphaKeysOverride != null)
        {
          defaultPrimaryGradient.alphaKeys = gradientAlphaKeysOverride;
          defaultSecondaryGradient.alphaKeys = gradientAlphaKeysOverride;
        }
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime2 = particleSystem.colorOverLifetime;
        ParticleSystem.MinMaxGradient color1 = ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime2).color;
        if (((ParticleSystem.MinMaxGradient) ref color1).mode == 3)
        {
          if (!reset)
          {
            Gradient gradient3 = gradient1;
            GradientColorKey[] colorKeys1 = new GradientColorKey[2];
            ParticleSystem.MinMaxGradient color2 = ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime2).color;
            colorKeys1[0] = ((ParticleSystem.MinMaxGradient) ref color2).gradientMin.colorKeys[0];
            colorKeys1[1] = new GradientColorKey(newGradientColor.Value, 0.803f);
            GradientAlphaKey[] alphaKeys1 = gradientAlphaKeysOverride;
            ParticleSystem.MinMaxGradient color3;
            if (alphaKeys1 == null)
            {
              color3 = ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime2).color;
              alphaKeys1 = ((ParticleSystem.MinMaxGradient) ref color3).gradientMin.alphaKeys;
            }
            gradient3.SetKeys(colorKeys1, alphaKeys1);
            Gradient gradient4 = gradient2;
            GradientColorKey[] colorKeys2 = new GradientColorKey[2];
            colorKeys2[0] = new GradientColorKey(newGradientColor.Value, 0.197f);
            color3 = ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime2).color;
            colorKeys2[1] = ((ParticleSystem.MinMaxGradient) ref color3).gradientMax.colorKeys[0];
            GradientAlphaKey[] alphaKeys2 = gradientAlphaKeysOverride;
            if (alphaKeys2 == null)
            {
              color3 = ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime2).color;
              alphaKeys2 = ((ParticleSystem.MinMaxGradient) ref color3).gradientMax.alphaKeys;
            }
            gradient4.SetKeys(colorKeys2, alphaKeys2);
            ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime2).color = new ParticleSystem.MinMaxGradient(gradient1, gradient2);
          }
          else
            ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime2).color = new ParticleSystem.MinMaxGradient(particleTransformsDefaultToSecondary.Contains(((UnityEngine.Object) particleSystem).name) ? defaultSecondaryGradient : defaultPrimaryGradient, particleTransformsDefaultToSecondary.Contains(((UnityEngine.Object) particleSystem).name) ? defaultPrimaryGradient : defaultSecondaryGradient);
        }
        else
        {
          ParticleSystem.MinMaxGradient color4 = ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime2).color;
          if (((ParticleSystem.MinMaxGradient) ref color4).mode != 1)
            return;
          if (!reset)
          {
            Gradient gradient5 = gradient1;
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            color4 = ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime2).color;
            colorKeys[0] = ((ParticleSystem.MinMaxGradient) ref color4).gradient.colorKeys[0];
            colorKeys[1] = new GradientColorKey(newGradientColor.Value, 0.803f);
            GradientAlphaKey[] alphaKeys = gradientAlphaKeysOverride;
            if (alphaKeys == null)
            {
              color4 = ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime2).color;
              alphaKeys = ((ParticleSystem.MinMaxGradient) ref color4).gradient.alphaKeys;
            }
            gradient5.SetKeys(colorKeys, alphaKeys);
            ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime2).color = ParticleSystem.MinMaxGradient.op_Implicit(gradient1);
          }
          else
            ((ParticleSystem.ColorOverLifetimeModule) ref colorOverLifetime2).color = ParticleSystem.MinMaxGradient.op_Implicit(particleTransformsDefaultToSecondary.Contains(((UnityEngine.Object) particleSystem).name) ? defaultSecondaryGradient : defaultPrimaryGradient);
        }
      }
    }

    public static bool TryGetChromaticData(
      SpellStatus spellStatus,
      SpellCastCharge spell,
      Type mainSpellType,
      out SpellCastData mainData,
      out SpellCastData chromaticData,
      out SpellCaster caster)
    {
      mainData = (SpellCastData) null;
      chromaticData = (SpellCastData) null;
      caster = (SpellCaster) null;
      SpellCastData spellInstance = spell.spellCaster.other.spellInstance;
      if (spellInstance == null)
        return false;
      SpellStatus spellStatus1 = spellStatus.spellId == ((CatalogData) spell).id ? spellStatus : (SpellStatus) null;
      SpellStatus spellStatus2 = spellStatus.spellId == ((CatalogData) spellInstance).id ? spellStatus : (SpellStatus) null;
      if (spell.GetType() == mainSpellType && spellStatus2 != null)
      {
        caster = spell.spellCaster.other;
        mainData = (SpellCastData) spell;
        chromaticData = spellInstance;
      }
      else
      {
        if (!(spellInstance.GetType() == mainSpellType) || spellStatus1 == null)
          return false;
        caster = spell.spellCaster;
        mainData = spellInstance;
        chromaticData = (SpellCastData) spell;
      }
      return true;
    }

    public static bool TryGetChromaticDataFromList(
      List<SpellStatus> spellStatuses,
      SpellCastCharge spell,
      Type mainSpellType,
      out SpellCastData mainData,
      out SpellCastData chromaticData,
      out SpellCaster caster)
    {
      mainData = (SpellCastData) null;
      chromaticData = (SpellCastData) null;
      caster = (SpellCaster) null;
      SpellCastData other = spell.spellCaster.other.spellInstance;
      if (other == null)
        return false;
      SpellStatus spellStatus1 = spellStatuses.Find((Predicate<SpellStatus>) (status => status.spellId == ((CatalogData) spell).id));
      SpellStatus spellStatus2 = spellStatuses.Find((Predicate<SpellStatus>) (status => status.spellId == ((CatalogData) other).id));
      if (spell.GetType() == mainSpellType && spellStatus2 != null)
      {
        caster = spell.spellCaster.other;
        mainData = (SpellCastData) spell;
        chromaticData = other;
      }
      else
      {
        if (!(other.GetType() == mainSpellType) || spellStatus1 == null)
          return false;
        caster = spell.spellCaster;
        mainData = other;
        chromaticData = (SpellCastData) spell;
      }
      return true;
    }
  }
}
