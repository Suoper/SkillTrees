// Decompiled with JetBrains decompiler
// Type: Arcana.Statuses.ArcaneStatus
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Misc;
using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Statuses
{
  internal class ArcaneStatus : Status
  {
    protected StatusDataArcane data;
    public EffectInstance accumulateEffect;
    public EffectInstance fullyChargedEffect;
    public FloatHandler instabilityGainMult;
    public FloatHandler instabilityLossMult;
    public static bool allowExplosionStatusApply;
    public static bool allowCollapse;
    protected bool isCharged = false;
    protected float chargedTime;

    public static FloatHandler GetInstabilityGainHandler(ThunderEntity entity)
    {
      FloatHandler floatHandler;
      return !entity.TryGetVariable<FloatHandler>("InstabilityGainMulti", ref floatHandler) ? entity.SetVariable<FloatHandler>("InstabilityGainMulti", new FloatHandler()) : floatHandler;
    }

    public static FloatHandler GetInstabilityLossHandler(ThunderEntity entity)
    {
      FloatHandler floatHandler;
      return !entity.TryGetVariable<FloatHandler>("InstabilityLossMulti", ref floatHandler) ? entity.SetVariable<FloatHandler>("InstabilityLossMulti", new FloatHandler()) : floatHandler;
    }

    public float Instability
    {
      get => this.value is float num ? num : 0.0f;
      set
      {
        double num = (double) this.entity.SetVariable<float>(nameof (Instability), Mathf.Clamp(value, 0.0f, this.MaxInstability));
        this.OnValueChange();
      }
    }

    public float MaxInstability => this.data.maxInstability;

    public float Intensity => this.Instability / this.MaxInstability;

    public bool ShouldCharge => (double) this.Instability >= (double) this.MaxInstability;

    public bool CanExplode
    {
      get => this.isCharged && (double) Time.time - (double) this.chargedTime >= 1.0;
    }

    protected virtual object GetValue() => (object) this.entity.GetVariable<float>("Instability");

    public virtual void Spawn(StatusData data, ThunderEntity entity)
    {
      base.Spawn(data, entity);
      this.data = data as StatusDataArcane;
      this.instabilityGainMult = ArcaneStatus.GetInstabilityGainHandler(entity);
      this.instabilityLossMult = ArcaneStatus.GetInstabilityLossHandler(entity);
    }

    public virtual bool AddHandler(
      object handler,
      float duration = float.PositiveInfinity,
      object parameter = null,
      bool playEffect = true)
    {
      if (!(parameter is float instability))
        return false;
      this.AddInstability(instability);
      duration = float.PositiveInfinity;
      return base.AddHandler(handler, duration, parameter, playEffect);
    }

    public virtual void FirstApply()
    {
      base.FirstApply();
      if (this.entity is Creature entity && this.accumulateEffect == null)
      {
        this.data.SpawnEffect((ThunderEntity) entity, this.data.accumulateEffectData, out this.accumulateEffect);
        this.accumulateEffect?.SetIntensity(this.Intensity);
        this.accumulateEffect?.Play(0, false, false);
        // ISSUE: method pointer
        entity.ragdoll.OnStateChange -= new Ragdoll.StateChange((object) this, __methodptr(OnStateChange));
        // ISSUE: method pointer
        entity.ragdoll.OnStateChange += new Ragdoll.StateChange((object) this, __methodptr(OnStateChange));
      }
      this.effectInstance?.SetIntensity(this.Intensity);
    }

    private void OnStateChange(
      Ragdoll.State previousState,
      Ragdoll.State newState,
      Ragdoll.PhysicStateChange physicsChange,
      EventTime time)
    {
      try
      {
        if (!(this.entity is Creature entity) || physicsChange == null || newState == 6)
          return;
        bool isStart = time == 0;
        if (this.accumulateEffect != null)
          this.ReparentEffect(this.accumulateEffect, entity, isStart);
        if (this.fullyChargedEffect == null)
          return;
        this.ReparentEffect(this.fullyChargedEffect, entity, isStart);
      }
      catch (NullReferenceException ex)
      {
        Debug.LogWarning((object) "Warning: Status Effect threw exception during creature state change.");
        Debug.LogException((Exception) ex);
      }
    }

    public void ReparentEffect(EffectInstance effect, Creature creature, bool isStart)
    {
      if ((UnityEngine.Object) creature == (UnityEngine.Object) null)
        return;
      if ((bool) (UnityEngine.Object) creature.GetRendererForVFX() && effect != null)
        effect.SetRenderer(isStart ? (Renderer) null : creature.GetRendererForVFX(), false);
      effect?.SetParent(isStart ? (Transform) null : creature.ragdoll.rootPart.meshBone.transform, false);
    }

    public virtual void FullRemove()
    {
      base.FullRemove();
      if (this.accumulateEffect != null)
      {
        // ISSUE: method pointer
        this.accumulateEffect.onEffectFinished -= new EffectInstance.EffectFinishEvent((object) null, __methodptr(OnEffectFinished));
        // ISSUE: method pointer
        this.accumulateEffect.onEffectFinished += new EffectInstance.EffectFinishEvent((object) null, __methodptr(OnEffectFinished));
        this.accumulateEffect?.End(false, -1f);
      }
      if (this.fullyChargedEffect == null)
        return;
      // ISSUE: method pointer
      this.fullyChargedEffect.onEffectFinished -= new EffectInstance.EffectFinishEvent((object) null, __methodptr(OnEffectFinished));
      // ISSUE: method pointer
      this.fullyChargedEffect.onEffectFinished += new EffectInstance.EffectFinishEvent((object) null, __methodptr(OnEffectFinished));
      this.fullyChargedEffect?.End(false, -1f);
    }

    public void AddInstability(float instability, bool onValueChange = true)
    {
      if (this.isCharged && this.entity is Creature entity && !entity.isPlayer)
        return;
      float instabilityToAdd = (double) instability > 0.0 ? instability * ValueHandler<float>.op_Implicit((ValueHandler<float>) this.instabilityGainMult) : instability * ValueHandler<float>.op_Implicit((ValueHandler<float>) this.instabilityLossMult);
      double num = (double) this.entity.SetVariable<float>("Instability", (Func<float, float>) (current => Mathf.Clamp(current + instabilityToAdd, 0.0f, this.MaxInstability)));
      if (!onValueChange)
        return;
      this.OnValueChange();
    }

    public virtual void Update()
    {
      base.Update();
      if (!(this.entity is Creature entity))
        return;
      if (!this.isCharged && this.ShouldCharge)
      {
        this.FullyCharged();
        this.isCharged = true;
      }
      this.AddInstability((float) -(entity.isPlayer ? (double) this.data.instabilityReductionPerSecondPlayer : (entity.isKilled ? (double) this.data.instabilityReductionPerSecondKilled : (double) this.data.instabilityReductionPerSecond)) * Time.deltaTime);
      if ((double) this.Instability <= 0.0 && (double) Time.time - (double) this.startTime > 1.0)
      {
        base.FullRemove();
        this.ClearHandlers();
      }
      else
      {
        this.effectInstance?.SetIntensity(this.Intensity);
        if (!this.isCharged && this.accumulateEffect != null)
          this.accumulateEffect?.SetIntensity(this.Intensity);
      }
    }

    public void FullyCharged()
    {
      Creature entity = this.entity as Creature;
      if ((UnityEngine.Object) entity == (UnityEngine.Object) null)
        return;
      base.FullRemove();
      if (!this.data.SpawnEffect(this.entity, this.data.fullyChargedEffectData, out this.fullyChargedEffect))
        return;
      this.fullyChargedEffect?.Play(0, false, false);
      this.chargedTime = Time.time;
      ((MonoBehaviour) entity).StartCoroutine(this.DoCharged(entity));
    }

    public IEnumerator DoCharged(Creature creature)
    {
      yield return (object) new WaitForSeconds(0.1f);
      // ISSUE: method pointer
      creature.OnDamageEvent += new Creature.DamageEvent((object) this, __methodptr(OnDamage));
      // ISSUE: method pointer
      creature.OnKillEvent += new Creature.KillEvent((object) this, __methodptr(OnKill));
      while (this.isCharged && (double) Time.time - (double) this.chargedTime < (double) this.data.chargeDuration)
        yield return (object) new WaitForEndOfFrame();
      if (this.isCharged && (double) Time.time - (double) this.chargedTime >= (double) this.data.chargeDuration)
      {
        if (ArcaneStatus.allowCollapse)
          ArcaneStatus.Collapse(this.data, creature);
        this.EndCharged();
      }
    }

    public void EndCharged()
    {
      this.Instability = 0.0f;
      this.isCharged = false;
      base.FullRemove();
      this.ClearHandlers();
    }

    public static void Explode(StatusDataArcane data, Creature creature)
    {
      Transform transformCopy = Utilities.GetTransformCopy(creature.ragdoll.targetPart.meshBone.transform);
      bool flag = (double) UnityEngine.Random.Range(0.0f, 1f) <= (double) StatusDataArcane.enemyExplodeKillChance && !creature.isPlayer;
      EffectInstance effectInstance = data?.explodeEffectData?.Spawn(transformCopy, true, (ColliderGroup) null, false);
      if (effectInstance != null)
      {
        // ISSUE: method pointer
        effectInstance.onEffectFinished -= new EffectInstance.EffectFinishEvent((object) null, __methodptr(OnEffectFinished));
        // ISSUE: method pointer
        effectInstance.onEffectFinished += new EffectInstance.EffectFinishEvent((object) null, __methodptr(OnEffectFinished));
        effectInstance.Play(0, false, false);
        ArcaneStatus.ApplyStatusForce(data, creature, !flag ? creature : (Creature) null);
      }
      if (!flag)
      {
        creature.ForceStagger(Utilities.GetRandomDirectionInCircle(((ThunderBehaviour) creature.ragdoll.targetPart).transform.position, 1f), (BrainModuleHitReaction.PushBehaviour.Effect) 2, (RagdollPart.Type) 4);
        creature.DamagePatched(creature.currentHealth * (creature.isPlayer ? StatusDataArcane.playerDamagePercentage : StatusDataArcane.enemyDamagePercentage), (DamageType) 4);
      }
      else
      {
        creature.Kill();
        for (int index = creature.ragdoll.parts.Count - 1; index >= 0; --index)
        {
          if ((UnityEngine.Object) creature.ragdoll.parts[index] != (UnityEngine.Object) creature.ragdoll.rootPart && creature.ragdoll.parts[index].sliceAllowed)
            creature.ragdoll.parts[index].TrySlice();
        }
      }
    }

    public static void Explode(
      StatusDataArcane data,
      Vector3 position,
      object handler,
      bool instantCharge = false)
    {
      EffectInstance effectInstance = data?.explodeEffectData?.Spawn(position, Quaternion.identity, (Transform) null, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
      if (effectInstance == null)
        return;
      // ISSUE: method pointer
      effectInstance.onEffectFinished -= new EffectInstance.EffectFinishEvent((object) null, __methodptr(OnEffectFinished));
      // ISSUE: method pointer
      effectInstance.onEffectFinished += new EffectInstance.EffectFinishEvent((object) null, __methodptr(OnEffectFinished));
      effectInstance.Play(0, false, false);
      ArcaneStatus.ApplyStatusForce(data, position, handler, new float?(data.maxInstability), fullStatusAmount: instantCharge);
    }

    public static void Collapse(StatusDataArcane data, Creature creature)
    {
      Transform transformCopy = Utilities.GetTransformCopy(creature.ragdoll.targetPart.meshBone.transform);
      EffectInstance effectInstance = data?.implodeEffectData?.Spawn(transformCopy, true, (ColliderGroup) null, false);
      if (effectInstance != null)
      {
        // ISSUE: method pointer
        effectInstance.onEffectFinished -= new EffectInstance.EffectFinishEvent((object) null, __methodptr(OnEffectFinished));
        // ISSUE: method pointer
        effectInstance.onEffectFinished += new EffectInstance.EffectFinishEvent((object) null, __methodptr(OnEffectFinished));
        effectInstance.Play(0, false, false);
        ArcaneStatus.ApplyStatusForce(data, creature, implode: true);
      }
      if (!data.explodeOnCollapse)
        return;
      ((MonoBehaviour) GameManager.local).StartCoroutine(ArcaneStatus.DelayExplode(data, creature));
    }

    public static void Collapse(
      StatusDataArcane data,
      Vector3 position,
      object handler,
      bool instantCharge = false)
    {
      EffectInstance effectInstance = data?.implodeEffectData?.Spawn(position, Quaternion.identity, (Transform) null, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
      if (effectInstance != null)
      {
        // ISSUE: method pointer
        effectInstance.onEffectFinished -= new EffectInstance.EffectFinishEvent((object) null, __methodptr(OnEffectFinished));
        // ISSUE: method pointer
        effectInstance.onEffectFinished += new EffectInstance.EffectFinishEvent((object) null, __methodptr(OnEffectFinished));
        effectInstance.Play(0, false, false);
        ArcaneStatus.ApplyStatusForce(data, position, handler, new float?(data.maxInstability), true, instantCharge);
      }
      if (!data.explodeOnCollapse)
        return;
      ((MonoBehaviour) GameManager.local).StartCoroutine(ArcaneStatus.DelayExplode(data, position, handler));
    }

    private static void OnEffectFinished(EffectInstance effect)
    {
      // ISSUE: method pointer
      effect.onEffectFinished -= new EffectInstance.EffectFinishEvent((object) null, __methodptr(OnEffectFinished));
      effect.Despawn();
    }

    public static IEnumerator DelayExplode(StatusDataArcane data, Creature creature)
    {
      yield return (object) new WaitForSeconds(data.explodeOnCollapseDelay);
      ArcaneStatus.Explode(data, creature);
    }

    public static IEnumerator DelayExplode(StatusDataArcane data, Vector3 position, object handler)
    {
      yield return (object) new WaitForSeconds(data.explodeOnCollapseDelay);
      ArcaneStatus.Explode(data, position, handler);
    }

    public void OnKill(CollisionInstance hit, EventTime time)
    {
      Creature entity = this.entity as Creature;
      if ((UnityEngine.Object) entity == (UnityEngine.Object) null)
        return;
      try
      {
        // ISSUE: method pointer
        entity.OnDamageEvent -= new Creature.DamageEvent((object) this, __methodptr(OnDamage));
        // ISSUE: method pointer
        entity.OnKillEvent -= new Creature.KillEvent((object) this, __methodptr(OnKill));
        if (!this.CanExplode)
          return;
        ArcaneStatus.Explode(this.data, entity);
        this.EndCharged();
      }
      catch (NullReferenceException ex)
      {
        Debug.LogException((Exception) ex);
      }
    }

    public void OnDamage(CollisionInstance hit, EventTime time)
    {
      Creature entity = this.entity as Creature;
      if ((UnityEngine.Object) entity == (UnityEngine.Object) null)
        return;
      DamageType damageType = hit.damageStruct.damageType;
      if (damageType - 1 > 3 && damageType != 6 || (UnityEngine.Object) hit.sourceColliderGroup == (UnityEngine.Object) null && hit.damageStruct.damageType != 6)
        return;
      Vector3 impactVelocity = hit.impactVelocity;
      RagdollPart hitRagdollPart = hit.damageStruct.hitRagdollPart;
      int num = (UnityEngine.Object) hitRagdollPart != (UnityEngine.Object) null ? (int) hitRagdollPart.type : (int) entity.ragdoll.targetPart.type;
      if (entity.ragdoll.state != 1)
        entity.MaxPush((Creature.PushType) 2, impactVelocity, (RagdollPart.Type) num);
      if (!this.CanExplode)
        return;
      ArcaneStatus.Explode(this.data, entity);
      this.EndCharged();
    }

    public virtual void Transfer(ThunderEntity other)
    {
      other.Inflict((StatusData) this.data, (object) this, float.PositiveInfinity, (object) this.Instability, true);
    }

    public static void ApplyStatusForce(
      StatusDataArcane data,
      Creature creature,
      Creature ignoredCreature = null,
      bool implode = false,
      bool fullStatusAmount = false)
    {
      if ((UnityEngine.Object) creature == (UnityEngine.Object) null)
        return;
      float num1 = implode ? -1f * data.effectForceStrength : data.effectForceStrength;
      HashSet<PhysicBody> physicBodySet = new HashSet<PhysicBody>();
      HashSet<Creature> creatureSet = new HashSet<Creature>();
      Transform transform = creature.ragdoll.targetPart.meshBone.transform;
      foreach (Collider collider in Physics.OverlapSphere(transform.position, data.effectForceRadius, data.radialMask, (QueryTriggerInteraction) 1))
      {
        if ((bool) (UnityEngine.Object) collider.attachedRigidbody && !physicBodySet.Contains(Extensions.GetPhysicBody((Component) collider)))
        {
          Creature componentInParent1 = ((Component) collider.attachedRigidbody).GetComponentInParent<Creature>();
          if (!((UnityEngine.Object) componentInParent1 != (UnityEngine.Object) null) || !componentInParent1.isPlayer && !((UnityEngine.Object) componentInParent1 == (UnityEngine.Object) ignoredCreature))
          {
            if ((UnityEngine.Object) componentInParent1 != (UnityEngine.Object) null && (UnityEngine.Object) componentInParent1 != (UnityEngine.Object) creature && !componentInParent1.isKilled && !creatureSet.Contains(componentInParent1))
            {
              if (ArcaneStatus.allowExplosionStatusApply)
              {
                float magnitude = (transform.position - componentInParent1.ragdoll.targetPart.meshBone.transform.position).magnitude;
                ArcaneStatus arcaneStatus;
                float num2 = ((ThunderEntity) creature).TryGetStatus<ArcaneStatus>((StatusData) data, ref arcaneStatus) ? arcaneStatus.Instability : data.maxInstability;
                ((ThunderEntity) componentInParent1).Inflict((StatusData) data, (object) creature, float.PositiveInfinity, (object) (float) ((double) num2 * (fullStatusAmount ? 1.0 : (double) magnitude / (double) data.effectForceRadius)), true);
              }
              componentInParent1.MaxPush((Creature.PushType) 0, Vector3.zero, (RagdollPart.Type) 0);
              componentInParent1.ragdoll.SetState((Ragdoll.State) 1);
              creatureSet.Add(componentInParent1);
            }
            physicBodySet.Add(Extensions.GetPhysicBody((Component) collider));
            collider.attachedRigidbody.AddExplosionForce(num1, transform.position, data.effectForceRadius, 1f, (ForceMode) 2);
            Breakable componentInParent2 = ((Component) collider.attachedRigidbody).GetComponentInParent<Breakable>();
            if ((UnityEngine.Object) componentInParent2 != (UnityEngine.Object) null && !componentInParent2.contactBreakOnly)
            {
              if ((double) data.effectForceStrength * (double) data.effectForceStrength > (double) componentInParent2.instantaneousBreakDamage)
                componentInParent2.Break();
              for (int index = 0; index < componentInParent2.subBrokenItems.Count; ++index)
              {
                PhysicBody physicBody = componentInParent2.subBrokenItems[index].physicBody;
                if (PhysicBody.op_Implicit(physicBody) && !physicBodySet.Contains(physicBody) && (bool) (UnityEngine.Object) physicBody.rigidBody)
                {
                  physicBody.rigidBody.AddExplosionForce(num1, transform.position, data.effectForceRadius, 1f, (ForceMode) 2);
                  physicBodySet.Add(physicBody);
                }
              }
              for (int index = 0; index < componentInParent2.subBrokenBodies.Count; ++index)
              {
                PhysicBody subBrokenBody = componentInParent2.subBrokenBodies[index];
                if (PhysicBody.op_Implicit(subBrokenBody) && !physicBodySet.Contains(subBrokenBody) && (bool) (UnityEngine.Object) subBrokenBody.rigidBody)
                {
                  subBrokenBody.rigidBody.AddExplosionForce(num1, transform.position, data.effectForceRadius, 1f, (ForceMode) 2);
                  physicBodySet.Add(subBrokenBody);
                }
              }
            }
          }
        }
      }
    }

    public static void ApplyStatusForce(
      StatusDataArcane data,
      Vector3 centerPosition,
      object handler,
      float? statusAmount = null,
      bool implode = false,
      bool fullStatusAmount = false)
    {
      float num1 = implode ? -1f * data.effectForceStrength : data.effectForceStrength;
      HashSet<PhysicBody> physicBodySet = new HashSet<PhysicBody>();
      HashSet<Creature> creatureSet = new HashSet<Creature>();
      foreach (Collider collider in Physics.OverlapSphere(centerPosition, data.effectForceRadius, data.radialMask, (QueryTriggerInteraction) 1))
      {
        if ((bool) (UnityEngine.Object) collider.attachedRigidbody && !physicBodySet.Contains(Extensions.GetPhysicBody((Component) collider)))
        {
          Creature componentInParent1 = ((Component) collider.attachedRigidbody).GetComponentInParent<Creature>();
          if (!((UnityEngine.Object) componentInParent1 != (UnityEngine.Object) null) || !componentInParent1.isPlayer)
          {
            if ((UnityEngine.Object) componentInParent1 != (UnityEngine.Object) null && !componentInParent1.isKilled && !creatureSet.Contains(componentInParent1))
            {
              if (ArcaneStatus.allowExplosionStatusApply && statusAmount.HasValue)
              {
                float magnitude = (centerPosition - componentInParent1.ragdoll.targetPart.meshBone.transform.position).magnitude;
                Creature creature = componentInParent1;
                StatusDataArcane statusDataArcane = data;
                object obj = handler;
                float? nullable = statusAmount;
                float num2 = fullStatusAmount ? 1f : magnitude / data.effectForceRadius;
                // ISSUE: variable of a boxed type
                __Boxed<float?> local = (ValueType) (nullable.HasValue ? new float?(nullable.GetValueOrDefault() * num2) : new float?());
                ((ThunderEntity) creature).Inflict((StatusData) statusDataArcane, obj, float.PositiveInfinity, (object) local, true);
              }
              componentInParent1.MaxPush((Creature.PushType) 0, Vector3.zero, (RagdollPart.Type) 0);
              componentInParent1.ragdoll.SetState((Ragdoll.State) 1);
              creatureSet.Add(componentInParent1);
            }
            physicBodySet.Add(Extensions.GetPhysicBody((Component) collider));
            collider.attachedRigidbody.AddExplosionForce(num1, centerPosition, data.effectForceRadius, 1f, (ForceMode) 2);
            Breakable componentInParent2 = ((Component) collider.attachedRigidbody).GetComponentInParent<Breakable>();
            if ((UnityEngine.Object) componentInParent2 != (UnityEngine.Object) null && !componentInParent2.contactBreakOnly)
            {
              if ((double) data.effectForceStrength * (double) data.effectForceStrength > (double) componentInParent2.instantaneousBreakDamage)
                componentInParent2.Break();
              for (int index = 0; index < componentInParent2.subBrokenItems.Count; ++index)
              {
                PhysicBody physicBody = componentInParent2.subBrokenItems[index].physicBody;
                if (PhysicBody.op_Implicit(physicBody) && !physicBodySet.Contains(physicBody) && (bool) (UnityEngine.Object) physicBody.rigidBody)
                {
                  physicBody.rigidBody.AddExplosionForce(num1, centerPosition, data.effectForceRadius, 1f, (ForceMode) 2);
                  physicBodySet.Add(physicBody);
                }
              }
              for (int index = 0; index < componentInParent2.subBrokenBodies.Count; ++index)
              {
                PhysicBody subBrokenBody = componentInParent2.subBrokenBodies[index];
                if (PhysicBody.op_Implicit(subBrokenBody) && !physicBodySet.Contains(subBrokenBody) && (bool) (UnityEngine.Object) subBrokenBody.rigidBody)
                {
                  subBrokenBody.rigidBody.AddExplosionForce(num1, centerPosition, data.effectForceRadius, 1f, (ForceMode) 2);
                  physicBodySet.Add(subBrokenBody);
                }
              }
            }
          }
        }
      }
    }
  }
}
