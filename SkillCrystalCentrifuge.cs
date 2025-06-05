// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillCrystalCentrifuge
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillCrystalCentrifuge : SkillData
  {
    public List<SkillSpellPair> skillSpellPairs;
    public AnimationCurve fullyChargedCurve;
    public string overchargedEffectId;
    public EffectData overchargedEffectData;
    public string overchargeEffectId;
    public EffectData overchargeEffectData;
    public Dictionary<Item, Coroutine> runningRoutines = new Dictionary<Item, Coroutine>();
    public Dictionary<Item, EffectInstance> runningEffectInstances = new Dictionary<Item, EffectInstance>();

    public virtual void OnCatalogRefresh()
    {
      base.OnCatalogRefresh();
      this.overchargedEffectData = Catalog.GetData<EffectData>(this.overchargedEffectId, true);
      this.overchargeEffectData = Catalog.GetData<EffectData>(this.overchargeEffectId, true);
    }

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      Item.OnItemSpawn -= new Action<Item>(this.OnItemSpawn);
      Item.OnItemSpawn += new Action<Item>(this.OnItemSpawn);
      Item.OnItemDespawn -= new Action<Item>(this.OnItemDespawn);
      Item.OnItemDespawn += new Action<Item>(this.OnItemDespawn);
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      Item.OnItemSpawn -= new Action<Item>(this.OnItemSpawn);
      Item.OnItemDespawn -= new Action<Item>(this.OnItemDespawn);
    }

    private void OnItemSpawn(Item item)
    {
      // ISSUE: method pointer
      item.OnTKSpinStart += new Item.TelekinesisSpinEvent((object) this, __methodptr(OnTKSpinStart));
      // ISSUE: method pointer
      item.OnTKSpinEnd += new Item.TelekinesisSpinEvent((object) this, __methodptr(OnTKSpinEnd));
    }

    private void OnItemDespawn(Item item)
    {
      // ISSUE: method pointer
      item.OnTKSpinStart -= new Item.TelekinesisSpinEvent((object) this, __methodptr(OnTKSpinStart));
      // ISSUE: method pointer
      item.OnTKSpinEnd -= new Item.TelekinesisSpinEvent((object) this, __methodptr(OnTKSpinEnd));
    }

    private void OnTKSpinStart(Handle held, bool spinning, EventTime eventTime)
    {
      if (eventTime == null || !spinning)
        return;
      for (int i = 0; i < this.skillSpellPairs.Count; i++)
      {
        if (held.item.imbues.HasImbue(this.skillSpellPairs[i].spellId) && (Player.currentCreature.HasSkill(this.skillSpellPairs[i].skillId) || this.skillSpellPairs[i].skillId == null))
        {
          Imbue imbue = held.item.imbues.First<Imbue>((Func<Imbue, bool>) (imbue1 => ((CatalogData) imbue1.spellCastBase).id == this.skillSpellPairs[i].spellId));
          this.runningRoutines.Add(held.item, ((MonoBehaviour) held.item).StartCoroutine(this.SpinRoutine(held.item, imbue)));
          ((ThunderEntity) held.item).SetVariable<bool>("Spinning", true);
        }
      }
    }

    private void OnTKSpinEnd(Handle held, bool spinning, EventTime eventTime)
    {
      if (eventTime == 0 | spinning)
        return;
      if (this.runningRoutines.ContainsKey(held.item) && this.runningRoutines[held.item] != null)
      {
        ((MonoBehaviour) held.item).StopCoroutine(this.runningRoutines[held.item]);
        this.runningRoutines.Remove(held.item);
      }
      ((ThunderEntity) held.item).SetVariable<bool>("Spinning", false);
    }

    public IEnumerator SpinRoutine(Item item, Imbue imbue)
    {
      while (((ThunderEntity) item).GetVariable<bool>("Spinning"))
      {
        float currentCharge = ((ThunderEntity) item).GetVariable<float>("SpinCharge");
        if ((double) currentCharge >= 1.0)
        {
          yield break;
        }
        else
        {
          foreach (SpellCaster spellCaster in item.tkHandlers)
            spellCaster.ragdollHand.playerHand.controlHand.HapticLoop((object) this, currentCharge, 0.01f);
          double num = (double) ((ThunderEntity) item).SetVariable<float>("SpinCharge", currentCharge + Time.deltaTime / 6.5f);
          yield return (object) Yielders.EndOfFrame;
        }
      }
      foreach (SpellCaster spellCaster in item.tkHandlers)
        spellCaster.ragdollHand.PlayHapticClipOver(this.fullyChargedCurve, 0.15f);
      this.overchargeEffectData.Spawn(((ThunderEntity) item).Bounds.center, Quaternion.identity, (Transform) null, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>()).Play(0, false, false);
      EffectInstance instance = this.overchargedEffectData.Spawn(imbue.colliderGroup.imbueEffectRenderer.transform, true, (ColliderGroup) null, false);
      instance.SetRenderer(imbue.colliderGroup.imbueEffectRenderer, false);
      instance.Play(0, false, false);
      Damager[] damagers = ((Component) item).GetComponentsInChildren<Damager>();
      for (int i = 0; i < damagers.Length; ++i)
      {
        if ((double) damagers[i].penetrationDepth > 0.0)
        {
          // ISSUE: method pointer
          damagers[i].OnPenetrateEvent += new Damager.PenetrationEvent((object) this, __methodptr(OnPenetrateEvent));
        }
      }
      yield return (object) Yielders.ForSeconds(5f);
      for (int i = 0; i < damagers.Length; ++i)
      {
        if ((double) damagers[i].penetrationDepth > 0.0)
        {
          // ISSUE: method pointer
          damagers[i].OnPenetrateEvent -= new Damager.PenetrationEvent((object) this, __methodptr(OnPenetrateEvent));
        }
      }
      instance.End(false, -1f);
    }

    private void OnPenetrateEvent(Damager damager, CollisionInstance collision, EventTime time)
    {
      if (!(collision?.targetColliderGroup?.collisionHandler?.Entity is Creature entity))
        return;
      Color currentColor = entity.brain.instance.GetModule<BrainModuleCrystal>(true).lerper.currentColor;
      SkillOverchargedCore.Detonate(entity, currentColor);
    }
  }
}
