// Decompiled with JetBrains decompiler
// Type: Arcana.Behaviors.SmoothFollowAndLookAt
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using UnityEngine;

#nullable disable
namespace Arcana.Behaviors
{
  public class SmoothFollowAndLookAt : MonoBehaviour
  {
    public Transform target;
    public float positionSpeed = 5f;
    public float rotationSpeed = 5f;
    public Vector3 positionOffset = Vector3.zero;
    public float dragFactor = 0.1f;

    public event SmoothFollowAndLookAt.OnUpdateHandler OnUpdateEvent;

    public void Update()
    {
      if ((Object) this.target == (Object) null)
        return;
      this.transform.position = Vector3.Lerp(this.transform.position, this.target.position + this.positionOffset, Time.deltaTime * this.positionSpeed);
      Vector3 forward = this.target.position - this.transform.position;
      if ((double) forward.magnitude > 0.10000000149011612)
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(forward), Time.deltaTime * this.rotationSpeed);
      SmoothFollowAndLookAt.OnUpdateHandler onUpdateEvent = this.OnUpdateEvent;
      if (onUpdateEvent == null)
        return;
      onUpdateEvent();
    }

    public delegate void OnUpdateHandler();
  }
}
