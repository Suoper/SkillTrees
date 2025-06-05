// Decompiled with JetBrains decompiler
// Type: Crystallic.Ending
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Modules;
using UnityEngine;

#nullable disable
namespace Crystallic
{
  public class Ending
  {
    public static Settings settings;
    public static bool isRunningCrystallicEnding;
    public static List<TowerLaser> lasers = new List<TowerLaser>();

    public static void StartCrystallicEnding(Tower tower)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      EventManager.onLevelUnload += Ending.\u003C\u003EO.\u003C0\u003E__OnLevelUnload ?? (Ending.\u003C\u003EO.\u003C0\u003E__OnLevelUnload = new EventManager.LevelLoadEvent((object) null, __methodptr(OnLevelUnload)));
      if (Ending.isRunningCrystallicEnding)
        return;
      Ending.isRunningCrystallicEnding = true;
      Utils.RunAfter((MonoBehaviour) tower, (Action) (() =>
      {
        Ending.TrapPlayer(tower);
        CrystalHuntProgressionModule progressionModule;
        if (GameModeManager.instance.currentGameMode.TryGetModule<CrystalHuntProgressionModule>(ref progressionModule))
          progressionModule.SetEndGameState((CrystalHuntProgressionModule.EndGameState) 4);
        Ending.settings = Catalog.GetData<Settings>("Settings", true);
        Utils.RunAfter((MonoBehaviour) tower, (Action) (() => Catalog.LoadAssetAsync<RuntimeAnimatorController>(Ending.settings.laserAnimatorControllerAddress, (Action<RuntimeAnimatorController>) (controller =>
        {
          EffectData data = Catalog.GetData<EffectData>(Ending.settings.endingMusicEffectId, true);
          foreach (AudioSource audioSource in UnityEngine.Object.FindObjectsOfType<AudioSource>())
          {
            if (((Component) audioSource).gameObject.name == "AudioMusicEntry")
              audioSource.Stop();
          }
          EffectInstance music1 = data.Spawn(((ThunderBehaviour) Player.currentCreature.ragdoll.targetPart).transform, true, (ColliderGroup) null, false);
          music1.Play(0, false, false);
          foreach (TowerLaserType towerLaserType in Enum.GetValues(typeof (TowerLaserType)))
          {
            TowerLaserType type = towerLaserType;
            GameObject gameObject = GameObject.Find("Laser Mobile " + (type == TowerLaserType.Front ? "Part  " : "Part ") + type.ToString().ToUpper());
            if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
            {
              TowerLaser laser = gameObject.gameObject.AddComponent<TowerLaser>();
              Utils.RunAfter((MonoBehaviour) laser, (Action) (() =>
              {
                laser.Init(type, controller, Ending.settings);
                Ending.lasers.Add(laser);
                if (type != TowerLaserType.Right)
                  return;
                Utils.RunAfter((MonoBehaviour) laser, (Action) (() => Catalog.LoadAssetAsync<GameObject>("Silk.Prefab.Waves", (Action<GameObject>) (obj =>
                {
                  GameObject o = UnityEngine.Object.Instantiate<GameObject>(obj);
                  Utils.RunAfter((MonoBehaviour) laser, (Action) (() =>
                  {
                    music1.Stop(0);
                    Ending.ReleasePlayer(tower);
                    Player.currentCreature.AddForce(-((ThunderBehaviour) Player.currentCreature).transform.forward * 2.5f, (ForceMode) 1, 1f, (CollisionHandler) null);
                    Shaker[] shakers = o.GetComponentsInChildren<Shaker>();
                    shakers[0].Begin();
                    shakers[1].Begin();
                    Utils.RunAfter((MonoBehaviour) shakers[1], (Action) (() => shakers[1].End()), 5f, false);
                    UnityEngine.Object.FindObjectOfType<DalgarianDoor>().CloseDoor();
                    Utils.RunAfter((MonoBehaviour) laser, (Action) (() =>
                    {
                      foreach (CreatureSpawner componentsInChild in o.GetComponentsInChildren<CreatureSpawner>())
                      {
                        componentsInChild.spawnAtRandomWaypoint = true;
                        componentsInChild.SetCreaturesToWaveNPCS();
                        componentsInChild.Spawn();
                      }
                      o.GetComponentsInChildren<WaveSpawner>()[1].StartWave("SoldierInvasion");
                    }), 10f, false);
                  }), 5f, false);
                }), "GameObject.Waves")), 25f, false);
              }), Ending.settings.endingTimings[type], false);
            }
          }
        }), "Controller")), 5f, false);
      }), 5f, false);
    }

    public static void TrapPlayer(Tower tower)
    {
      Player.TogglePlayerMovement(false);
      Player.TogglePlayerJump(false);
      Player.local.creature.mana.casterRight.DisallowCasting((object) tower);
      Player.local.creature.mana.casterRight.DisableSpellWheel((object) tower);
      Player.local.creature.mana.casterLeft.DisallowCasting((object) tower);
      Player.local.creature.mana.casterLeft.DisableSpellWheel((object) tower);
      Player.local.locomotion.physicBody.isKinematic = true;
      ((Behaviour) Player.local.locomotion).enabled = false;
      ((ThunderEntity) Player.currentCreature).SetPhysicModifier((object) tower, new float?(0.0f), 1f, -1f);
      Vector3 startPosition = ((ThunderBehaviour) Player.local).transform.position;
      Extensions.ProgressiveAction((MonoBehaviour) Player.currentCreature, tower.trapToTargetDuration, (Action<float>) (t => Player.local.creature.currentLocomotion.physicBody.transform.position = Vector3.Lerp(startPosition, Player.local.GetPlayerPositionRelativeToHead(tower.shootingPosition.position), tower.levitationForceCurve.Evaluate(t))));
    }

    public static void ReleasePlayer(Tower tower)
    {
      Player.TogglePlayerMovement(true);
      Player.TogglePlayerJump(true);
      Player.local.creature.mana.casterRight.AllowCasting((object) tower);
      Player.local.creature.mana.casterRight.AllowSpellWheel((object) tower);
      Player.local.creature.mana.casterLeft.AllowCasting((object) tower);
      Player.local.creature.mana.casterLeft.AllowSpellWheel((object) tower);
      Player.local.locomotion.physicBody.isKinematic = false;
      ((Behaviour) Player.local.locomotion).enabled = true;
      ((ThunderEntity) Player.currentCreature).SetPhysicModifier((object) tower, new float?(1f), 1f, -1f);
    }

    private static void OnLevelUnload(
      LevelData levelData,
      LevelData.Mode mode,
      EventTime eventTime)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      EventManager.onLevelUnload -= Ending.\u003C\u003EO.\u003C0\u003E__OnLevelUnload ?? (Ending.\u003C\u003EO.\u003C0\u003E__OnLevelUnload = new EventManager.LevelLoadEvent((object) null, __methodptr(OnLevelUnload)));
      Ending.isRunningCrystallicEnding = false;
    }
  }
}
