// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SkillArcaneBlink
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
namespace Arcana.Skills
{
  internal class SkillArcaneBlink : SpellSkillData
  {
    [ModOptionCategory("Blink", 10)]
    [ModOptionButton]
    [ModOption("Keep Player's Velocity", "retain the player's velocity at the time of teleportation, instead of fully halting them. Will not reset fall damage. (Recommended for advanced users)", order = 1)]
    public static bool keepVelocity;
    [ModOptionCategory("Blink", 10)]
    [ModOptionButton]
    [ModOption("Do Bounds Check", "Experimental bounds checking for teleports, enables fallback. Enables safer teleportation while losing positional accuracy.", order = 2)]
    public static bool doBoundsCheck;
    public float initialDelay = 0.2f;
    public float floorCheckSphereRadius = 0.1f;
    public float boundsSearchStep = 0.2f;
    public int boundsSearchRetries = 5;
    public float gripWindow = 3f;
    public string[] itemCategories;
    public string[] itemIds;
    public string grabEffectId;
    public string eyeEffectId;
    public EffectData grabEffectData;
    public EffectData eyeEffectData;

    public event SkillArcaneBlink.OnTeleport OnTeleportEvent;

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.grabEffectData = Catalog.GetData<EffectData>(this.grabEffectId, true);
      this.eyeEffectData = Catalog.GetData<EffectData>(this.eyeEffectId, true);
    }

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      // ISSUE: method pointer
      EventManager.OnItemRelease -= new EventManager.ItemRelease((object) this, __methodptr(OnItemRelease));
      // ISSUE: method pointer
      EventManager.OnItemRelease += new EventManager.ItemRelease((object) this, __methodptr(OnItemRelease));
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      // ISSUE: method pointer
      EventManager.OnItemRelease -= new EventManager.ItemRelease((object) this, __methodptr(OnItemRelease));
    }

    public void Teleport(RagdollHand hand, Handle handle)
    {
      Vector3 position = ((ThunderBehaviour) Player.local).transform.position;
      EffectInstance effectInstance = this.grabEffectData?.Spawn(((ThunderBehaviour) hand).transform, true, (ColliderGroup) null, false);
      effectInstance?.SetHaptic(hand.side, Catalog.gameData.haptics.spellSelected);
      effectInstance?.Play(0, false, false);
      this.eyeEffectData?.Spawn(Player.local.head.cam.transform.position + Player.local.head.cam.transform.forward * 0.3f, Player.local.head.cam.transform.rotation, ((Component) Player.local.head).transform, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>())?.Play(0, false, false);
      Vector3 teleportPoint = this.CalculateTeleportPoint(hand, handle);
      Player.local.Teleport(teleportPoint, ((Component) Player.local.head).transform.rotation, SkillArcaneBlink.keepVelocity, true);
      hand.Grab(handle, true, false);
      SkillArcaneBlink.OnTeleport onTeleportEvent = this.OnTeleportEvent;
      if (onTeleportEvent == null)
        return;
      onTeleportEvent(position, ((ThunderBehaviour) handle).transform.position);
    }

    private Vector3 CalculateTeleportPoint(RagdollHand hand, Handle handle)
    {
      Vector3 position1 = ((ThunderBehaviour) Player.local).transform.position;
      for (float num1 = 0.0f; (double) num1 < (double) this.boundsSearchRetries; num1 += this.boundsSearchStep)
      {
        Vector3 position2 = ((ThunderBehaviour) handle).transform.position;
        Vector3 vector3_1 = position1 - ((ThunderBehaviour) handle).transform.position;
        Vector3 vector3_2 = vector3_1.normalized * num1;
        Vector3 vector3_3 = position2 + vector3_2;
        vector3_1 = ((Component) Player.local.head).transform.position - ((ThunderBehaviour) Player.local).transform.position;
        float magnitude = vector3_1.magnitude;
        Vector3 teleportPoint = vector3_3 - Vector3.up * magnitude - (((ThunderBehaviour) hand).transform.position - ((Component) Player.local.head).transform.position);
        Vector3 vector3_4 = teleportPoint + Vector3.up * magnitude;
        RaycastHit raycastHit1;
        RaycastHit raycastHit2;
        if (Physics.SphereCast(teleportPoint, this.floorCheckSphereRadius, Vector3.up, ref raycastHit1, magnitude + 0.2f, this.GetCollisionMask()) && !Physics.SphereCast(vector3_3, this.floorCheckSphereRadius, Vector3.down, ref raycastHit2, magnitude, this.GetCollisionMask()) && (double) ((RaycastHit) ref raycastHit1).distance < (double) magnitude + (double) this.floorCheckSphereRadius)
        {
          teleportPoint += Vector3.down * (magnitude - ((RaycastHit) ref raycastHit1).distance);
          vector3_4 = teleportPoint + Vector3.up * magnitude;
        }
        RaycastHit raycastHit3;
        Vector3 vector3_5;
        if (Physics.SphereCast(vector3_4, this.floorCheckSphereRadius, Vector3.down, ref raycastHit3, float.PositiveInfinity, (int) Player.local.locomotion.groundMask))
        {
          if ((double) ((RaycastHit) ref raycastHit3).distance < (double) magnitude)
          {
            teleportPoint += Vector3.up * (float) ((double) magnitude - (double) ((RaycastHit) ref raycastHit3).distance - (double) this.floorCheckSphereRadius / 4.0);
            vector3_5 = teleportPoint + Vector3.up * magnitude;
          }
        }
        else
        {
          teleportPoint += Vector3.up * (magnitude * 2f + this.floorCheckSphereRadius);
          vector3_5 = teleportPoint + Vector3.up * magnitude;
        }
        vector3_1 = ((Component) Player.local.head).transform.position - ((ThunderBehaviour) Player.local).transform.position;
        float num2 = (float) ((double) vector3_1.magnitude / 2.0 - 0.05000000074505806);
        if (!SkillArcaneBlink.doBoundsCheck || !Physics.CheckSphere(Vector3.Lerp(((ThunderBehaviour) Player.local).transform.position, ((Component) Player.local.head).transform.position, 0.5f), num2, this.GetCollisionMask()))
          return teleportPoint;
      }
      return position1;
    }

    public void OnItemRelease(Handle handle, RagdollHand ragdollHand, bool throwing)
    {
      bool flag = ((IEnumerable<string>) this.itemCategories).Contains<string>(handle?.item?.data?.category) || ((IEnumerable<string>) this.itemIds).Contains<string>(((CatalogData) handle?.item?.data)?.id);
      int num1;
      if (handle == null)
      {
        num1 = 0;
      }
      else
      {
        float? flyThrowAngle = handle.item?.data?.flyThrowAngle;
        float num2 = 0.0f;
        num1 = (double) flyThrowAngle.GetValueOrDefault() <= (double) num2 & flyThrowAngle.HasValue ? 1 : 0;
      }
      if (num1 != 0)
        Debug.Log((object) ("Arcane Blink attempt detected with weapon " + ((CatalogData) handle?.item?.data)?.id + " which has a flyThrowAngle of 0.0f! Blink halted!"));
      if ((ragdollHand != null ? (!ragdollHand.creature?.isPlayer.GetValueOrDefault() ? 1 : 0) : 1) != 0 || !flag || !handle.item.imbues.Any<Imbue>((Func<Imbue, bool>) (imbue => imbue.spellCastBase is ArcaneBolt)) || !handle.item.isFlying)
        return;
      handle.item.SetPhysicModifier((object) this, new float?(0.0f), 1f, -1f, -1f, -1f, (EffectData) null);
      ((MonoBehaviour) ragdollHand.creature).StartCoroutine(this.FlowControl(ragdollHand, handle));
    }

    public IEnumerator FlowControl(RagdollHand hand, Handle handle)
    {
      float startTime = Time.time;
      bool gripped = false;
      yield return (object) new WaitForSeconds(this.initialDelay);
      hand.caster.telekinesis.Disable((object) this);
      while ((double) Time.time - (double) startTime < (double) this.gripWindow)
      {
        if (hand.playerHand.controlHand.gripPressed && (UnityEngine.Object) hand.grabbedHandle == (UnityEngine.Object) null)
        {
          gripped = true;
          break;
        }
        yield return (object) null;
      }
      hand.caster.telekinesis.Enable((object) this);
      handle.item.SetPhysicModifier((object) this, new float?(1f), 1f, -1f, -1f, -1f, (EffectData) null);
      if (gripped)
        this.Teleport(hand, handle);
    }

    public int GetCollisionMask()
    {
      return 0 | 1 << GameManager.GetLayer((LayerName) 0) | 1 << GameManager.GetLayer((LayerName) 1);
    }

    public delegate void OnTeleport(Vector3 originalPosition, Vector3 newPosition);
  }
}
