// Decompiled with JetBrains decompiler
// Type: Arcana.Behaviors.FireSpear
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Behaviors
{
  public class FireSpear : ThunderBehaviour
  {
    public float speed = 24f;
    public float duration = 5f;
    public float penetrationDepthRequirementRatio = 0.6f;
    public Vector3 initialVelocity;
    public float aliveTime;
    private bool isThrown = false;
    public Item item;
    public ItemModuleFireSpear module;
    public SpellCastCharge imbueSpell;
    public EffectInstance effect;

    public virtual ManagedLoops EnabledManagedLoops => (ManagedLoops) 1;

    protected void Awake()
    {
      this.item = ((Component) this).GetComponent<Item>();
      this.imbueSpell = Catalog.GetData<SpellCastCharge>("Fire", true);
      this.module = ((Component) this.item).GetComponent<ItemModuleFireSpear>();
    }

    protected virtual void Start()
    {
      foreach (Damager damager in this.item.mainCollisionHandler.damagers.Where<Damager>((Func<Damager, bool>) (damager => damager.type == 1)))
      {
        MaxDepthDetector maxDepthDetector = this.gameObject.AddComponent<MaxDepthDetector>();
        maxDepthDetector.Activate(damager, this.penetrationDepthRequirementRatio);
        maxDepthDetector.OnPenetrateMaxDepthEvent -= new MaxDepthDetector.OnPenetrateMaxDepth(this.OnPenetrationRequirement);
        maxDepthDetector.OnPenetrateMaxDepthEvent += new MaxDepthDetector.OnPenetrateMaxDepth(this.OnPenetrationRequirement);
      }
    }

    public virtual void Fire(Vector3 direction, EffectData effectData)
    {
      this.item.SetColliders(true, false);
      this.item.physicBody.isKinematic = false;
      this.item.physicBody.useGravity = false;
      this.item.mainCollisionHandler.active = true;
      this.initialVelocity = direction.normalized * this.speed;
      this.item.physicBody.velocity = this.initialVelocity;
      ((ThunderBehaviour) this.item).transform.rotation = Quaternion.LookRotation(direction);
      this.effect = effectData.Spawn(this.transform, true, (ColliderGroup) null, false);
      this.effect?.Play(0, false, false);
      this.item.Throw(1f, (Item.FlyDetection) 2);
      this.aliveTime = Time.time;
      this.isThrown = true;
    }

    protected virtual void ManagedFixedUpdate()
    {
      base.ManagedFixedUpdate();
      if (this.item == null)
        return;
      if (this.isThrown && (double) Time.time - (double) this.aliveTime > (double) this.aliveTime)
      {
        this.item.physicBody.velocity = this.initialVelocity;
        ((ThunderBehaviour) this.item).transform.rotation = Quaternion.LookRotation(this.initialVelocity);
      }
      if ((double) Time.time - (double) this.aliveTime <= (double) this.duration)
        return;
      this.Despawn();
    }

    private void OnPenetrationRequirement(
      Damager damager,
      CollisionInstance collision,
      Vector3 velocity,
      float depth)
    {
      CollisionHandler collisionHandler = collision?.targetColliderGroup?.collisionHandler;
      if ((UnityEngine.Object) collisionHandler == (UnityEngine.Object) null || collisionHandler.isBreakable || collisionHandler.isItem || collisionHandler.isRagdollPart)
        return;
      this.isThrown = false;
      this.item.physicBody.useGravity = true;
    }

    public void Despawn()
    {
      if (this.effect != null && this.effect.isPlaying)
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        FireSpear.\u003C\u003Ec__DisplayClass17_0 cDisplayClass170 = new FireSpear.\u003C\u003Ec__DisplayClass17_0()
        {
          \u003C\u003E4__this = this,
          handler = (EffectInstance.EffectFinishEvent) null
        };
        // ISSUE: reference to a compiler-generated field
        // ISSUE: method pointer
        cDisplayClass170.handler = new EffectInstance.EffectFinishEvent((object) cDisplayClass170, __methodptr(\u003CDespawn\u003Eb__0));
        // ISSUE: reference to a compiler-generated field
        this.effect.onEffectFinished -= cDisplayClass170.handler;
        // ISSUE: reference to a compiler-generated field
        this.effect.onEffectFinished += cDisplayClass170.handler;
      }
      this.effect?.End(false, -1f);
    }

    private void DespawnItem()
    {
      foreach (RagdollHand handler in this.item.handlers)
        handler.TryRelease();
      foreach (UnityEngine.Object @object in ((IEnumerable<MaxDepthDetector>) ((Component) this).GetComponents<MaxDepthDetector>()).ToArray<MaxDepthDetector>())
        UnityEngine.Object.Destroy(@object);
      this.imbueSpell = (SpellCastCharge) null;
      ((ThunderEntity) this.item).Despawn();
      this.item = (Item) null;
    }

    private void OnDestroy() => this.Despawn();
  }
}
