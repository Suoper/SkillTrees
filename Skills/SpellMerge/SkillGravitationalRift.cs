// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SpellMerge.SkillGravitationalRift
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Misc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Skills.SpellMerge
{
  internal class SkillGravitationalRift : SpellMergeData
  {
    public float spellHandSeparationMaxAngle = 45f;
    public float spellRadius = 20f;
    public int targetLimit = 10;
    public string floatingStatusId = "Floating";
    public StatusData floatingStatusData;
    public float floatForceMagnitude = 1f;
    public float portalPushForceMagnitude = 20f;
    private float portalPullDuration = 3f;
    public float portalSpawnDistance = 5f;
    private int portalRayCount = 5;
    public string portalEffectId = "SpellArcaneGravitationalRiftOrbPortal";
    public EffectData portalEffectData;
    private Coroutine routine;
    public static bool isSpellActive;

    public float PortalStopThreshold => 0.3f;

    private Vector3 PortalDirection => Vector3.up;

    public virtual void OnCatalogRefresh()
    {
      base.OnCatalogRefresh();
      this.floatingStatusData = Catalog.GetData<StatusData>(this.floatingStatusId, true);
      this.portalEffectData = Catalog.GetData<EffectData>(this.portalEffectId, true);
      // ISSUE: method pointer
      EventManager.onLevelUnload -= new EventManager.LevelLoadEvent((object) this, __methodptr(OnLevelUnload));
      // ISSUE: method pointer
      EventManager.onLevelUnload += new EventManager.LevelLoadEvent((object) this, __methodptr(OnLevelUnload));
    }

    private void OnLevelUnload(LevelData level, LevelData.Mode mode, EventTime time)
    {
      if (time == 1)
        return;
      SkillGravitationalRift.isSpellActive = false;
      if (this.routine != null)
        ((MonoBehaviour) this.mana).StopCoroutine(this.routine);
      this.routine = (Coroutine) null;
    }

    public virtual bool CanMerge() => !SkillGravitationalRift.isSpellActive;

    public virtual void Unload()
    {
      base.Unload();
      if (!LevelManager.isLoadingLocked)
        return;
      if (this.routine != null)
        ((MonoBehaviour) this.mana).StopCoroutine(this.routine);
      this.routine = (Coroutine) null;
    }

    public virtual void Merge(bool active)
    {
      base.Merge(active);
      PlayerHand handLeft = Player.local.handLeft;
      PlayerHand handRight = Player.local.handRight;
      Vector3 from1 = ((ThunderBehaviour) Player.local).transform.rotation * handLeft.controlHand.GetHandVelocity();
      Vector3 from2 = ((ThunderBehaviour) Player.local).transform.rotation * handRight.controlHand.GetHandVelocity();
      if (active || SkillGravitationalRift.isSpellActive || (double) from1.magnitude < (double) SpellCaster.throwMinHandVelocity || (double) from2.magnitude < (double) SpellCaster.throwMinHandVelocity || (double) Vector3.Angle(from1, ((ThunderBehaviour) handLeft).transform.position - this.mana.mergePoint.position) > (double) this.spellHandSeparationMaxAngle && (double) Vector3.Angle(from2, ((ThunderBehaviour) handRight).transform.position - this.mana.mergePoint.position) > (double) this.spellHandSeparationMaxAngle || (double) this.currentCharge < (double) this.minCharge)
        return;
      Creature[] creaturesInRadius = Utilities.GetCreaturesInRadius(this.mana.mergePoint.position, this.spellRadius, this.targetLimit);
      if (Utils.IsNullOrEmpty((Array) creaturesInRadius))
        return;
      SkillGravitationalRift.isSpellActive = true;
      this.routine = ((MonoBehaviour) this.mana).StartCoroutine(this.PortalRoutine(creaturesInRadius));
    }

    public IEnumerator PortalRoutine(Creature[] targets)
    {
      Creature[] creatureArray1 = targets;
      for (int index = 0; index < creatureArray1.Length; ++index)
      {
        Creature target = creatureArray1[index];
        this.PrepareTarget(target);
        target = (Creature) null;
      }
      creatureArray1 = (Creature[]) null;
      yield return (object) new WaitForSeconds(2f);
      Vector3 direction = this.PortalDirection;
      (Vector3 portalLocation, Vector3 portalNormal) = this.GetPortalLocation(this.GetAverageCreaturePosition(targets), direction);
      EffectInstance portal = this.CreatePortalEffect(portalLocation, portalNormal);
      yield return (object) new WaitForSeconds(2f);
      int completed = 0;
      Creature[] creatureArray2 = targets;
      for (int index = 0; index < creatureArray2.Length; ++index)
      {
        Creature target = creatureArray2[index];
        this.UpdateCreatureCollision(target, true);
        this.CreatureDropAll(target);
        ((MonoBehaviour) this.mana).StartAndTrackCoroutine(this.HandleCreatureMovement(target, portalLocation, this.portalPullDuration), (Action) (() => ++completed));
        target = (Creature) null;
      }
      creatureArray2 = (Creature[]) null;
      while (completed < targets.Length)
        yield return (object) new WaitForEndOfFrame();
      yield return (object) new WaitForSeconds(3f);
      this.Cleanup(targets, portal);
      SkillGravitationalRift.isSpellActive = false;
      this.routine = (Coroutine) null;
      yield return (object) 0;
    }

    private void PrepareTarget(Creature creature)
    {
      ((ThunderEntity) creature).Inflict(this.floatingStatusData, (object) this.mana, float.PositiveInfinity, (object) null, true);
      creature.MaxPush((Creature.PushType) 0, Vector3.zero, (RagdollPart.Type) 0);
      creature.ragdoll.SetState((Ragdoll.State) 1);
      creature.AddForce(Vector3.up * this.floatForceMagnitude, (ForceMode) 2, 1f, (CollisionHandler) null);
    }

    private void Cleanup(Creature[] creatures, EffectInstance portal)
    {
      foreach (Creature creature in creatures)
      {
        ((ThunderEntity) creature).RemovePhysicModifier((object) this);
        this.UpdateCreatureCollision(creature);
        ((ThunderEntity) creature).Despawn();
      }
      portal.End(false, -1f);
    }

    private void CreatureDropAll(Creature creature)
    {
      creature.handLeft.TryRelease();
      creature.handRight.TryRelease();
      foreach (Holder holder in creature.holders)
      {
        if ((UnityEngine.Object) holder != (UnityEngine.Object) null && holder != null && holder.items.Count > 0)
          holder.UnSnapAll();
      }
    }

    private Vector3 GetAverageCreaturePosition(Creature[] creatures)
    {
      if (creatures == null || creatures.Length == 0)
        return Vector3.zero;
      Vector3 zero = Vector3.zero;
      foreach (Creature creature in creatures)
        zero += ((ThunderBehaviour) creature.ragdoll.targetPart).transform.position;
      return zero / (float) creatures.Length;
    }

    private (Vector3, Vector3) GetPortalLocation(
      Vector3 averageCreaturePosition,
      Vector3 direction,
      int offsetIndex = 0)
    {
      float num = 3.3709f;
      Vector3 vector3_1 = averageCreaturePosition;
      Vector3 vector3_2 = -direction.normalized;
      float a = this.portalSpawnDistance + (float) offsetIndex * 0.2f;
      for (int index1 = 0; index1 < this.portalRayCount; ++index1)
      {
        for (int index2 = 0; index2 < this.portalRayCount; ++index2)
        {
          Vector3 vector3_3 = new Vector3((float) ((double) index1 / (double) (this.portalRayCount - 1) * (double) num - (double) num / 2.0), (float) ((double) index2 / (double) (this.portalRayCount - 1) * (double) num - (double) num / 2.0), 0.0f);
          RaycastHit raycastHit;
          if (Physics.Raycast(vector3_1 + Quaternion.LookRotation(direction) * vector3_3, direction, ref raycastHit, this.portalSpawnDistance, this.GetPortalRaycastMask()))
            a = Mathf.Min(a, ((RaycastHit) ref raycastHit).distance);
        }
      }
      return (vector3_1 + direction.normalized * (a + 0.01f), vector3_2);
    }

    private EffectInstance CreatePortalEffect(Vector3 portalLocation, Vector3 portalNormal)
    {
      EffectInstance portalEffect = this.portalEffectData?.Spawn(portalLocation, Quaternion.AngleAxis(Vector3.Angle(Vector3.forward, portalNormal), Vector3.Cross(Vector3.forward, portalNormal)), (Transform) null, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
      portalEffect?.Play(0, false, false);
      return portalEffect;
    }

    private void UpdateCreatureCollision(Creature creature, bool modify = false)
    {
      foreach (Collider componentsInChild in ((Component) creature).GetComponentsInChildren<Collider>())
        componentsInChild.enabled = !modify;
    }

    private IEnumerator HandleCreatureMovement(
      Creature creature,
      Vector3 portalLocation,
      float duration)
    {
      float elapsed = 0.0f;
      while ((double) elapsed < (double) duration)
      {
        yield return (object) new WaitForFixedUpdate();
        Vector3 direction = (portalLocation - ((ThunderBehaviour) creature.ragdoll.targetPart).transform.position).normalized;
        float distance = Vector3.Distance(((ThunderBehaviour) creature.ragdoll.targetPart).transform.position, portalLocation);
        if ((double) distance >= (double) this.PortalStopThreshold)
        {
          creature.AddForce(direction * this.portalPushForceMagnitude, (ForceMode) 5, 1f, (CollisionHandler) null);
          elapsed += Time.fixedDeltaTime;
          direction = new Vector3();
        }
        else
          break;
      }
      List<RagdollPart> parts = creature.ragdoll.parts;
      foreach (RagdollPart part in parts.Where<RagdollPart>((Func<RagdollPart, bool>) (part => PhysicBody.op_Implicit(part.physicBody))))
      {
        part.physicBody.velocity = Vector3.zero;
        part.physicBody.angularVelocity = Vector3.zero;
      }
    }

    private int GetPortalRaycastMask()
    {
      return 0 | 1 << GameManager.GetLayer((LayerName) 0) | 1 << GameManager.GetLayer((LayerName) 1) | 1 << GameManager.GetLayer((LayerName) 10);
    }
  }
}
