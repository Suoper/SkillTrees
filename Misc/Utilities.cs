// Decompiled with JetBrains decompiler
// Type: Arcana.Misc.Utilities
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using ThunderRoad.DebugViz;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

#nullable disable
namespace Arcana.Misc
{
  internal static class Utilities
  {
    public static bool MostlyX(this Vector3 vec)
    {
      return (double) Mathf.Abs(vec.x) > (double) Mathf.Abs(vec.y) && (double) Mathf.Abs(vec.x) > (double) Mathf.Abs(vec.z);
    }

    public static bool MostlyY(this Vector3 vec)
    {
      return (double) Mathf.Abs(vec.y) > (double) Mathf.Abs(vec.x) && (double) Mathf.Abs(vec.y) > (double) Mathf.Abs(vec.z);
    }

    public static bool MostlyZ(this Vector3 vec)
    {
      return (double) Mathf.Abs(vec.z) > (double) Mathf.Abs(vec.x) && (double) Mathf.Abs(vec.z) > (double) Mathf.Abs(vec.y);
    }

    public static bool MostlyXY(this Vector3 vec)
    {
      return (double) Mathf.Abs(vec.x) > (double) Mathf.Abs(vec.z) || (double) Mathf.Abs(vec.y) > (double) Mathf.Abs(vec.z);
    }

    public static bool MostlyYZ(this Vector3 vec)
    {
      return (double) Mathf.Abs(vec.y) > (double) Mathf.Abs(vec.x) || (double) Mathf.Abs(vec.z) > (double) Mathf.Abs(vec.x);
    }

    public static bool MostlyXZ(this Vector3 vec)
    {
      return (double) Mathf.Abs(vec.x) > (double) Mathf.Abs(vec.y) || (double) Mathf.Abs(vec.z) > (double) Mathf.Abs(vec.y);
    }

    public static Transform GetTransformCopy(Transform transform)
    {
      if ((UnityEngine.Object) transform == (UnityEngine.Object) null)
        return (Transform) null;
      Transform transform1 = new GameObject().transform;
      transform1.position = transform.position;
      transform1.rotation = transform.rotation;
      transform1.localScale = transform.localScale;
      return transform1;
    }

    public static Vector3 GetWorldHandVelocity(Side side)
    {
      Vector3 handVelocity = PlayerControl.GetHand(side).GetHandVelocity();
      return ((ThunderBehaviour) Player.local).transform.rotation * handVelocity - ((ThunderEntity) Player.local.creature).Velocity;
    }

    public static Creature[] GetCreaturesInRadius(Vector3 center, float radius, int count)
    {
      return Creature.allActive.Where<Creature>((Func<Creature, bool>) (creature => !creature.isKilled && !creature.isPlayer && !creature.isCulled && (double) (((ThunderBehaviour) creature.ragdoll.targetPart).transform.position - center).sqrMagnitude < (double) radius * (double) radius)).Take<Creature>(count).ToArray<Creature>();
    }

    public static Creature[] GetCreaturesInCone(
      Vector3 center,
      Vector3 direction,
      float radius,
      float angle,
      int count,
      bool ignoreVertical = false)
    {
      List<Creature> creatureList = new List<Creature>();
      foreach (Creature creaturesInRadiu in Utilities.GetCreaturesInRadius(center, radius, count))
      {
        Vector3 vector3 = ((ThunderBehaviour) creaturesInRadiu.ragdoll.targetPart).transform.position - center;
        if (ignoreVertical)
          vector3.y = 0.0f;
        if ((double) Vector3.Angle(vector3.normalized, direction.normalized) < (double) angle / 2.0)
          creatureList.Add(creaturesInRadiu);
      }
      return creatureList.ToArray();
    }

    public static Vector3 GetRandomVelocityBetweenVectors(
      Vector3 from,
      Vector3 to,
      float magnitude,
      float maxAngle = 0.0f,
      float minAngle = 0.0f)
    {
      float max = Vector3.Angle(from.normalized, to.normalized);
      float num = Mathf.Clamp(minAngle, 0.0f, max);
      float maxInclusive = Mathf.Clamp(maxAngle, num, max);
      float angle = UnityEngine.Random.Range(num, maxInclusive);
      Vector3 axis = Vector3.Cross(from, to).normalized;
      if (axis == Vector3.zero)
        axis = Vector3.up;
      return Quaternion.AngleAxis(angle, axis) * from * magnitude;
    }

    public static Vector3 GetRandomVelocityInCone(
      Vector3 centerDirection,
      float coneAngle,
      float magnitude,
      float minAngle = 0.0f)
    {
      centerDirection.Normalize();
      float f1 = UnityEngine.Random.Range(minAngle, coneAngle) * ((float) Math.PI / 180f);
      float f2 = UnityEngine.Random.Range(0.0f, 6.28318548f);
      Vector3 vector3 = new Vector3(Mathf.Cos(f2) * Mathf.Sin(f1), Mathf.Sin(f2) * Mathf.Sin(f1), Mathf.Cos(f1));
      return Quaternion.FromToRotation(Vector3.forward, centerDirection) * vector3 * magnitude;
    }

    public static Vector3 GetRandomDirectionInCircle(
      Vector3 centerPoint,
      float magnitude,
      float yVariance = 0.0f)
    {
      float f = UnityEngine.Random.Range(0.0f, 6.28318548f);
      return new Vector3(Mathf.Cos(f), 0.0f, Mathf.Sin(f))
      {
        y = UnityEngine.Random.Range(-yVariance, yVariance)
      } * magnitude;
    }

    public static Vector3 GetRandomPointInCircle(
      Vector3 centerPoint,
      float maxRadius,
      float minRadius = 0.0f,
      float yVariance = 0.0f)
    {
      float num = UnityEngine.Random.Range(minRadius, maxRadius);
      float f = UnityEngine.Random.Range(0.0f, 6.28318548f);
      return centerPoint + new Vector3(Mathf.Cos(f) * num, 0.0f, Mathf.Sin(f) * num)
      {
        y = UnityEngine.Random.Range(-yVariance, yVariance)
      };
    }

    public static Vector3 GetPointOnArc(
      Vector3 position,
      Vector3 normal,
      int index,
      int numberOfPoints,
      float radius = 1f,
      float totalAngle = 200f)
    {
      float num1 = -90f;
      float num2 = (float) (((double) totalAngle / 2.0 + (double) num1) * (Math.PI / 180.0));
      float num3 = ((float) ((-((double) totalAngle / 2.0) + (double) num1) * (Math.PI / 180.0)) - num2) / (float) (numberOfPoints - 1);
      Vector3 normalized1 = Vector3.Cross(normal, Vector3.up).normalized;
      if ((double) normalized1.sqrMagnitude < 1.0 / 1000.0)
        normalized1 = Vector3.Cross(normal, Vector3.forward).normalized;
      Vector3 normalized2 = Vector3.Cross(normal, normalized1).normalized;
      float f = num2 + (float) index * num3;
      Vector3 vector3 = Mathf.Cos(f) * normalized1 + Mathf.Sin(f) * normalized2;
      return Vector3.ProjectOnPlane(position + vector3 * radius - position, normal) + position;
    }

    public static Vector3 GetPointOnCircle(
      Vector3 position,
      Vector3 normal,
      int index,
      int numberOfPoints,
      float radius = 1f)
    {
      float num = 360f / (float) numberOfPoints;
      float f = (float) ((double) index * (double) num * (Math.PI / 180.0));
      Vector3 normalized1 = Vector3.Cross(normal, Vector3.up).normalized;
      if ((double) normalized1.sqrMagnitude < 1.0 / 1000.0)
        normalized1 = Vector3.Cross(normal, Vector3.forward).normalized;
      Vector3 normalized2 = Vector3.Cross(normal, normalized1).normalized;
      Vector3 vector3 = Mathf.Cos(f) * normalized1 + Mathf.Sin(f) * normalized2;
      return Vector3.ProjectOnPlane(position + vector3 * radius - position, normal) + position;
    }

    public static Vector3 RoughClosestPoint(this Creature creature, Vector3 point)
    {
      Vector3 position = ((ThunderBehaviour) creature.ragdoll.targetPart).transform.position;
      return new Ray(position, point - position).GetPoint(creature.GetHeight() / 2f);
    }

    public static int GetProjectileRaycastMask()
    {
      return 0 | 1 << GameManager.GetLayer((LayerName) 9) | 1 << GameManager.GetLayer((LayerName) 8) | 1 << GameManager.GetLayer((LayerName) 21) | 1 << GameManager.GetLayer((LayerName) 1) | 1 << GameManager.GetLayer((LayerName) 10) | 1 << GameManager.GetLayer((LayerName) 24) | 1 << GameManager.GetLayer((LayerName) 11);
    }

    public static bool IsLayerInMask(int layer, LayerMask mask) => (mask.value & 1 << layer) != 0;

    public static void StartAndTrackCoroutine(
      this MonoBehaviour handler,
      IEnumerator coroutine,
      Action onComplete)
    {
      handler.StartCoroutine(coroutine);
      if (onComplete == null)
        return;
      onComplete();
    }

    public static IEnumerable<T> GetEnumValues<T>() => (IEnumerable<T>) Enum.GetValues(typeof (T));

    public static IEnumerator LoadResource<T>(string address, string id, out T resource) where T : new()
    {
      T localResource = default (T);
      IEnumerator enumerator = Catalog.LoadLocationCoroutine<T>(address, (Action<IResourceLocation>) (resourceLocation => Addressables.LoadAssetAsync<T>((object) address).Completed += (Action<AsyncOperationHandle<T>>) (handle =>
      {
        if (handle.Status == 1)
        {
          Debug.Log((object) ("Loaded resource: " + address));
          localResource = handle.Result;
        }
        else
          Debug.LogWarning((object) ("Failed to load resource: " + address));
      })), id);
      resource = localResource;
      return enumerator;
    }

    public static void DamagePatched(this Creature creature, float damage, DamageType damageType)
    {
      Creature creature1 = creature;
      DamageStruct damageStruct;
      // ISSUE: explicit constructor call
      ((DamageStruct) ref damageStruct).\u002Ector(damageType, damage);
      damageStruct.hitRagdollPart = creature.ragdoll.targetPart;
      CollisionInstance collisionInstance = new CollisionInstance(damageStruct, (MaterialData) null, (MaterialData) null);
      creature1.Damage(collisionInstance);
    }

    public class Draw
    {
      public static Lines VizDrawCircle(
        object obj,
        Vector3 center,
        Vector3 normal,
        float radius,
        float width,
        Color color,
        int segments = 36)
      {
        Vector3[] vector3Array = new Vector3[segments];
        for (int index = 0; index < segments; ++index)
          vector3Array[index] = Utilities.GetPointOnCircle(center, normal, index, segments, radius);
        return Viz.Lines(obj, true).SetLoop(true).Color(color).SetPoints(vector3Array).Show().Width(width, -1f);
      }
    }
  }
}
