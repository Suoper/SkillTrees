// Decompiled with JetBrains decompiler
// Type: Arcana.Behaviors.ArcaneOrbStatusApplicator
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Skills.SpellMerge;
using Arcana.Statuses;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Behaviors
{
  public class ArcaneOrbStatusApplicator : ThunderBehaviour
  {
    public Zone zone;
    public Item item;
    public ItemMagicAreaProjectile projectile;
    public SkillArcaneOrb skill;
    public float startTime;

    public void Form(ItemMagicAreaProjectile projectile, SkillArcaneOrb skill)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      ArcaneOrbStatusApplicator.\u003C\u003Ec__DisplayClass5_0 cDisplayClass50 = new ArcaneOrbStatusApplicator.\u003C\u003Ec__DisplayClass5_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass50.skill = skill;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass50.\u003C\u003E4__this = this;
      this.projectile = projectile;
      // ISSUE: reference to a compiler-generated field
      this.skill = cDisplayClass50.skill;
      this.item = ((ItemMagicProjectile) projectile).item;
      // ISSUE: method pointer
      this.item.OnDespawnEvent -= new Item.SpawnEvent((object) cDisplayClass50, __methodptr(\u003CForm\u003Eg__Despawn\u007C1));
      // ISSUE: method pointer
      this.item.OnDespawnEvent += new Item.SpawnEvent((object) cDisplayClass50, __methodptr(\u003CForm\u003Eg__Despawn\u007C1));
      ((Collider) this.gameObject.AddComponent<SphereCollider>()).isTrigger = true;
      this.zone = this.gameObject.AddComponent<Zone>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.zone.statusIDs = new List<StatusEntry>()
      {
        StatusEntry.op_Implicit((cDisplayClass50.skill.projectileStatusId, cDisplayClass50.skill.projectileStatusDuration, new float?(cDisplayClass50.skill.projectileStatusTransfer)))
      };
      this.zone.playStatusEffects = true;
      this.zone.statusOnPlayer = false;
      this.zone.statusOnCreature = true;
      this.zone.statusOnItem = true;
      this.zone.constantStatus = true;
      this.zone.forceTransform = this.transform;
      this.zone.forcePlayer = false;
      this.zone.forceNonCreatures = false;
      this.zone.disableArmorDetection = true;
      this.zone.ignoreIdleCreatures = false;
      this.zone.breakBreakables = true;
      this.startTime = Time.time;
      // ISSUE: reference to a compiler-generated field
      this.zone.SetRadius(cDisplayClass50.skill.projectileEffectRadius);
      // ISSUE: reference to a compiler-generated method
      ((MonoBehaviour) this).StartCoroutine(cDisplayClass50.\u003CForm\u003Eg__DespawnRoutine\u007C0());
    }

    public void Update()
    {
      foreach (Creature key in this.zone.creaturesInZone.Keys)
      {
        if (!key.isKilled && !key.isPlayer)
          key.ForceStagger(Vector3.down, (BrainModuleHitReaction.PushBehaviour.Effect) 1, (RagdollPart.Type) 4);
        ArcaneStatus arcaneStatus;
        if (((ThunderEntity) key).TryGetStatus<ArcaneStatus>(this.skill.projectileStatusData, ref arcaneStatus) && this.skill.projectileStatusData is StatusDataArcane projectileStatusData && arcaneStatus.CanExplode)
        {
          ArcaneStatus.Explode(projectileStatusData, key);
          arcaneStatus.EndCharged();
        }
      }
    }
  }
}
