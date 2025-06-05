// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillCrystallicDive
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using Crystallic.AI;
using Crystallic.Skill.Spell;
using System;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Skill;
using ThunderRoad.Skill.Spell;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillCrystallicDive : SkillSpellPunch
  {
    [ModOption("Dive Force", "Controls the force applied to creatures, multiplied by distance.")]
    [ModOptionCategory("Crystallic Dive", 6)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 100f, 0.5f)]
    public static float force = 4f;
    [ModOption("Dive Angle", "Controls the hand angle to trigger a dive.")]
    [ModOptionCategory("Crystallic Dive", 6)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 100f, 0.5f)]
    public static float maxAngle = 70f;
    [ModOption("Dive Radius", "Controls the radius of force applied to creatures.")]
    [ModOptionCategory("Crystallic Dive", 6)]
    [ModOptionSlider]
    [ModOptionFloatValues(1f, 100f, 0.5f)]
    public static float radius = 4f;
    public static SkillStatusPair active;
    public float distanceMult = 1f;
    public float fallDamageScale;
    protected float groundDistance;
    protected bool isDiving;
    public float maxHeightDistance = 3f;
    public float minDownwardVelocity = 5f;
    public float minHeight = 4f;
    public Vector2 minMaxShakeIntensity = new Vector2(0.005f, 0.01f);
    protected EffectInstance playerEffect;
    protected EffectData playerEffectData;
    public string playerEffectId;
    public bool shake = true;
    public AnimationCurve shakeCurve = AnimationCurve.EaseInOut(0.0f, 1f, 1f, 0.0f);
    public float shakeDuration = 0.5f;
    public float shakeIntensity = 1f;
    public float shockwaveDamage = 30f;
    protected EffectData shockwaveEffectData;
    public string shockwaveEffectId;
    public int shockwavePushLevel = 2;
    public float upwardsModifier;
    protected EffectInstance whooshEffect;
    protected EffectData whooshEffectData;
    public static string spellId = "Body";
    public string whooshEffectId;

    public event SkillCrystallicDive.OnDive OnDiveStop;

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      ((SpellSkillData) this).OnSkillUnloaded(skillData, creature);
      this.whooshEffect?.End(false, -1f);
    }

    public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
    {
      ((SkillData) this).OnLateSkillsLoaded(skillData, creature);
      SkillThickSkin.SetRandomness(new Vector2(0.0f, 3f));
    }

    public virtual void OnCatalogRefresh()
    {
      base.OnCatalogRefresh();
      this.shockwaveEffectData = Catalog.GetData<EffectData>(this.shockwaveEffectId, true);
      this.playerEffectData = Catalog.GetData<EffectData>(this.playerEffectId, true);
      this.whooshEffectData = Catalog.GetData<EffectData>(this.whooshEffectId, true);
    }

    public virtual void OnFist(PlayerHand hand, bool gripping)
    {
      if (!gripping || Player.currentCreature.airHelper.inAir)
        base.OnFist(hand, gripping);
      if (!gripping || !Player.currentCreature.airHelper.inAir || this.isDiving || !(hand.ragdollHand?.caster?.spellInstance is SpellCastCrystallic))
        return;
      hand.ragdollHand.HapticTick(1f, true);
    }

    public virtual void OnPunchStart(RagdollHand hand, Vector3 velocity)
    {
      if (!Player.currentCreature.airHelper.inAir || this.isDiving || !(hand.caster?.spellInstance is SpellCastCrystallic) || (double) Vector3.Angle(velocity, Vector3.down) >= (double) SkillCrystallicDive.maxAngle)
        return;
      Player.local.locomotion.physicBody.AddForce(velocity * this.playerDashForce, (ForceMode) 2);
      this.whooshEffect = this.whooshEffectData?.Spawn(((ThunderBehaviour) hand).transform, true, (ColliderGroup) null, false);
      this.whooshEffect?.Play(0, false, false);
      RaycastHit raycastHit;
      float num;
      this.groundDistance = Player.local.locomotion.SphereCastGround(this.minHeight + this.maxHeightDistance, ref raycastHit, ref num) ? ((RaycastHit) ref raycastHit).distance : this.minHeight + this.maxHeightDistance;
      ((ValueHandler<float>) Player.fallDamageScale).Add((object) this, this.fallDamageScale);
      // ISSUE: method pointer
      Player.currentCreature.airHelper.OnGroundEvent -= new AirHelper.AirEvent((object) this, __methodptr(OnGround));
      // ISSUE: method pointer
      Player.currentCreature.airHelper.OnGroundEvent += new AirHelper.AirEvent((object) this, __methodptr(OnGround));
      this.playerEffect?.End(false, -1f);
      this.playerEffect = this.playerEffectData?.Spawn(((ThunderBehaviour) Player.local).transform.position - Vector3.up * 1.5f + Vector3.ProjectOnPlane(((Component) Player.local.head).transform.forward, Vector3.up).normalized, Quaternion.identity, ((ThunderBehaviour) Player.local).transform, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>());
      this.playerEffect?.Play(0, false, false);
    }

    public void OnGround(Creature playerCreature)
    {
      // ISSUE: method pointer
      Player.currentCreature.airHelper.OnGroundEvent -= new AirHelper.AirEvent((object) this, __methodptr(OnGround));
      Utils.RunAfter((MonoBehaviour) Player.currentCreature, (Action) (() => ((ValueHandler<float>) Player.fallDamageScale).Remove((object) this)), 0.5f, false);
      this.playerEffect?.End(false, -1f);
      this.playerEffect = (EffectInstance) null;
      if (Player.currentCreature.airHelper.Climbing || (double) Player.local.locomotion.velocity.y > -(double) this.minDownwardVelocity || (double) this.groundDistance <= (double) this.minHeight)
        return;
      float num1 = Mathf.InverseLerp(this.minHeight, this.minHeight + this.maxHeightDistance, this.groundDistance);
      double num2 = 1.0 + (double) num1 * (double) this.distanceMult;
      this.whooshEffect?.End(false, -1f);
      EffectInstance effectInstance = this.shockwaveEffectData?.Spawn(((ThunderBehaviour) Player.currentCreature).transform.position, Quaternion.LookRotation(Vector3.forward, Vector3.up), (Transform) null, (CollisionInstance) null, true, (ColliderGroup) null, false, num1, 1f, Array.Empty<Type>());
      effectInstance.Play(0, false, false);
      effectInstance.SetColorImmediate(Dye.GetEvaluatedColor("Body", SkillCrystallicDive.spellId));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      List<ThunderEntity> thunderEntityList = ThunderEntity.InRadiusNaive(((ThunderBehaviour) Player.currentCreature).transform.position, SkillCrystallicDive.radius, SkillCrystallicDive.\u003C\u003EO.\u003C0\u003E__AllButPlayer ?? (SkillCrystallicDive.\u003C\u003EO.\u003C0\u003E__AllButPlayer = new Func<ThunderEntity, bool>(Filter.AllButPlayer)), (List<ThunderEntity>) null);
      for (int index = 0; index < thunderEntityList.Count; ++index)
      {
        ThunderEntity thunderEntity = thunderEntityList[index];
        Vector3 vector3 = thunderEntityList[index].Center - ((ThunderBehaviour) Player.currentCreature.ragdoll.targetPart).transform.position;
        thunderEntity.AddExplosionForce(SkillCrystallicDive.force * (float) num2, ((ThunderBehaviour) Player.currentCreature.ragdoll.targetPart).transform.position, SkillCrystallicDive.radius * (float) num2, this.upwardsModifier, (ForceMode) 2, (CollisionHandler) null);
        if (thunderEntity is Item obj)
        {
          Breakable breakable = obj.breakable;
          if ((UnityEngine.Object) breakable != (UnityEngine.Object) null && !breakable.contactBreakOnly)
            breakable.Break();
        }
        if (thunderEntity is Creature creature)
        {
          if (this.shockwavePushLevel > 0)
            creature.TryPush((Creature.PushType) 0, vector3, this.shockwavePushLevel, (RagdollPart.Type) 0);
          if ((double) this.shockwaveDamage > 0.0)
            creature.Damage(this.shockwaveDamage * Mathf.Clamp01(vector3.magnitude / SkillCrystallicDive.radius));
          BrainModuleCrystal module = creature.brain.instance.GetModule<BrainModuleCrystal>(true);
          module.Crystallise(5f);
          module.SetColor(Dye.GetEvaluatedColor(module.lerper.currentSpellId, SkillCrystallicDive.spellId), SkillCrystallicDive.spellId);
          if (SkillCrystallicDive.active != null)
            ((ThunderEntity) creature).Inflict(SkillCrystallicDive.active.statusId, (object) this, SkillCrystallicDive.active.statusDuration, (object) SkillCrystallicDive.active.statusParameter, SkillCrystallicDive.active.playEffects);
        }
      }
      SkillCrystallicDive.OnDive onDiveStop = this.OnDiveStop;
      if (onDiveStop != null)
        onDiveStop();
      if (!this.shake)
        return;
      Shaker.ShakePlayer(this.shakeDuration, this.shakeIntensity, this.shakeCurve, this.minMaxShakeIntensity);
    }

    public delegate void OnDive();
  }
}
