// Decompiled with JetBrains decompiler
// Type: Crystallic.GolemPart
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic
{
  public class GolemPart : ThunderBehaviour
  {
    public Part part;
    public List<GolemCrystal> crystals = new List<GolemCrystal>();
    public List<Collider> colliders = new List<Collider>();
    public List<Collider> hitboxes = new List<Collider>();
    public List<Handle> ladders = new List<Handle>();
    public List<GolemPart> childParts = new List<GolemPart>();

    public virtual void Awake()
    {
      this.Clear();
      Transform matchingChild1 = this.transform.GetMatchingChild("Crystal");
      Transform matchingChild2 = this.transform.GetMatchingChild("Ladder");
      Transform matchingChild3 = this.transform.GetMatchingChild("Collider");
      if ((UnityEngine.Object) matchingChild1 != (UnityEngine.Object) null)
      {
        foreach (GolemCrystal componentsInChild in matchingChild1.GetComponentsInChildren<GolemCrystal>())
          this.crystals.Add(componentsInChild);
      }
      if ((UnityEngine.Object) matchingChild2 != (UnityEngine.Object) null)
      {
        foreach (Handle componentsInChild in matchingChild2.GetComponentsInChildren<Handle>())
          this.ladders.Add(componentsInChild);
      }
      if ((UnityEngine.Object) matchingChild3 != (UnityEngine.Object) null)
      {
        foreach (Collider componentsInChild in matchingChild3.GetComponentsInChildren<Collider>((bool) (UnityEngine.Object) this.transform))
        {
          if (!componentsInChild.isTrigger)
            this.colliders.Add(componentsInChild);
        }
      }
      foreach (Collider inImmediateChild in this.transform.GetComponentsInImmediateChildren<Collider>())
      {
        if (inImmediateChild.isTrigger)
          this.hitboxes.Add(inImmediateChild);
      }
    }

    public void OnDestroy() => this.Clear();

    public virtual EffectInstance SpawnEffect(
      EffectData effectData,
      bool useTransform,
      bool parentToPart)
    {
      if (useTransform)
        return effectData.Spawn(this.transform, true, (ColliderGroup) null, false);
      Transform transform = parentToPart ? this.transform : (Transform) null;
      return effectData.Spawn(this.transform.position, this.transform.rotation, transform, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
    }

    public virtual void Clear()
    {
      this.crystals.Clear();
      this.ladders.Clear();
      this.colliders.Clear();
      this.hitboxes.Clear();
      this.childParts.Clear();
    }

    public virtual void UpdateChildParts()
    {
      this.childParts.AddRange((IEnumerable<GolemPart>) ((Component) this).GetComponentsInChildren<GolemPart>());
    }
  }
}
