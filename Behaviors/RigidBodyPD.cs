// Decompiled with JetBrains decompiler
// Type: Arcana.Behaviors.RigidBodyPD
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using UnityEngine;

#nullable disable
namespace Arcana.Behaviors
{
  public class RigidBodyPD : MonoBehaviour
  {
    [Header("Rigidbody")]
    public Rigidbody rb;
    [Header("Position")]
    public bool usePosition = true;
    public Transform targetPosition;
    public float floatStrength = 10f;
    public float damping = 5f;
    public bool clampSpeed = false;
    [Header("Rotation")]
    public RigidBodyPD.LookMode lookMode = RigidBodyPD.LookMode.None;
    public Quaternion targetRotation = Quaternion.identity;
    public Transform lookTarget;
    public float rotationSpeed = 10f;
    public float maxSpinSpeed = 720f;
    public float spinFalloffDistance = 5f;
    public bool usePD = true;

    public void Toggle(bool active) => this.usePD = active;

    public void SetTarget(Transform target) => this.targetPosition = target;

    public void Awake()
    {
      Rigidbody component;
      if (!((Object) this.rb == (Object) null) || !this.TryGetComponent<Rigidbody>(out component))
        return;
      this.rb = component;
    }

    public void LateUpdate()
    {
      if (!this.usePD || !(bool) (Object) this.rb || !(bool) (Object) this.targetPosition)
        return;
      if (this.usePosition)
      {
        Vector3 vector3 = this.targetPosition.position - this.transform.position;
        if (this.clampSpeed)
          vector3 = vector3.normalized;
        this.rb.AddForce(vector3 * this.floatStrength + -this.rb.velocity * this.damping, (ForceMode) 5);
      }
      if (this.lookMode == 0)
        return;
      Vector3 vector3_1 = this.targetPosition.position - this.transform.position;
      Vector3 normalized = vector3_1.normalized;
      switch (this.lookMode)
      {
        case RigidBodyPD.LookMode.LookRotation:
          if ((double) normalized.sqrMagnitude > 0.0099999997764825821)
          {
            this.rb.MoveRotation(Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(normalized), this.rotationSpeed * Time.fixedDeltaTime));
            break;
          }
          break;
        case RigidBodyPD.LookMode.RotatonOverDistance:
          if ((double) normalized.sqrMagnitude > 0.0099999997764825821)
          {
            this.rb.MoveRotation(this.rb.rotation * Quaternion.AngleAxis(this.maxSpinSpeed * Mathf.Clamp01(Vector3.Distance(this.transform.position, this.targetPosition.position) / this.spinFalloffDistance) * Time.deltaTime, this.transform.up));
            break;
          }
          break;
        case RigidBodyPD.LookMode.LookAtTarget:
          if (!((Object) this.lookTarget == (Object) null))
          {
            vector3_1 = this.lookTarget.position - this.transform.position;
            this.rb.MoveRotation(Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(vector3_1.normalized), this.rotationSpeed * Time.fixedDeltaTime));
            break;
          }
          break;
        case RigidBodyPD.LookMode.LookInDirection:
          this.rb.MoveRotation(Quaternion.Slerp(this.transform.rotation, this.targetRotation, this.rotationSpeed * Time.fixedDeltaTime));
          break;
        case RigidBodyPD.LookMode.AlignWithTarget:
          this.rb.MoveRotation(Quaternion.Slerp(this.transform.rotation, this.targetPosition.rotation, this.rotationSpeed * Time.fixedDeltaTime));
          break;
      }
    }

    public enum LookMode
    {
      None,
      LookRotation,
      RotatonOverDistance,
      LookAtTarget,
      LookInDirection,
      AlignWithTarget,
    }
  }
}
