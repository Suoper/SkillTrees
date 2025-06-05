// Decompiled with JetBrains decompiler
// Type: Arcana.Behaviors.RotateAroundCenter
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Behaviors
{
  public class RotateAroundCenter : ThunderBehaviour
  {
    public Vector3 centerPosition;
    public float speed = 10f;
    public float radius = 5f;
    public float rotationOffset = 0.0f;
    public Vector3 rotationAxis = Vector3.up;
    public float sineWaveAmplitude = 1f;
    public float sineWaveFrequency = 1f;
    public bool moveInSineWave = true;
    public bool rotateTowardsDirection = true;
    private float currentAngle = 0.0f;
    private float offsetAngle = 0.0f;
    private float sineWaveOffset = 0.0f;

    public event RotateAroundCenter.OnDestroyHandler OnDestroyEvent;

    public void UpdatePosition(Vector3 centerPosition, Vector3 rotationAxis)
    {
      this.centerPosition = centerPosition;
      this.rotationAxis = rotationAxis;
    }

    public void Update() => this.RotateAroundCenterPosition();

    private void RotateAroundCenterPosition()
    {
      this.currentAngle += -this.speed * Time.deltaTime;
      this.offsetAngle = this.currentAngle + this.rotationOffset;
      this.currentAngle %= 360f;
      this.offsetAngle %= 360f;
      this.rotationAxis.Normalize();
      Vector3 vector3_1 = Vector3.Cross(this.rotationAxis, (double) Mathf.Abs(Vector3.Dot(this.rotationAxis, Vector3.up)) > 0.99000000953674316 ? Vector3.forward : Vector3.up);
      vector3_1.Normalize();
      Vector3 vector3_2 = vector3_1 * this.radius;
      Vector3 vector3_3 = Quaternion.AngleAxis(this.offsetAngle, this.rotationAxis) * vector3_2;
      this.sineWaveOffset = this.moveInSineWave ? Mathf.Sin(Time.time * this.sineWaveFrequency) * this.sineWaveAmplitude : 0.0f;
      Vector3 position = this.transform.position;
      this.transform.position = this.centerPosition + vector3_3 + this.rotationAxis * this.sineWaveOffset;
      if (!this.rotateTowardsDirection)
        return;
      this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(this.transform.position - position), Time.deltaTime * (Mathf.Abs(this.speed) / 3f));
    }

    public void OnDestroy()
    {
      RotateAroundCenter.OnDestroyHandler onDestroyEvent = this.OnDestroyEvent;
      if (onDestroyEvent == null)
        return;
      onDestroyEvent();
    }

    public delegate void OnDestroyHandler();
  }
}
