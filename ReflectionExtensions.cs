// Decompiled with JetBrains decompiler
// Type: ReflectionExtensions
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System.Reflection;

#nullable disable
public static class ReflectionExtensions
{
  public static object GetField(this object obj, string fieldName, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
  {
    FieldInfo field = obj.GetType().GetField(fieldName, flags);
    return field != (FieldInfo) null ? field.GetValue(obj) : (object) null;
  }

  public static MethodInfo GetMethod(this object obj, string methodName, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
  {
    MethodInfo method = obj.GetType().GetMethod(methodName, flags);
    return method != (MethodInfo) null ? method : (MethodInfo) null;
  }

  public static void InvokeMethod(this object obj, string methodName, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
  {
    MethodInfo method = obj.GetType().GetMethod(methodName, flags);
    if (!(method != (MethodInfo) null))
      return;
    method.Invoke(obj, (object[]) null);
  }
}
