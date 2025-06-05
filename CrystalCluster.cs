// Decompiled with JetBrains decompiler
// Type: Crystallic.CrystalCluster
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic
{
  public class CrystalCluster : ThunderBehaviour
  {
    public bool ignorePlayer = true;
    public SpellCastCharge spellCastCharge;
    public Coroutine despawnRoutine;
    protected TriggerDetector detector;
    protected EffectInstance dropEffect;
    protected HashSet<ThunderEntity> entitiesThisFrame;
    protected bool isPlayingEffect;
    public Lerper lerper;
    protected Rigidbody rb;
    protected float startTime;
    protected CapsuleCollider trigger;
    protected EffectInstance wallEffect;
    public WaterHandler waterHandler;

    public virtual ManagedLoops EnabledManagedLoops => (ManagedLoops) 2;

    private void Awake()
    {
      this.lerper = new Lerper();
      this.entitiesThisFrame = new HashSet<ThunderEntity>();
      this.spellCastCharge = Catalog.GetData<SpellCastCharge>("Crystallic", true);
      this.waterHandler = new WaterHandler(false, false);
      // ISSUE: method pointer
      this.waterHandler.OnWaterEnter += new WaterHandler.SimpleDelegate((object) this, __methodptr(OnWaterEnter));
    }

    private void OnCollisionEnter(Collision other)
    {
      if (this.isPlayingEffect)
        return;
      Rigidbody rigidbody = other.rigidbody;
      if ((UnityEngine.Object) rigidbody != (UnityEngine.Object) null && !rigidbody.isKinematic)
        return;
      this.dropEffect?.End(false, -1f);
      this.wallEffect?.Play(0, false, false);
      ((Collider) this.trigger).enabled = true;
      this.isPlayingEffect = true;
    }

    public void OnTriggerStay(Collider other)
    {
      ThunderEntity componentInParent = ((Component) other.attachedRigidbody)?.GetComponentInParent<ThunderEntity>();
      if ((UnityEngine.Object) componentInParent == (UnityEngine.Object) null || !this.entitiesThisFrame.Add(componentInParent) || componentInParent is Creature creature && creature.isPlayer && this.ignorePlayer || !((UnityEngine.Object) (componentInParent as Creature) == (UnityEngine.Object) null))
        ;
    }

    public static CrystalCluster Create(Vector3 position, Quaternion rotation = default (Quaternion))
    {
      return new GameObject(nameof (CrystalCluster))
      {
        transform = {
          position = position,
          rotation = rotation
        }
      }.AddComponent<CrystalCluster>();
    }

    private void OnWaterEnter() => this.Despawn();

    public void SetColor(Color color, string spellId)
    {
      ParticleSystem[] particleSystems = this.wallEffect.GetParticleSystems();
      this.lerper.SetColor(color, particleSystems, spellId, 0.01f);
    }

    public void Init(
      string spellId,
      EffectData dropEffectData,
      EffectData wallEffectData,
      float thickness,
      float height,
      float heatRadius,
      float downwardForce,
      float duration,
      bool allowXZMotion = false,
      bool drop = true)
    {
      if (drop)
      {
        this.dropEffect = dropEffectData?.Spawn(this.transform, (CollisionInstance) null, true, (ColliderGroup) null, false, 0.0f, 1f, Array.Empty<Type>());
        this.dropEffect?.Play(0, false, false);
      }
      if (wallEffectData != null)
        this.wallEffect = wallEffectData?.Spawn(this.transform, (CollisionInstance) null, false, (ColliderGroup) null, false, 0.0f, 1f, Array.Empty<Type>());
      this.SetColor(Dye.GetEvaluatedColor(this.lerper.currentSpellId, spellId), spellId);
      foreach (ThunderEntity inRadiu in ThunderEntity.InRadius(this.transform.position, thickness / 0.5f, (Func<ThunderEntity, bool>) null, (List<ThunderEntity>) null))
      {
        if (inRadiu is Creature creature && (bool) (UnityEngine.Object) creature && !creature.isPlayer)
        {
          creature.TryPush((Creature.PushType) 0, (((ThunderBehaviour) creature).transform.position - this.transform.position).normalized * 2f, 1, (RagdollPart.Type) 0);
          creature.ragdoll.SetState((Ragdoll.State) 1);
          ((ThunderEntity) creature).AddExplosionForce(50f, this.transform.position, thickness, 0.2f, (ForceMode) 1, (CollisionHandler) null);
          BrainModuleCrystal module = creature.brain.instance.GetModule<BrainModuleCrystal>(true);
          module.Crystallise(5f);
          module.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, this.lerper.currentSpellId), this.lerper.currentSpellId);
        }
      }
      this.startTime = Time.time;
      if (!drop && this.wallEffect != null)
        this.wallEffect?.Play(0, false, false);
      this.rb = this.gameObject.AddComponent<Rigidbody>();
      if (!drop)
        this.rb.isKinematic = true;
      this.Despawn(duration);
    }

    protected virtual void ManagedUpdate()
    {
      base.ManagedUpdate();
      this.wallEffect?.SetIntensity(Mathf.InverseLerp(0.0f, 1f, Time.time - this.startTime));
      this.entitiesThisFrame.Clear();
      this.waterHandler.Update(this.transform.position, this.transform.position.y, this.transform.position.y + 0.1f, 0.1f);
    }

    public void Despawn(float duration = 0.0f)
    {
      if (this.despawnRoutine != null)
        ((MonoBehaviour) this).StopCoroutine(this.despawnRoutine);
      this.despawnRoutine = ((MonoBehaviour) this).StartCoroutine(this.DespawnRoutine(duration));
      foreach (ThunderEntity inRadiu in ThunderEntity.InRadius(this.transform.position, 2f, (Func<ThunderEntity, bool>) null, (List<ThunderEntity>) null))
      {
        if ((bool) (UnityEngine.Object) inRadiu && inRadiu is Creature creature && (bool) (UnityEngine.Object) creature && !creature.isPlayer)
          creature.TryPush((Creature.PushType) 0, (((ThunderBehaviour) creature).transform.position - this.transform.position).normalized * 2f, 1, (RagdollPart.Type) 0);
      }
    }

    public IEnumerator DespawnRoutine(float duration)
    {
      CrystalCluster flameWall = this;
      yield return (object) new WaitForSeconds(duration);
      flameWall.wallEffect?.SetParent((Transform) null, false);
      flameWall.wallEffect?.End(false, -1f);
      flameWall.wallEffect?.End(false, -1f);
      UnityEngine.Object.Destroy((UnityEngine.Object) this);
      yield return (object) 0;
    }
  }
}
