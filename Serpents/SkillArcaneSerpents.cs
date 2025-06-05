// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.Serpents.SkillArcaneSerpents
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Behaviors;
using Arcana.Misc;
using Arcana.Spells;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Arcana.Skills.Serpents
{
  public class SkillArcaneSerpents : SpellSkillData
  {
    public static int maxSerpents = 5;
    public string serpentHandleId = "ObjectHandleLight";
    public string serpentHandleEffectId = "SpellArcaneSerpentHoldPoint";
    public string readyEffectId = "HitArcaneSerpent";
    public float serpentTargetRadius = 20f;
    public bool requireUseOnMax = true;
    public Serpent.SerpentData serpentData;
    public SkillArcaneSerpents.SerpentHandle currentHandle;
    public HandleData serpentHandleData;
    public EffectData readyEffectData;
    public EffectData serpentHandleEffectData;
    public static List<Serpent> serpents;

    public event SkillArcaneSerpents.SerpentListChange OnSerpentListChange;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.serpentData.LoadCatalogData();
      this.serpentHandleData = Catalog.GetData<HandleData>(this.serpentHandleId, true);
      this.serpentHandleEffectData = Catalog.GetData<EffectData>(this.serpentHandleEffectId, true);
      this.readyEffectData = Catalog.GetData<EffectData>(this.readyEffectId, true);
      SkillArcaneSerpents.serpents = new List<Serpent>();
    }

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      // ISSUE: method pointer
      EventManager.onLevelLoad -= new EventManager.LevelLoadEvent((object) this, __methodptr(OnLevelLoad));
      // ISSUE: method pointer
      EventManager.onLevelLoad += new EventManager.LevelLoadEvent((object) this, __methodptr(OnLevelLoad));
      // ISSUE: method pointer
      EventManager.onLevelUnload -= new EventManager.LevelLoadEvent((object) this, __methodptr(OnLevelLoad));
      // ISSUE: method pointer
      EventManager.onLevelUnload += new EventManager.LevelLoadEvent((object) this, __methodptr(OnLevelLoad));
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      // ISSUE: method pointer
      EventManager.onLevelLoad -= new EventManager.LevelLoadEvent((object) this, __methodptr(OnLevelLoad));
      // ISSUE: method pointer
      EventManager.onLevelUnload -= new EventManager.LevelLoadEvent((object) this, __methodptr(OnLevelLoad));
    }

    private void OnLevelLoad(LevelData leveldata, LevelData.Mode mode, EventTime eventtime)
    {
      this.ClearSerpents();
    }

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      switch (spell)
      {
        case ArcaneBolt arcaneBolt:
          // ISSUE: method pointer
          arcaneBolt.OnSpellCastEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellChargeStart));
          // ISSUE: method pointer
          arcaneBolt.OnSpellCastEvent += new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellChargeStart));
          // ISSUE: method pointer
          arcaneBolt.OnSpellStopEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellChargeStop));
          // ISSUE: method pointer
          arcaneBolt.OnSpellStopEvent += new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellChargeStop));
          // ISSUE: method pointer
          arcaneBolt.OnSpellThrowEvent -= new SpellCastCharge.SpellThrowEvent((object) this, __methodptr(OnSpellThrowEvent));
          // ISSUE: method pointer
          arcaneBolt.OnSpellThrowEvent += new SpellCastCharge.SpellThrowEvent((object) this, __methodptr(OnSpellThrowEvent));
          arcaneBolt.OnSprayStartEvent -= new ArcaneBolt.SprayEvent(this.OnSpellChargeStop);
          arcaneBolt.OnSprayStartEvent += new ArcaneBolt.SprayEvent(this.OnSpellChargeStop);
          arcaneBolt.OnSprayStopEvent -= new ArcaneBolt.SprayEvent(this.OnSpellChargeStart);
          arcaneBolt.OnSprayStopEvent += new ArcaneBolt.SprayEvent(this.OnSpellChargeStart);
          break;
        case SpellMergeData spellMergeData:
          // ISSUE: method pointer
          spellMergeData.OnMergeStartEvent -= new SpellMergeData.MergeEvent((object) this, __methodptr(OnSpellMergeStart));
          // ISSUE: method pointer
          spellMergeData.OnMergeStartEvent += new SpellMergeData.MergeEvent((object) this, __methodptr(OnSpellMergeStart));
          break;
      }
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      switch (spell)
      {
        case ArcaneBolt arcaneBolt:
          // ISSUE: method pointer
          arcaneBolt.OnSpellCastEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellChargeStart));
          // ISSUE: method pointer
          arcaneBolt.OnSpellStopEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellChargeStop));
          // ISSUE: method pointer
          arcaneBolt.OnSpellThrowEvent -= new SpellCastCharge.SpellThrowEvent((object) this, __methodptr(OnSpellThrowEvent));
          arcaneBolt.OnSprayStartEvent -= new ArcaneBolt.SprayEvent(this.OnSpellChargeStop);
          arcaneBolt.OnSprayStopEvent -= new ArcaneBolt.SprayEvent(this.OnSpellChargeStart);
          if (caster?.other?.spellInstance is ArcaneBolt)
            break;
          this.ClearSerpents();
          break;
        case SpellMergeData spellMergeData:
          // ISSUE: method pointer
          spellMergeData.OnMergeStartEvent -= new SpellMergeData.MergeEvent((object) this, __methodptr(OnSpellMergeStart));
          break;
      }
    }

    private void OnSpellChargeStart(SpellCastCharge spell)
    {
      if (SkillArcaneSerpents.serpents.Count < SkillArcaneSerpents.maxSerpents || SkillArcaneSerpents.serpents.Any<Serpent>((Func<Serpent, bool>) (serpent => serpent.orbitingObject && (UnityEngine.Object) serpent.ObjectOrbitTransform != (UnityEngine.Object) this.currentHandle?.Transform || serpent.isAttacking)) || spell.spellCaster.isMerging)
        return;
      this.GetNewHandle(spell);
    }

    private void OnSpellChargeStop(SpellCastCharge spell)
    {
      if (this.currentHandle == null || this.currentHandle.IsHanded)
        return;
      if (spell.spellCaster.other.spellInstance is ArcaneBolt spellInstance && spell.spellCaster.other.isFiring && !spell.spellCaster.other.isMerging && !spell.spellCaster.other.isSpraying)
      {
        this.GetNewHandle((SpellCastCharge) spellInstance);
        this.SerpentsTargetHandle();
      }
      else
      {
        this.currentHandle?.DestroyHandle();
        foreach (Serpent serpent in SkillArcaneSerpents.serpents.Where<Serpent>((Func<Serpent, bool>) (x => x.orbitingObject && !x.isAttacking && x.orbitHandler == null)))
          serpent.ResetOrbit(serpent.data.teleportReturnTimeout);
      }
    }

    private void OnSpellMergeStart(SpellMergeData mergeData)
    {
      if (mergeData?.mana?.casterLeft?.spellInstance != null)
        this.OnSpellChargeStop(mergeData.mana.casterLeft.spellInstance as SpellCastCharge);
      if (mergeData?.mana?.casterRight?.spellInstance == null)
        return;
      this.OnSpellChargeStop(mergeData.mana.casterRight.spellInstance as SpellCastCharge);
    }

    private void OnSpellThrowEvent(SpellCastCharge spell, Vector3 velocity)
    {
      if (SkillArcaneSerpents.serpents.Count >= SkillArcaneSerpents.maxSerpents && this.requireUseOnMax)
      {
        this.ClearSerpents();
      }
      else
      {
        this.AddSerpent(spell.spellCaster.Orb.position, velocity.normalized, new Side?(spell.spellCaster.side));
        this.OnSpellChargeStop(spell);
      }
    }

    public SkillArcaneSerpents.SerpentHandle GetNewHandle(SpellCastCharge spell)
    {
      this.currentHandle?.DestroyHandle();
      this.currentHandle = new SkillArcaneSerpents.SerpentHandle(this, spell.spellCaster);
      this.SerpentsTargetHandle();
      // ISSUE: method pointer
      this.currentHandle.OnGrab -= new Handle.GrabEvent((object) this, __methodptr(OnHandleGrab));
      // ISSUE: method pointer
      this.currentHandle.OnGrab += new Handle.GrabEvent((object) this, __methodptr(OnHandleGrab));
      // ISSUE: method pointer
      this.currentHandle.OnUnGrab -= new Handle.GrabEvent((object) this, __methodptr(OnHandleUnGrab));
      // ISSUE: method pointer
      this.currentHandle.OnUnGrab += new Handle.GrabEvent((object) this, __methodptr(OnHandleUnGrab));
      return this.currentHandle;
    }

    private void OnHandleGrab(RagdollHand hand, Handle handle, EventTime time)
    {
      if (!this.currentHandle.Equal(handle))
        return;
      this.currentHandle.Transform.SetPositionAndRotation(((ThunderBehaviour) hand).transform.position, Quaternion.LookRotation(-hand.axisPalm));
      this.currentHandle.PhysicBody.isKinematic = false;
      this.currentHandle.Transform.SetParent((Transform) null);
      this.currentHandle.PlayHandleEffect(this.serpentHandleEffectData);
    }

    private void OnHandleUnGrab(RagdollHand hand, Handle handle, EventTime time)
    {
      if (time != 1)
        return;
      bool flag = (bool) (UnityEngine.Object) hand.playerHand && (double) PlayerControl.GetHand(hand.side).GetHandVelocity().magnitude * (1.0 / (double) Time.timeScale) > (double) Catalog.gameData.throwVelocity;
      this.currentHandle?.DestroyHandle();
      if (flag)
        this.ThrowSerpents(((ThunderBehaviour) hand).transform.position, PlayerControl.GetHand(hand.side).GetHandVelocity());
      else if (hand.otherHand.caster.spellInstance is ArcaneBolt spellInstance && hand.otherHand.caster.isFiring)
      {
        this.GetNewHandle((SpellCastCharge) spellInstance);
      }
      else
      {
        foreach (Serpent serpent in SkillArcaneSerpents.serpents.Where<Serpent>((Func<Serpent, bool>) (serpent => serpent.orbitingObject)))
          serpent.ResetOrbit(serpent.data.teleportReturnTimeout);
      }
    }

    public void ThrowSerpents(Vector3 position, Vector3 velocity)
    {
      Creature[] creaturesInRadius = Utilities.GetCreaturesInRadius(position, this.serpentTargetRadius, Math.Max(SkillArcaneSerpents.serpents.Count, this.serpentData.maxChain));
      if (creaturesInRadius.Length == 0)
        return;
      for (int index = 0; index < SkillArcaneSerpents.serpents.Count; ++index)
      {
        Serpent serpent = SkillArcaneSerpents.serpents[index];
        if (index < creaturesInRadius.Length)
          serpent.SetTargets(((IEnumerable<Creature>) Extensions.Shuffle<Creature>(creaturesInRadius)).ToList<Creature>());
        else
          serpent.ResetOrbit(serpent.data.teleportReturnTimeout);
      }
    }

    public void SerpentsTargetHandle(object handler = null)
    {
      foreach (Serpent serpent in SkillArcaneSerpents.serpents.Where<Serpent>((Func<Serpent, bool>) (x => x.orbitHandler == handler && !x.isAttacking)))
      {
        serpent.AssignNewOrbit(this.currentHandle.Transform, true, timeLimit: serpent.data.teleportTargetTimeout, handler: handler);
        serpent.OnOrbitChangeEndEvent -= new Serpent.OnOrbitChangeEnd(this.OnOrbitChangeEnd);
        serpent.OnOrbitChangeEndEvent += new Serpent.OnOrbitChangeEnd(this.OnOrbitChangeEnd);
      }
    }

    public void OnOrbitChangeEnd(Serpent serpent, Transform orbitObject, float endTime)
    {
      if (this.currentHandle == null || SkillArcaneSerpents.serpents.Any<Serpent>((Func<Serpent, bool>) (s => s.IsChangingOrbit())) || SkillArcaneSerpents.serpents.Any<Serpent>((Func<Serpent, bool>) (s => s.orbitHandler != null)))
        return;
      serpent.OnOrbitChangeEndEvent -= new Serpent.OnOrbitChangeEnd(this.OnOrbitChangeEnd);
      this.currentHandle.PlayHandleEffect(this.readyEffectData);
      this.currentHandle.EnableHandle();
    }

    public void AddSerpent(Vector3 position, Vector3 facing, Side? castSide)
    {
      if (SkillArcaneSerpents.serpents.Count >= SkillArcaneSerpents.maxSerpents)
        return;
      Side side = (Side) ((object) castSide ?? (UnityEngine.Random.Range(0, 2) == 0 ? (object) 0 : (object) 1));
      if (SkillArcaneSerpents.serpents.Count > 0)
        side = SkillArcaneSerpents.serpents[SkillArcaneSerpents.serpents.Count - 1].side == side ? Utils.Other(side) : side;
      Serpent serpent = new GameObject(SkillArcaneSerpents.serpents.Count > 1 ? "Arcane Serpent" : "Sheree Anahit Skibidi").AddComponent<Serpent>();
      serpent.Form(this.serpentData, position, facing, Player.currentCreature, side);
      SkillArcaneSerpents.serpents.Add(serpent);
      SkillArcaneSerpents.SerpentListChange serpentListChange = this.OnSerpentListChange;
      if (serpentListChange != null)
        serpentListChange(serpent, true);
      Serpent.OnDespawn handler = (Serpent.OnDespawn) null;
      handler = (Serpent.OnDespawn) (instance =>
      {
        instance.OnDespawnEvent -= handler;
        SkillArcaneSerpents.serpents.Remove(instance);
      });
      serpent.OnDespawnEvent -= handler;
      serpent.OnDespawnEvent += handler;
    }

    public void ClearSerpents()
    {
      this.currentHandle?.DestroyHandle();
      foreach (Serpent serpent in SkillArcaneSerpents.serpents.ToArray())
      {
        serpent.Despawn();
        SkillArcaneSerpents.SerpentListChange serpentListChange = this.OnSerpentListChange;
        if (serpentListChange != null)
          serpentListChange(serpent, false);
      }
    }

    public delegate void SerpentListChange(Serpent serpent, bool alive);

    public class SerpentHandle
    {
      private int activeEffects = 0;
      public SkillArcaneSerpents skill;
      private Handle serpentHandle;
      public List<EffectInstance> serpentHandleEffectInstances = new List<EffectInstance>();

      public Transform Transform => ((ThunderBehaviour) this.serpentHandle)?.transform;

      public PhysicBody PhysicBody => this.serpentHandle?.physicBody;

      public bool IsHanded => this.serpentHandle != null && this.serpentHandle.IsHanded();

      public event Handle.GrabEvent OnGrab
      {
        add
        {
          Handle.GrabEvent grabEvent1 = this.OnGrab;
          Handle.GrabEvent grabEvent2;
          do
          {
            grabEvent2 = grabEvent1;
            grabEvent1 = Interlocked.CompareExchange<Handle.GrabEvent>(ref this.OnGrab, (Handle.GrabEvent) Delegate.Combine((Delegate) grabEvent2, (Delegate) value), grabEvent2);
          }
          while (grabEvent1 != grabEvent2);
        }
        remove
        {
          Handle.GrabEvent grabEvent1 = this.OnGrab;
          Handle.GrabEvent grabEvent2;
          do
          {
            grabEvent2 = grabEvent1;
            grabEvent1 = Interlocked.CompareExchange<Handle.GrabEvent>(ref this.OnGrab, (Handle.GrabEvent) Delegate.Remove((Delegate) grabEvent2, (Delegate) value), grabEvent2);
          }
          while (grabEvent1 != grabEvent2);
        }
      }

      public event Handle.GrabEvent OnUnGrab
      {
        add
        {
          Handle.GrabEvent grabEvent1 = this.OnUnGrab;
          Handle.GrabEvent grabEvent2;
          do
          {
            grabEvent2 = grabEvent1;
            grabEvent1 = Interlocked.CompareExchange<Handle.GrabEvent>(ref this.OnUnGrab, (Handle.GrabEvent) Delegate.Combine((Delegate) grabEvent2, (Delegate) value), grabEvent2);
          }
          while (grabEvent1 != grabEvent2);
        }
        remove
        {
          Handle.GrabEvent grabEvent1 = this.OnUnGrab;
          Handle.GrabEvent grabEvent2;
          do
          {
            grabEvent2 = grabEvent1;
            grabEvent1 = Interlocked.CompareExchange<Handle.GrabEvent>(ref this.OnUnGrab, (Handle.GrabEvent) Delegate.Remove((Delegate) grabEvent2, (Delegate) value), grabEvent2);
          }
          while (grabEvent1 != grabEvent2);
        }
      }

      public SerpentHandle(SkillArcaneSerpents skill, SpellCaster caster)
      {
        this.skill = skill;
        GameObject gameObject = ((Component) new GameObject().AddComponent<Rigidbody>()).gameObject;
        gameObject.transform.SetParent(((Component) caster).transform);
        gameObject.transform.SetPositionAndRotation(caster.Orb.position, caster.Orb.rotation);
        Handle handle = gameObject.AddComponent<Handle>();
        handle.physicBody.isKinematic = true;
        handle.physicBody.useGravity = false;
        handle.handOverlapColliders = new List<Collider>();
        ((Interactable) handle).Load((InteractableData) skill.serpentHandleData);
        ((InteractableData) handle.data).localizationId = "Serpents";
        ((InteractableData) handle.data).highlightDefaultTitle = "Serpents";
        ((InteractableData) handle.data).highlightDefaultTitle = "Serpents";
        ((Interactable) handle).SetTouch(false);
        handle.SetTelekinesis(false);
        ((Interactable) handle).allowedHandSide = caster.other.side == null ? (Interactable.HandSide) 1 : (Interactable.HandSide) 2;
        // ISSUE: method pointer
        handle.Grabbed -= new Handle.GrabEvent((object) this, __methodptr(OnGrabEvent));
        // ISSUE: method pointer
        handle.Grabbed += new Handle.GrabEvent((object) this, __methodptr(OnGrabEvent));
        // ISSUE: method pointer
        handle.UnGrabbed -= new Handle.GrabEvent((object) this, __methodptr(OnUnGrabEvent));
        // ISSUE: method pointer
        handle.UnGrabbed += new Handle.GrabEvent((object) this, __methodptr(OnUnGrabEvent));
        this.serpentHandle = handle;
      }

      ~SerpentHandle() => this.DestroyHandle();

      public void OnGrabEvent(RagdollHand hand, Handle handle, EventTime time)
      {
        // ISSUE: reference to a compiler-generated field
        this.OnGrab?.Invoke(hand, handle, time);
      }

      public void OnUnGrabEvent(RagdollHand hand, Handle handle, EventTime time)
      {
        // ISSUE: reference to a compiler-generated field
        this.OnUnGrab?.Invoke(hand, handle, time);
      }

      public void EnableHandle() => ((Interactable) this.serpentHandle).SetTouch(true);

      public void DestroyHandle()
      {
        if (((ThunderBehaviour) this.serpentHandle)?.gameObject == null)
          return;
        foreach (Serpent serpent in SkillArcaneSerpents.serpents.Where<Serpent>((Func<Serpent, bool>) (x => x.orbitingObject && (UnityEngine.Object) x.ObjectOrbitTransform == (UnityEngine.Object) ((ThunderBehaviour) this.serpentHandle).transform && x.orbitHandler == null)))
          serpent.ResetOrbit(serpent.data.teleportReturnTimeout);
        foreach (EffectInstance handleEffectInstance in this.serpentHandleEffectInstances)
        {
          if (handleEffectInstance != null)
            handleEffectInstance.ForceStop();
        }
        ((Interactable) this.serpentHandle).SetTouch(false);
        ((MonoBehaviour) GameManager.local).StartCoroutine(DestroyCoroutine(this.serpentHandle));
        this.skill.currentHandle = (SkillArcaneSerpents.SerpentHandle) null;

        IEnumerator DestroyCoroutine(Handle handle)
        {
          yield return (object) new WaitUntil((Func<bool>) (() => this.activeEffects <= 0));
          UnityEngine.Object.Destroy((UnityEngine.Object) ((ThunderBehaviour) handle).gameObject);
        }
      }

      public void PlayHandleEffect(EffectData effectData)
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        SkillArcaneSerpents.SerpentHandle.\u003C\u003Ec__DisplayClass22_0 cDisplayClass220 = new SkillArcaneSerpents.SerpentHandle.\u003C\u003Ec__DisplayClass22_0();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass220.\u003C\u003E4__this = this;
        if (((ThunderBehaviour) this.serpentHandle)?.gameObject == null)
          return;
        // ISSUE: reference to a compiler-generated field
        cDisplayClass220.handler = (EffectInstance.EffectFinishEvent) null;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: method pointer
        cDisplayClass220.handler = new EffectInstance.EffectFinishEvent((object) cDisplayClass220, __methodptr(\u003CPlayHandleEffect\u003Eb__0));
        EffectInstance effectInstance = effectData.Spawn(((ThunderBehaviour) this.serpentHandle).transform, true, (ColliderGroup) null, false);
        // ISSUE: reference to a compiler-generated field
        effectInstance.onEffectFinished -= cDisplayClass220.handler;
        // ISSUE: reference to a compiler-generated field
        effectInstance.onEffectFinished += cDisplayClass220.handler;
        effectInstance.Play(0, false, false);
        this.serpentHandleEffectInstances.Add(effectInstance);
        ++this.activeEffects;
      }

      public bool Equal(Handle handle) => (UnityEngine.Object) handle == (UnityEngine.Object) this.serpentHandle;
    }
  }
}
