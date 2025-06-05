// Decompiled with JetBrains decompiler
// Type: Crystallic.Settings
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System.Collections.Generic;
using ThunderRoad;

#nullable disable
namespace Crystallic
{
  public class Settings : CustomData
  {
    [ModOption("Debug Mode", "Debugging option for testers. This will spam your log, please do not complain to me if you find it annoying.")]
    [ModOptionCategory("Debug", 99)]
    public static bool debug;
    public string endingMusicEffectId;
    public Dictionary<TowerLaserType, float> endingTimings = new Dictionary<TowerLaserType, float>();
    public string laserAnimatorControllerAddress;
    public string laserFireEffectId;
    public string laserLoadEffectId;
    public string laserMechanicsEffectId;
    public Dictionary<string, List<string>> skills = new Dictionary<string, List<string>>();
    public string spellId;
    public string wellEffectId = "EndingWell";
  }
}
