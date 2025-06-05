// Decompiled with JetBrains decompiler
// Type: Arcana.Spells.ArcaneMerge
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Skills.SpellMerge;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Spells
{
  public class ArcaneMerge : SpellMergeData
  {
    public string defaultSkillAssignmentId = "Skill_ArcaneOrb";
    public SpellArcaneMergeSkillData defaultSkillData;

    public event ArcaneMerge.OnLoad OnLoadEvent;

    public event ArcaneMerge.OnUnload OnUnloadEvent;

    public event ArcaneMerge.OnMerge OnMergeEvent;

    public event ArcaneMerge.OnFixedUpdate OnFixedUpdateEvent;

    public event ArcaneMerge.OnUpdate OnUpdateEvent;

    public event ArcaneMerge.OnThrow OnThrowEvent;

    public virtual void OnCatalogRefresh()
    {
      base.OnCatalogRefresh();
      this.defaultSkillData = Catalog.GetData<SpellArcaneMergeSkillData>(this.defaultSkillAssignmentId, true);
    }

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnSkillLoaded(skillData, creature);
      ((SkillData) this.defaultSkillData)?.OnSkillLoaded(skillData, creature);
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnSkillUnloaded(skillData, creature);
      ((SkillData) this.defaultSkillData)?.OnSkillUnloaded(skillData, creature);
    }

    public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnLateSkillsLoaded(skillData, creature);
      ((SkillData) this.defaultSkillData)?.OnLateSkillsLoaded(skillData, creature);
    }

    public virtual void Load(Mana mana)
    {
      base.Load(mana);
      this.defaultSkillData?.OnLoad(mana);
      ArcaneMerge.OnLoad onLoadEvent = this.OnLoadEvent;
      if (onLoadEvent == null)
        return;
      onLoadEvent(mana);
    }

    public virtual void Unload()
    {
      base.Unload();
      this.defaultSkillData?.OnUnload();
      ArcaneMerge.OnUnload onUnloadEvent = this.OnUnloadEvent;
      if (onUnloadEvent == null)
        return;
      onUnloadEvent();
    }

    public virtual void Merge(bool active)
    {
      base.Merge(active);
      this.defaultSkillData?.OnMerge(this, active);
      ArcaneMerge.OnMerge onMergeEvent = this.OnMergeEvent;
      if (onMergeEvent == null)
        return;
      onMergeEvent(this, active);
    }

    public virtual void FixedUpdate()
    {
      base.FixedUpdate();
      this.defaultSkillData?.OnFixedUpdate(this, Time.fixedDeltaTime);
      ArcaneMerge.OnFixedUpdate fixedUpdateEvent = this.OnFixedUpdateEvent;
      if (fixedUpdateEvent == null)
        return;
      fixedUpdateEvent(this, Time.fixedDeltaTime);
    }

    public virtual void Update()
    {
      base.Update();
      this.defaultSkillData?.OnUpdate(this, Time.deltaTime);
      ArcaneMerge.OnUpdate onUpdateEvent = this.OnUpdateEvent;
      if (onUpdateEvent == null)
        return;
      onUpdateEvent(this, Time.deltaTime);
    }

    public virtual void Throw(Vector3 velocity)
    {
      base.Throw(velocity);
      this.defaultSkillData?.OnThrow(this, velocity);
      ArcaneMerge.OnThrow onThrowEvent = this.OnThrowEvent;
      if (onThrowEvent == null)
        return;
      onThrowEvent(this, velocity);
    }

    public delegate void OnLoad(Mana mana);

    public delegate void OnUnload();

    public delegate void OnMerge(ArcaneMerge merge, bool active);

    public delegate void OnFixedUpdate(ArcaneMerge merge, float deltaTime);

    public delegate void OnUpdate(ArcaneMerge merge, float deltaTime);

    public delegate void OnThrow(ArcaneMerge merge, Vector3 velocity);
  }
}
