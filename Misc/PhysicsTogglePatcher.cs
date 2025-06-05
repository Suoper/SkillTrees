// Decompiled with JetBrains decompiler
// Type: Arcana.Misc.PhysicsTogglePatcher
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Behaviors;
using Arcana.Skills.SpellMerge;
using HarmonyLib;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Misc
{
  [HarmonyPatch(typeof (PhysicsToggleManager), "HasSpecialOverrideAtProximity")]
  public class PhysicsTogglePatcher
  {
    [HarmonyPostfix]
    public static void HasSpecialOverrideAtProximity(
      PhysicsToggleManager __instance,
      ref bool __result,
      Creature creature)
    {
      if (__result || (Object) Player.currentCreature == (Object) null)
        return;
      SpellMergeData mergeInstance = Player.currentCreature.mana.mergeInstance;
      if (!(mergeInstance is SkillPyroclasticLance pyroclasticLance))
      {
        if (mergeInstance is SkillThunderbond skillThunderbond && (!DragonStorm.active || !Extensions.PointInRadius(((ThunderBehaviour) creature.ragdoll.rootPart).transform.position, DragonStorm.lastStormPosition, skillThunderbond.stormRadius)))
          __result = true;
      }
      else
      {
        foreach (BeamManager beamManager in pyroclasticLance.beamManagers)
        {
          if (!pyroclasticLance.beamActive || !new Bounds(((ThunderBehaviour) __instance).transform.position, Vector3.one * 3f).IntersectRay(beamManager.beamRay))
          {
            __result = true;
            break;
          }
        }
      }
      if (BeamManager.all != null)
      {
        foreach (BeamManager beamManager in BeamManager.all)
        {
          if (beamManager.beamActive && !new Bounds(((ThunderBehaviour) __instance).transform.position, Vector3.one * 3f).IntersectRay(beamManager.beamRay))
            __result = true;
        }
      }
      if (!DragonStorm.active || !Extensions.PointInRadius(((ThunderBehaviour) creature.ragdoll.rootPart).transform.position, DragonStorm.lastStormPosition, DragonStorm.lastRadius))
        return;
      __result = true;
    }
  }
}
