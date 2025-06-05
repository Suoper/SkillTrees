// Decompiled with JetBrains decompiler
// Type: Crystallic.Stinger
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using Crystallic.Skill;
using Crystallic.Skill.Spell;
using System;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic
{
  public class Stinger : ThunderBehaviour
  {
    public static HashSet<Stinger> all = new HashSet<Stinger>();
    public static string projectileItemId = "CrystallicProjectile";
    private static ItemData itemData;
    public Item item;
    public Creature creature;
    public EffectData hitEffectData;
    public Lerper lerper;
    private EffectInstance projectileEffect;
    private EffectInstance projectileTrailEffect;
    public SpellCastCrystallic spellCastCrystallic;

    public static event Stinger.StingerEvent onStingerSpawn;

    public event Stinger.OnStingerStab onStingerStab;

    public static Stinger SpawnStinger(
      EffectData effectData,
      EffectData trailEffectData,
      EffectData hitEffectData,
      Vector3 position,
      Quaternion rotation,
      Vector3 velocity,
      float lifetime,
      SpellCastCrystallic spellCastCrystallic,
      Creature owner = null,
      bool forceReleaseOnSpawn = false)
    {
      Stinger stinger = (Stinger) null;
      if (Stinger.itemData == null)
        Stinger.itemData = Catalog.GetData<ItemData>(Stinger.projectileItemId, true);
      Stinger.itemData.SpawnAsync((Action<Item>) (item =>
      {
        item.Despawn(10f);
        if ((bool) (UnityEngine.Object) owner && !owner.isPlayer)
        {
          Vector3 normalized = (((ThunderBehaviour) Player.currentCreature.ragdoll.targetPart).transform.position - ((ThunderBehaviour) item).transform.position).normalized;
          ((ThunderEntity) item).AddForce(normalized * velocity.magnitude * 3f, (ForceMode) 1, (CollisionHandler) null);
        }
        else if ((UnityEngine.Object) owner == (UnityEngine.Object) null)
          owner = Player.currentCreature;
        ((ThunderEntity) item).AddForce(((ThunderBehaviour) item).transform.forward * (velocity.magnitude * 2.15f), (ForceMode) 1, (CollisionHandler) null);
        Extensions.GetOrAddComponent<Stinger>(((ThunderBehaviour) item).gameObject).Init(item, effectData, trailEffectData, hitEffectData, owner, spellCastCrystallic, forceReleaseOnSpawn);
      }), new Vector3?(position), new Quaternion?(rotation), (Transform) null, true, (List<ContentCustomData>) null, (Item.Owner) 0);
      return stinger;
    }

    public void Init(
      Item item,
      EffectData effectData,
      EffectData trailEffectData,
      EffectData hitEffectData,
      Creature owner,
      SpellCastCrystallic spellCastCrystallic,
      bool forceReleaseOnSpawn = false)
    {
      Stinger.all.Add(this);
      this.lerper = new Lerper();
      // ISSUE: method pointer
      item.OnDespawnEvent += new Item.SpawnEvent((object) this, __methodptr(OnDespawnEvent));
      this.SetColor(Color.white, "Crystallic");
      this.item = item;
      this.creature = owner;
      this.spellCastCrystallic = spellCastCrystallic;
      // ISSUE: method pointer
      ((Component) item).GetComponentInChildren<Damager>().OnPenetrateEvent += new Damager.PenetrationEvent((object) this, __methodptr(OnPenetrateEvent));
      this.projectileEffect = effectData?.Spawn(((ThunderBehaviour) item).transform, true, (ColliderGroup) null, false);
      this.projectileTrailEffect = trailEffectData?.Spawn(((ThunderBehaviour) item).transform, true, (ColliderGroup) null, false);
      this.hitEffectData = hitEffectData;
      this.projectileEffect?.Play(0, false, false);
      this.projectileTrailEffect?.Play(0, false, false);
      Stinger.StingerEvent onStingerSpawn = Stinger.onStingerSpawn;
      if (onStingerSpawn != null)
        onStingerSpawn(this);
      if (!forceReleaseOnSpawn)
        return;
      SkillHyperintensity.ForceInvokeRelease((SpellCastCrystallic) null);
    }

    private void OnDespawnEvent(EventTime eventTime)
    {
      if (eventTime != 0)
        return;
      Stinger.all.Remove(this);
    }

    private void OnPenetrateEvent(Damager damager, CollisionInstance collision, EventTime time)
    {
      if (time == 0)
        return;
      Item item = collision?.sourceColliderGroup?.collisionHandler?.item;
      Item obj = collision?.targetColliderGroup?.collisionHandler?.item;
      Creature entity = collision?.targetColliderGroup?.collisionHandler?.Entity as Creature;
      this.projectileTrailEffect.End(false, -1f);
      if (!(bool) (UnityEngine.Object) item)
        return;
      // ISSUE: method pointer
      damager.OnPenetrateEvent -= new Damager.PenetrationEvent((object) this, __methodptr(OnPenetrateEvent));
      Stinger.OnStingerStab onStingerStab = this.onStingerStab;
      if (onStingerStab != null)
        onStingerStab(this, damager, collision, entity);
      if (!(bool) (UnityEngine.Object) obj && !(bool) (UnityEngine.Object) entity)
        Utils.RunAfter((MonoBehaviour) this, (Action) (() => item.physicBody.rigidBody.isKinematic = true), 0.05f, false);
      if ((double) collision.impactVelocity.magnitude > 3.5)
      {
        EffectInstance effectInstance = this.hitEffectData?.Spawn(collision.contactPoint, Quaternion.LookRotation(collision.contactNormal, ((ThunderBehaviour) collision.sourceColliderGroup).transform.up), ((Component) collision.targetCollider).transform, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
        effectInstance.Play(0, false, false);
        effectInstance.SetColorImmediate(this.lerper.currentColor);
      }
      if ((bool) (UnityEngine.Object) entity && (UnityEngine.Object) entity != (UnityEngine.Object) this.creature && !collision.targetMaterial.isMetal)
      {
        BrainModuleCrystal module = entity.brain.instance.GetModule<BrainModuleCrystal>(true);
        if (!entity.isPlayer)
          ((ThunderEntity) entity).AddExplosionForce(60f, collision.contactPoint, 3f, 0.1f, (ForceMode) 1, (CollisionHandler) null);
        module.Crystallise(5f);
        module.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, this.lerper.currentSpellId), this.lerper.currentSpellId);
      }
      GolemBrainModuleCrystal brainModule;
      if (!((UnityEngine.Object) ((Component) collision.targetCollider).GetComponentInParent<GolemBrain>() != (UnityEngine.Object) null) || !((GolemController) Golem.local).Brain().TryGetModule<GolemBrainModuleCrystal>(out brainModule))
        return;
      brainModule.Crystallise(5f);
    }

    public void SetColor(Color color, string target, float time = 0.1f)
    {
      if (this.lerper.isLerping)
        return;
      List<ParticleSystem> particleSystemList = new List<ParticleSystem>();
      particleSystemList.AddRange((IEnumerable<ParticleSystem>) this.projectileEffect.GetParticleSystems());
      particleSystemList.AddRange((IEnumerable<ParticleSystem>) this.projectileTrailEffect.GetParticleSystems());
      this.lerper.SetColor(color, particleSystemList.ToArray(), target, time);
    }

    public delegate void OnStingerStab(
      Stinger stinger,
      Damager damager,
      CollisionInstance collisionInstance,
      Creature hitCreature = null);

    public delegate void StingerEvent(Stinger stinger);
  }
}
