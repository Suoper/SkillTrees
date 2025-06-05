// Decompiled with JetBrains decompiler
// Type: Crystallic.GolemLithoweb
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic
{
  public class GolemLithoweb : GolemAbility
  {
    public EffectData tetherEffectData;
    public EffectData snapEffectData;
    public string snapEffectId = "GravitySnap";
    public string tetherEffectId = "GravityTether";
    public GolemController.AttackMotion attackMotion = (GolemController.AttackMotion) 19;
    public EffectInstance tetherEffectInstance;
    public ConfigurableJoint joint;

    public virtual void Begin(GolemController golem)
    {
      base.Begin(golem);
      this.tetherEffectData = Catalog.GetData<EffectData>(this.tetherEffectId, true);
      this.snapEffectData = Catalog.GetData<EffectData>(this.snapEffectId, true);
      golem.PerformAttackMotion(this.attackMotion, (Action) null);
    }

    public virtual void OnUpdate()
    {
      base.OnUpdate();
      if ((double) (this.golem.magicSprayPoints[0].transform.position - ((ThunderBehaviour) Player.currentCreature.ragdoll.targetPart).transform.position).sqrMagnitude <= 6.25 || this.tetherEffectInstance != null)
        return;
      this.tetherEffectInstance = this.tetherEffectData.Spawn(this.golem.magicSprayPoints[0], true, (ColliderGroup) null, false);
      this.tetherEffectInstance.SetSourceAndTarget(this.golem.magicSprayPoints[0], ((ThunderBehaviour) Player.currentCreature.ragdoll.targetPart).transform);
      this.tetherEffectInstance.Play(0, false, false);
      Rigidbody source = new GameObject("MainBody").AddComponent<Rigidbody>();
      source.isKinematic = true;
      source.useGravity = false;
      ((Component) source).transform.parent = this.golem.magicSprayPoints[0].transform;
      ((Component) source).transform.position = this.golem.magicSprayPoints[0].transform.position;
      ((Component) source).transform.rotation = this.golem.magicSprayPoints[0].transform.rotation;
      ((ThunderEntity) Player.currentCreature).Inflict("Floating", (object) this, float.PositiveInfinity, (object) null, true);
      this.joint = Utils.CreateConfigurableJoint(source, Extensions.GetPhysicBody((Component) Player.local).rigidBody, 5000f, 35f, 2.5f, 5f, 0.1f, motion: (ConfigurableJointMotion) 1);
      Utils.RunAfter((MonoBehaviour) Player.currentCreature, (Action) (() =>
      {
        ((Joint) this.joint).connectedBody = (Rigidbody) null;
        this.tetherEffectInstance.End(false, -1f);
        ((ThunderEntity) Player.currentCreature).Remove("Floating", (object) this);
        UnityEngine.Object.Destroy((UnityEngine.Object) this.joint);
      }), 5f, false);
    }
  }
}
