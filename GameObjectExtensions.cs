// Decompiled with JetBrains decompiler
// Type: GameObjectExtensions
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
public static class GameObjectExtensions
{
  public static bool TryGetComponents<T>(this GameObject gameObject, out T[] components) where T : Component
  {
    components = gameObject.GetComponents<T>();
    return components.Length != 0;
  }

  public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component) where T : Component
  {
    component = gameObject.GetComponentInParent<T>();
    return (Object) component != (Object) null;
  }

  public static bool TryGetComponentsInParent<T>(this GameObject gameObject, out T[] components) where T : Component
  {
    components = gameObject.GetComponentsInParent<T>();
    return components.Length != 0;
  }

  public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component) where T : Component
  {
    component = gameObject.GetComponentInChildren<T>();
    return (Object) component != (Object) null;
  }

  public static bool TryGetComponentsInChildren<T>(this GameObject gameObject, out T[] components) where T : Component
  {
    components = gameObject.GetComponentsInChildren<T>();
    return components.Length != 0;
  }

  public static List<T> GetComponentsInImmediateChildren<T>(this Transform origin) where T : Component
  {
    List<T> immediateChildren = new List<T>();
    foreach (Component component1 in origin)
    {
      T component2 = component1.GetComponent<T>();
      if ((Object) component2 != (Object) null)
        immediateChildren.Add(component2);
    }
    return immediateChildren;
  }

  public static T GetComponentInImmediateChildren<T>(this Transform origin) where T : Component
  {
    T immediateChildren = default (T);
    foreach (Component component1 in origin)
    {
      T component2 = component1.GetComponent<T>();
      if ((Object) component2 != (Object) null)
        immediateChildren = component2;
    }
    return immediateChildren;
  }

  public static List<T> GetComponentsInImmediateParent<T>(this Transform origin) where T : Component
  {
    List<T> inImmediateParent = new List<T>();
    inImmediateParent.AddRange((IEnumerable<T>) origin.parent.GetComponents<T>());
    return inImmediateParent;
  }

  public static T GetComponentInImmediateParent<T>(this Transform origin) where T : Component
  {
    T obj = default (T);
    return origin.parent.GetComponent<T>();
  }

  public static Transform GetChildByNameRecursive(this Transform parent, string nameToCheck)
  {
    foreach (Transform parent1 in parent)
    {
      if (parent1.name == nameToCheck)
        return parent1;
      Transform childByNameRecursive = parent1.GetChildByNameRecursive(nameToCheck);
      if ((Object) childByNameRecursive != (Object) null)
        return childByNameRecursive;
    }
    return (Transform) null;
  }

  public static List<Transform> GetChildrenByNameRecursive(
    this Transform parent,
    string nameToCheck)
  {
    List<Transform> childrenByNameRecursive = new List<Transform>();
    foreach (Transform parent1 in parent)
    {
      if (parent1.name == nameToCheck)
        childrenByNameRecursive.Add(parent1);
      childrenByNameRecursive.AddRange((IEnumerable<Transform>) parent1.GetChildrenByNameRecursive(nameToCheck));
    }
    return childrenByNameRecursive;
  }

  public static Transform GetMatchingChild(this Transform origin, string keyword)
  {
    foreach (Transform matchingChild in origin)
    {
      if (matchingChild.name.Contains(keyword))
        return matchingChild;
    }
    return (Transform) null;
  }

  public static List<Transform> GetMatchingChildren(this Transform origin, string keyword)
  {
    List<Transform> matchingChildren = new List<Transform>();
    foreach (Transform transform in origin)
    {
      if (transform.name.Contains(keyword))
        matchingChildren.Add(transform);
    }
    return matchingChildren;
  }

  public static void SmoothLookAt(this Transform transform, Transform target, float duration)
  {
    ((MonoBehaviour) GameManager.local).StartCoroutine(GameObjectExtensions.SmoothLookRoutine(transform, target, duration));
  }

  private static IEnumerator SmoothLookRoutine(
    Transform transform,
    Transform target,
    float duration)
  {
    float timeElapsed = 0.0f;
    Quaternion initialRotation = transform.rotation;
    Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
    while ((double) timeElapsed < (double) duration)
    {
      transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, timeElapsed / duration);
      timeElapsed += Time.deltaTime;
      yield return (object) null;
    }
    transform.rotation = targetRotation;
  }
}
