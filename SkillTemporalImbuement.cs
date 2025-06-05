// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillTemporalImbuement
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ThunderRoad;
using ThunderRoad.Skill.SpellPower;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillTemporalImbuement : SkillSlowTimeData
  {
    public Dictionary<string, Gradient> defaults = new Dictionary<string, Gradient>();
    public List<SkillSpellPair> skillSpellPairs = new List<SkillSpellPair>();
    public Color startColor;
    public List<Imbue> imbuesActive = new List<Imbue>();

    public virtual void OnImbueLoad(SpellData spell, Imbue imbue)
    {
      base.OnImbueLoad(spell, imbue);
      SpellCastCharge spellCastBase = imbue.spellCastBase;
      if (!this.defaults.ContainsKey(((CatalogData) spellCastBase).id))
      {
        Effect effect = spellCastBase.imbueEffect.effects.First<Effect>((Func<Effect, bool>) (e => e is EffectShader));
        Gradient gradient = effect?.GetType()?.GetField("currentMainGradient", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue((object) effect) as Gradient;
        this.defaults.Add(((CatalogData) spellCastBase).id, gradient);
        if (Settings.debug && gradient != null)
          Debug.Log((object) ("Saving default gradient for spell " + ((CatalogData) spellCastBase).id + ". gradient is: \n Color keys: \n " + string.Join(" \n - ", (IEnumerable<string>) ((IEnumerable<GradientColorKey>) gradient.colorKeys).Select<GradientColorKey, string>((Func<GradientColorKey, string>) (key => key.color.ToString())).ToList<string>()) + " \n Alpha keys: \n " + string.Join<GradientAlphaKey>(" \n - ", (IEnumerable<GradientAlphaKey>) ((IEnumerable<GradientAlphaKey>) gradient.alphaKeys).ToList<GradientAlphaKey>())));
      }
      if (this.imbuesActive.Contains(imbue))
        return;
      bool flag = false;
      for (int index = 0; index < this.skillSpellPairs.Count; ++index)
      {
        if (this.skillSpellPairs[index].IsValid(imbue.imbueCreature, spell))
          flag = true;
      }
      if (flag)
        this.imbuesActive.Add(imbue);
    }

    public virtual void OnImbueUnload(SpellData spell, Imbue imbue)
    {
      base.OnImbueUnload(spell, imbue);
      if (!this.imbuesActive.Contains(imbue))
        return;
      this.imbuesActive.Remove(imbue);
    }

    public virtual void OnImbueHit(
      SpellCastCharge spellData,
      float amount,
      bool fired,
      CollisionInstance hit,
      EventTime eventTime)
    {
      base.OnImbueHit(spellData, amount, fired, hit, eventTime);
      if (eventTime != null || !SkillSlowTimeData.timeSlowed)
        return;
      Item obj = hit?.sourceColliderGroup?.collisionHandler?.item;
      for (int index = 0; index < obj.imbues.Count; ++index)
      {
        Imbue imbue = obj.imbues[index];
        if (imbue.spellCastBase == spellData && this.imbuesActive.Contains(imbue))
        {
          RagdollPart ragdollPart = hit?.targetColliderGroup?.collisionHandler?.ragdollPart;
          if ((bool) (UnityEngine.Object) ragdollPart && !ragdollPart.hasMetalArmor)
          {
            ((ThunderEntity) ragdollPart.ragdoll.creature).Inflict("Slowed", (object) this, 5f, (object) null, true);
            ragdollPart.ragdoll.creature.brain.instance.GetModule<BrainModuleCrystal>(true).SetColor(Dye.GetEvaluatedColor("Mind", ((CatalogData) imbue.spellCastBase).id), ((CatalogData) imbue.spellCastBase).id);
          }
        }
      }
    }

    public override void OnSlowMotionEnter(SpellPowerSlowTime spellPowerSlowTime, float scale)
    {
      base.OnSlowMotionEnter(spellPowerSlowTime, scale);
      Imbue imbue = (Imbue) null;
      try
      {
        for (int index = 0; index < this.imbuesActive.Count; ++index)
        {
          imbue = this.imbuesActive[index];
          Color evaluatedColor = Dye.GetEvaluatedColor("Mind", ((CatalogData) this.imbuesActive[index].spellCastBase).id);
          ImbueBehavior component = ((ThunderBehaviour) this.imbuesActive[index]).gameObject.GetComponent<ImbueBehavior>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
            component.imbueEffectInstance.SetColorImmediate(evaluatedColor);
          if (Settings.debug)
            Debug.Log((object) string.Format("Dying imbue: {0} to endColor: {1}", (object) ((CatalogData) this.imbuesActive[index].spellCastBase).id, (object) evaluatedColor));
          this.SetShaderColor(this.imbuesActive[index].spellCastBase, Utils.CreateGradient(this.startColor, evaluatedColor), 0.5f);
        }
      }
      catch (Exception ex)
      {
        Debug.LogError((object) string.Format("Exception caught while attempting to modify shader gradient of: {0}! Exception: {1}", (object) ((CatalogData) imbue?.spellCastBase)?.id, (object) ex));
      }
    }

    public override void OnSlowMotionExit(SpellPowerSlowTime spellPowerSlowTime)
    {
      base.OnSlowMotionExit(spellPowerSlowTime);
      Imbue imbue = (Imbue) null;
      try
      {
        for (int index = 0; index < this.imbuesActive.Count; ++index)
        {
          imbue = this.imbuesActive[index];
          ImbueBehavior component = ((ThunderBehaviour) this.imbuesActive[index]).gameObject.GetComponent<ImbueBehavior>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
            component.imbueEffectInstance.SetColorImmediate(component.handler.colorModifier);
          Gradient gradient = this.defaults[((CatalogData) this.imbuesActive[index].spellCastBase).id];
          if (Settings.debug)
            Debug.Log((object) ("Clearing effectShader gradient for imbue: " + ((CatalogData) this.imbuesActive[index].spellCastBase).id + ". Default gradient is: \n Color keys: \n " + string.Join(" \n - ", (IEnumerable<string>) ((IEnumerable<GradientColorKey>) gradient.colorKeys).Select<GradientColorKey, string>((Func<GradientColorKey, string>) (key => key.color.ToString())).ToList<string>()) + " \n Alpha keys: \n " + string.Join<GradientAlphaKey>(" \n - ", (IEnumerable<GradientAlphaKey>) ((IEnumerable<GradientAlphaKey>) gradient.alphaKeys).ToList<GradientAlphaKey>())));
          this.SetShaderColor(this.imbuesActive[index].spellCastBase, gradient, 0.5f);
        }
      }
      catch (Exception ex)
      {
        Debug.LogError((object) string.Format("Exception caught while attempting to modify shader gradient of: {0}! Exception: {1}", (object) ((CatalogData) imbue?.spellCastBase)?.id, (object) ex));
      }
    }

    public void SetShaderColor(SpellCastCharge spellCastCharge, Gradient gradient, float time)
    {
      ((MonoBehaviour) spellCastCharge.spellCaster).StartCoroutine(this.LerpShaderColor(spellCastCharge, gradient, time));
    }

    public IEnumerator LerpShaderColor(
      SpellCastCharge spellCastCharge,
      Gradient gradient,
      float time)
    {
      if (spellCastCharge != null && !((UnityEngine.Object) spellCastCharge.imbue == (UnityEngine.Object) null) && (double) spellCastCharge.imbue.energy != 0.0)
      {
        Effect effectShader = spellCastCharge.imbueEffect.effects.First<Effect>((Func<Effect, bool>) (e => e is EffectShader));
        Gradient mainGradient = effectShader?.GetType()?.GetField("currentMainGradient", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue((object) effectShader) as Gradient;
        int keyCount = Mathf.Min(mainGradient.colorKeys.Length, gradient.colorKeys.Length);
        float timeElapsed = 0.0f;
        while ((double) timeElapsed < (double) time)
        {
          timeElapsed += Time.deltaTime;
          GradientColorKey[] blendedColorKeys = new GradientColorKey[keyCount];
          for (int i = 0; i < keyCount; ++i)
          {
            Color startColor = mainGradient.colorKeys[i].color;
            Color endColor = gradient.colorKeys[i].color;
            blendedColorKeys[i] = new GradientColorKey(Color.Lerp(startColor, endColor, spellCastCharge.imbue.energy / 100f), Mathf.Lerp(mainGradient.colorKeys[i].time, gradient.colorKeys[i].time, spellCastCharge.imbue.energy / 100f));
            startColor = new Color();
            endColor = new Color();
          }
          Gradient blendedGradient = new Gradient();
          blendedGradient.colorKeys = blendedColorKeys;
          blendedGradient.alphaKeys = mainGradient.alphaKeys;
          effectShader.SetMainGradient(blendedGradient);
          spellCastCharge.imbueEffect.SetIntensity(spellCastCharge.imbue.energy);
          spellCastCharge.imbueEffect.SetColorImmediate(Color.Lerp(mainGradient.Evaluate(spellCastCharge.imbue.energy / 100f), gradient.Evaluate(spellCastCharge.imbue.energy / 100f), spellCastCharge.imbue.energy / 100f));
          spellCastCharge.imbue.Transfer(spellCastCharge, spellCastCharge.imbue.energy, (Creature) null);
          yield return (object) null;
          blendedColorKeys = (GradientColorKey[]) null;
          blendedGradient = (Gradient) null;
        }
        effectShader.SetMainGradient(gradient);
      }
    }
  }
}
