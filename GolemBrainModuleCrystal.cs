// Decompiled with JetBrains decompiler
// Type: Crystallic.AI.GolemBrainModuleCrystal
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic.AI
{
  public class GolemBrainModuleCrystal : GolemBrainModule
  {
    public bool isCrystallised;
    protected List<EffectInstance> instances = new List<EffectInstance>();
    public Lerper lerper;
    public EffectData lowerArmLeftGolemData;
    public EffectData lowerArmRightGolemData;
    public EffectData lowerLegLeftGolemData;
    public EffectData lowerLegRightGolemData;
    public EffectData torsoGolemData;
    public EffectData upperArmLeftGolemData;
    public EffectData upperArmRightGolemData;
    public EffectData upperLegLeftGolemData;
    public EffectData upperLegRightGolemData;

    public override void Load(Golem golem)
    {
      base.Load(golem);
      this.lerper = new Lerper();
      this.lowerArmLeftGolemData = Catalog.GetData<EffectData>("LowerArmLeftGolem", true);
      this.lowerArmRightGolemData = Catalog.GetData<EffectData>("LowerArmRightGolem", true);
      this.upperArmLeftGolemData = Catalog.GetData<EffectData>("UpperArmLeftGolem", true);
      this.upperArmRightGolemData = Catalog.GetData<EffectData>("UpperArmRightGolem", true);
      this.torsoGolemData = Catalog.GetData<EffectData>("TorsoGolem", true);
      this.lowerLegLeftGolemData = Catalog.GetData<EffectData>("LowerLegLeftGolem", true);
      this.lowerLegRightGolemData = Catalog.GetData<EffectData>("LowerLegRightGolem", true);
      this.upperLegLeftGolemData = Catalog.GetData<EffectData>("UpperLegLeftGolem", true);
      this.upperLegRightGolemData = Catalog.GetData<EffectData>("UpperLegRightGolem", true);
    }

    public static IEnumerator AdjustAnimatorSpeed(bool active, Animator animator, int steps)
    {
      float stepValue = 1f / (float) steps;
      float currentSpeed = animator.speed;
      for (int i = 0; i < steps; ++i)
      {
        if (active)
          currentSpeed += stepValue;
        else
          currentSpeed -= stepValue;
        currentSpeed = Mathf.Clamp(currentSpeed, 0.0f, 1f);
        animator.speed = currentSpeed;
        yield return (object) new WaitForSeconds(0.1f);
      }
    }

    public override void Unload(Golem golem)
    {
      base.Unload(golem);
      ((GolemController) Golem.local).speed = FloatHandler.op_Implicit(1f);
      Golem.local.allowMelee = true;
      this.SetColor(Dye.GetEvaluatedColor(this.lerper.currentSpellId, "Crystallic"), "Crystallic");
      ((MonoBehaviour) this).StartCoroutine(GolemBrainModuleCrystal.AdjustAnimatorSpeed(true, ((GolemController) Golem.local).animator, 20));
      this.isCrystallised = false;
    }

    public void SetColor(Color target, string spellId, float time = 1f)
    {
      ParticleSystem[] particleSystems = this.instances.GetParticleSystems();
      this.lerper.SetColor(target, particleSystems, spellId, time);
    }

    public void Crystallise(float duration, bool crystallise = true)
    {
      if (this.isCrystallised)
        return;
      ((MonoBehaviour) this).StartCoroutine(this.CrystalliseRoutine(duration, crystallise));
    }

    private IEnumerator CrystalliseRoutine(float duration, bool crystallise = true)
    {
      this.isCrystallised = true;
      this.instances.AddRange((IEnumerable<EffectInstance>) ((GolemController) Golem.local).Brain().PlayFullBodyEffect(5f, this.upperArmLeftGolemData, this.upperArmRightGolemData, this.lowerArmLeftGolemData, this.lowerArmRightGolemData, this.torsoGolemData, this.upperLegLeftGolemData, this.upperLegRightGolemData, this.lowerLegLeftGolemData, this.lowerLegRightGolemData));
      if (crystallise)
        ((MonoBehaviour) this).StartCoroutine(GolemBrainModuleCrystal.AdjustAnimatorSpeed(false, ((GolemController) Golem.local).animator, 20));
      yield return (object) new WaitForSeconds(0.25f);
      if (crystallise)
      {
        ((GolemController) Golem.local).speed = FloatHandler.op_Implicit(0.0f);
        Golem.local.allowMelee = false;
      }
      yield return (object) new WaitForSeconds(duration);
      ((GolemController) Golem.local).speed = FloatHandler.op_Implicit(1f);
      Golem.local.allowMelee = true;
      this.SetColor(Dye.GetEvaluatedColor(this.lerper.currentSpellId, "Crystallic"), "Crystallic");
      if (crystallise)
        ((MonoBehaviour) this).StartCoroutine(GolemBrainModuleCrystal.AdjustAnimatorSpeed(true, ((GolemController) Golem.local).animator, 20));
      this.isCrystallised = false;
    }
  }
}
