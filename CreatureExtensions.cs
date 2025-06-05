// Decompiled with JetBrains decompiler
// Type: CreatureExtensions
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using ThunderRoad;
using UnityEngine;

#nullable disable
public static class CreatureExtensions
{
  public static bool GetClosestPart(
    this Ragdoll ragdoll,
    Vector3 position,
    float maxDistance,
    out RagdollPart ragdollPart)
  {
    ragdollPart = (RagdollPart) null;
    float num1 = maxDistance;
    foreach (RagdollPart part in ragdoll.parts)
    {
      float num2 = Vector3.Distance(((ThunderBehaviour) part).transform.position, position);
      if ((double) num2 < (double) num1)
      {
        num1 = num2;
        ragdollPart = part;
      }
    }
    return (Object) ragdollPart != (Object) null;
  }

  public static bool Active(this Creature creature) => !creature.isKilled && !creature.isCulled;

  public static void DamagePatched(this Creature creature, float damage, DamageType damageType)
  {
    Creature creature1 = creature;
    DamageStruct damageStruct;
    // ISSUE: explicit constructor call
    ((DamageStruct) ref damageStruct).\u002Ector(damageType, damage);
    damageStruct.hitRagdollPart = creature.ragdoll.targetPart;
    CollisionInstance collisionInstance = new CollisionInstance(damageStruct, (MaterialData) null, (MaterialData) null);
    creature1.Damage(collisionInstance);
  }

  public static void Disarm(this Creature creature)
  {
    creature.handLeft.TryRelease();
    creature.handRight.TryRelease();
  }

  public static Creature GetClosestCreature(this Creature creature, float maxDistance)
  {
    Creature closestCreature = (Creature) null;
    float num1 = float.PositiveInfinity;
    foreach (Creature creature1 in Creature.allActive)
    {
      if (!creature.isPlayer)
      {
        float num2 = Vector3.Distance(((ThunderBehaviour) creature.ragdoll.targetPart).transform.position, ((ThunderBehaviour) creature).transform.position);
        if ((double) num2 < (double) num1 && (double) num2 <= (double) maxDistance)
        {
          closestCreature = creature1;
          num1 = num2;
        }
      }
    }
    return closestCreature;
  }

  public static void Shred(this Creature creature)
  {
    creature.Kill();
    for (int index = creature.ragdoll.parts.Count - 1; index >= 0; --index)
    {
      RagdollPart part = creature.ragdoll.parts[index];
      if ((Object) creature.ragdoll.rootPart != (Object) part && part.sliceAllowed)
        part.TrySlice();
    }
  }
}
