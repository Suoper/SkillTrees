// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillCrystallicQuasar
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using Crystallic.Skill.Spell;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillCrystallicQuasar : SpellSkillData
  {
    [ModOption("Drain Charge", "Decides whether or not this skill drains spell charge. (Warning, very OP, not recommended for normal CH playthroughs!)")]
    [ModOptionCategory("Crystallic Quasar", 7)]
    public static bool drainCharge = true;
    [ModOption("Charge Drain", "Controls how much charge is drained over the span of a second.")]
    [ModOptionCategory("Crystallic Quasar", 7)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.1f, 100f, 0.05f)]
    public static float chargeDrain = 0.2f;
    [ModOption("Haptic Intensity", "Controls how strong the haptic feedback is for the beam.")]
    [ModOptionCategory("Crystallic Quasar", 7)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.1f, 100f, 0.1f)]
    public static float hapticIntensity = 1f;
    [ModOption("Dismemberment Allowance", "Controls how close the beam hit point has to be to a limb to dismember it.")]
    [ModOptionCategory("Crystallic Quasar", 7)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.1f, 100f, 0.075f)]
    public static float dismembermentDistance = 0.3f;
    [ModOption("Beam Max Distance", "Controls the max distance of the Raycast, this decides how far the beam can shootShardshot.")]
    [ModOptionCategory("Crystallic Quasar", 7)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.1f, 100f, 0.1f)]
    public static float beamMaxDistance = 7.5f;
    public static EffectInstance beamLeftEffectInstance;
    public static EffectInstance beamLeftImpactEffectInstance;
    public static EffectInstance beamRightEffectInstance;
    public static EffectInstance beamRightImpactEffectInstance;
    public static bool leftActive;
    public static bool rightActive;
    private static string status = "";
    public EffectData beamEffectData;
    public string beamEffectId;
    public float beamHandLocomotionVelocityCorrectionMultiplier = 1f;
    public float beamHandPositionDamperMultiplier = 1f;
    public float beamHandPositionSpringMultiplier = 1f;
    public float beamHandRotationDamperMultiplier = 0.6f;
    public float beamHandRotationSpringMultiplier = 0.2f;
    public EffectData beamImpactEffectData;
    public string beamImpactEffectId;
    public LayerMask beamMask;
    public List<string> blacklistMaterialIds;
    public GameObject impactLeftGameObject;
    public GameObject impactRightGameObject;

    public static void SetStatus(string status) => SkillCrystallicQuasar.status = status;

    public event SkillCrystallicQuasar.OnQuasarHit onQuasarHit;

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      this.impactLeftGameObject = new GameObject();
      this.impactRightGameObject = new GameObject();
      SkillHyperintensity.onSpellOvercharge += new SkillHyperintensity.OnSpellOvercharge(this.OnSpellOvercharge);
      SkillHyperintensity.onSpellReleased += new SkillHyperintensity.OnSpellReleased(this.OnSpellReleased);
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      SkillHyperintensity.onSpellOvercharge -= new SkillHyperintensity.OnSpellOvercharge(this.OnSpellOvercharge);
      SkillHyperintensity.onSpellReleased -= new SkillHyperintensity.OnSpellReleased(this.OnSpellReleased);
    }

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      // ISSUE: method pointer
      spellCastCrystallic.OnSpellStopEvent += new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellStopEvent));
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      if (!(spell is SpellCastCrystallic spellCastCrystallic))
        return;
      // ISSUE: method pointer
      spellCastCrystallic.OnSpellStopEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellStopEvent));
    }

    private void OnSpellOvercharge(SpellCastCrystallic spellCastCrystallic)
    {
      if (spellCastCrystallic.spellCaster.side == 1)
      {
        spellCastCrystallic.allowSpray = true;
        spellCastCrystallic.onSprayStart -= new SpellCastCrystallic.SprayEvent(this.OnSprayStart);
        spellCastCrystallic.onSprayLoop -= new SpellCastCrystallic.SprayEvent(this.OnSprayLoop);
        spellCastCrystallic.onSprayEnd -= new SpellCastCrystallic.SprayEvent(this.OnSprayEnd);
        spellCastCrystallic.onSprayStart += new SpellCastCrystallic.SprayEvent(this.OnSprayStart);
        spellCastCrystallic.onSprayLoop += new SpellCastCrystallic.SprayEvent(this.OnSprayLoop);
        spellCastCrystallic.onSprayEnd += new SpellCastCrystallic.SprayEvent(this.OnSprayEnd);
      }
      else
      {
        spellCastCrystallic.allowSpray = true;
        spellCastCrystallic.onSprayStart -= new SpellCastCrystallic.SprayEvent(this.OnSprayStart);
        spellCastCrystallic.onSprayLoop -= new SpellCastCrystallic.SprayEvent(this.OnSprayLoop);
        spellCastCrystallic.onSprayEnd -= new SpellCastCrystallic.SprayEvent(this.OnSprayEnd);
        spellCastCrystallic.onSprayStart += new SpellCastCrystallic.SprayEvent(this.OnSprayStart);
        spellCastCrystallic.onSprayLoop += new SpellCastCrystallic.SprayEvent(this.OnSprayLoop);
        spellCastCrystallic.onSprayEnd += new SpellCastCrystallic.SprayEvent(this.OnSprayEnd);
      }
    }

    private void OnSpellReleased(SpellCastCrystallic spellCastCrystallic)
    {
      if (spellCastCrystallic == null || (Object) spellCastCrystallic.spellCaster == (Object) null)
        return;
      if (spellCastCrystallic.spellCaster.side == 1)
      {
        Player.local.handLeft.controlHand.StopHapticLoop((object) this);
        spellCastCrystallic.spellCaster.ragdollHand.playerHand.link.RemoveJointModifier((object) this);
        spellCastCrystallic.allowSpray = (double) spellCastCrystallic.currentCharge > 0.10000000149011612;
        SkillCrystallicQuasar.beamLeftEffectInstance?.End(false, -1f);
        EffectInstance leftEffectInstance = SkillCrystallicQuasar.beamLeftEffectInstance;
        if (leftEffectInstance != null)
          leftEffectInstance.ForceStop((ParticleSystemStopBehavior) 0);
        SkillCrystallicQuasar.beamLeftEffectInstance = (EffectInstance) null;
      }
      else
      {
        Player.local.handRight.controlHand.StopHapticLoop((object) this);
        spellCastCrystallic.spellCaster?.ragdollHand.playerHand.link.RemoveJointModifier((object) this);
        spellCastCrystallic.allowSpray = (double) spellCastCrystallic.currentCharge > 0.10000000149011612;
        SkillCrystallicQuasar.beamRightEffectInstance?.End(false, -1f);
        EffectInstance rightEffectInstance = SkillCrystallicQuasar.beamRightEffectInstance;
        if (rightEffectInstance != null)
          rightEffectInstance.ForceStop((ParticleSystemStopBehavior) 0);
        SkillCrystallicQuasar.beamRightEffectInstance = (EffectInstance) null;
      }
    }

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.beamEffectData = Catalog.GetData<EffectData>(this.beamEffectId, true);
      this.beamImpactEffectData = Catalog.GetData<EffectData>(this.beamImpactEffectId, true);
    }

    private void OnSpellStopEvent(SpellCastCharge spell)
    {
      if (spell.spellCaster.side == 1)
      {
        Player.local.handLeft.controlHand.StopHapticLoop((object) this);
        SkillCrystallicQuasar.beamLeftEffectInstance?.End(false, -1f);
        EffectInstance leftEffectInstance = SkillCrystallicQuasar.beamLeftEffectInstance;
        if (leftEffectInstance != null)
          leftEffectInstance.ForceStop((ParticleSystemStopBehavior) 0);
        SkillCrystallicQuasar.beamLeftEffectInstance = (EffectInstance) null;
        SkillCrystallicQuasar.leftActive = false;
        SpellCastCrystallic spellCastCrystallic = spell as SpellCastCrystallic;
        spellCastCrystallic.onSprayStart -= new SpellCastCrystallic.SprayEvent(this.OnSprayStart);
        spellCastCrystallic.onSprayLoop -= new SpellCastCrystallic.SprayEvent(this.OnSprayLoop);
        spellCastCrystallic.onSprayEnd -= new SpellCastCrystallic.SprayEvent(this.OnSprayEnd);
      }
      else
      {
        Player.local.handRight.controlHand.StopHapticLoop((object) this);
        SkillCrystallicQuasar.beamRightEffectInstance?.End(false, -1f);
        EffectInstance rightEffectInstance = SkillCrystallicQuasar.beamRightEffectInstance;
        if (rightEffectInstance != null)
          rightEffectInstance.ForceStop((ParticleSystemStopBehavior) 0);
        SkillCrystallicQuasar.beamRightEffectInstance = (EffectInstance) null;
        SkillCrystallicQuasar.rightActive = false;
        SpellCastCrystallic spellCastCrystallic = spell as SpellCastCrystallic;
        spellCastCrystallic.onSprayStart -= new SpellCastCrystallic.SprayEvent(this.OnSprayStart);
        spellCastCrystallic.onSprayLoop -= new SpellCastCrystallic.SprayEvent(this.OnSprayLoop);
        spellCastCrystallic.onSprayEnd -= new SpellCastCrystallic.SprayEvent(this.OnSprayEnd);
      }
    }

    private void OnSprayStart(SpellCastCrystallic spellCastCrystallic)
    {
      if (spellCastCrystallic.spellCaster.side == 1)
      {
        SkillCrystallicQuasar.beamLeftEffectInstance = this.beamEffectData?.Spawn(spellCastCrystallic.spellCaster.magicSource.transform, true, (ColliderGroup) null, false);
        SkillCrystallicQuasar.beamLeftEffectInstance?.Play(0, false, false);
        SkillCrystallicQuasar.beamLeftEffectInstance.SetColorImmediate(spellCastCrystallic.currentColor);
        SkillCrystallicQuasar.leftActive = true;
        Player.local.handLeft.link.SetJointModifier((object) this, this.beamHandPositionSpringMultiplier, this.beamHandPositionDamperMultiplier, this.beamHandRotationSpringMultiplier, this.beamHandRotationDamperMultiplier, this.beamHandLocomotionVelocityCorrectionMultiplier);
      }
      else
      {
        SkillCrystallicQuasar.beamRightEffectInstance = this.beamEffectData?.Spawn(spellCastCrystallic.spellCaster.magicSource.transform, true, (ColliderGroup) null, false);
        SkillCrystallicQuasar.beamRightEffectInstance?.Play(0, false, false);
        SkillCrystallicQuasar.beamRightEffectInstance.SetColorImmediate(spellCastCrystallic.currentColor);
        SkillCrystallicQuasar.rightActive = true;
        Player.local.handRight.link.SetJointModifier((object) this, this.beamHandPositionSpringMultiplier, this.beamHandPositionDamperMultiplier, this.beamHandRotationSpringMultiplier, this.beamHandRotationDamperMultiplier, this.beamHandLocomotionVelocityCorrectionMultiplier);
      }
    }

    private void OnSprayLoop(SpellCastCrystallic spellCastCrystallic)
    {
      if (spellCastCrystallic.spellCaster.side == 1)
      {
        if (SkillCrystallicQuasar.drainCharge)
          spellCastCrystallic.TryDrainCharge(SkillCrystallicQuasar.chargeDrain);
        if ((double) spellCastCrystallic.currentCharge <= 0.10000000149011612)
          return;
        Player.local.handLeft.controlHand.HapticLoop((object) this, SkillCrystallicQuasar.hapticIntensity, 0.01f);
        RaycastHit raycastHit;
        if (Physics.Raycast(spellCastCrystallic.spellCaster.magicSource.transform.position, spellCastCrystallic.spellCaster.magicSource.transform.up, ref raycastHit, SkillCrystallicQuasar.beamMaxDistance))
        {
          Creature componentInParent = ((Component) ((RaycastHit) ref raycastHit).collider)?.GetComponentInParent<Creature>();
          this.impactLeftGameObject.transform.position = ((RaycastHit) ref raycastHit).point;
          if (!(bool) (Object) componentInParent)
          {
            if (SkillCrystallicQuasar.beamLeftImpactEffectInstance == null)
              SkillCrystallicQuasar.beamLeftImpactEffectInstance = this.beamImpactEffectData.Spawn(this.impactLeftGameObject.transform, true, (ColliderGroup) null, false);
            if (SkillCrystallicQuasar.beamLeftImpactEffectInstance != null && !SkillCrystallicQuasar.beamLeftImpactEffectInstance.isPlaying)
            {
              SkillCrystallicQuasar.beamLeftImpactEffectInstance?.Play(0, false, false);
              SkillCrystallicQuasar.beamLeftImpactEffectInstance.SetColorImmediate(spellCastCrystallic.currentColor);
            }
          }
          if (!((Object) ((RaycastHit) ref raycastHit).collider != (Object) null) || !(bool) (Object) componentInParent || componentInParent.isPlayer)
            return;
          RagdollPart ragdollPart = (RagdollPart) null;
          BrainModuleCrystal module = componentInParent?.brain?.instance?.GetModule<BrainModuleCrystal>(true);
          module.Crystallise(5f);
          module.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, spellCastCrystallic.spellId), spellCastCrystallic.spellId);
          module.isCrystallised = true;
          if (!string.IsNullOrEmpty(SkillCrystallicQuasar.status))
            ((ThunderEntity) componentInParent).Inflict(SkillCrystallicQuasar.status, (object) this, 5f, (object) (float) (30.0 * (double) Time.deltaTime), true);
          if ((bool) (Object) componentInParent && componentInParent.ragdoll.GetClosestPart(((RaycastHit) ref raycastHit).point, SkillCrystallicQuasar.dismembermentDistance, out ragdollPart) && (bool) (Object) ragdollPart && ragdollPart.sliceAllowed && (Object) ragdollPart != (Object) componentInParent.ragdoll.rootPart && !ragdollPart.hasMetalArmor)
          {
            ragdollPart?.TrySlice();
            componentInParent?.Kill();
          }
          SkillCrystallicQuasar.OnQuasarHit onQuasarHit = this.onQuasarHit;
          if (onQuasarHit != null)
            onQuasarHit(new SkillCrystallicQuasar.QuasarImpact(((RaycastHit) ref raycastHit).collider, ((RaycastHit) ref raycastHit).point, ((RaycastHit) ref raycastHit).normal, ragdollPart, (ThunderEntity) componentInParent), spellCastCrystallic);
        }
        else
        {
          SkillCrystallicQuasar.beamLeftImpactEffectInstance?.End(false, -1f);
          SkillCrystallicQuasar.beamLeftImpactEffectInstance = (EffectInstance) null;
        }
      }
      else
      {
        if (SkillCrystallicQuasar.drainCharge)
          spellCastCrystallic.TryDrainCharge(SkillCrystallicQuasar.chargeDrain);
        if ((double) spellCastCrystallic.currentCharge <= 0.10000000149011612)
          return;
        Player.local.handRight.controlHand.HapticLoop((object) this, SkillCrystallicQuasar.hapticIntensity, 0.01f);
        RaycastHit raycastHit;
        if (Physics.Raycast(spellCastCrystallic.spellCaster.magicSource.transform.position, spellCastCrystallic.spellCaster.magicSource.transform.up, ref raycastHit, SkillCrystallicQuasar.beamMaxDistance))
        {
          Creature componentInParent = ((Component) ((RaycastHit) ref raycastHit).collider)?.GetComponentInParent<Creature>();
          this.impactRightGameObject.transform.position = ((RaycastHit) ref raycastHit).point;
          if (!(bool) (Object) componentInParent)
          {
            if (SkillCrystallicQuasar.beamRightImpactEffectInstance == null)
              SkillCrystallicQuasar.beamRightImpactEffectInstance = this.beamImpactEffectData.Spawn(this.impactRightGameObject.transform, true, (ColliderGroup) null, false);
            if (SkillCrystallicQuasar.beamRightImpactEffectInstance != null && !SkillCrystallicQuasar.beamRightImpactEffectInstance.isPlaying)
            {
              SkillCrystallicQuasar.beamRightImpactEffectInstance?.Play(0, false, false);
              SkillCrystallicQuasar.beamRightImpactEffectInstance.SetColorImmediate(spellCastCrystallic.currentColor);
            }
          }
          if ((Object) ((RaycastHit) ref raycastHit).collider != (Object) null && (bool) (Object) componentInParent && !componentInParent.isPlayer)
          {
            RagdollPart ragdollPart = (RagdollPart) null;
            BrainModuleCrystal module = componentInParent?.brain?.instance?.GetModule<BrainModuleCrystal>(true);
            module.Crystallise(5f);
            module.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, spellCastCrystallic.spellId), spellCastCrystallic.spellId);
            module.isCrystallised = true;
            if ((bool) (Object) componentInParent && componentInParent.ragdoll.GetClosestPart(((RaycastHit) ref raycastHit).point, SkillCrystallicQuasar.dismembermentDistance, out ragdollPart) && (bool) (Object) ragdollPart && ragdollPart.sliceAllowed && (Object) ragdollPart != (Object) componentInParent.ragdoll.rootPart && !ragdollPart.hasMetalArmor)
            {
              ragdollPart?.TrySlice();
              componentInParent?.Kill();
            }
            SkillCrystallicQuasar.OnQuasarHit onQuasarHit = this.onQuasarHit;
            if (onQuasarHit != null)
              onQuasarHit(new SkillCrystallicQuasar.QuasarImpact(((RaycastHit) ref raycastHit).collider, ((RaycastHit) ref raycastHit).point, ((RaycastHit) ref raycastHit).normal, ragdollPart, (ThunderEntity) componentInParent), spellCastCrystallic);
          }
        }
        else
        {
          SkillCrystallicQuasar.beamRightImpactEffectInstance?.End(false, -1f);
          SkillCrystallicQuasar.beamRightImpactEffectInstance = (EffectInstance) null;
        }
      }
    }

    private void OnSprayEnd(SpellCastCrystallic spellCastCrystallic)
    {
      if (spellCastCrystallic.spellCaster.side == 1)
      {
        Player.local.handLeft.controlHand.StopHapticLoop((object) this);
        SkillCrystallicQuasar.beamLeftEffectInstance?.End(false, -1f);
        SkillCrystallicQuasar.beamLeftEffectInstance.ForceStop((ParticleSystemStopBehavior) 0);
        SkillCrystallicQuasar.leftActive = false;
        if (SkillCrystallicQuasar.beamLeftImpactEffectInstance == null || !SkillCrystallicQuasar.beamLeftImpactEffectInstance.isPlaying)
          return;
        SkillCrystallicQuasar.beamLeftImpactEffectInstance.Stop(0);
      }
      else
      {
        Player.local.handRight.controlHand.StopHapticLoop((object) this);
        SkillCrystallicQuasar.beamRightEffectInstance?.End(false, -1f);
        SkillCrystallicQuasar.beamRightEffectInstance.ForceStop((ParticleSystemStopBehavior) 0);
        SkillCrystallicQuasar.rightActive = false;
        if (SkillCrystallicQuasar.beamRightImpactEffectInstance != null && SkillCrystallicQuasar.beamRightImpactEffectInstance.isPlaying)
          SkillCrystallicQuasar.beamRightImpactEffectInstance.Stop(0);
      }
    }

    public delegate void OnQuasarHit(
      SkillCrystallicQuasar.QuasarImpact impact,
      SpellCastCrystallic spellCastCrystallic);

    public class QuasarImpact
    {
      public Collider hitCollider;
      public ThunderEntity hitEntity;
      public Vector3 hitNormal;
      public Vector3 hitPoint;
      public RagdollPart hitRagdollPart;

      public QuasarImpact(
        Collider hitCollider,
        Vector3 hitPoint,
        Vector3 hitNormal,
        RagdollPart hitRagdollPart,
        ThunderEntity hitEntity)
      {
        this.hitCollider = hitCollider;
        this.hitPoint = hitPoint;
        this.hitNormal = hitNormal;
        this.hitRagdollPart = hitRagdollPart;
        this.hitEntity = hitEntity;
      }
    }
  }
}
