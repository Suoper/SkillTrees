// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillBoltAbsorption
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using Crystallic.Skill.Spell;
using System;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Skill.Spell;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillBoltAbsorption : SkillAbsorptionData
  {
    private readonly float cooldown = 0.075f;
    private float lastHitTime = 1f;
    protected EffectData boltEffectData;
    public string boltEffectId = "Thunderbolt";
    protected EffectData zapEffectData;
    public string zapEffectId = "BoltZap";

    public static event SkillBoltAbsorption.OnBoltAbsorptionTriggered onBoltAbsorptionTriggered;

    public static event SkillBoltAbsorption.End onEnd;

    public override void OnCatalogRefresh()
    {
      base.OnCatalogRefresh();
      this.zapEffectData = Catalog.GetData<EffectData>(this.zapEffectId, true);
      this.boltEffectData = Catalog.GetData<EffectData>(this.boltEffectId, true);
    }

    protected override void OnStingerSpawnWithSpell(Stinger stinger)
    {
      base.OnStingerSpawnWithSpell(stinger);
      this.boltEffectData.Spawn(stinger.transform, true, (ColliderGroup) null, false).Play(0, false, false);
    }

    protected override void OnSpellStopEvent(SpellCastCharge spell)
    {
      base.OnSpellStopEvent(spell);
      SkillBoltAbsorption.End onEnd = SkillBoltAbsorption.onEnd;
      if (onEnd == null)
        return;
      onEnd();
    }

    public override void PlayAbsorbEffect(SpellCastCharge main)
    {
      base.PlayAbsorbEffect(main);
      bool flag = Player.currentCreature.handLeft.caster.spellInstance != null && !string.IsNullOrEmpty(((CatalogData) Player.currentCreature.handLeft.caster.spellInstance).id) && Player.currentCreature.handRight.caster.spellInstance != null && !string.IsNullOrEmpty(((CatalogData) Player.currentCreature.handRight.caster.spellInstance).id);
      if ((!(((CatalogData) Player.currentCreature?.handLeft?.caster?.spellInstance)?.id == "Lightning") || !Player.currentCreature.handLeft.caster.isFiring || !(((CatalogData) Player.currentCreature?.handRight?.caster?.spellInstance)?.id == "Crystallic") || !Player.currentCreature.handRight.caster.isFiring) && (!(((CatalogData) Player.currentCreature?.handRight?.caster?.spellInstance)?.id == "Lightning") || !Player.currentCreature.handRight.caster.isFiring || !(((CatalogData) Player.currentCreature?.handLeft?.caster?.spellInstance)?.id == "Crystallic") || !Player.currentCreature.handLeft.caster.isFiring) || !flag)
        return;
      if (main is SpellCastLightning spellCastLightning)
      {
        Transform orb1 = main.spellCaster.Orb;
        Transform orb2 = main.spellCaster.other.Orb;
        Vector3? nullable1 = new Vector3?();
        Vector3? nullable2 = new Vector3?();
        spellCastLightning.PlayBolt(orb1, orb2, nullable1, nullable2, (Gradient) null);
        SkillBoltAbsorption.OnBoltAbsorptionTriggered absorptionTriggered = SkillBoltAbsorption.onBoltAbsorptionTriggered;
        if (absorptionTriggered == null)
          return;
        absorptionTriggered(Dye.GetEvaluatedColor("Lightning", "Lightning"), main.spellCaster.other.spellInstance as SpellCastCrystallic, main as SpellCastLightning);
      }
      else
      {
        if (!(main.spellCaster.other.spellInstance is SpellCastLightning spellInstance))
          return;
        spellInstance.PlayBolt(((SpellCastCharge) spellInstance).spellCaster.Orb, main.spellCaster.Orb, new Vector3?(), new Vector3?(), (Gradient) null);
        SkillBoltAbsorption.OnBoltAbsorptionTriggered absorptionTriggered = SkillBoltAbsorption.onBoltAbsorptionTriggered;
        if (absorptionTriggered != null)
          absorptionTriggered(Dye.GetEvaluatedColor("Lightning", "Lightning"), main as SpellCastCrystallic, main.spellCaster.other.spellInstance as SpellCastLightning);
      }
    }

    protected override void OnStingerStab(
      Stinger stinger,
      Damager damager,
      CollisionInstance collisionInstance,
      Creature hitCreature)
    {
      base.OnStingerStab(stinger, damager, collisionInstance, hitCreature);
      bool flag = Player.currentCreature.handLeft.caster.spellInstance != null && !string.IsNullOrEmpty(((CatalogData) Player.currentCreature.handLeft.caster.spellInstance).id) && Player.currentCreature.handRight.caster.spellInstance != null && !string.IsNullOrEmpty(((CatalogData) Player.currentCreature.handRight.caster.spellInstance).id);
      if ((!(((CatalogData) Player.currentCreature?.handLeft.caster?.spellInstance)?.id == "Lightning") || !Player.currentCreature.handLeft.caster.isFiring || !(((CatalogData) Player.currentCreature?.handRight?.caster?.spellInstance)?.id == "Crystallic") || !Player.currentCreature.handRight.caster.isFiring) && (!(((CatalogData) Player.currentCreature?.handRight?.caster?.spellInstance)?.id == "Lightning") || !Player.currentCreature.handRight.caster.isFiring || !(((CatalogData) Player.currentCreature?.handLeft?.caster?.spellInstance)?.id == "Crystallic") || !Player.currentCreature.handLeft.caster.isFiring) || !flag)
        return;
      ((ThunderEntity) hitCreature).Inflict("Electrocute", (object) this, 5f, (object) null, true);
      SpellCastLightning spellCastLightning = ((CatalogData) Player.currentCreature.handLeft.caster.spellInstance).id == "Lightning" ? Player.currentCreature.handLeft.caster.spellInstance as SpellCastLightning : Player.currentCreature.handRight.caster.spellInstance as SpellCastLightning;
      spellCastLightning?.PlayBolt(stinger.spellCastCrystallic.spellCaster.Orb, ((SpellCastCharge) spellCastLightning).spellCaster.Orb, new Vector3?(), new Vector3?(), (Gradient) null);
      spellCastLightning?.PlayBolt(stinger.spellCastCrystallic.spellCaster.Orb, ((ThunderBehaviour) hitCreature.ragdoll.targetPart).transform, new Vector3?(), new Vector3?(), (Gradient) null);
      foreach (Creature inRadiu in Creature.InRadius(collisionInstance.contactPoint, 5f, (Func<Creature, bool>) null, (List<Creature>) null))
      {
        if (!inRadiu.isPlayer)
        {
          for (int index = 0; index < UnityEngine.Random.Range(1, 2); ++index)
          {
            this.zapEffectData.Spawn(((ThunderBehaviour) inRadiu.ragdoll.targetPart).transform, true, (ColliderGroup) null, false).Play(0, false, false);
            spellCastLightning?.PlayBolt(((ThunderBehaviour) hitCreature.ragdoll.targetPart).transform, ((ThunderBehaviour) inRadiu.ragdoll.targetPart).transform, new Vector3?(), new Vector3?(), (Gradient) null);
          }
          BrainModuleCrystal module = inRadiu.brain.instance.GetModule<BrainModuleCrystal>(true);
          module.Crystallise(5f);
          module.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, this.spellId), this.spellId);
          ((ThunderEntity) inRadiu).Inflict("Electrocute", (object) this, 5f, (object) null, true);
        }
      }
    }

    protected override void OnShardshotHitWhileAbsorbing(
      SpellCastCrystallic spellCastCrystallic,
      ThunderEntity entity,
      SpellCastCrystallic.ShardshotHit hitInfo)
    {
      base.OnShardshotHitWhileAbsorbing(spellCastCrystallic, entity, hitInfo);
      if ((double) Time.time - (double) this.lastHitTime < (double) this.cooldown)
        return;
      this.lastHitTime = Time.time;
      if (entity is Creature creature)
        creature.brain.instance.GetModule<BrainModuleCrystal>(true).SetColor(Dye.GetEvaluatedColor(creature.brain.instance.GetModule<BrainModuleCrystal>(true).lerper.currentSpellId, this.spellId), this.spellId);
      SpellCastLightning spellInstance = spellCastCrystallic.spellCaster.other.spellInstance as SpellCastLightning;
      foreach (Creature inRadiu in Creature.InRadius(hitInfo.hitPoint, 2.5f, (Func<Creature, bool>) null, (List<Creature>) null))
      {
        if (!inRadiu.isPlayer && !((UnityEngine.Object) inRadiu == (UnityEngine.Object) entity))
        {
          this.zapEffectData.Spawn(((ThunderBehaviour) inRadiu.ragdoll.targetPart).transform, true, (ColliderGroup) null, false).Play(0, false, false);
          spellInstance.PlayBolt(((Component) hitInfo.hitCollider).transform, ((ThunderBehaviour) inRadiu.ragdoll.targetPart).transform, new Vector3?(), new Vector3?(), (Gradient) null);
          ((ThunderEntity) inRadiu).Inflict("Electrocute", (object) this, 5f, (object) null, true);
          BrainModuleCrystal module = inRadiu.brain.instance.GetModule<BrainModuleCrystal>(true);
          module.Crystallise(5f);
          module.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, this.spellId), this.spellId);
        }
      }
    }

    public delegate void OnBoltAbsorptionTriggered(
      Color color,
      SpellCastCrystallic main,
      SpellCastLightning other);

    public delegate void End();
  }
}
