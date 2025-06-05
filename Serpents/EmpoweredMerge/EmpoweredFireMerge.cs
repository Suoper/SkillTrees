// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.Serpents.EmpoweredMerge.EmpoweredFireMerge
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Behaviors;
using Arcana.Misc;
using Arcana.Skills.SpellMerge;
using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Skills.Serpents.EmpoweredMerge
{
  public class EmpoweredFireMerge : EmpoweredMergeData
  {
    private List<EmpoweredFireMerge.BeamOrbit> beamOrbits;

    public override void OnLoad(SkillArcaneEmpoweringBond skill, SpellMergeData spell)
    {
      base.OnLoad(skill, spell);
      if (!(this.catalogData is SkillPyroclasticLance catalogData))
        return;
      this.beamOrbits = new List<EmpoweredFireMerge.BeamOrbit>();
      for (int index = 0; index < Math.Min(Math.Min(catalogData.beamCount, SkillArcaneSerpents.maxSerpents), SkillArcaneSerpents.serpents.Count); ++index)
      {
        RotateAroundCenter rotateAroundCenter = new GameObject().AddComponent<RotateAroundCenter>();
        rotateAroundCenter.radius = 1f;
        rotateAroundCenter.speed = 280f;
        rotateAroundCenter.moveInSineWave = false;
        rotateAroundCenter.rotateTowardsDirection = false;
        rotateAroundCenter.rotationOffset = (float) ((double) index / (double) Math.Min(catalogData.beamCount, SkillArcaneSerpents.maxSerpents) * 360.0);
        EmpoweredFireMerge.BeamOrbit beamOrbit = new EmpoweredFireMerge.BeamOrbit(this.skillArcaneEmpoweringBond, rotateAroundCenter, SkillArcaneSerpents.serpents[index], catalogData.beamManagers[index]);
        catalogData.OnBeamStartEvent -= new SkillPyroclasticLance.OnBeam(beamOrbit.Start);
        catalogData.OnBeamStartEvent += new SkillPyroclasticLance.OnBeam(beamOrbit.Start);
        catalogData.OnBeamUpdateEvent -= new SkillPyroclasticLance.OnBeam(beamOrbit.Update);
        catalogData.OnBeamUpdateEvent += new SkillPyroclasticLance.OnBeam(beamOrbit.Update);
        catalogData.OnBeamEndEvent -= new SkillPyroclasticLance.OnBeam(beamOrbit.Reset);
        catalogData.OnBeamEndEvent += new SkillPyroclasticLance.OnBeam(beamOrbit.Reset);
        this.beamOrbits.Add(beamOrbit);
      }
    }

    public override void OnUnload(SpellMergeData spell)
    {
      base.OnUnload(spell);
      if (!(this.catalogData is SkillPyroclasticLance catalogData) || Utils.IsNullOrEmpty((ICollection) this.beamOrbits))
        return;
      foreach (EmpoweredFireMerge.BeamOrbit beamOrbit in this.beamOrbits)
      {
        catalogData.OnBeamStartEvent -= new SkillPyroclasticLance.OnBeam(beamOrbit.Start);
        catalogData.OnBeamUpdateEvent -= new SkillPyroclasticLance.OnBeam(beamOrbit.Update);
        catalogData.OnBeamEndEvent -= new SkillPyroclasticLance.OnBeam(beamOrbit.Reset);
        beamOrbit.Destroy();
      }
      this.beamOrbits.Clear();
    }

    public class BeamOrbit
    {
      public RotateAroundCenter rotateAroundCenter;
      public Serpent serpent;
      public BeamManager beamManager;
      public bool isSerpentAttached;
      public bool isOrbitAttached;
      private SkillArcaneEmpoweringBond empoweredSkill;

      public BeamOrbit(
        SkillArcaneEmpoweringBond empoweredSkill,
        RotateAroundCenter rotateAroundCenter,
        Serpent serpent,
        BeamManager beamManager)
      {
        this.rotateAroundCenter = rotateAroundCenter;
        this.serpent = serpent;
        this.beamManager = beamManager;
        this.empoweredSkill = empoweredSkill;
        this.isSerpentAttached = false;
        this.isOrbitAttached = false;
      }

      public void Start(SkillPyroclasticLance skill, BeamManager beam, Mana mana)
      {
        if (beam != this.beamManager || !SkillArcaneEmpoweringBond.AllowEmpoweredMerge)
          return;
        this.serpent.AssignNewOrbit(this.beamManager.BeamStart, true, timeLimit: this.serpent.data.teleportTargetTimeout);
        this.serpent.LoadRotationData(new float?(0.03f), new float?(0.05f), new float?(135f), new float?(30f));
        this.serpent.movementMultiplier = 1.2f;
        this.serpent.tempScale = new float?(1.75f);
        this.serpent.OnOrbitChangeEndEvent -= new Serpent.OnOrbitChangeEnd(this.OnOrbitChange);
        this.serpent.OnOrbitChangeEndEvent += new Serpent.OnOrbitChangeEnd(this.OnOrbitChange);
      }

      private void OnOrbitChange(Serpent serpent, Transform orbitObject, float endTime)
      {
        this.beamManager.overrideBeamControl = true;
        ((MonoBehaviour) GameManager.local).StartCoroutine(Action());

        IEnumerator Action()
        {
          yield return (object) new WaitForEndOfFrame();
          if ((UnityEngine.Object) orbitObject == (UnityEngine.Object) this.beamManager.BeamStart)
          {
            this.isSerpentAttached = true;
            this.isOrbitAttached = false;
            serpent.movementMultiplier = 1.4f;
            serpent.tempScale = new float?(2f);
            yield return (object) new WaitForSeconds(0.2f);
            serpent.AssignNewOrbit(this.rotateAroundCenter.transform, true, timeLimit: serpent.data.teleportTargetTimeout);
          }
          else if ((UnityEngine.Object) orbitObject == (UnityEngine.Object) this.rotateAroundCenter.transform)
          {
            this.isOrbitAttached = true;
            this.isSerpentAttached = false;
            this.serpent.OnOrbitChangeEndEvent -= new Serpent.OnOrbitChangeEnd(this.OnOrbitChange);
          }
          yield return (object) new WaitForEndOfFrame();
        }
      }

      public void Update(SkillPyroclasticLance skill, BeamManager beam, Mana mana)
      {
        if (beam != this.beamManager)
          return;
        Vector3 rhs = Vector3.Slerp(mana.casterLeft.magicSource.up, mana.casterRight.magicSource.up, 0.5f);
        Vector3 lhs = mana.casterRight.magicSource.position - mana.casterLeft.magicSource.position;
        RotateAroundCenter rotateAroundCenter = this.rotateAroundCenter;
        Vector3 position = this.empoweredSkill.MergeTarget.position;
        Vector3 vector3 = Vector3.Cross(lhs, rhs);
        Vector3 normalized1 = vector3.normalized;
        rotateAroundCenter.UpdatePosition(position, normalized1);
        if (!this.beamManager.overrideBeamControl)
          return;
        if (this.isSerpentAttached && !this.isOrbitAttached)
          this.beamManager.beamOrigin = new Vector3?(this.serpent.transform.position);
        else if (this.isOrbitAttached && !this.isSerpentAttached)
          this.beamManager.beamOrigin = new Vector3?(this.rotateAroundCenter.transform.position);
        this.beamManager.overrideBeamRotation = new float?(Mathf.Lerp(this.beamManager.overrideBeamRotation.GetValueOrDefault(3f), 99f, 0.05f));
        ref Ray local = ref this.beamManager.beamRay;
        vector3 = this.beamManager.BeamStart.position - this.empoweredSkill.MergeTarget.position;
        Vector3 normalized2 = vector3.normalized;
        local.direction = normalized2;
      }

      public void Reset(SkillPyroclasticLance skill, BeamManager beam, Mana mana)
      {
        if (beam != this.beamManager)
          return;
        this.serpent.OnOrbitChangeEndEvent -= new Serpent.OnOrbitChangeEnd(this.OnOrbitChange);
        if (this.isOrbitAttached || this.isSerpentAttached)
          this.empoweredSkill.AssignMergeSerpent(this.serpent);
        this.beamManager.overrideBeamControl = false;
        this.beamManager.overrideBeamRotation = new float?();
        this.isSerpentAttached = false;
        this.isOrbitAttached = false;
      }

      public void Destroy()
      {
        this.serpent.OnOrbitChangeEndEvent -= new Serpent.OnOrbitChangeEnd(this.OnOrbitChange);
        if (this.isOrbitAttached || this.isSerpentAttached)
          this.empoweredSkill.AssignMergeSerpent(this.serpent);
        this.beamManager.overrideBeamControl = false;
        this.beamManager.overrideBeamRotation = new float?();
        UnityEngine.Object.Destroy((UnityEngine.Object) this.rotateAroundCenter.gameObject);
      }
    }
  }
}
