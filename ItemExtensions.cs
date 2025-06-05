// Decompiled with JetBrains decompiler
// Type: ItemExtensions
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
public static class ItemExtensions
{
  public static bool HasImbue(this List<Imbue> imbues, string id)
  {
    bool flag = false;
    for (int index = 0; index < imbues.Count; ++index)
    {
      if (((CatalogData) imbues[index].spellCastBase).id == id)
        flag = true;
    }
    return flag;
  }

  public static void PlayHapticClip(this Item item, AnimationCurve curve, float time)
  {
    foreach (RagdollHand handler in item.handlers)
      handler.PlayHapticClipOver(curve, time);
  }

  public static void PointItemFlyRefAtTarget(
    this Item item,
    Vector3 target,
    float lerpFactor,
    Vector3? upDir = null)
  {
    Vector3 upwards = upDir ?? Vector3.up;
    if ((bool) (Object) item.flyDirRef)
      ((ThunderBehaviour) item).transform.rotation = Quaternion.Slerp(((ThunderBehaviour) item).transform.rotation * item.flyDirRef.localRotation, Quaternion.LookRotation(target, upwards), lerpFactor) * Quaternion.Inverse(item.flyDirRef.localRotation);
    else if ((bool) (Object) item.holderPoint)
    {
      ((ThunderBehaviour) item).transform.rotation = Quaternion.Slerp(((ThunderBehaviour) item).transform.rotation * item.holderPoint.localRotation, Quaternion.LookRotation(target, upwards), lerpFactor) * Quaternion.Inverse(item.holderPoint.localRotation);
    }
    else
    {
      Quaternion rotation = Quaternion.LookRotation(((ThunderBehaviour) item).transform.up, upwards);
      ((ThunderBehaviour) item).transform.rotation = Quaternion.Slerp(((ThunderBehaviour) item).transform.rotation * rotation, Quaternion.LookRotation(target, upwards), lerpFactor) * Quaternion.Inverse(rotation);
    }
  }

  public static Quaternion GetFlyDirRefLocalRotation(this Item item)
  {
    return Quaternion.Inverse(((ThunderBehaviour) item).transform.rotation) * item.flyDirRef.rotation;
  }

  public static void IgnoreCollider(this Item item, Collider collider, bool ignore)
  {
    foreach (ColliderGroup colliderGroup in item.colliderGroups)
    {
      foreach (Collider collider1 in colliderGroup.colliders)
        Physics.IgnoreCollision(collider, collider1, ignore);
    }
  }

  public static Collider GetFurthestCollider(this ColliderGroup colliderGroup, Vector3 point)
  {
    Collider furthestCollider = (Collider) null;
    foreach (Collider collider in colliderGroup.colliders)
    {
      if ((Object) furthestCollider == (Object) null || (double) Vector3.Distance(((Component) collider).transform.position, point) > (double) Vector3.Distance(((Component) furthestCollider).transform.position, point))
        furthestCollider = collider;
    }
    return furthestCollider;
  }

  public static Collider GetClosestCollider(this ColliderGroup colliderGroup, Vector3 point)
  {
    Collider closestCollider = (Collider) null;
    foreach (Collider collider in colliderGroup.colliders)
    {
      if ((Object) closestCollider == (Object) null || (double) Vector3.Distance(((Component) collider).transform.position, point) < (double) Vector3.Distance(((Component) closestCollider).transform.position, point))
        closestCollider = collider;
    }
    return closestCollider;
  }
}
