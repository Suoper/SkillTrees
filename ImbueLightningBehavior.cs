// Decompiled with JetBrains decompiler
// Type: Crystallic.ImbueLightningBehavior
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using Crystallic.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Skill.Spell;
using UnityEngine;

#nullable disable
namespace Crystallic
{
  public class ImbueLightningBehavior : ImbueBehavior
  {
    public string boltEffectId = "SpellLightningBoltZapSingle";
    public AnimationCurve zapCurve = new AnimationCurve(new Keyframe[3]
    {
      new Keyframe(0.0f, 0.5f),
      new Keyframe(0.05f, 10f),
      new Keyframe(0.1f, 0.5f)
    });
    public string zapEffectId = "BoltZap";
    private readonly Vector2 boltCooldown = new Vector2(0.15f, 0.1f);
    protected EffectData boltEffectData;
    private float lastBoltTime;
    protected SpellCastCharge spellCastLightning;
    protected EffectData zapEffectData;

    public void Update()
    {
      if (!this.isActive)
        return;
      Item obj = this.imbue?.colliderGroup?.collisionHandler?.item;
      if (!(bool) (UnityEngine.Object) obj || !obj.isFlying)
        return;
      foreach (ThunderEntity inRadiu in ThunderEntity.InRadius(((ThunderBehaviour) this.imbue).transform.position, 5f, (Func<ThunderEntity, bool>) null, (List<ThunderEntity>) null))
        this.TrySpawnBolt(inRadiu);
    }

    public override void Activate(Imbue imbue, SkillCrystalImbueHandler handler)
    {
      base.Activate(imbue, handler);
      this.spellCastLightning = imbue.spellCastBase;
      this.boltEffectData = Catalog.GetData<EffectData>(this.boltEffectId, true);
      this.zapEffectData = Catalog.GetData<EffectData>(this.zapEffectId, true);
    }

    public void TrySpawnBolt(ThunderEntity thunderEntity)
    {
      if ((double) Time.time - (double) this.lastBoltTime <= (double) UnityEngine.Random.Range(this.boltCooldown.x, this.boltCooldown.y))
        return;
      this.lastBoltTime = Time.time;
      if (thunderEntity is Creature creature && (bool) (UnityEngine.Object) creature && (UnityEngine.Object) creature != (UnityEngine.Object) this.imbue.imbueCreature)
      {
        creature?.TryPush((Creature.PushType) 0, (((ThunderBehaviour) creature.ragdoll.targetPart).transform.position - this.transform.position).normalized, 1, (RagdollPart.Type) 0);
        ((ThunderEntity) creature)?.Inflict("Electrocute", (object) this, 5f, (object) null, true);
        BrainModuleCrystal module = creature.brain.instance.GetModule<BrainModuleCrystal>(true);
        module?.Crystallise(5f);
        module?.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, "Lightning"), "Lightning");
        this.SpawnBolt(this.transform, ((ThunderBehaviour) thunderEntity).transform);
      }
      else if (thunderEntity is Item obj && (bool) (UnityEngine.Object) obj && (UnityEngine.Object) obj.holder == (UnityEngine.Object) null && Utils.IsNullOrEmpty((ICollection) obj.magnets))
      {
        if ((bool) (UnityEngine.Object) obj && (bool) (UnityEngine.Object) obj?.mainHandler && (UnityEngine.Object) obj?.mainHandler?.creature != (UnityEngine.Object) null && (UnityEngine.Object) obj.mainHandler.creature != (UnityEngine.Object) this.imbue.imbueCreature)
        {
          obj?.mainHandler?.creature?.TryPush((Creature.PushType) 0, (((ThunderBehaviour) obj.mainHandler.creature.ragdoll.targetPart).transform.position - ((ThunderBehaviour) obj).transform.position).normalized, 1, (RagdollPart.Type) 0);
          ((ThunderEntity) obj?.mainHandler?.creature)?.Inflict("Electrocute", (object) this, 5f, (object) null, true);
          if (obj != null && obj.handles.Count > 0)
          {
            foreach (Handle handle in obj?.handles)
              handle.Release();
          }
        }
        foreach (ColliderGroup colliderGroup in obj.colliderGroups)
        {
          if ((bool) (UnityEngine.Object) colliderGroup && (UnityEngine.Object) colliderGroup.imbueEffectRenderer != (UnityEngine.Object) null && colliderGroup != null)
            colliderGroup.imbue?.Transfer(this.spellCastLightning, 30f * Time.deltaTime, (Creature) null);
        }
        if ((UnityEngine.Object) obj.lastHandler != (UnityEngine.Object) null && obj != null)
          obj.lastHandler?.PlayHapticClipOver(this.zapCurve, 0.25f);
        this.SpawnBolt(this.transform, ((ThunderBehaviour) thunderEntity).transform);
      }
    }

    public override void Hit(
      CollisionInstance collisionInstance,
      SpellCastCharge spellCastCharge,
      Creature hitCreature = null,
      Item hitItem = null)
    {
      base.Hit(collisionInstance, spellCastCharge, hitCreature, hitItem);
      foreach (ThunderEntity inRadiu in ThunderEntity.InRadius(collisionInstance.contactPoint, 4f, (Func<ThunderEntity, bool>) null, (List<ThunderEntity>) null))
      {
        if ((!((UnityEngine.Object) hitCreature != (UnityEngine.Object) null) || !((UnityEngine.Object) inRadiu == (UnityEngine.Object) hitCreature)) && (!((UnityEngine.Object) hitItem != (UnityEngine.Object) null) || !((UnityEngine.Object) inRadiu == (UnityEngine.Object) hitItem)))
          this.TrySpawnBolt(inRadiu);
      }
    }

    public void SpawnBolt(Transform source, Transform target)
    {
      GameObject gameObject1 = new GameObject(source.name);
      gameObject1.transform.position = source.position;
      gameObject1.transform.rotation = source.rotation;
      gameObject1.transform.parent = target;
      UnityEngine.Object.Destroy((UnityEngine.Object) gameObject1, 1f);
      GameObject gameObject2 = new GameObject(target.name);
      gameObject2.transform.position = target.position;
      gameObject2.transform.rotation = target.rotation;
      gameObject2.transform.parent = target;
      UnityEngine.Object.Destroy((UnityEngine.Object) gameObject2, 1f);
      this.zapEffectData.Spawn(target.transform, true, (ColliderGroup) null, false).Play(0, false, false);
      SpellCastLightning.PlayBolt(this.boltEffectData, gameObject1.transform, gameObject2.transform, new Vector3?(gameObject1.transform.position), new Vector3?(gameObject2.transform.position), (Gradient) null);
    }
  }
}
