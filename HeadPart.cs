// Decompiled with JetBrains decompiler
// Type: Crystallic.HeadPart
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic
{
  public class HeadPart : GolemPart
  {
    public GameObject faceplate;
    public GameObject vfxTarget;
    public HeadPart.CrystalSocket crystalSocket;

    public override void Awake()
    {
      this.faceplate = this.transform.GetMatchingChild("Faceplate").gameObject;
      this.vfxTarget = this.transform.GetMatchingChild("vfx_golem_roar_scan").gameObject;
      this.crystalSocket = this.transform.GetMatchingChild("Crystal").gameObject.AddComponent<HeadPart.CrystalSocket>();
    }

    public class CrystalSocket : ThunderBehaviour
    {
      public ConfigurableJoint heldJoint;
      public Item crystal;
      public ParticleSystem deathParticleSystem;

      public void Awake()
      {
        this.heldJoint = ((Component) this).GetComponentInChildren<ConfigurableJoint>();
        this.crystal = ((Component) this).GetComponentInChildren<Item>();
        this.deathParticleSystem = this.transform.GetMatchingChild("vfx_golem_CrystalTearingExplosion").gameObject.GetComponent<ParticleSystem>();
      }
    }
  }
}
