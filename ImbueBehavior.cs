// Decompiled with JetBrains decompiler
// Type: Crystallic.ImbueBehavior
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using Crystallic.Skill;
using Crystallic.Skill.Spell;
using System;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic
{
  public class ImbueBehavior : ThunderBehaviour
  {
    public bool isActive;
    public Imbue imbue;
    public EffectInstance imbueEffectInstance;
    public float cooldown = 0.01f;
    public SkillCrystalImbueHandler handler;
    public EffectData imbueEffectData;
    public EffectData imbueHitEffectData;
    private float lastTime;
    public Color currentColor = Color.black;

    public virtual void Activate(Imbue imbue, SkillCrystalImbueHandler handler)
    {
      this.isActive = true;
      this.handler = handler;
      this.imbue = imbue;
      // ISSUE: method pointer
      imbue.OnImbueHit += new Imbue.ImbueHitEvent((object) this, __methodptr(Hit));
      // ISSUE: method pointer
      imbue.OnImbueSpellChange += new Imbue.ImbueLevelChangeEvent((object) this, __methodptr(SpellChange));
      this.imbueEffectData = Catalog.GetData<EffectData>(handler.imbueEffectId, true);
      this.imbueHitEffectData = Catalog.GetData<EffectData>(handler.imbueHitEffectId, true);
      this.imbueEffectInstance = this.imbueEffectData?.Spawn(((ThunderBehaviour) imbue).transform.position, ((ThunderBehaviour) imbue).transform.rotation, ((ThunderBehaviour) imbue).transform, (CollisionInstance) null, true, imbue.colliderGroup, false, imbue.EnergyRatio, 1f, (Type[]) null);
      if ((bool) (UnityEngine.Object) imbue.colliderGroup.imbueEffectRenderer)
        this.imbueEffectInstance?.SetRenderer(imbue.colliderGroup.imbueEffectRenderer, false);
      this.imbueEffectInstance?.Play(0, false, false);
      this.imbueEffectInstance.SetColorImmediate(handler.colorModifier);
    }

    private void SpellChange(
      Imbue imbue1,
      SpellCastCharge spellData,
      float amount,
      float change,
      EventTime eventTime)
    {
      if (eventTime != 0)
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this);
    }

    private void Hit(
      SpellCastCharge spellCastCharge,
      float something,
      bool otherthing,
      CollisionInstance hit,
      EventTime eventTime)
    {
      if ((double) Time.time - (double) this.lastTime <= (double) this.cooldown || (double) hit.impactVelocity.magnitude < (double) SpellCastCrystallic.imbueHitVelocity || (double) hit.impactVelocity.magnitude > (double) this.handler.minMaxImpactVelocity.y || hit.targetMaterial.isMetal)
        return;
      this.lastTime = Time.time;
      RagdollPart ragdollPart = hit?.targetColliderGroup?.collisionHandler?.ragdollPart;
      Item entity = hit?.targetColliderGroup?.collisionHandler?.Entity as Item;
      if ((bool) (UnityEngine.Object) ragdollPart && (UnityEngine.Object) ragdollPart.ragdoll.creature != (UnityEngine.Object) this.imbue.imbueCreature && this.handler.crystallise && !ragdollPart.hasMetalArmor)
      {
        BrainModuleCrystal module = ragdollPart.ragdoll.creature.brain.instance.GetModule<BrainModuleCrystal>(true);
        module.Crystallise(this.handler.crystalliseDuration);
        module.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, this.handler.spellId), this.handler.spellId);
      }
      if ((UnityEngine.Object) ragdollPart != (UnityEngine.Object) null)
        this.Hit(hit, spellCastCharge, ragdollPart.ragdoll.creature, entity);
      this.imbueHitEffectData?.Spawn(hit.contactPoint, Quaternion.LookRotation(hit.contactNormal, ((ThunderBehaviour) hit.sourceColliderGroup).transform.up), ((Component) hit.targetCollider).transform, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>())?.Play(0, false, false);
    }

    public void SetColorModifier(Color color) => this.currentColor = color;

    public void ClearColorModifier() => this.currentColor = Color.black;

    public virtual void Hit(
      CollisionInstance collisionInstance,
      SpellCastCharge spellCastCharge,
      Creature hitCreature = null,
      Item hitItem = null)
    {
    }

    public virtual void Deactivate()
    {
      this.isActive = false;
      this.imbueEffectInstance.ForceStop((ParticleSystemStopBehavior) 0);
      this.imbueEffectInstance?.End(false, -1f);
      if (this.imbueEffectInstance == null)
        return;
      // ISSUE: method pointer
      this.imbue.OnImbueHit -= new Imbue.ImbueHitEvent((object) this, __methodptr(Hit));
      // ISSUE: method pointer
      this.imbue.OnImbueSpellChange -= new Imbue.ImbueLevelChangeEvent((object) this, __methodptr(SpellChange));
      this.imbueEffectInstance = (EffectInstance) null;
    }
  }
}
