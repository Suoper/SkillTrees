// Decompiled with JetBrains decompiler
// Type: Arcana.Misc.UnityExtensions
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using UnityEngine;

#nullable disable
namespace Arcana.Misc
{
  internal static class UnityExtensions
  {
    public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component) where T : Component
    {
      component = gameObject.GetComponentInParent<T>();
      return (Object) component != (Object) null;
    }

    public static bool TryGetComponentInParent<T>(this Component component, out T component2) where T : Component
    {
      return component.gameObject.TryGetComponentInParent<T>(out component2);
    }

    public static bool TryGetComponentInParent<T>(this Transform transform, out T component) where T : Component
    {
      return transform.gameObject.TryGetComponentInParent<T>(out component);
    }
  }
}
