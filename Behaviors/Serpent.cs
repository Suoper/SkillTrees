// Decompiled with JetBrains decompiler
// Type: Arcana.Behaviors.Serpent
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Misc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Behaviors
{
  public class Serpent : ThunderBehaviour
  {
    public Serpent.SerpentData data;
    public Side side;
    public RagdollPart part;
    public EffectInstance effect;
    public RotateAroundCenter rotateAroundCenter;
    public ParticleLifetimeScaler particleLifetimeScaler;
    public bool orbitingObject = false;
    private Vector3 objectPosition;
    public Vector3 objectOrbitNormal;
    public Vector3 randomObjectOrbitNormal = UnityEngine.Random.onUnitSphere;
    private bool useRandomOrbitNormal = true;
    private Vector3 armPosition;
    private Vector3 armOrbitNormal;
    private bool doOrbitChange = false;
    private bool doOrbitEnd = false;
    private float orbitChangeStart;
    private float orbitChangeTimeLimit = float.MaxValue;
    public object orbitHandler = (object) null;
    public List<Creature> targetCreatures;
    public Creature ignoredCreature;
    public HashSet<Creature> hitCreatures;
    public bool isAttacking = false;
    public BrainModuleHitReaction.PushBehaviour.Effect staggerType = (BrainModuleHitReaction.PushBehaviour.Effect) 2;
    private bool switchTargets = false;
    public float? tempScale = new float?();
    private Vector3 baseScale;
    public float movementMultiplier = 1f;
    private Func<Serpent.OnAttackFinish> onAttackFinish;
    private Coroutine attackRoutine;
    public static List<Serpent> allActive;

    public Transform ObjectOrbitTransform { get; private set; }

    public bool IsChangingOrbit()
    {
      return (UnityEngine.Object) this.transform?.parent == (UnityEngine.Object) null || this.doOrbitChange;
    }

    private Vector3 GetTargetPosition()
    {
      return !this.orbitingObject ? this.armPosition : this.objectPosition;
    }

    private Vector3 GetTargetOrbitNormal()
    {
      if (this.isAttacking)
        return Vector3.up;
      return !this.orbitingObject ? this.armOrbitNormal : this.objectOrbitNormal;
    }

    public event Serpent.OnOrbitChangeStart OnOrbitChangeStartEvent;

    public event Serpent.OnOrbitChangeEnd OnOrbitChangeEndEvent;

    public event Serpent.OnDespawn OnDespawnEvent;

    public event Serpent.OnCreatureHit OnCreatureHitEvent;

    public virtual ManagedLoops EnabledManagedLoops => (ManagedLoops) 6;

    public void Form(
      Serpent.SerpentData data,
      Vector3 position,
      Vector3 facing,
      Creature creature,
      Side side)
    {
      this.Form(data, position, facing, (Transform) null, creature, side);
    }

    public void Form(
      Serpent.SerpentData data,
      Vector3 position,
      Vector3 facing,
      Transform orbitObjectTransform)
    {
      this.Form(data, position, facing, orbitObjectTransform, (Creature) null, (Side) 0);
    }

    public void Form(
      Serpent.SerpentData data,
      Vector3 position,
      Vector3 facing,
      Transform orbitObjectTransform,
      Creature creature,
      Side side)
    {
      this.data = data;
      this.transform.position = position;
      this.transform.rotation = Quaternion.LookRotation(facing);
      this.baseScale = this.transform.localScale;
      this.ignoredCreature = creature;
      if ((bool) (UnityEngine.Object) orbitObjectTransform)
        this.ObjectOrbitTransform = orbitObjectTransform;
      this.side = side;
      if ((bool) (UnityEngine.Object) creature)
        this.part = creature.ragdoll.GetPartByName(side == null ? "RightForeArm" : "LeftForeArm");
      this.ResetRotation();
      this.effect = this.data.serpentEffectData.Spawn(this.transform, true, (ColliderGroup) null, false);
      this.effect?.Play(0, false, false);
      this.particleLifetimeScaler = this.gameObject.AddComponent<ParticleLifetimeScaler>();
      this.particleLifetimeScaler.apply = true;
      if (Serpent.allActive == null)
        Serpent.allActive = new List<Serpent>();
      Serpent.allActive.Add(this);
      // ISSUE: method pointer
      EventManager.onCreatureDespawn -= new EventManager.CreatureDespawnedEvent((object) this, __methodptr(OnTargetDespawn));
      // ISSUE: method pointer
      EventManager.onCreatureDespawn += new EventManager.CreatureDespawnedEvent((object) this, __methodptr(OnTargetDespawn));
      // ISSUE: method pointer
      EventManager.onLevelUnload -= new EventManager.LevelLoadEvent((object) this, __methodptr(OnLevelChange));
      // ISSUE: method pointer
      EventManager.onLevelUnload += new EventManager.LevelLoadEvent((object) this, __methodptr(OnLevelChange));
    }

    public void OnLevelChange(LevelData levelData, LevelData.Mode mode, EventTime eventTime)
    {
      this.Despawn(false);
    }

    protected virtual void ManagedUpdate()
    {
      base.ManagedUpdate();
      if (this.switchTargets)
      {
        this.AssignNewOrbit(((ThunderBehaviour) this.targetCreatures.First<Creature>().ragdoll.targetPart).transform, false, true);
        this.switchTargets = false;
      }
      if (this.doOrbitChange)
      {
        this.transform.SetParent((Transform) null);
        this.orbitingObject = (UnityEngine.Object) this.ObjectOrbitTransform != (UnityEngine.Object) null;
        this.orbitChangeStart = Time.time;
        Serpent.OnOrbitChangeStart changeStartEvent = this.OnOrbitChangeStartEvent;
        if (changeStartEvent != null)
          changeStartEvent(this, this.ObjectOrbitTransform, this.orbitChangeStart);
      }
      if ((bool) (UnityEngine.Object) this.part)
      {
        this.armPosition = ((ThunderBehaviour) this.part).transform.position - this.part.upDirection.normalized * this.data.slide;
        this.armOrbitNormal = this.part.upDirection.normalized;
      }
      if ((bool) (UnityEngine.Object) this.ObjectOrbitTransform)
      {
        this.objectPosition = this.ObjectOrbitTransform.position;
        this.objectOrbitNormal = this.useRandomOrbitNormal ? this.randomObjectOrbitNormal : this.ObjectOrbitTransform.forward;
      }
      Transform transform = this.transform;
      Vector3 vector3;
      if (!this.isAttacking || !this.data.scaleOnAttack)
      {
        Vector3 localScale = this.transform.localScale;
        Vector3? nullable;
        if (!this.tempScale.HasValue)
        {
          nullable = new Vector3?(this.baseScale);
        }
        else
        {
          Vector3 one = Vector3.one;
          float? tempScale = this.tempScale;
          nullable = tempScale.HasValue ? new Vector3?(one * tempScale.GetValueOrDefault()) : new Vector3?();
        }
        Vector3 b = nullable.Value;
        double t = (double) Time.deltaTime * ((bool) (UnityEngine.Object) this.transform.parent ? 10.0 : 1.0);
        vector3 = Vector3.Lerp(localScale, b, (float) t);
      }
      else
        vector3 = Vector3.Lerp(this.transform.localScale, Vector3.one * this.data.attackScale, Time.deltaTime * ((bool) (UnityEngine.Object) this.transform.parent ? 10f : 1f));
      transform.localScale = vector3;
      if (!this.doOrbitEnd)
        return;
      Serpent.OnOrbitChangeEnd orbitChangeEndEvent = this.OnOrbitChangeEndEvent;
      if (orbitChangeEndEvent != null)
        orbitChangeEndEvent(this, this.ObjectOrbitTransform, Time.time - this.orbitChangeStart);
      this.doOrbitEnd = false;
    }

    protected virtual void ManagedLateUpdate()
    {
      base.ManagedLateUpdate();
      if ((bool) (UnityEngine.Object) this.rotateAroundCenter)
        this.rotateAroundCenter.UpdatePosition(this.GetTargetPosition(), this.GetTargetOrbitNormal());
      if ((UnityEngine.Object) this.transform.parent != (UnityEngine.Object) null)
        return;
      Transform transform = this.rotateAroundCenter.transform;
      if ((double) Time.time - (double) this.orbitChangeStart > (double) this.orbitChangeTimeLimit)
        this.TeleportToMovementTarget(transform);
      else
        this.DoOrbitChangeMovement(transform, Time.deltaTime);
      if ((double) Vector3.Distance(transform.position, this.transform.position) < 0.0099999997764825821 && !this.doOrbitChange && !this.switchTargets)
      {
        this.transform.SetParent(transform);
        this.transform.position = transform.position;
        this.transform.rotation = transform.rotation;
        this.doOrbitEnd = true;
      }
      this.doOrbitChange = false;
    }

    private void DoOrbitChangeMovement(Transform target, float deltaTime)
    {
      Vector3 normalized = (target.position - this.transform.position).normalized;
      float num1 = Vector3.Angle(this.transform.forward, normalized);
      float num2 = Mathf.Max(this.data.minSpeed * this.movementMultiplier, (this.isAttacking || (double) Vector3.Distance(target.position, this.transform.position) > 1.0 ? this.data.attackSpeed : this.data.maxSpeed) * this.movementMultiplier * Mathf.Cos((float) Math.PI / 180f * num1));
      Vector3 vector3 = this.transform.position + this.transform.forward * num2 * this.movementMultiplier * deltaTime;
      bool flag = (double) (target.position - this.transform.position).magnitude > 0.3;
      this.transform.position = Vector3.MoveTowards(this.transform.position, flag ? vector3 : target.position, num2 * this.movementMultiplier * deltaTime);
      float num3 = Mathf.Lerp(this.data.minTurnSpeed * this.movementMultiplier, this.data.maxTurnSpeed * this.movementMultiplier, num1 / 180f);
      Quaternion b = Quaternion.LookRotation(normalized);
      this.transform.rotation = flag ? Quaternion.Slerp(this.transform.rotation, b, num3 * deltaTime) : Quaternion.LookRotation((target.position - this.transform.position).normalized);
    }

    private void TeleportToMovementTarget(Transform target)
    {
      EffectInstance effectInstance1 = this.data.teleportEffectData?.Spawn(this.transform.position, Quaternion.identity, (Transform) null, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
      foreach (EffectParticle effectParticle in effectInstance1.effects.OfType<EffectParticle>())
        ((Component) effectParticle.rootParticleSystem).transform.localScale = !this.isAttacking || !this.data.scaleOnAttack ? Vector3.one : this.transform.localScale + Vector3.one;
      effectInstance1.Play(0, false, false);
      this.transform.position = target.position;
      this.transform.rotation = target.rotation;
      EffectInstance effectInstance2 = this.data.teleportEffectData?.Spawn(this.transform.position, Quaternion.identity, this.transform, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
      foreach (EffectParticle effectParticle in effectInstance2.effects.OfType<EffectParticle>())
        ((Component) effectParticle.rootParticleSystem).transform.localScale = !this.isAttacking || !this.data.scaleOnAttack ? Vector3.one : this.transform.localScale + Vector3.one;
      effectInstance2.Play(0, false, false);
    }

    public void SetTargets(List<Creature> targets, Func<Serpent.OnAttackFinish> onFinish = null)
    {
      this.targetCreatures = targets;
      if (this.targetCreatures.Count > this.data.maxChain)
        targets.RemoveRange(this.data.maxChain, targets.Count - this.data.maxChain);
      this.hitCreatures = new HashSet<Creature>();
      this.AssignNewOrbit(((ThunderBehaviour) targets.First<Creature>().ragdoll.targetPart).transform, false, true);
      this.OnOrbitChangeEndEvent -= new Serpent.OnOrbitChangeEnd(this.OnAttackTargetReached);
      this.OnOrbitChangeEndEvent += new Serpent.OnOrbitChangeEnd(this.OnAttackTargetReached);
      this.rotateAroundCenter.radius = this.data.attackOrbitRadius;
      this.isAttacking = true;
      this.onAttackFinish = onFinish;
    }

    public void AssignHandler(object handler = null) => this.orbitHandler = handler;

    public void AssignNewOrbit(
      Transform orbitObject,
      bool useRandomOrbitNormal,
      bool doSineMovement = false,
      float timeLimit = 3.40282347E+38f,
      object handler = null)
    {
      if (handler != null && handler != this.orbitHandler)
        return;
      this.ObjectOrbitTransform = orbitObject;
      this.rotateAroundCenter.moveInSineWave = doSineMovement;
      this.useRandomOrbitNormal = useRandomOrbitNormal;
      this.orbitChangeTimeLimit = timeLimit;
      this.doOrbitChange = true;
    }

    public void ResetOrbit(float timeLimit = 3.40282347E+38f, object handler = null)
    {
      if (handler != null && handler != this.orbitHandler)
        return;
      this.OnOrbitChangeEndEvent -= new Serpent.OnOrbitChangeEnd(this.OnAttackTargetReached);
      this.ObjectOrbitTransform = (Transform) null;
      this.targetCreatures = (List<Creature>) null;
      this.isAttacking = false;
      this.hitCreatures = (HashSet<Creature>) null;
      this.rotateAroundCenter.moveInSineWave = true;
      this.orbitChangeTimeLimit = timeLimit;
      this.doOrbitChange = true;
    }

    public void Despawn(bool invokeEvent = true)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      Serpent.\u003C\u003Ec__DisplayClass68_0 cDisplayClass680 = new Serpent.\u003C\u003Ec__DisplayClass68_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass680.\u003C\u003E4__this = this;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.particleLifetimeScaler);
      UnityEngine.Object.Destroy((UnityEngine.Object) this.rotateAroundCenter);
      if (invokeEvent)
      {
        Serpent.OnDespawn onDespawnEvent = this.OnDespawnEvent;
        if (onDespawnEvent != null)
          onDespawnEvent(this);
      }
      // ISSUE: reference to a compiler-generated field
      cDisplayClass680.onEffectFinished = (EffectInstance.EffectFinishEvent) null;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      cDisplayClass680.onEffectFinished = new EffectInstance.EffectFinishEvent((object) cDisplayClass680, __methodptr(\u003CDespawn\u003Eb__0));
      // ISSUE: reference to a compiler-generated field
      this.effect.onEffectFinished -= cDisplayClass680.onEffectFinished;
      // ISSUE: reference to a compiler-generated field
      this.effect.onEffectFinished += cDisplayClass680.onEffectFinished;
      this.effect?.End(false, -1f);
    }

    public void ResetRotation()
    {
      if (!(bool) (UnityEngine.Object) this.rotateAroundCenter)
      {
        this.rotateAroundCenter = new GameObject().AddComponent<RotateAroundCenter>();
        this.rotateAroundCenter.transform.position = (bool) (UnityEngine.Object) this.part ? ((ThunderBehaviour) this.part).transform.position : this.ObjectOrbitTransform.position;
      }
      this.rotateAroundCenter.radius = this.data.GetRandomRadius();
      this.rotateAroundCenter.speed = this.data.GetRandomSpeed();
      this.rotateAroundCenter.sineWaveAmplitude = this.data.GetRandomSineAmplitude();
      this.rotateAroundCenter.sineWaveFrequency = this.data.GetRandomSineFrequency();
      this.rotateAroundCenter.rotationOffset = (float) UnityEngine.Random.Range(0, 360);
      this.rotateAroundCenter.moveInSineWave = this.data.doSineMovement;
    }

    public void LoadRotationData(
      float? radius = null,
      float? radiusVariance = null,
      float? speed = null,
      float? speedVariance = null,
      float? sineAmplitude = null,
      float? sineAmplitudeVariance = null,
      float? sineFrequency = null,
      float? sineFrequencyVariance = null,
      bool? doSineMovement = null)
    {
      RotateAroundCenter rotateAroundCenter1 = this.rotateAroundCenter;
      float? nullable1 = radius;
      float? nullable2;
      float? nullable3;
      double minInclusive1;
      if (!nullable1.HasValue)
      {
        float armTargetRadius = this.data.armTargetRadius;
        nullable2 = radiusVariance;
        nullable3 = nullable2.HasValue ? new float?(armTargetRadius - nullable2.GetValueOrDefault()) : new float?();
        minInclusive1 = (double) nullable3 ?? (double) this.data.armTargetRadiusVariance;
      }
      else
        minInclusive1 = (double) nullable1.GetValueOrDefault();
      nullable1 = radius;
      double maxInclusive1;
      if (!nullable1.HasValue)
      {
        float armTargetRadius = this.data.armTargetRadius;
        nullable2 = radiusVariance;
        nullable3 = nullable2.HasValue ? new float?(armTargetRadius + nullable2.GetValueOrDefault()) : new float?();
        maxInclusive1 = (double) nullable3 ?? (double) this.data.armTargetRadiusVariance;
      }
      else
        maxInclusive1 = (double) nullable1.GetValueOrDefault();
      double num1 = (double) UnityEngine.Random.Range((float) minInclusive1, (float) maxInclusive1);
      rotateAroundCenter1.radius = (float) num1;
      RotateAroundCenter rotateAroundCenter2 = this.rotateAroundCenter;
      nullable1 = speed;
      double minInclusive2;
      if (!nullable1.HasValue)
      {
        float armTargetSpeed = this.data.armTargetSpeed;
        nullable2 = speedVariance;
        nullable3 = nullable2.HasValue ? new float?(armTargetSpeed - nullable2.GetValueOrDefault()) : new float?();
        minInclusive2 = (double) nullable3 ?? (double) this.data.armTargetSpeedVariance;
      }
      else
        minInclusive2 = (double) nullable1.GetValueOrDefault();
      nullable1 = speed;
      double maxInclusive2;
      if (!nullable1.HasValue)
      {
        float armTargetSpeed = this.data.armTargetSpeed;
        nullable2 = speedVariance;
        nullable3 = nullable2.HasValue ? new float?(armTargetSpeed + nullable2.GetValueOrDefault()) : new float?();
        maxInclusive2 = (double) nullable3 ?? (double) this.data.armTargetSpeedVariance;
      }
      else
        maxInclusive2 = (double) nullable1.GetValueOrDefault();
      double num2 = (double) UnityEngine.Random.Range((float) minInclusive2, (float) maxInclusive2);
      rotateAroundCenter2.speed = (float) num2;
      this.rotateAroundCenter.speed = UnityEngine.Random.Range(0, 2) == 0 ? this.rotateAroundCenter.speed : -this.rotateAroundCenter.speed;
      RotateAroundCenter rotateAroundCenter3 = this.rotateAroundCenter;
      nullable1 = sineAmplitude;
      double minInclusive3;
      if (!nullable1.HasValue)
      {
        float targetSineAmplitude = this.data.armTargetSineAmplitude;
        nullable2 = sineAmplitudeVariance;
        nullable3 = nullable2.HasValue ? new float?(targetSineAmplitude - nullable2.GetValueOrDefault()) : new float?();
        minInclusive3 = (double) nullable3 ?? (double) this.data.armTargetSineAmplitudeVariance;
      }
      else
        minInclusive3 = (double) nullable1.GetValueOrDefault();
      nullable1 = sineAmplitude;
      double maxInclusive3;
      if (!nullable1.HasValue)
      {
        float targetSineAmplitude = this.data.armTargetSineAmplitude;
        nullable2 = sineAmplitudeVariance;
        nullable3 = nullable2.HasValue ? new float?(targetSineAmplitude + nullable2.GetValueOrDefault()) : new float?();
        maxInclusive3 = (double) nullable3 ?? (double) this.data.armTargetSineAmplitudeVariance;
      }
      else
        maxInclusive3 = (double) nullable1.GetValueOrDefault();
      double num3 = (double) UnityEngine.Random.Range((float) minInclusive3, (float) maxInclusive3);
      rotateAroundCenter3.sineWaveAmplitude = (float) num3;
      RotateAroundCenter rotateAroundCenter4 = this.rotateAroundCenter;
      nullable1 = sineFrequency;
      double minInclusive4;
      if (!nullable1.HasValue)
      {
        float targetSineFrequency = this.data.armTargetSineFrequency;
        nullable2 = sineFrequencyVariance;
        nullable3 = nullable2.HasValue ? new float?(targetSineFrequency - nullable2.GetValueOrDefault()) : new float?();
        minInclusive4 = (double) nullable3 ?? (double) this.data.armTargetSineFrequencyVariance;
      }
      else
        minInclusive4 = (double) nullable1.GetValueOrDefault();
      nullable1 = sineFrequency;
      double maxInclusive4;
      if (!nullable1.HasValue)
      {
        float targetSineFrequency = this.data.armTargetSineFrequency;
        nullable2 = sineFrequencyVariance;
        nullable3 = nullable2.HasValue ? new float?(targetSineFrequency + nullable2.GetValueOrDefault()) : new float?();
        maxInclusive4 = (double) nullable3 ?? (double) this.data.armTargetSineFrequencyVariance;
      }
      else
        maxInclusive4 = (double) nullable1.GetValueOrDefault();
      double num4 = (double) UnityEngine.Random.Range((float) minInclusive4, (float) maxInclusive4);
      rotateAroundCenter4.sineWaveFrequency = (float) num4;
      this.rotateAroundCenter.rotationOffset = (float) UnityEngine.Random.Range(0, 360);
      this.rotateAroundCenter.moveInSineWave = ((int) doSineMovement ?? (this.data.doSineMovement ? 1 : 0)) != 0;
    }

    public void OnAttackTargetReached(Serpent serpent, Transform orbitObject, float endTime)
    {
      Creature component = (Creature) null;
      if (!(bool) (UnityEngine.Object) orbitObject || !orbitObject.TryGetComponentInParent<Creature>(out component) || (UnityEngine.Object) component == (UnityEngine.Object) this.ignoredCreature || this.hitCreatures.Contains(component) || Utils.IsNullOrEmpty((ICollection) this.targetCreatures) || (UnityEngine.Object) this.targetCreatures.First<Creature>() != (UnityEngine.Object) component)
      {
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
          this.ResetOrbit(this.data.teleportReturnFromAttackTimeout);
        else
          this.attackRoutine = ((MonoBehaviour) this).StartCoroutine(this.TargetSwapPause(component));
      }
      else
      {
        this.hitCreatures.Add(component);
        EffectInstance effectInstance = this.data.hitEffectData?.Spawn(this.transform.position, Quaternion.identity, (Transform) null, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
        foreach (EffectParticle effectParticle in effectInstance.effects.OfType<EffectParticle>())
          ((Component) effectParticle.rootParticleSystem).transform.localScale = !this.isAttacking || !this.data.scaleOnAttack ? Vector3.one : this.transform.localScale + Vector3.one;
        effectInstance.Play(0, false, false);
        component.DamagePatched(this.data.damage, (DamageType) 4);
        component.ForceStagger(((ThunderBehaviour) component.ragdoll.targetPart).transform.position - this.transform.position, this.staggerType, (RagdollPart.Type) 4);
        Serpent.OnCreatureHit creatureHitEvent = this.OnCreatureHitEvent;
        if (creatureHitEvent != null)
          creatureHitEvent(this, component);
        this.attackRoutine = ((MonoBehaviour) this).StartCoroutine(this.TargetSwapPause(component));
      }
    }

    private IEnumerator TargetSwapPause(Creature creature)
    {
      float startTime = Time.time;
      while ((double) Time.time - (double) startTime < (double) this.data.targetChangeDelay)
        yield return (object) new WaitForEndOfFrame();
      yield return (object) new WaitForEndOfFrame();
      this.ChangeTargets(creature);
    }

    private bool ChangeTargets(Creature currentTarget)
    {
      if ((UnityEngine.Object) currentTarget == (UnityEngine.Object) null)
        return false;
      this.targetCreatures?.Remove(currentTarget);
      this.targetCreatures?.RemoveAll((Predicate<Creature>) (x => x.isKilled || (UnityEngine.Object) x == (UnityEngine.Object) null));
      if (!Utils.IsNullOrEmpty((ICollection) this.targetCreatures))
      {
        this.switchTargets = true;
      }
      else
      {
        if (this.onAttackFinish != null)
        {
          Serpent.OnAttackFinish onAttackFinish = this.onAttackFinish();
          this.onAttackFinish = (Func<Serpent.OnAttackFinish>) null;
          this.isAttacking = false;
          return false;
        }
        this.rotateAroundCenter.radius = this.data.GetRandomRadius();
        if (this.data.resetOnAttackFinish)
        {
          this.isAttacking = false;
          this.ResetOrbit(this.data.teleportReturnFromAttackTimeout);
        }
        else
          this.Despawn();
      }
      return true;
    }

    private void OnTargetDespawn(Creature creature, EventTime eventTime)
    {
      if (!this.isAttacking || (UnityEngine.Object) creature != (UnityEngine.Object) this.targetCreatures.First<Creature>() || eventTime != null || this.attackRoutine == null)
        return;
      ((MonoBehaviour) this).StopCoroutine(this.attackRoutine);
      this.ChangeTargets(creature);
    }

    public delegate void OnOrbitChangeStart(
      Serpent serpent,
      Transform orbitObject,
      float startTime);

    public delegate void OnOrbitChangeEnd(Serpent serpent, Transform orbitObject, float endTime);

    public delegate void OnDespawn(Serpent serpent);

    public delegate void OnCreatureHit(Serpent serpent, Creature creature);

    public delegate void OnAttackFinish();

    public class SerpentData
    {
      public string serpentEffectId = "SpellArcaneSerpent";
      public EffectData serpentEffectData;
      public string hitEffectId = "HitArcaneSerpent";
      public EffectData hitEffectData;
      public string teleportEffectId = "SpellArcaneSerpentTeleport";
      public EffectData teleportEffectData;
      public float damage = 30f;
      public float targetChangeDelay = 2.5f;
      public float attackOrbitRadius = 0.5f;
      public float attackSpeed = 7f;
      public int maxChain = 2;
      public bool resetOnAttackFinish = true;
      public bool scaleOnAttack = true;
      public float attackScale = 8f;
      public float maxSpeed = 1.75f;
      public float minSpeed = 0.25f;
      public float maxTurnSpeed = 30f;
      public float minTurnSpeed = 0.1f;
      public float armTargetRadius = 0.07f;
      public float armTargetRadiusVariance = 0.02f;
      public float armTargetSpeed = 180f;
      public float armTargetSpeedVariance = 45f;
      public float armTargetSineAmplitude = 0.05f;
      public float armTargetSineAmplitudeVariance = 0.02f;
      public float armTargetSineFrequency = 2.75f;
      public float armTargetSineFrequencyVariance = 0.5f;
      public bool doSineMovement = true;
      public float slide = 0.17f;
      public float teleportReturnTimeout = 2.5f;
      public float teleportReturnFromAttackTimeout = 5f;
      public float teleportTargetTimeout = 1.25f;

      public void LoadCatalogData()
      {
        this.serpentEffectData = Catalog.GetData<EffectData>(this.serpentEffectId, true);
        this.hitEffectData = Catalog.GetData<EffectData>(this.hitEffectId, true);
        this.teleportEffectData = Catalog.GetData<EffectData>(this.teleportEffectId, true);
        Debug.LogError((object) "Serpent Data Loaded");
      }

      public float GetRandomRadius()
      {
        return UnityEngine.Random.Range(this.armTargetRadius - this.armTargetRadiusVariance, this.armTargetRadius + this.armTargetRadiusVariance);
      }

      public float GetRandomSpeed()
      {
        float num = UnityEngine.Random.Range(this.armTargetSpeed - this.armTargetSpeedVariance, this.armTargetSpeed + this.armTargetSpeedVariance);
        return UnityEngine.Random.Range(0, 2) == 0 ? num : -num;
      }

      public float GetRandomSineAmplitude()
      {
        return UnityEngine.Random.Range(this.armTargetSineAmplitude - this.armTargetSineAmplitudeVariance, this.armTargetSineAmplitude + this.armTargetSineAmplitudeVariance);
      }

      public float GetRandomSineFrequency()
      {
        return UnityEngine.Random.Range(this.armTargetSineFrequency - this.armTargetSineFrequencyVariance, this.armTargetSineFrequency + this.armTargetSineFrequencyVariance);
      }
    }
  }
}
