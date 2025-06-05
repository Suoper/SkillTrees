// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.Lithoweb
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using System.Collections;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class Lithoweb : ThunderBehaviour
  {
    [ModOption("Lithoweb Spring", "The spring applied to the joint connecting both limbs, this is the value that decides how tightly two limbs are bound, from loosely floaty to tight.")]
    [ModOptionCategory("Lithoweb", 20)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 10000f, 0.5f)]
    public static float spring = 200f;
    [ModOption("Lithoweb Damper", "The damping applied to the joint connecting both limbs, this acts as a smoother, damping out movement to act floaty.")]
    [ModOptionCategory("Lithoweb", 20)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 10000f, 0.5f)]
    public static float damper = 30f;
    [ModOption("Min Lithoweb Distance", "The min distance two limbs can be from one another.")]
    [ModOptionCategory("Lithoweb", 20)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.1f, 100f, 0.1f)]
    public static float minDistance = 1f;
    [ModOption("Max Lithoweb Distance", "The max distance two limbs can be from one another.")]
    [ModOptionCategory("Lithoweb", 20)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.1f, 100f, 0.1f)]
    public static float maxDistance = 1.5f;
    [ModOption("Lithoweb Snap Spring", "The spring applied to the joint after the lifetime expires, limbs will be slammed together.")]
    [ModOptionCategory("Lithoweb", 20)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 10000f, 0.5f)]
    public static float snapSpring = 2000f;
    [ModOption("Lithoweb Float Drag", "The drag creatures experience while tied with a lithoweb.")]
    [ModOptionCategory("Lithoweb", 20)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 10f, 0.5f)]
    public static float floatDrag = 2f;
    [ModOption("Lithoweb Lifetime", "Controls the lifetime of lithowebs")]
    [ModOptionCategory("Lithoweb", 20)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 100f, 0.5f)]
    public static float lifetime = 2.5f;
    [ModOption("Lithoweb Slam Force Multiplier", "Controls how hard limbs will be thrown into the ground upon expiring.")]
    [ModOptionCategory("Lithoweb", 20)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 100f, 0.5f)]
    public static float slamForceMult = 50f;
    private Creature creature;
    public ConfigurableJoint joint;
    public StatusData statusData;
    public EffectData tetherEffectData;
    public EffectData slamEffectData;
    public EffectInstance tetherEffectInstance;
    public string tetherEffectId = "GravityTether";
    public Item slicer;
    public RagdollPart source;
    public RagdollPart target;
    public EffectData snapEffectData;
    public string snapEffectId = "GravitySnap";

    public void Init(
      Item slicer,
      RagdollPart source,
      RagdollPart target,
      bool overrideJointDefaults = false,
      float overrideSpring = 0.0f,
      float overrideDamper = 0.0f,
      float overrideMinDistance = 0.0f,
      float overrideMaxDistance = 0.0f,
      float overrideMassScale = 0.0f,
      float overrideLifetime = 0.0f)
    {
      this.creature = source.ragdoll.creature;
      // ISSUE: method pointer
      this.creature.OnDespawnEvent += new Creature.DespawnEvent((object) this, __methodptr(OnDespawnEvent));
      this.statusData = Catalog.GetData<StatusData>("Floating", true);
      this.tetherEffectData = Catalog.GetData<EffectData>(this.tetherEffectId, true);
      this.slamEffectData = Catalog.GetData<EffectData>("SpellGravityPush", true);
      this.snapEffectData = Catalog.GetData<EffectData>(this.snapEffectId, true);
      this.slicer = slicer;
      this.source = source;
      this.target = target;
      ((ThunderEntity) this.creature).Inflict(this.statusData, (object) this, float.PositiveInfinity, (object) new FloatingParams(1f, Lithoweb.floatDrag, 1f, true), true);
      if (this.tetherEffectInstance != null)
        this.tetherEffectInstance.End(false, -1f);
      this.tetherEffectInstance = this.tetherEffectData.Spawn(((Component) source.physicBody.rigidBody).gameObject.transform, true, (ColliderGroup) null, false);
      this.tetherEffectInstance.SetSource(((Component) source.physicBody.rigidBody).gameObject.transform);
      this.tetherEffectInstance.SetTarget(((Component) target.physicBody.rigidBody).gameObject.transform);
      this.tetherEffectInstance.Play(0, false, false);
      this.joint = !overrideJointDefaults ? Utils.CreateConfigurableJoint(source?.physicBody.rigidBody, target?.physicBody.rigidBody, Lithoweb.spring, Lithoweb.damper, Lithoweb.minDistance, Lithoweb.maxDistance, 0.1f) : Utils.CreateConfigurableJoint(source?.physicBody.rigidBody, target?.physicBody.rigidBody, overrideSpring, overrideDamper, overrideMinDistance, overrideMaxDistance, overrideMassScale);
      if (!((UnityEngine.Object) source != (UnityEngine.Object) null) || !((UnityEngine.Object) target != (UnityEngine.Object) null))
        return;
      ((MonoBehaviour) this).StartCoroutine(this.AutoExpireRoutine(!overrideJointDefaults ? Lithoweb.lifetime : overrideLifetime));
    }

    private void OnDespawnEvent(EventTime eventTime)
    {
      if (eventTime != 0)
        return;
      // ISSUE: method pointer
      this.creature.OnDespawnEvent -= new Creature.DespawnEvent((object) this, __methodptr(OnDespawnEvent));
      this.TryDeactivate();
    }

    public IEnumerator AutoExpireRoutine(float autoExpireTime)
    {
      yield return (object) Yielders.ForSeconds(autoExpireTime);
      ((ThunderEntity) this.creature).Remove(this.statusData, (object) this);
      yield return (object) Yielders.ForSeconds(1.75f);
      this.source.physicBody.AddForce(Vector3.down * Lithoweb.slamForceMult, (ForceMode) 1);
      this.slamEffectData.Spawn(((Component) this.source.physicBody.rigidBody).gameObject.transform.position + new Vector3(0.0f, 1f, 0.0f), Quaternion.LookRotation(Vector3.down), (Transform) null, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>()).Play(0, false, false);
      this.snapEffectData.Spawn(((Component) this.joint).transform, true, (ColliderGroup) null, false).Play(0, false, false);
      this.TryDeactivate();
    }

    public void TryDeactivate()
    {
      if ((bool) (UnityEngine.Object) this.joint)
      {
        ((Joint) this.joint).connectedBody = (Rigidbody) null;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.joint);
      }
      this.tetherEffectInstance?.End(false, -1f);
      ((ThunderEntity) this.creature).Remove(this.statusData, (object) this);
      UnityEngine.Object.Destroy((UnityEngine.Object) this);
    }
  }
}
