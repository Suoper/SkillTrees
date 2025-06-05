// Decompiled with JetBrains decompiler
// Type: Crystallic.Modules.InvasionModule
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using System.Collections;
using ThunderRoad;
using ThunderRoad.Modules;
using UnityEngine;

#nullable disable
namespace Crystallic.Modules
{
  public class InvasionModule : GameModeModule
  {
    [ModOption("Music Volume")]
    [ModOptionCategory("Invasion", 12)]
    [ModOptionFloatValues(0.0f, 1f, 0.05f)]
    public static float volume = 1f;
    [ModOption("Can Invade")]
    [ModOptionCategory("Invasion", 12)]
    public static bool canInvade = true;
    public static bool invasionActive;
    public static string overrideTableId = "SoldierAlertMelee";
    public static string overrideWaveId = "SoldierInvasion";
    public static string loopMusicEffectId = "InvasionMusic";
    public static EffectData loopMusicEffectData;
    public static EffectInstance loopMusicEffect;
    private static InvasionContent invasionContent;
    public static float musicVolume;

    public static bool CanInvade
    {
      get
      {
        CrystalHuntSaveData crystalHuntSaveData;
        return InvasionModule.canInvade && GameModeManager.instance.currentGameMode.TryGetGameModeSaveData<CrystalHuntSaveData>(ref crystalHuntSaveData) && crystalHuntSaveData.endGameState != 1;
      }
    }

    [ModOption("Force Start Invasion", "Forces the invasion to start on the home map, even in sandbox mode.")]
    [ModOptionCategory("Debug", 99)]
    [ModOptionButton]
    public static void ForceStartInvasion(bool _)
    {
      if ((UnityEngine.Object) Level.current == (UnityEngine.Object) null || (UnityEngine.Object) Player.currentCreature == (UnityEngine.Object) null)
        return;
      if (((CatalogData) Level.current.data).id != "Home")
      {
        Debug.LogWarning((object) "Cannot start invasion, Player is not on the home map!");
      }
      else
      {
        InvasionModule.loopMusicEffectData = Catalog.GetData<EffectData>(InvasionModule.loopMusicEffectId, true);
        InvasionContent.GetCurrent().invasionComplete = false;
        InvasionModule.TryStartInvasion(UnityEngine.Object.FindObjectOfType<HomeTower>());
        InvasionModule.ChangeMusic();
      }
    }

    public virtual IEnumerator OnLoadCoroutine()
    {
      InvasionModule.loopMusicEffectData = Catalog.GetData<EffectData>(InvasionModule.loopMusicEffectId, true);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      EventManager.onPossess += InvasionModule.\u003C\u003EO.\u003C0\u003E__Begin ?? (InvasionModule.\u003C\u003EO.\u003C0\u003E__Begin = new EventManager.PossessEvent((object) null, __methodptr(Begin)));
      return ((Module) this).OnLoadCoroutine();
    }

    public virtual void Update()
    {
      ((Module) this).Update();
      if (!InvasionModule.invasionActive || InvasionModule.loopMusicEffect == null)
        return;
      InvasionModule.loopMusicEffect.SetIntensity(InvasionModule.volume);
    }

    public virtual void OnUnload()
    {
      ((Module) this).OnUnload();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      EventManager.onPossess -= InvasionModule.\u003C\u003EO.\u003C0\u003E__Begin ?? (InvasionModule.\u003C\u003EO.\u003C0\u003E__Begin = new EventManager.PossessEvent((object) null, __methodptr(Begin)));
    }

    public static void Begin(Creature creature, EventTime eventTime)
    {
      if (eventTime == 0)
        return;
      HomeTower objectOfType = UnityEngine.Object.FindObjectOfType<HomeTower>();
      InvasionModule.invasionContent = InvasionContent.GetCurrent();
      if (!Player.currentCreature.HasSkill("Crystallic") || CrystalHuntProgressionModule.GetProgressLevel() <= 2 || CrystalHuntProgressionModule.GetProgressLevel() >= CrystalHuntProgressionModule.endRaidLevel || InvasionModule.invasionContent.invasionComplete)
        return;
      InvasionModule.TryStartInvasion(objectOfType);
      InvasionModule.ChangeMusic();
    }

    public static void TryStartInvasion(HomeTower homeTower)
    {
      if (!InvasionModule.CanInvade || !(bool) (UnityEngine.Object) homeTower)
        return;
      InvasionModule.invasionActive = true;
      CreatureSpawner[] spawners = UnityEngine.Object.FindObjectsOfType<CreatureSpawner>(true);
      foreach (CreatureSpawner creatureSpawner in spawners)
        creatureSpawner.creatureTableID = InvasionModule.overrideTableId;
      WaveSpawner[] waveSpawners = UnityEngine.Object.FindObjectsOfType<WaveSpawner>(true);
      CreatureSpawner creatureSpawner1 = new GameObject("InitialSpawner").AddComponent<CreatureSpawner>();
      creatureSpawner1.creatureTableID = InvasionModule.overrideTableId;
      ((Component) creatureSpawner1).transform.position = new Vector3(45.219f, 2.005f, -56.397f);
      ((Component) creatureSpawner1).transform.rotation = new Quaternion(0.0f, -60.844f, 0.0f, 0.0f);
      creatureSpawner1.Spawn();
      foreach (WaveSpawner waveSpawner in waveSpawners)
        waveSpawner.startWaveId = InvasionModule.overrideWaveId;
      homeTower.StartRaid();
      Utils.RunAfter((MonoBehaviour) homeTower, (Action) (() =>
      {
        foreach (CreatureSpawner creatureSpawner2 in spawners)
        {
          creatureSpawner2.enemyConfigType = (CreatureSpawner.EnemyConfigType) 4;
          creatureSpawner2.SetCreaturesToWaveNPCS();
          creatureSpawner2.Spawn();
        }
        foreach (WaveSpawner waveSpawner in waveSpawners)
        {
          waveSpawner.StopWave(false);
          waveSpawner.StartWave(InvasionModule.overrideWaveId);
        }
        foreach (Creature creature in Creature.allActive)
          Player.currentCreature.brain.instance.GetModule<BrainModulePlayer>(true).SetExposure((BrainModulePlayer.Exposure) 3);
      }), 0.5f, false);
    }

    public static void ChangeMusic()
    {
      ((MonoBehaviour) GameManager.local).StartCoroutine(InvasionModule.FadeMusic(1f, 0.0f, 5f));
      InvasionModule.loopMusicEffect = InvasionModule.loopMusicEffectData.Spawn(((ThunderBehaviour) Player.currentCreature).transform, true, (ColliderGroup) null, false);
      Utils.RunAfter((MonoBehaviour) Player.currentCreature, (Action) (() =>
      {
        InvasionModule.loopMusicEffect.Play(0, false, false);
        InvasionModule.loopMusicEffect.SetIntensity(InvasionModule.musicVolume);
      }), 5f, false);
    }

    public static IEnumerator FadeMusic(float start, float end, float time)
    {
      float startTime = Time.realtimeSinceStartup;
      ThunderRoadSettings.current.audioMixer.SetFloat("MusicVolume", Utils.PercentageToDecibels(start));
      while ((double) Time.realtimeSinceStartup - (double) startTime <= (double) time)
      {
        ThunderRoadSettings.current.audioMixer.SetFloat("MusicVolume", Utils.PercentageToDecibels(Mathf.Lerp(start, end, (Time.realtimeSinceStartup - startTime) / time)));
        yield return (object) 0;
      }
      ThunderRoadSettings.current.audioMixer.SetFloat("MusicVolume", Utils.PercentageToDecibels(end));
    }
  }
}
