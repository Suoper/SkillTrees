// Decompiled with JetBrains decompiler
// Type: Crystallic.ForceFieldPresetData
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic
{
  [Serializable]
  public class ForceFieldPresetData : CustomData
  {
    public ParticleSystemForceFieldShape shape;
    public float startRange;
    public float endRange;
    public Vector3 direction;
    public float gravityStrength;
    public float gravityFocus;
    public float rotationSpeed;
    public float rotationAttraction;
    public Vector2 rotationRandomness;
    public float dragStrength;
    public bool mutliplyBySize;
    public bool multiplyByVelocity;

    public ParticleSystemForceField Create(
      EffectInstance effectInstance,
      Vector3 position,
      Quaternion rotation,
      Transform parent = null)
    {
      ParticleSystemForceField systemForceField = this.Setup(effectInstance != null ? ((Component) effectInstance.GetRootParticleSystem())?.gameObject : (GameObject) null);
      ((Component) systemForceField).transform.position = position;
      ((Component) systemForceField).transform.rotation = rotation;
      ((Component) systemForceField).transform.parent = parent;
      return systemForceField;
    }

    public ParticleSystemForceField Create(EffectInstance effectInstance, Transform transform)
    {
      ParticleSystemForceField systemForceField = this.Setup(effectInstance != null ? ((Component) effectInstance.GetRootParticleSystem())?.gameObject : (GameObject) null);
      ((Component) systemForceField).transform.position = transform.position;
      ((Component) systemForceField).transform.rotation = transform.rotation;
      ((Component) systemForceField).transform.parent = transform;
      return systemForceField;
    }

    public ParticleSystemForceField Setup(GameObject gameObject)
    {
      ParticleSystemForceField systemForceField = gameObject?.AddComponent<ParticleSystemForceField>();
      if ((UnityEngine.Object) systemForceField == (UnityEngine.Object) null)
        return (ParticleSystemForceField) null;
      systemForceField.shape = this.shape;
      systemForceField.startRange = this.startRange;
      systemForceField.endRange = this.endRange;
      systemForceField.directionX = ParticleSystem.MinMaxCurve.op_Implicit(this.direction.x);
      systemForceField.directionY = ParticleSystem.MinMaxCurve.op_Implicit(this.direction.y);
      systemForceField.directionZ = ParticleSystem.MinMaxCurve.op_Implicit(this.direction.z);
      systemForceField.gravity = ParticleSystem.MinMaxCurve.op_Implicit(this.gravityStrength);
      systemForceField.rotationSpeed = ParticleSystem.MinMaxCurve.op_Implicit(this.rotationSpeed);
      systemForceField.rotationAttraction = ParticleSystem.MinMaxCurve.op_Implicit(this.rotationAttraction);
      systemForceField.rotationRandomness = this.rotationRandomness;
      systemForceField.drag = ParticleSystem.MinMaxCurve.op_Implicit(this.dragStrength);
      systemForceField.multiplyDragByParticleSize = this.mutliplyBySize;
      systemForceField.multiplyDragByParticleVelocity = this.multiplyByVelocity;
      return systemForceField;
    }
  }
}
