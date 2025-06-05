// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillBoltbomb
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using System;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Skill;
using ThunderRoad.Skill.Spell;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillBoltbomb : SpellSkillData
  {
    [ModOption("Bolt Hits", "Controls how many bolts it takes to generate a boltbomb. Each time this count is reached the counter is reset.")]
    [ModOptionCategory("Boltbomb", 30)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 1000f, 1f)]
    public static float boltHits = 50f;
    protected EffectData boltEffectData;
    public string boltEffectId = "Thunderbolt";
    protected EffectData zapEffectData;
    public string zapEffectId = "BoltZap";
    protected EffectData thunderboltEffectData;
    public string thunderboltEffectId = "SpellThunderbolt";
    protected EffectData thunderboltImpactEffectData;
    public string thunderboltImpactEffectId = "SpellThunderboltImpact";

    public virtual void OnCatalogRefresh()
    {
      ((SkillData) this).OnCatalogRefresh();
      this.zapEffectData = Catalog.GetData<EffectData>(this.zapEffectId, true);
      this.boltEffectData = Catalog.GetData<EffectData>(this.boltEffectId, true);
      this.thunderboltEffectData = Catalog.GetData<EffectData>(this.thunderboltEffectId, true);
      this.thunderboltImpactEffectData = Catalog.GetData<EffectData>(this.thunderboltImpactEffectId, true);
    }

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is SpellCastLightning spellCastLightning))
        return;
      // ISSUE: method pointer
      spellCastLightning.OnBoltHitColliderGroupEvent -= new SpellCastLightning.BoltHitColliderGroupEvent((object) this, __methodptr(OnBoltHitColliderGroupEvent));
      // ISSUE: method pointer
      spellCastLightning.OnBoltHitColliderGroupEvent += new SpellCastLightning.BoltHitColliderGroupEvent((object) this, __methodptr(OnBoltHitColliderGroupEvent));
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      if (!(spell is SpellCastLightning spellCastLightning))
        return;
      // ISSUE: method pointer
      spellCastLightning.OnBoltHitColliderGroupEvent -= new SpellCastLightning.BoltHitColliderGroupEvent((object) this, __methodptr(OnBoltHitColliderGroupEvent));
    }

    private void OnBoltHitColliderGroupEvent(
      SpellCastLightning spell,
      ColliderGroup colliderGroup,
      Vector3 position,
      Vector3 normal,
      Vector3 velocity,
      float intensity,
      ColliderGroup source,
      HashSet<ThunderEntity> seenEntities)
    {
      if (!(colliderGroup.collisionHandler.Entity is Creature entity))
        return;
      int num = ((ThunderEntity) entity).GetVariable<int>("BoltHits") + 1;
      ((ThunderEntity) entity).SetVariable<int>("BoltHits", num);
      if ((double) num >= (double) SkillBoltbomb.boltHits)
      {
        GameObject gameObject = new GameObject("Bolt Temp");
        gameObject.transform.position = ((ThunderBehaviour) entity.ragdoll.targetPart).transform.position + new Vector3(0.0f, (float) UnityEngine.Random.Range(4, 6), 0.0f);
        gameObject.transform.rotation = Quaternion.LookRotation(Vector3.down);
        EffectInstance effectInstance = this.thunderboltEffectData.Spawn(gameObject.transform, true, (ColliderGroup) null, false);
        effectInstance.SetSourceAndTarget(gameObject.transform, ((ThunderBehaviour) entity.ragdoll.targetPart).transform);
        effectInstance.Play(0, false, false);
        this.thunderboltImpactEffectData.Spawn(((ThunderBehaviour) entity.ragdoll.targetPart).transform, true, (ColliderGroup) null, false).Play(0, false, false);
        ((ThunderEntity) entity).SetVariable<int>("BoltHits", 0);
        this.boltEffectData.Spawn(((ThunderBehaviour) entity.ragdoll.targetPart).transform, true, (ColliderGroup) null, false).Play(0, false, false);
        BrainModuleCrystal module1 = entity.brain.instance.GetModule<BrainModuleCrystal>(true);
        module1.Crystallise(5f);
        module1.SetColor(Dye.GetEvaluatedColor(module1.lerper.currentSpellId, "Lightning"), "Lightning");
        foreach (Creature inRadiu in Creature.InRadius(((ThunderBehaviour) entity.ragdoll.targetPart).transform.position, 5f, (Func<Creature, bool>) null, (List<Creature>) null))
        {
          if (!inRadiu.isPlayer)
          {
            for (int index = 0; index < UnityEngine.Random.Range(1, 2); ++index)
            {
              this.zapEffectData.Spawn(((ThunderBehaviour) inRadiu.ragdoll.targetPart).transform, true, (ColliderGroup) null, false).Play(0, false, false);
              spell?.PlayBolt(((ThunderBehaviour) entity.ragdoll.targetPart).transform, ((ThunderBehaviour) inRadiu.ragdoll.targetPart).transform, new Vector3?(), new Vector3?(), (Gradient) null);
            }
            BrainModuleCrystal module2 = inRadiu.brain.instance.GetModule<BrainModuleCrystal>(true);
            module2.Crystallise(5f);
            module2.SetColor(Dye.GetEvaluatedColor(module1.lerper.currentSpellId, "Lightning"), "Lightning");
            ((ThunderEntity) inRadiu).Inflict("Electrocute", (object) this, 5f, (object) null, true);
          }
        }
      }
    }
  }
}
