// Decompiled with JetBrains decompiler
// Type: Arcana.Behaviors.GravitonLink
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Behaviors
{
  public class GravitonLink : ThunderBehaviour
  {
    public SkillArcaneGravitonCoil skill;
    public Creature mainCreature;
    public Dictionary<Creature, GravitonLink.GravitonLinkData> linkedCreatures;
    private float startTime;

    public void Link(
      SkillArcaneGravitonCoil skill,
      int linkMax,
      float linkRadius,
      float linkDuration)
    {
      this.skill = skill;
      this.linkedCreatures = new Dictionary<Creature, GravitonLink.GravitonLinkData>();
      this.mainCreature = ((Component) this).GetComponentInParent<Creature>();
      foreach (Creature key in Creature.allActive.Where<Creature>((Func<Creature, bool>) (creature => (UnityEngine.Object) creature != (UnityEngine.Object) this.mainCreature & !creature.isKilled && !creature.isPlayer && !creature.isCulled && !this.linkedCreatures.ContainsKey(creature) && !SkillArcaneGravitonCoil.linkedCreatures.Contains(creature) && (double) (((ThunderBehaviour) creature.ragdoll.targetPart).transform.position - ((ThunderBehaviour) this.mainCreature.ragdoll.targetPart).transform.position).sqrMagnitude < (double) linkRadius * (double) linkRadius)).Take<Creature>(linkMax).ToArray<Creature>())
      {
        GravitonLink.GravitonLinkData gravitonLinkData = new GravitonLink.GravitonLinkData()
        {
          target = key,
          targetOffset = ((ThunderBehaviour) key.ragdoll.targetPart).transform.position - ((ThunderBehaviour) this.mainCreature.ragdoll.targetPart).transform.position,
          effect = skill.linkEffectData?.Spawn(((ThunderBehaviour) this.mainCreature.ragdoll.targetPart).transform, true, (ColliderGroup) null, false)
        };
        gravitonLinkData.effect?.SetSourceAndTarget(((ThunderBehaviour) this.mainCreature.ragdoll.targetPart).transform, ((ThunderBehaviour) key.ragdoll.targetPart).transform);
        gravitonLinkData.effect?.Play(0, false, false);
        ((ThunderEntity) key).Inflict(skill.linkStatusData, (object) this, float.PositiveInfinity, (object) null, true);
        key.MaxPush((Creature.PushType) 0, Vector3.zero, (RagdollPart.Type) 0);
        key.ragdoll.SetState((Ragdoll.State) 1);
        this.linkedCreatures.Add(key, gravitonLinkData);
        SkillArcaneGravitonCoil.linkedCreatures.Add(key);
      }
      this.startTime = Time.time;
    }

    public void Unlink()
    {
      foreach (GravitonLink.GravitonLinkData gravitonLinkData in this.linkedCreatures.Values)
      {
        gravitonLinkData.effect?.End(false, -1f);
        ((ThunderEntity) gravitonLinkData.target).ClearByHandler((object) this);
      }
      this.linkedCreatures.Clear();
    }

    public void Update()
    {
      if ((double) Time.time - (double) this.startTime < (double) this.skill.linkDuration)
        return;
      this.Unlink();
      UnityEngine.Object.Destroy((UnityEngine.Object) this);
    }

    public void FixedUpdate()
    {
      if (this.linkedCreatures.Count == 0)
        return;
      foreach (GravitonLink.GravitonLinkData gravitonLinkData in this.linkedCreatures.Values)
      {
        Rigidbody component;
        if (!((Component) gravitonLinkData.target).TryGetComponent<Rigidbody>(out component))
        {
          Debug.Log((object) "No rigidbody on target");
        }
        else
        {
          Vector3 vector3 = (gravitonLinkData.targetOffset - ((ThunderBehaviour) gravitonLinkData.target.ragdoll.targetPart).transform.position) * 10f;
          component.AddForce(vector3, (ForceMode) 5);
        }
      }
    }

    public struct GravitonLinkData
    {
      public Creature target;
      public EffectInstance effect;
      public Vector3 targetOffset;
    }
  }
}
