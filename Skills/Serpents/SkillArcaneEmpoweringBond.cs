// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.Serpents.SkillArcaneEmpoweringBond
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Behaviors;
using Arcana.Skills.Serpents.EmpoweredMerge;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Arcana.Skills.Serpents
{
  public class SkillArcaneEmpoweringBond : SpellSkillData
  {
    public string serpentSkillId = "Skill_ArcaneSerpents";
    private SkillArcaneSerpents serpentSkill;
    public List<EmpoweredMergeData> mergeDatas;
    private bool isMerging;
    private EmpoweredMergeData currentMergeData;
    private static bool isMergeEmpowering;
    private Coroutine resetRoutine;

    public Transform MergeTarget { get; private set; }

    public static List<Serpent> MergeSerpents { get; private set; } = new List<Serpent>();

    public static bool AllowEmpoweredMerge
    {
      get
      {
        return SkillArcaneEmpoweringBond.isMergeEmpowering && SkillArcaneSerpents.serpents.Count >= SkillArcaneSerpents.maxSerpents && !SkillArcaneSerpents.serpents.Any<Serpent>((Func<Serpent, bool>) (x => x.isAttacking || !(x.orbitHandler is SkillArcaneEmpoweringBond)));
      }
    }

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      foreach (CatalogData mergeData in this.mergeDatas)
        mergeData.OnCatalogRefresh();
    }

    public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnLateSkillsLoaded(skillData, creature);
      SkillArcaneSerpents skillArcaneSerpents;
      if (!creature.TryGetSkill<SkillArcaneSerpents>(this.serpentSkillId, ref skillArcaneSerpents))
        return;
      this.serpentSkill = skillArcaneSerpents;
      foreach (EmpoweredMergeData mergeData in this.mergeDatas)
      {
        SpellMergeData spellMergeData;
        if (creature.TryGetSkill<SpellMergeData>(mergeData.mergeSkillId, ref spellMergeData))
        {
          // ISSUE: method pointer
          spellMergeData.OnMergeStartEvent -= new SpellMergeData.MergeEvent((object) this, __methodptr(OnMergeStart));
          // ISSUE: method pointer
          spellMergeData.OnMergeStartEvent += new SpellMergeData.MergeEvent((object) this, __methodptr(OnMergeStart));
          // ISSUE: method pointer
          spellMergeData.OnMergeEndEvent -= new SpellMergeData.MergeEvent((object) this, __methodptr(OnMergeEnd));
          // ISSUE: method pointer
          spellMergeData.OnMergeEndEvent += new SpellMergeData.MergeEvent((object) this, __methodptr(OnMergeEnd));
        }
      }
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      SkillArcaneSerpents skillArcaneSerpents;
      if (!creature.TryGetSkill<SkillArcaneSerpents>(this.serpentSkillId, ref skillArcaneSerpents))
        return;
      this.serpentSkill = (SkillArcaneSerpents) null;
      foreach (EmpoweredMergeData mergeData in this.mergeDatas)
      {
        SpellMergeData spellMergeData;
        if (creature.TryGetSkill<SpellMergeData>(mergeData.mergeSkillId, ref spellMergeData))
        {
          // ISSUE: method pointer
          spellMergeData.OnMergeStartEvent -= new SpellMergeData.MergeEvent((object) this, __methodptr(OnMergeStart));
          // ISSUE: method pointer
          spellMergeData.OnMergeEndEvent -= new SpellMergeData.MergeEvent((object) this, __methodptr(OnMergeEnd));
        }
      }
    }

    private void OnMergeStart(SpellMergeData spell)
    {
      this.currentMergeData = this.mergeDatas.FirstOrDefault<EmpoweredMergeData>((Func<EmpoweredMergeData, bool>) (x => x.mergeSkillId == ((CatalogData) spell).id));
      if (this.currentMergeData == null)
        return;
      this.MergeTarget = spell.mana.mergePoint;
      this.currentMergeData.OnLoad(this, spell);
      this.isMerging = true;
      // ISSUE: method pointer
      spell.mana.casterLeft.ragdollHand.playerHand.OnFistEvent -= new PlayerHand.FistEvent((object) this, __methodptr(OnGrip));
      // ISSUE: method pointer
      spell.mana.casterLeft.ragdollHand.playerHand.OnFistEvent += new PlayerHand.FistEvent((object) this, __methodptr(OnGrip));
      // ISSUE: method pointer
      spell.mana.casterRight.ragdollHand.playerHand.OnFistEvent -= new PlayerHand.FistEvent((object) this, __methodptr(OnGrip));
      // ISSUE: method pointer
      spell.mana.casterRight.ragdollHand.playerHand.OnFistEvent += new PlayerHand.FistEvent((object) this, __methodptr(OnGrip));
    }

    private void OnMergeEnd(SpellMergeData spell)
    {
      if (this.currentMergeData == null)
        return;
      SpellMergeData mergeData1 = this.currentMergeData.mergeData;
      EmpoweredMergeData currentMergeData1 = this.currentMergeData;
      // ISSUE: virtual method pointer
      SpellMergeData.MergeEvent mergeEvent1 = new SpellMergeData.MergeEvent((object) currentMergeData1, __vmethodptr(currentMergeData1, OnUpdate));
      mergeData1.OnMergeUpdateEvent -= mergeEvent1;
      SpellMergeData mergeData2 = this.currentMergeData.mergeData;
      EmpoweredMergeData currentMergeData2 = this.currentMergeData;
      // ISSUE: virtual method pointer
      SpellMergeData.MergeEvent mergeEvent2 = new SpellMergeData.MergeEvent((object) currentMergeData2, __vmethodptr(currentMergeData2, OnFixedUpdate));
      mergeData2.OnMergeFixedUpdate -= mergeEvent2;
      this.currentMergeData.OnUnload(spell);
      // ISSUE: method pointer
      spell.mana.casterLeft.ragdollHand.playerHand.OnFistEvent -= new PlayerHand.FistEvent((object) this, __methodptr(OnGrip));
      // ISSUE: method pointer
      spell.mana.casterRight.ragdollHand.playerHand.OnFistEvent -= new PlayerHand.FistEvent((object) this, __methodptr(OnGrip));
      this.isMerging = false;
      this.currentMergeData = (EmpoweredMergeData) null;
      this.MergeTarget = (Transform) null;
      if (Utils.IsNullOrEmpty((ICollection) SkillArcaneEmpoweringBond.MergeSerpents))
        return;
      this.resetRoutine = ((MonoBehaviour) GameManager.local).StartCoroutine(this.ResetEmpower(this.currentMergeData));
    }

    private void OnGrip(PlayerHand hand, bool gripping)
    {
      if (!this.isMerging)
        return;
      if (gripping && hand.GetOtherHand().isFist)
      {
        if (this.resetRoutine != null)
          ((MonoBehaviour) GameManager.local).StopCoroutine(this.resetRoutine);
        foreach (Serpent serpent in SkillArcaneSerpents.serpents.Where<Serpent>((Func<Serpent, bool>) (x => x.orbitHandler == null && !x.isAttacking)).ToList<Serpent>())
          this.AssignMergeSerpent(serpent);
        SpellMergeData mergeData1 = this.currentMergeData.mergeData;
        EmpoweredMergeData currentMergeData1 = this.currentMergeData;
        // ISSUE: virtual method pointer
        SpellMergeData.MergeEvent mergeEvent1 = new SpellMergeData.MergeEvent((object) currentMergeData1, __vmethodptr(currentMergeData1, OnUpdate));
        mergeData1.OnMergeUpdateEvent -= mergeEvent1;
        SpellMergeData mergeData2 = this.currentMergeData.mergeData;
        EmpoweredMergeData currentMergeData2 = this.currentMergeData;
        // ISSUE: virtual method pointer
        SpellMergeData.MergeEvent mergeEvent2 = new SpellMergeData.MergeEvent((object) currentMergeData2, __vmethodptr(currentMergeData2, OnUpdate));
        mergeData2.OnMergeUpdateEvent += mergeEvent2;
        SpellMergeData mergeData3 = this.currentMergeData.mergeData;
        EmpoweredMergeData currentMergeData3 = this.currentMergeData;
        // ISSUE: virtual method pointer
        SpellMergeData.MergeEvent mergeEvent3 = new SpellMergeData.MergeEvent((object) currentMergeData3, __vmethodptr(currentMergeData3, OnFixedUpdate));
        mergeData3.OnMergeFixedUpdate -= mergeEvent3;
        SpellMergeData mergeData4 = this.currentMergeData.mergeData;
        EmpoweredMergeData currentMergeData4 = this.currentMergeData;
        // ISSUE: virtual method pointer
        SpellMergeData.MergeEvent mergeEvent4 = new SpellMergeData.MergeEvent((object) currentMergeData4, __vmethodptr(currentMergeData4, OnFixedUpdate));
        mergeData4.OnMergeFixedUpdate += mergeEvent4;
        SkillArcaneEmpoweringBond.isMergeEmpowering = true;
      }
      else
      {
        if (Utils.IsNullOrEmpty((ICollection) SkillArcaneEmpoweringBond.MergeSerpents))
          return;
        this.resetRoutine = ((MonoBehaviour) GameManager.local).StartCoroutine(this.ResetEmpower(this.currentMergeData));
      }
    }

    private IEnumerator ResetEmpower(EmpoweredMergeData currentData)
    {
      yield return (object) new WaitForEndOfFrame();
      yield return (object) new WaitForSeconds(0.1f);
      foreach (Serpent serpent in SkillArcaneEmpoweringBond.MergeSerpents.ToList<Serpent>())
        this.ReleaseSerpent(serpent);
      if (this.currentMergeData != null)
      {
        SpellMergeData mergeData1 = currentData.mergeData;
        EmpoweredMergeData empoweredMergeData1 = currentData;
        // ISSUE: virtual method pointer
        SpellMergeData.MergeEvent mergeEvent1 = new SpellMergeData.MergeEvent((object) empoweredMergeData1, __vmethodptr(empoweredMergeData1, OnUpdate));
        mergeData1.OnMergeUpdateEvent -= mergeEvent1;
        SpellMergeData mergeData2 = currentData.mergeData;
        EmpoweredMergeData empoweredMergeData2 = currentData;
        // ISSUE: virtual method pointer
        SpellMergeData.MergeEvent mergeEvent2 = new SpellMergeData.MergeEvent((object) empoweredMergeData2, __vmethodptr(empoweredMergeData2, OnFixedUpdate));
        mergeData2.OnMergeFixedUpdate -= mergeEvent2;
      }
      SkillArcaneEmpoweringBond.isMergeEmpowering = false;
    }

    public void AssignMergeSerpent(Serpent serpent)
    {
      serpent.AssignHandler((object) this);
      serpent.AssignNewOrbit(this.MergeTarget, true, timeLimit: serpent.data.teleportTargetTimeout, handler: (object) this);
      serpent.LoadRotationData(new float?(0.14f), new float?(0.05f), new float?(360f), new float?(90f));
      serpent.movementMultiplier = 1.2f;
      serpent.tempScale = new float?(1.25f);
      serpent.objectOrbitNormal = UnityEngine.Random.onUnitSphere;
      if (SkillArcaneEmpoweringBond.MergeSerpents.Contains(serpent))
        return;
      SkillArcaneEmpoweringBond.MergeSerpents.Add(serpent);
    }

    private void ReleaseSerpent(Serpent serpent)
    {
      if (serpent.orbitHandler != this)
        return;
      serpent.ResetOrbit(serpent.data.teleportReturnTimeout);
      serpent.objectOrbitNormal = UnityEngine.Random.onUnitSphere;
      serpent.movementMultiplier = 1f;
      serpent.tempScale = new float?();
      serpent.ResetRotation();
      serpent.AssignHandler();
      SkillArcaneEmpoweringBond.MergeSerpents.Remove(serpent);
    }

    public void ReleaseAll()
    {
      foreach (Serpent serpent in SkillArcaneEmpoweringBond.MergeSerpents.ToList<Serpent>())
        this.ReleaseSerpent(serpent);
      SkillArcaneEmpoweringBond.MergeSerpents.Clear();
    }
  }
}
