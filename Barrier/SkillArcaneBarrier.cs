// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.Barrier.SkillArcaneBarrier
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Spells;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Arcana.Skills.Barrier
{
  internal class SkillArcaneBarrier : SpellSkillData
  {
    public string barrierItemId;
    public string barrierEffectId;
    public ItemData barrierItemData;
    public EffectData barrierEffectData;
    public float barrierDistance = 0.25f;
    public float barrierScale = 0.8f;
    public HashSet<SkillArcaneBarrier.Barrier> barriers;

    public Vector3 GetBarrierPosition(SpellCastCharge spell)
    {
      return spell.spellCaster.Orb.position + spell.spellCaster.fire.forward * this.barrierDistance;
    }

    public Quaternion GetBarrierRotation(SpellCastCharge spell)
    {
      return Quaternion.LookRotation(spell.spellCaster.fire.forward.normalized, Vector3.up);
    }

    public event SkillArcaneBarrier.BarrierEvent OnBarrierStartEvent;

    public event SkillArcaneBarrier.BarrierEvent OnBarrierUpdateEvent;

    public event SkillArcaneBarrier.BarrierEvent OnBarrierStopEvent;

    public event SkillArcaneBarrier.BarrierGripEvent OnBarrierGripEvent;

    public event SkillArcaneBarrier.BarrierCollisionEvent OnBarrierHitEvent;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.barrierItemData = Catalog.GetData<ItemData>(this.barrierItemId, true);
      this.barrierEffectData = Catalog.GetData<EffectData>(this.barrierEffectId, true);
    }

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      switch (spell)
      {
        case ArcaneBolt arcaneBolt:
          // ISSUE: method pointer
          caster.ragdollHand.playerHand.OnFistEvent -= new PlayerHand.FistEvent((object) this, __methodptr(OnFist));
          // ISSUE: method pointer
          caster.ragdollHand.playerHand.OnFistEvent += new PlayerHand.FistEvent((object) this, __methodptr(OnFist));
          // ISSUE: method pointer
          arcaneBolt.OnSpellFixedUpdateEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnFixedCasterUpdate));
          // ISSUE: method pointer
          arcaneBolt.OnSpellFixedUpdateEvent += new SpellCastCharge.SpellEvent((object) this, __methodptr(OnFixedCasterUpdate));
          break;
        case SpellMergeData spellMergeData:
          // ISSUE: method pointer
          spellMergeData.OnMergeStartEvent -= new SpellMergeData.MergeEvent((object) this, __methodptr(OnMergeStart));
          // ISSUE: method pointer
          spellMergeData.OnMergeStartEvent += new SpellMergeData.MergeEvent((object) this, __methodptr(OnMergeStart));
          break;
      }
      this.barriers = new HashSet<SkillArcaneBarrier.Barrier>();
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      SpellData spellData = spell;
      if (!(spellData is ArcaneBolt arcaneBolt))
      {
        if (spellData is SpellMergeData spellMergeData)
        {
          // ISSUE: method pointer
          spellMergeData.OnMergeStartEvent -= new SpellMergeData.MergeEvent((object) this, __methodptr(OnMergeStart));
        }
      }
      else
      {
        // ISSUE: method pointer
        caster.ragdollHand.playerHand.OnFistEvent -= new PlayerHand.FistEvent((object) this, __methodptr(OnFist));
        // ISSUE: method pointer
        arcaneBolt.OnSpellFixedUpdateEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnFixedCasterUpdate));
      }
      if (this.barriers == null)
        return;
      foreach (SkillArcaneBarrier.Barrier barrier in this.barriers.ToList<SkillArcaneBarrier.Barrier>())
        barrier.Despawn();
      this.barriers.Clear();
    }

    private void OnFist(PlayerHand hand, bool gripping)
    {
      SpellCaster caster = hand?.ragdollHand?.caster;
      ArcaneBolt spellInstance;
      int num;
      if (!((UnityEngine.Object) caster == (UnityEngine.Object) null))
      {
        spellInstance = caster?.spellInstance as ArcaneBolt;
        num = spellInstance == null ? 1 : 0;
      }
      else
        num = 1;
      if (num != 0)
        return;
      if (spellInstance.Ready)
      {
        spellInstance.endOnGrip = false;
        spellInstance.allowThrow = false;
        this.CreateBarrier((SpellCastCharge) spellInstance);
      }
      else
      {
        if (gripping)
          return;
        spellInstance.endOnGrip = true;
        spellInstance.allowThrow = true;
        this.RemoveBarrier((SpellCastCharge) spellInstance);
      }
    }

    private void OnFixedCasterUpdate(SpellCastCharge spell)
    {
      int num = !spell.spellCaster.isFiring ? 0 : (spell.Ready ? 1 : 0);
      spell.endOnGrip = num == 0;
      this.UpdateBarrier(spell);
    }

    private void OnMergeStart(SpellMergeData mergeData)
    {
      Debug.Log((object) "Removing Barriers on Merge!");
      if (mergeData?.mana?.casterLeft?.spellInstance != null)
        this.RemoveBarrier((SpellCastCharge) (mergeData.mana.casterLeft.spellInstance as ArcaneBolt));
      if (mergeData?.mana?.casterRight?.spellInstance == null)
        return;
      this.RemoveBarrier((SpellCastCharge) (mergeData.mana.casterRight.spellInstance as ArcaneBolt));
    }

    private void CreateBarrier(SpellCastCharge spell)
    {
      foreach (SkillArcaneBarrier.Barrier barrier in this.barriers.Where<SkillArcaneBarrier.Barrier>((Func<SkillArcaneBarrier.Barrier, bool>) (existing => existing.MatchBarrier(spell))))
      {
        barrier.Despawn();
        this.barriers.Remove(barrier);
      }
      this.barrierItemData?.SpawnAsync((Action<Item>) (item =>
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        SkillArcaneBarrier.\u003C\u003Ec__DisplayClass33_1 cDisplayClass331 = new SkillArcaneBarrier.\u003C\u003Ec__DisplayClass33_1();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass331.CS\u0024\u003C\u003E8__locals1 = this;
        // ISSUE: reference to a compiler-generated field
        cDisplayClass331.barrier = new SkillArcaneBarrier.Barrier(this, item, spell.spellCaster.mana.creature, spell.spellCaster.side, this.barrierScale);
        // ISSUE: reference to a compiler-generated field
        this.barriers.Add(cDisplayClass331.barrier);
        ((ThunderBehaviour) item).transform.SetPositionAndRotation(this.GetBarrierPosition(spell), this.GetBarrierRotation(spell));
        item.physicBody.isKinematic = true;
        item.SetParryMagic(false);
        item.DisallowDespawn = true;
        ((ThunderEntity) item).SetVariable<bool>("cullingDetectionEnabled", false);
        ((ThunderBehaviour) item).transform.localScale = Vector3.one * this.barrierScale;
        // ISSUE: reference to a compiler-generated field
        cDisplayClass331.barrier.TryStartEffect(this.barrierEffectData);
        // ISSUE: method pointer
        item.collisionHandlers[0].OnCollisionStartEvent -= new CollisionHandler.CollisionEvent((object) cDisplayClass331, __methodptr(\u003CCreateBarrier\u003Eg__OnCollisionStart\u007C2));
        // ISSUE: method pointer
        item.collisionHandlers[0].OnCollisionStartEvent += new CollisionHandler.CollisionEvent((object) cDisplayClass331, __methodptr(\u003CCreateBarrier\u003Eg__OnCollisionStart\u007C2));
        // ISSUE: method pointer
        item.collisionHandlers[0].OnCollisionStopEvent -= new CollisionHandler.CollisionEvent((object) this, __methodptr(\u003CCreateBarrier\u003Eg__OnCollisionStop\u007C3));
        // ISSUE: method pointer
        item.collisionHandlers[0].OnCollisionStopEvent -= new CollisionHandler.CollisionEvent((object) this, __methodptr(\u003CCreateBarrier\u003Eg__OnCollisionStop\u007C3));
        SkillArcaneBarrier.BarrierEvent barrierStartEvent = this.OnBarrierStartEvent;
        if (barrierStartEvent == null)
          return;
        // ISSUE: reference to a compiler-generated field
        barrierStartEvent(spell, cDisplayClass331.barrier);
      }), new Vector3?(), new Quaternion?(), (Transform) null, true, (List<ContentCustomData>) null, (Item.Owner) 0);
    }

    private void UpdateBarrier(SpellCastCharge spell)
    {
      SkillArcaneBarrier.Barrier barrier = this.barriers.FirstOrDefault<SkillArcaneBarrier.Barrier>((Func<SkillArcaneBarrier.Barrier, bool>) (x => x.MatchBarrier(spell)));
      if (barrier == null)
        return;
      barrier.UpdateBarrier(this.GetBarrierPosition(spell), this.GetBarrierRotation(spell));
      SkillArcaneBarrier.BarrierEvent barrierUpdateEvent = this.OnBarrierUpdateEvent;
      if (barrierUpdateEvent == null)
        return;
      barrierUpdateEvent(spell, barrier);
    }

    private void RemoveBarrier(SpellCastCharge spell)
    {
      SkillArcaneBarrier.Barrier barrier = this.barriers.FirstOrDefault<SkillArcaneBarrier.Barrier>((Func<SkillArcaneBarrier.Barrier, bool>) (x => x.MatchBarrier(spell)));
      if (barrier == null || (UnityEngine.Object) barrier.item == (UnityEngine.Object) null)
        return;
      barrier.Despawn();
      this.barriers.Remove(barrier);
      SkillArcaneBarrier.BarrierEvent barrierStopEvent = this.OnBarrierStopEvent;
      if (barrierStopEvent == null)
        return;
      barrierStopEvent(spell, barrier);
    }

    private int GetBarrierLayerMask()
    {
      return 0 | 1 << GameManager.GetLayer((LayerName) 9) | 1 << GameManager.GetLayer((LayerName) 8) | 1 << GameManager.GetLayer((LayerName) 11) | 1 << GameManager.GetLayer((LayerName) 23) | 1 << GameManager.GetLayer((LayerName) 18) | 1 << GameManager.GetLayer((LayerName) 21);
    }

    public delegate void BarrierEvent(SpellCastCharge spell, SkillArcaneBarrier.Barrier barrier);

    public delegate void BarrierGripEvent(SkillArcaneBarrier.Barrier barrier);

    public delegate void BarrierCollisionEvent(
      SpellCastCharge spell,
      SkillArcaneBarrier.Barrier barrier,
      CollisionInstance collision);

    public class Barrier
    {
      public SkillArcaneBarrier skill;
      public Creature creature;
      public Item item;
      public Side side;
      public List<EffectInstance> barrierEffects;
      public float scale;

      public Barrier(
        SkillArcaneBarrier skill,
        Item item,
        Creature creature,
        Side side,
        float scale = 1f)
      {
        this.skill = skill;
        this.creature = creature;
        this.item = item;
        this.side = side;
        this.scale = scale;
        this.barrierEffects = new List<EffectInstance>();
      }

      public void UpdateBarrier(Vector3 position, Quaternion rotation)
      {
        if ((UnityEngine.Object) this.item != (UnityEngine.Object) null)
        {
          this.item.physicBody.MovePosition(position);
          this.item.physicBody.MoveRotation(rotation);
        }
        else
          Debug.Log((object) "Looping spray without an Item!");
      }

      public void TryStartEffect(EffectData effectData)
      {
        EffectInstance effectInstance = effectData?.Spawn(((ThunderBehaviour) this.item).transform, true, (ColliderGroup) null, false);
        if (effectInstance != null)
        {
          this.barrierEffects.Add(effectInstance);
          effectInstance?.Play(0, false, false);
        }
        this.UpdateEffectScale(this.scale);
      }

      public void Despawn()
      {
        foreach (EffectInstance barrierEffect in this.barrierEffects)
        {
          barrierEffect.SetParent((Transform) null, false);
          barrierEffect?.End(false, -1f);
        }
        this.barrierEffects.Clear();
        ((ThunderEntity) this.item).Despawn();
      }

      public IEnumerator Despawn(float delay)
      {
        yield return (object) new WaitForSeconds(delay);
        this.Despawn();
      }

      private void UpdateEffectScale(float scale)
      {
        foreach (EffectInstance barrierEffect in this.barrierEffects)
          barrierEffect.SetSize(scale + 0.2f);
      }

      public bool MatchBarrier(SpellCastCharge spell)
      {
        return (UnityEngine.Object) spell.spellCaster.mana.creature == (UnityEngine.Object) this.creature && spell.spellCaster.side == this.side;
      }

      public bool MatchBarrier(PlayerHand hand)
      {
        return (UnityEngine.Object) ((RagdollPart) hand.ragdollHand).ragdoll.creature == (UnityEngine.Object) this.creature && hand.side == this.side;
      }

      public SkillArcaneBarrier.Barrier Other()
      {
        return this.skill.barriers.FirstOrDefault<SkillArcaneBarrier.Barrier>((Func<SkillArcaneBarrier.Barrier, bool>) (x => x.MatchBarrier(this.side == null ? this.creature.mana.casterLeft.ragdollHand.playerHand : this.creature.mana.casterRight.ragdollHand.playerHand)));
      }
    }
  }
}
