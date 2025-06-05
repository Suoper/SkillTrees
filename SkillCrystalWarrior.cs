// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillCrystalWarrior
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
  public class SkillCrystalWarrior : SkillSpellPunch
  {
    public List<SkillStatusPair> skillStatusPairs;
    public SkillStatusPair active;
    public string armEffectId;
    public string deflectEffectId;
    public AnimationCurve hapticCurve = new AnimationCurve(new Keyframe[3]
    {
      new Keyframe(0.0f, 10f),
      new Keyframe(0.05f, 25f),
      new Keyframe(0.1f, 10f)
    });
    public EffectData imbueCollisionEffectData;
    private bool leftGripping;
    public Lerper lerper;
    public EffectData lowerArmData;
    public EffectInstance lowerLeftArmInstance;
    public EffectInstance lowerRightArmInstance;
    private string leftCurrent = "Body";
    private string rightCurrent = "Body";
    private bool rightGripping;

    public virtual void OnCatalogRefresh()
    {
      base.OnCatalogRefresh();
      this.lerper = new Lerper();
      this.lowerArmData = Catalog.GetData<EffectData>(this.armEffectId, true);
      this.imbueCollisionEffectData = Catalog.GetData<EffectData>(this.deflectEffectId, true);
    }

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      ((SpellSkillData) this).OnSkillLoaded(skillData, creature);
      // ISSUE: method pointer
      PlayerControl.local.OnButtonPressEvent -= new PlayerControl.ButtonEvent((object) this, __methodptr(OnButtonPressEvent));
      // ISSUE: method pointer
      PlayerControl.local.OnButtonPressEvent += new PlayerControl.ButtonEvent((object) this, __methodptr(OnButtonPressEvent));
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      ((SpellSkillData) this).OnSkillUnloaded(skillData, creature);
      // ISSUE: method pointer
      PlayerControl.local.OnButtonPressEvent -= new PlayerControl.ButtonEvent((object) this, __methodptr(OnButtonPressEvent));
    }

    public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnLateSkillsLoaded(skillData, creature);
      SkillThickSkin.SetRandomness(new Vector2(0.0f, 4f));
    }

    public virtual void OnSpellLoad(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellLoad(spell, caster);
      if (!(spell is SpellCastCharge spellCastCharge))
        return;
      // ISSUE: method pointer
      spellCastCharge.OnSpellCastEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellCastEvent));
      // ISSUE: method pointer
      spellCastCharge.OnSpellStopEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellStopEvent));
      // ISSUE: method pointer
      spellCastCharge.OnSpellCastEvent += new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellCastEvent));
      // ISSUE: method pointer
      spellCastCharge.OnSpellStopEvent += new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellStopEvent));
    }

    public virtual void OnSpellUnload(SpellData spell, SpellCaster caster = null)
    {
      base.OnSpellUnload(spell, caster);
      if (!(spell is SpellCastCharge spellCastCharge))
        return;
      // ISSUE: method pointer
      spellCastCharge.OnSpellCastEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellCastEvent));
      // ISSUE: method pointer
      spellCastCharge.OnSpellStopEvent -= new SpellCastCharge.SpellEvent((object) this, __methodptr(OnSpellStopEvent));
    }

    private void OnSpellCastEvent(SpellCastCharge spell)
    {
      Utils.RunAfter((MonoBehaviour) spell.spellCaster.mana, (Action) (() =>
      {
        SkillCrystallicDive.spellId = ((CatalogData) spell).id;
        for (int index = 0; index < this.skillStatusPairs.Count; ++index)
        {
          if (this.skillStatusPairs[index].spellId == ((CatalogData) spell).id)
            this.active = this.skillStatusPairs[index];
        }
        SkillCrystallicDive.active = this.active;
        Side side = spell.spellCaster.side;
        if (side != null)
        {
          if (side != 1 || this.leftGripping || !this.rightGripping)
            return;
          this.SetColor(Dye.GetEvaluatedColor("Body", ((CatalogData) spell).id), ((CatalogData) spell).id, (Side) 0);
          this.rightCurrent = ((CatalogData) spell).id;
        }
        else
        {
          if (!this.leftGripping || this.rightGripping)
            return;
          this.SetColor(Dye.GetEvaluatedColor("Body", ((CatalogData) spell).id), ((CatalogData) spell).id, (Side) 1);
          this.leftCurrent = ((CatalogData) spell).id;
        }
      }), 0.1f, false);
    }

    private void OnSpellStopEvent(SpellCastCharge spell)
    {
      SkillCrystallicDive.spellId = "Body";
      this.active = (SkillStatusPair) null;
      SkillCrystallicDive.active = (SkillStatusPair) null;
      Side side = spell.spellCaster.side;
      if (side != null)
      {
        if (side != 1 || this.leftGripping || !this.rightGripping)
          return;
        this.SetColor(Dye.GetEvaluatedColor("Body", "Body"), "Body", (Side) 0);
        this.rightCurrent = "Body";
      }
      else
      {
        if (!this.leftGripping || this.rightGripping)
          return;
        this.SetColor(Dye.GetEvaluatedColor("Body", "Body"), "Body", (Side) 1);
        this.leftCurrent = "Body";
      }
    }

    private void OnButtonPressEvent(
      PlayerControl.Hand hand,
      PlayerControl.Hand.Button button,
      bool pressed)
    {
      if (button == 1 & pressed)
      {
        if (this.leftGripping && hand.side == 1)
        {
          this.lowerLeftArmInstance.End(false, -1f);
        }
        else
        {
          if (!this.rightGripping || hand.side != 0)
            return;
          this.lowerRightArmInstance.End(false, -1f);
        }
      }
      else if (button == 0 & pressed && this.leftGripping && hand.side == 1)
      {
        this.lowerLeftArmInstance.End(false, -1f);
        this.lowerLeftArmInstance.ForceStop((ParticleSystemStopBehavior) 0);
        // ISSUE: method pointer
        Player.currentCreature.OnDamageEvent -= new Creature.DamageEvent((object) this, __methodptr(OnDamageEvent));
      }
      else
      {
        if (!(button == 0 & pressed) || !this.rightGripping || hand.side != 0)
          return;
        this.lowerRightArmInstance.End(false, -1f);
        this.lowerRightArmInstance.ForceStop((ParticleSystemStopBehavior) 0);
        // ISSUE: method pointer
        Player.currentCreature.OnDamageEvent -= new Creature.DamageEvent((object) this, __methodptr(OnDamageEvent));
      }
    }

    public virtual void OnFist(PlayerHand hand, bool gripping)
    {
      base.OnFist(hand, gripping);
      if (gripping)
      {
        // ISSUE: method pointer
        Player.currentCreature.OnDamageEvent += new Creature.DamageEvent((object) this, __methodptr(OnDamageEvent));
      }
      else
      {
        // ISSUE: method pointer
        Player.currentCreature.OnDamageEvent -= new Creature.DamageEvent((object) this, __methodptr(OnDamageEvent));
      }
      Side side = hand.side;
      if (side != null)
      {
        if (side != 1)
          return;
        this.leftGripping = gripping;
        if (gripping)
        {
          RagdollPart partByName = hand.ragdollHand.creature.ragdoll.GetPartByName("LeftForeArm");
          this.lowerLeftArmInstance = this.lowerArmData.Spawn(((ThunderBehaviour) partByName).transform.position, Quaternion.LookRotation(partByName.upDirection, partByName.forwardDirection), ((ThunderBehaviour) partByName).transform, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
          this.lowerLeftArmInstance?.Play(0, false, false);
          this.SetColor(Dye.GetEvaluatedColor("Body", this.leftCurrent), this.leftCurrent, (Side) 1);
        }
        else
        {
          if (this.lowerLeftArmInstance == null)
            return;
          this.lowerLeftArmInstance?.End(false, -1f);
          this.lowerLeftArmInstance.ForceStop((ParticleSystemStopBehavior) 0);
        }
      }
      else
      {
        this.rightGripping = gripping;
        if (gripping)
        {
          RagdollPart partByName = hand.ragdollHand.creature.ragdoll.GetPartByName("RightForeArm");
          this.lowerRightArmInstance = this.lowerArmData.Spawn(((ThunderBehaviour) partByName).transform.position, Quaternion.LookRotation(partByName.upDirection, partByName.forwardDirection), ((ThunderBehaviour) partByName).transform, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
          this.lowerRightArmInstance?.Play(0, false, false);
          this.SetColor(Dye.GetEvaluatedColor("Body", this.rightCurrent), this.rightCurrent, (Side) 0);
        }
        else
        {
          if (this.lowerRightArmInstance == null)
            return;
          this.lowerRightArmInstance?.End(false, -1f);
          this.lowerRightArmInstance.ForceStop((ParticleSystemStopBehavior) 0);
        }
      }
    }

    private void OnDamageEvent(CollisionInstance collisioninstance, EventTime eventtime)
    {
      PlayerHand playerHand = Player.local.handLeft.isFist ? Player.local.handLeft : Player.local.handRight;
      if ((double) Vector3.Distance(collisioninstance.contactPoint, ((ThunderBehaviour) playerHand).transform.position) > 0.550000011920929)
        return;
      collisioninstance.ignoreDamage = true;
      Player.currentCreature.Heal(collisioninstance.damageStruct.damage);
      collisioninstance.damageStruct.damage = 0.0f;
      collisioninstance.skipVignette = true;
      this.imbueCollisionEffectData?.Spawn(collisioninstance.contactPoint, Quaternion.identity, ((Component) collisioninstance.targetCollider).transform, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>()).Play(0, false, false);
      playerHand.ragdollHand.PlayHapticClipOver(this.hapticCurve, 1f);
    }

    public virtual void OnPunchHit(RagdollHand hand, CollisionInstance hit, bool fist)
    {
      base.OnPunchHit(hand, hit, fist);
      if (!fist)
        return;
      RagdollPart ragdollPart = hit?.targetColliderGroup?.collisionHandler?.ragdollPart;
      if ((bool) (UnityEngine.Object) ragdollPart && !ragdollPart.ragdoll.creature.isPlayer && !ragdollPart.hasMetalArmor)
      {
        BrainModuleCrystal module = ragdollPart.ragdoll.creature.brain.instance.GetModule<BrainModuleCrystal>(true);
        module.Crystallise(5f);
        if (this.active != null)
          this.active.Inflict(ragdollPart.ragdoll.creature);
        module.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, hand.side == 1 ? this.leftCurrent : this.rightCurrent), hand.side == 1 ? this.leftCurrent : this.rightCurrent);
      }
      hand.PlayHapticClipOver(this.hapticCurve, 1f);
    }

    public void SetColor(Color color, string spellId, Side side, float time = 0.15f)
    {
      ParticleSystem[] particleSystems1;
      if (side != 1)
      {
        EffectInstance rightArmInstance = this.lowerRightArmInstance;
        particleSystems1 = rightArmInstance != null ? rightArmInstance.GetParticleSystems() : (ParticleSystem[]) null;
      }
      else
      {
        EffectInstance lowerLeftArmInstance = this.lowerLeftArmInstance;
        particleSystems1 = lowerLeftArmInstance != null ? lowerLeftArmInstance.GetParticleSystems() : (ParticleSystem[]) null;
      }
      ParticleSystem[] particleSystems2 = particleSystems1;
      this.lerper.SetColor(color, particleSystems2, spellId, time);
    }
  }
}
