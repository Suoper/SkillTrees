// Decompiled with JetBrains decompiler
// Type: Crystallic.ImbueGravityBehavior
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.Skill;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using ThunderRoad.Skill.Spell;
using UnityEngine;

#nullable disable
namespace Crystallic
{
  public class ImbueGravityBehavior : ImbueBehavior
  {
    [ModOption("Joint Spring", "The spring applied to the joint connecting two physicBodies, this is the value that decides how tightly two limbs are bound, from loosely floaty to tight.")]
    [ModOptionCategory("Lithohammer", 23)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 10000f, 0.5f)]
    public static float spring = 550f;
    [ModOption("Joint Damper", "The damping applied to the joint connecting two physicBodies, this acts as a smoother, damping out movement to act floaty.")]
    [ModOptionCategory("Lithohammer", 23)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 10000f, 0.5f)]
    public static float damper = 30f;
    [ModOption("Min Joint Distance", "The min distance two physicBodies can be from one another.")]
    [ModOptionCategory("Lithohammer", 23)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.1f, 100f, 0.1f)]
    public static float minDistance = 1f;
    [ModOption("Max Joint Distance", "The max distance two physicBodies can be from one another.")]
    [ModOptionCategory("Lithohammer", 23)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.1f, 100f, 0.1f)]
    public static float maxDistance = 15f;
    [ModOption("Joint Lifetime", "The lifetime of each joint.")]
    [ModOptionCategory("Lithohammer", 23)]
    [ModOptionSlider]
    [ModOptionFloatValues(0.1f, 100f, 0.1f)]
    public static float lifetime = 3f;
    public StatusData statusData;
    public EffectData tetherEffectData;
    public EffectData snapEffectData;
    public string snapEffectId = "GravitySnap";
    public SpellCastGravity spellCastGravity;
    public string tetherEffectId = "GravityTether";
    public Dictionary<Creature, JointEffect> jointedBodies = new Dictionary<Creature, JointEffect>();

    public override void Activate(Imbue imbue, SkillCrystalImbueHandler handler)
    {
      base.Activate(imbue, handler);
      // ISSUE: method pointer
      EventManager.onCreatureDespawn += new EventManager.CreatureDespawnedEvent((object) this, __methodptr(OnCreatureDespawn));
      this.tetherEffectData = Catalog.GetData<EffectData>(this.tetherEffectId, true);
      this.snapEffectData = Catalog.GetData<EffectData>(this.snapEffectId, true);
      this.statusData = Catalog.GetData<StatusData>("Floating", true);
      this.spellCastGravity = Catalog.GetData<SpellCastCharge>("Gravity", true) as SpellCastGravity;
    }

    private void OnCreatureDespawn(Creature creature, EventTime eventTime)
    {
      if (!this.jointedBodies.ContainsKey(creature))
        return;
      Object.Destroy((Object) this.jointedBodies[creature].configurableJoint);
      this.jointedBodies[creature].effectInstance.End(false, -1f);
      this.jointedBodies.Remove(creature);
    }

    public override void Hit(
      CollisionInstance collisionInstance,
      SpellCastCharge spellCastCharge,
      Creature hitCreature = null,
      Item hitItem = null)
    {
      base.Hit(collisionInstance, spellCastCharge, hitCreature, hitItem);
      Item source = collisionInstance?.sourceColliderGroup?.collisionHandler?.item;
      RagdollPart ragdollPart = collisionInstance?.targetColliderGroup?.collisionHandler?.ragdollPart;
      if (!(bool) (Object) ragdollPart || !(bool) (Object) source || ragdollPart.ragdoll.creature.isPlayer || (double) collisionInstance.impactVelocity.magnitude <= 18.0)
        return;
      this.TryCreateJoint(collisionInstance, source, ragdollPart);
    }

    public IEnumerator JointExpirationRoutine(Item source, RagdollPart ragdollPart)
    {
      JointEffect jointEffect = this.jointedBodies[ragdollPart.ragdoll.creature];
      yield return (object) Yielders.ForSeconds(1f);
      float startTime = Time.time;
      bool velocityMet = false;
      while ((double) Time.time - (double) startTime < 2.0)
      {
        if ((double) source.physicBody.velocity.magnitude < 12.5)
        {
          velocityMet = true;
          break;
        }
        yield return (object) Yielders.EndOfFrame;
      }
      if (velocityMet)
      {
        RagdollPart currentPart = ragdollPart;
        while ((Object) currentPart != (Object) null)
        {
          if (currentPart.sliceAllowed)
          {
            currentPart?.TrySlice();
            currentPart.ragdoll.creature.Kill();
            yield return (object) Yielders.ForSeconds(ImbueGravityBehavior.lifetime);
            break;
          }
          currentPart = currentPart.parentPart;
          yield return (object) Yielders.EndOfFrame;
        }
        currentPart = (RagdollPart) null;
      }
      if (jointEffect != null && (Object) jointEffect.configurableJoint != (Object) null)
      {
        this.snapEffectData?.Spawn(((Component) jointEffect?.configurableJoint)?.transform, true, (ColliderGroup) null, false).Play(0, false, false);
        jointEffect?.effectInstance.End(false, -1f);
        ((SpellCastCharge) this.spellCastGravity).readyEffectData.Spawn(((Component) jointEffect?.configurableJoint).transform, true, (ColliderGroup) null, false).Play(0, false, false);
        ((Joint) jointEffect.configurableJoint).breakForce = 0.0f;
        Object.Destroy((Object) jointEffect.configurableJoint);
        this.jointedBodies.Remove(ragdollPart.ragdoll.creature);
      }
    }

    public void TryCreateJoint(
      CollisionInstance collisionInstance,
      Item source,
      RagdollPart target)
    {
      if (this.jointedBodies.ContainsKey(target.ragdoll.creature))
        return;
      EffectInstance effectInstance = this.tetherEffectData.Spawn(((Component) collisionInstance.sourceCollider).transform, true, (ColliderGroup) null, false);
      effectInstance.SetSource(((Component) collisionInstance.sourceCollider).transform);
      effectInstance.SetTarget(((ThunderBehaviour) target).transform);
      effectInstance.Play(0, false, false);
      ConfigurableJoint configurableJoint = Utils.CreateConfigurableJoint(source.physicBody.rigidBody, target?.physicBody.rigidBody, ImbueGravityBehavior.spring, ImbueGravityBehavior.damper, ImbueGravityBehavior.minDistance, ImbueGravityBehavior.maxDistance, 1f);
      this.jointedBodies.Add(target.ragdoll.creature, new JointEffect(effectInstance, configurableJoint));
      ((ThunderEntity) target.ragdoll.creature).Remove(this.statusData, (object) this);
      ((MonoBehaviour) this).StartCoroutine(this.JointExpirationRoutine(source, target));
    }

    public override void Deactivate()
    {
      base.Deactivate();
      foreach (KeyValuePair<Creature, JointEffect> keyValuePair in this.jointedBodies.ToList<KeyValuePair<Creature, JointEffect>>())
      {
        Creature key = keyValuePair.Key;
        JointEffect jointEffect = keyValuePair.Value;
        if (jointEffect != null && jointEffect.effectInstance != null)
          jointEffect.effectInstance.End(false, -1f);
        if ((Object) jointEffect?.configurableJoint != (Object) null)
        {
          ((Joint) jointEffect.configurableJoint).breakForce = 0.0f;
          Object.Destroy((Object) jointEffect.configurableJoint);
        }
        this.jointedBodies.Remove(key);
      }
    }
  }
}
