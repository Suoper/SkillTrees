// Decompiled with JetBrains decompiler
// Type: Utils
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
public static class Utils
{
  public static void AddForce(this Player player, Vector3 direction, float speed)
  {
    Player.local.locomotion.velocity = direction.normalized * Player.local.locomotion.velocity.magnitude;
    Player.local.locomotion.physicBody.AddForce(direction.normalized * speed, (ForceMode) 2);
  }

  public static Vector3 GenerateOffsetVector(
    Vector3 origin,
    Vector2 minMaxX,
    Vector2 minMaxZ,
    float minDistFromOrigin,
    float yOffset,
    bool log = false)
  {
    float num1;
    float num2;
    do
    {
      num1 = UnityEngine.Random.Range(minMaxX.x, minMaxX.y);
      num2 = UnityEngine.Random.Range(minMaxZ.x, minMaxZ.y);
    }
    while ((double) Mathf.Sqrt((float) ((double) num1 * (double) num1 + (double) num2 * (double) num2)) < (double) minDistFromOrigin);
    Vector3 offsetVector = new Vector3(origin.x + num1, origin.y + yOffset, origin.z + num2);
    if (log)
      Debug.Log((object) string.Format("X Offset: {0}, Z Offset: {1}", (object) num1, (object) num2));
    return offsetVector;
  }

  public static Vector3 GenerateVectorBasedOn(
    Transform origin,
    Vector2 minMaxX,
    Vector2 minMaxZ,
    float yOffset,
    bool log = false)
  {
    Vector3 forward = origin.forward;
    Vector3 right = origin.right;
    float num1 = UnityEngine.Random.Range(minMaxZ.x, minMaxZ.y);
    float num2 = UnityEngine.Random.Range(minMaxX.x, minMaxX.y);
    Vector3 vector3_1 = forward * num1 + right * num2;
    Vector3 vector3_2 = origin.position + vector3_1;
    if (log)
      Debug.Log((object) string.Format("Forward Offset: {0}, Lateral Offset: {1}", (object) num1, (object) num2));
    return new Vector3(vector3_2.x, origin.position.y * yOffset, vector3_2.z);
  }

  public static List<Vector3> GetListedPointsOnHemisphere(
    Vector3 origin,
    Vector3 direction,
    Vector2 minMaxCount,
    float exclusionRadius,
    float offsetFromOrigin)
  {
    List<Vector3> pointsOnHemisphere = new List<Vector3>();
    int num = UnityEngine.Random.Range(Mathf.FloorToInt(minMaxCount.x), Mathf.FloorToInt(minMaxCount.y) + 1);
    for (int index = 0; index < num; ++index)
    {
      Vector3 onUnitSphere;
      do
      {
        onUnitSphere = UnityEngine.Random.onUnitSphere;
      }
      while ((double) Vector3.Dot(onUnitSphere, direction) < (double) Mathf.Cos(exclusionRadius * ((float) Math.PI / 180f)));
      Vector3 vector3 = origin + onUnitSphere * offsetFromOrigin;
      pointsOnHemisphere.Add(vector3);
    }
    return pointsOnHemisphere;
  }

  public static Vector3 GetDeflectNormal(
    float beamLength,
    Vector3 defaultNormal,
    Transform origin,
    LayerMask mask)
  {
    List<RaycastHit> raycastHitList = new List<RaycastHit>();
    for (int index = 0; index < 3; ++index)
    {
      RaycastHit hit;
      if (Utils.Raycast(Quaternion.AngleAxis(120f * (float) index, Vector3.forward) * Vector3.up * 0.01f, origin, mask, beamLength, out hit))
        raycastHitList.Add(hit);
    }
    switch (raycastHitList.Count)
    {
      case 0:
        return defaultNormal;
      case 3:
        Plane plane = new Plane();
        ref Plane local = ref plane;
        RaycastHit raycastHit1 = raycastHitList[0];
        Vector3 point1 = ((RaycastHit) ref raycastHit1).point;
        RaycastHit raycastHit2 = raycastHitList[1];
        Vector3 point2 = ((RaycastHit) ref raycastHit2).point;
        RaycastHit raycastHit3 = raycastHitList[2];
        Vector3 point3 = ((RaycastHit) ref raycastHit3).point;
        local = new Plane(point1, point2, point3);
        if ((double) Vector3.Dot(plane.normal, origin.forward) > 0.0)
          plane.Flip();
        return plane.normal;
      default:
        Vector3 vector3_1 = defaultNormal;
        for (int index = 0; index < raycastHitList.Count; ++index)
        {
          Vector3 vector3_2 = vector3_1;
          RaycastHit raycastHit4 = raycastHitList[index];
          Vector3 normal = ((RaycastHit) ref raycastHit4).normal;
          vector3_1 = vector3_2 + normal;
        }
        return vector3_1 / (float) (raycastHitList.Count + 1);
    }
  }

  public static bool Raycast(
    Vector3 offset,
    Transform origin,
    LayerMask mask,
    float distance,
    out RaycastHit hit)
  {
    return Physics.Raycast(origin.position + origin.TransformPoint(offset), origin.forward, ref hit, distance, (int) mask, (QueryTriggerInteraction) 1);
  }

  public static ConfigurableJoint CreateConfigurableJoint(
    Rigidbody source,
    Rigidbody target,
    float spring,
    float damper,
    float minDistance,
    float maxDistance,
    float massScale,
    bool enableCollision = true,
    ConfigurableJointMotion motion = 2)
  {
    ConfigurableJoint configurableJoint = ((Component) target).gameObject.AddComponent<ConfigurableJoint>();
    ((Joint) configurableJoint).connectedBody = source;
    ((Joint) configurableJoint).enableCollision = enableCollision;
    SoftJointLimit softJointLimit1 = new SoftJointLimit();
    ((SoftJointLimit) ref softJointLimit1).contactDistance = minDistance;
    ((SoftJointLimit) ref softJointLimit1).limit = maxDistance;
    SoftJointLimit softJointLimit2 = softJointLimit1;
    configurableJoint.linearLimit = softJointLimit2;
    JointDrive jointDrive1 = new JointDrive();
    ((JointDrive) ref jointDrive1).positionSpring = spring;
    ((JointDrive) ref jointDrive1).positionDamper = damper;
    ((JointDrive) ref jointDrive1).maximumForce = float.PositiveInfinity;
    JointDrive jointDrive2 = jointDrive1;
    configurableJoint.xDrive = jointDrive2;
    configurableJoint.yDrive = jointDrive2;
    configurableJoint.zDrive = jointDrive2;
    ((Joint) configurableJoint).massScale = massScale;
    configurableJoint.angularXMotion = (ConfigurableJointMotion) 2;
    configurableJoint.angularYMotion = (ConfigurableJointMotion) 2;
    configurableJoint.angularZMotion = (ConfigurableJointMotion) 2;
    return configurableJoint;
  }
}
