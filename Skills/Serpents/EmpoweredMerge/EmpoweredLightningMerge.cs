// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.Serpents.EmpoweredMerge.EmpoweredLightningMerge
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Behaviors;
using Arcana.Misc;
using Arcana.Skills.SpellMerge;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using ThunderRoad.Skill.Spell;
using UnityEngine;

#nullable disable
namespace Arcana.Skills.Serpents.EmpoweredMerge
{
  public class EmpoweredLightningMerge : EmpoweredMergeData
  {
    public float movementMultiplier = 1.2f;
    public float tempScale = 6f;
    public float lightningNodeLifetimeFactor = 0.5f;
    public float teleportTimeout = 999f;
    private Transform center;
    private float radius;
    private List<Transform> targets;
    public string skillArcwireId = "Arcwire";
    private SkillArcwire skillArcwire;
    private SkillThunderbond skillThunderbond;
    private Dictionary<Serpent, LightningTrailNode> lightningTrailNodes;
    private Dictionary<Serpent, float> lastNodeTime;
    private float timeBetweenNodes = 0.5f;
    private bool forceNodeOnOrbitReached = true;
    private float startTime;

    public override void OnCatalogRefresh()
    {
      base.OnCatalogRefresh();
      this.skillArcwire = Catalog.GetData<SkillArcwire>(this.skillArcwireId, true);
      this.skillThunderbond = Catalog.GetData<SkillThunderbond>("Skill_Thunderbond", true);
    }

    public override void OnLoad(SkillArcaneEmpoweringBond skill, SpellMergeData spell)
    {
      base.OnLoad(skill, spell);
      if (!(spell is SkillThunderbond skillThunderbond))
        return;
      skillThunderbond.OnThunderbondStartEvent -= new SkillThunderbond.ThunderbondEvent(this.OnThunderbondStart);
      skillThunderbond.OnThunderbondStartEvent += new SkillThunderbond.ThunderbondEvent(this.OnThunderbondStart);
      skillThunderbond.OnThunderbondEndEvent -= new SkillThunderbond.ThunderbondEvent(this.OnThunderbondEnd);
      skillThunderbond.OnThunderbondEndEvent += new SkillThunderbond.ThunderbondEvent(this.OnThunderbondEnd);
      Debug.Log((object) string.Format("Loaded Empowered Skill - Thunderbond - {0}", (object) skillThunderbond));
    }

    private void OnThunderbondStart(SkillThunderbond skill, DragonStorm storm)
    {
      if (!SkillArcaneEmpoweringBond.AllowEmpoweredMerge)
        return;
      Debug.Log((object) "Starting Empowered Thunderbond");
      this.startTime = storm.startTime;
      this.center = Utilities.GetTransformCopy(storm.transform);
      this.targets = new List<Transform>();
      this.lightningTrailNodes = new Dictionary<Serpent, LightningTrailNode>();
      this.lastNodeTime = new Dictionary<Serpent, float>();
      this.radius = skill.stormEyeRadius;
      foreach (Serpent serpent in SkillArcaneSerpents.serpents)
      {
        serpent.AssignHandler((object) this);
        serpent.OnOrbitChangeEndEvent -= new Serpent.OnOrbitChangeEnd(this.OnOrbitChange);
        serpent.OnOrbitChangeEndEvent += new Serpent.OnOrbitChangeEnd(this.OnOrbitChange);
        this.OnOrbitChange(serpent, (Transform) null, 0.0f);
        ((MonoBehaviour) GameManager.local).StartCoroutine(this.ArcwireRoutine(serpent));
      }
    }

    private void OnThunderbondEnd(SkillThunderbond skill, DragonStorm storm)
    {
      if (this.center == null)
        return;
      Debug.Log((object) "Ending Empowered Thunderbond");
      foreach (Serpent serpent in SkillArcaneSerpents.serpents.Where<Serpent>((Func<Serpent, bool>) (x => x.orbitHandler == this)))
      {
        serpent.OnOrbitChangeEndEvent -= new Serpent.OnOrbitChangeEnd(this.OnOrbitChange);
        serpent.ResetOrbit(serpent.data.teleportReturnTimeout, (object) this);
        serpent.objectOrbitNormal = UnityEngine.Random.onUnitSphere;
        serpent.movementMultiplier = 1f;
        serpent.tempScale = new float?();
        serpent.ResetRotation();
        serpent.AssignHandler();
      }
      UnityEngine.Object.Destroy((UnityEngine.Object) this.center.gameObject);
      this.center = (Transform) null;
      foreach (Component component in this.targets.Where<Transform>((Func<Transform, bool>) (target => (bool) (UnityEngine.Object) target && (bool) (UnityEngine.Object) target.gameObject)))
        UnityEngine.Object.Destroy((UnityEngine.Object) component.gameObject);
      this.targets.Clear();
    }

    private void OnOrbitChange(Serpent serpent, Transform orbitObject, float endTime)
    {
      Vector3 randomPointInCircle = Utilities.GetRandomPointInCircle(this.center.position, this.radius, this.radius / 6f, this.radius / 12f);
      Transform transform = new GameObject("Target").transform;
      this.targets.Add(transform);
      transform.position = randomPointInCircle;
      if (this.forceNodeOnOrbitReached)
        this.PlaceArcwire(serpent);
      serpent.AssignNewOrbit(transform, true, timeLimit: this.teleportTimeout, handler: (object) this);
      serpent.LoadRotationData(new float?(0.14f), new float?(0.05f), new float?(360f), new float?(90f));
      serpent.movementMultiplier = this.movementMultiplier;
      serpent.tempScale = new float?(this.tempScale);
      serpent.objectOrbitNormal = UnityEngine.Random.onUnitSphere;
      if (!((UnityEngine.Object) orbitObject != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) orbitObject.gameObject);
    }

    private void PlaceArcwire(Serpent serpent)
    {
      LightningTrailNode lightningTrailNode1 = this.lastNodeTime.ContainsKey(serpent) ? this.lightningTrailNodes[serpent] : (LightningTrailNode) null;
      if ((UnityEngine.Object) lightningTrailNode1 != (UnityEngine.Object) null)
      {
        ((ThunderBehaviour) lightningTrailNode1).transform.SetParent((Transform) null);
        ((ThunderBehaviour) lightningTrailNode1).transform.position = serpent.transform.position;
        ((ThunderBehaviour) lightningTrailNode1).transform.rotation = serpent.transform.rotation;
      }
      LightningTrailNode lightningTrailNode2 = LightningTrailNode.New(serpent.transform.position, this.skillArcwire, (Imbue) null, Player.local.creature, serpent.transform, lightningTrailNode1, false);
      lightningTrailNode2.duration = new float?(this.skillThunderbond.stormDuration - (Time.time - this.startTime) * this.lightningNodeLifetimeFactor);
      this.lightningTrailNodes[serpent] = lightningTrailNode2;
      this.lastNodeTime[serpent] = Time.time;
    }

    private IEnumerator ArcwireRoutine(Serpent serpent)
    {
      while (DragonStorm.active)
      {
        if (!this.lastNodeTime.ContainsKey(serpent) || (double) Time.time - (double) this.lastNodeTime[serpent] > (double) this.timeBetweenNodes)
          this.PlaceArcwire(serpent);
        yield return (object) new WaitForEndOfFrame();
      }
    }
  }
}
