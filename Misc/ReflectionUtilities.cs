// Decompiled with JetBrains decompiler
// Type: Arcana.Misc.ReflectionUtilities
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using System;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace Arcana.Misc
{
  public static class ReflectionUtilities
  {
    public static bool TryGetPrivate<T>(this object obj, string name, out T value)
    {
      value = default (T);
      if (obj == null)
        return false;
      FieldInfo field = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
      if (field == (FieldInfo) null)
      {
        Debug.LogWarning((object) string.Format("Could not find field {0} on type {1}", (object) name, (object) obj.GetType()));
        return false;
      }
      object obj1 = field.GetValue(obj);
      switch (obj1)
      {
        case null:
        case T _:
          ref T local = ref value;
          if (!(obj1 is T obj2))
            obj2 = default (T);
          local = obj2;
          return true;
        default:
          Debug.LogWarning((object) string.Format("Could not get value of field {0} on object {1}. Result was {2} ({3})", (object) field.Name, obj, obj1, (object) obj1.GetType()));
          return false;
      }
    }

    public static bool TrySetPrivate<T>(this object obj, string name, T value)
    {
      if (obj == null)
        return false;
      FieldInfo field = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
      if (field == (FieldInfo) null)
      {
        Debug.LogWarning((object) string.Format("Could not find field {0} on type {1}", (object) name, (object) obj.GetType()));
        return false;
      }
      if (field.FieldType != typeof (T))
      {
        Debug.LogWarning((object) string.Format("Field {0} on type {1} is not of type {2}", (object) name, (object) obj.GetType(), (object) typeof (T)));
        return false;
      }
      field.SetValue(obj, (object) value);
      return true;
    }

    public static bool TryGetPrivateType(this object obj, string name, out Type fieldType)
    {
      fieldType = (Type) null;
      if (obj == null)
        return false;
      FieldInfo field = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
      if (field == (FieldInfo) null)
      {
        Debug.LogWarning((object) string.Format("Could not find field {0} on type {1}", (object) name, (object) obj.GetType()));
        return false;
      }
      fieldType = field.FieldType;
      return true;
    }

    public static bool TryInvokePrivateMethod(
      this object obj,
      string methodName,
      object[] parameters,
      out object result)
    {
      result = (object) null;
      if (obj == null)
      {
        Console.WriteLine("Object is null.");
        return false;
      }
      Type type = obj.GetType();
      MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
      if (method == (MethodInfo) null)
      {
        Console.WriteLine(string.Format("Private method '{0}' not found in {1}.", (object) methodName, (object) type));
        return false;
      }
      try
      {
        result = method.Invoke(obj, parameters);
        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error invoking method '" + methodName + "': " + ex.Message);
        return false;
      }
    }

    public static MethodInfo GetMethod(
      this object obj,
      string methodName,
      BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
    {
      MethodInfo method = obj.GetType().GetMethod(methodName, bindingFlags);
      return method != (MethodInfo) null ? method : (MethodInfo) null;
    }

    public static EventInfo GetEvent(this Type type, string eventName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
    {
      EventInfo eventInfo = type.GetEvent(eventName, bindingFlags);
      return eventInfo != (EventInfo) null ? eventInfo : (EventInfo) null;
    }
  }
}
