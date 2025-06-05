// Decompiled with JetBrains decompiler
// Type: Crystallic.ImbueFireBehavior
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.Skill;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic
{
  public class ImbueFireBehavior : ImbueBehavior
  {
    public EffectData detonateEffectData;
    public string detonateEffectId = "RemoteDetonation";
    public AnimationCurve flameCurve = new AnimationCurve(new Keyframe[3]
    {
      new Keyframe(0.0f, 0.5f),
      new Keyframe(0.05f, 30f),
      new Keyframe(0.1f, 0.5f)
    });

    public override void Activate(Imbue imbue, SkillCrystalImbueHandler handler)
    {
      base.Activate(imbue, handler);
      this.detonateEffectData = Catalog.GetData<EffectData>(this.detonateEffectId, true);
    }

    public override void Hit(
      CollisionInstance collisionInstance,
      SpellCastCharge spellCastCharge,
      Creature hitCreature = null,
      Item hitItem = null)
    {
      base.Hit(collisionInstance, spellCastCharge, hitCreature, hitItem);
      if (!(bool) (Object) hitCreature)
        return;
      Item entity = collisionInstance?.sourceColliderGroup?.collisionHandler?.Entity as Item;
      ((ThunderEntity) hitCreature).SetVariable<int>("HasDetonated", ((ThunderEntity) hitCreature).GetVariable<int>("HasDetonated") + 1);
      int num;
      if (!(bool) (Object) entity || !(bool) (Object) hitCreature || !((Object) hitCreature != (Object) this.imbue.imbueCreature) || !((ThunderEntity) hitCreature).TryGetVariable<int>("HasDetonated", ref num) || num != 2)
        return;
      this.detonateEffectData?.Spawn(((ThunderBehaviour) hitCreature.ragdoll.targetPart).transform, true, (ColliderGroup) null, false).Play(0, false, false);
      ((ThunderEntity) hitCreature).Inflict("Burning", (object) this, float.PositiveInfinity, (object) 100, true);
      if (!hitCreature.isPlayer && hitCreature != null)
        ((ThunderEntity) hitCreature).AddExplosionForce(70f, collisionInstance.contactPoint, 5f, 0.1f, (ForceMode) 1, (CollisionHandler) null);
      entity.PlayHapticClip(this.flameCurve, 0.25f);
      ((ThunderEntity) entity)?.AddForce((((ThunderBehaviour) entity).transform.position - ((ThunderBehaviour) hitCreature).transform.position).normalized * 2f, (ForceMode) 1, (CollisionHandler) null);
    }
  }
}
