// Decompiled with JetBrains decompiler
// Type: Crystallic.Part
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;

#nullable disable
namespace Crystallic
{
  [Flags]
  public enum Part
  {
    Hips = 0,
    RightUpperLeg = 1,
    LeftUpperLeg = 2,
    RightLowerLeg = LeftUpperLeg | RightUpperLeg, // 0x00000003
    LeftLowerLeg = 4,
    Spine = LeftLowerLeg | RightLowerLeg, // 0x00000007
    Chest = 8,
    Neck = Chest | LeftUpperLeg, // 0x0000000A
    Head = Neck | RightUpperLeg, // 0x0000000B
    RightShoulder = Chest | LeftLowerLeg | RightUpperLeg, // 0x0000000D
    RightUpperArm = Neck | LeftLowerLeg, // 0x0000000E
    LeftUpperArm = RightUpperArm | RightUpperLeg, // 0x0000000F
    RightLowerArm = 16, // 0x00000010
    LeftLowerArm = RightLowerArm | RightUpperLeg, // 0x00000011
  }
}
