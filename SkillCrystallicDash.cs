// Decompiled with JetBrains decompiler
// Type: Crystallic.Skill.SkillCrystallicDash
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System;
using System.Collections.Generic;
using ThunderRoad;
using ThunderRoad.Skill;
using UnityEngine;

#nullable disable
namespace Crystallic.Skill
{
  public class SkillCrystallicDash : SpellSkillData
  {
    private readonly Dictionary<Item, SkillSpellDash> items = new Dictionary<Item, SkillSpellDash>();
    public Interactable.Action action;
    public List<string> allowedAuthors = new List<string>();
    public List<string> allowedCategories = new List<string>();
    public List<ItemFlags> allowedFlags = new List<ItemFlags>();
    public List<string> allowedItems = new List<string>();
    public List<string> allowedSlots = new List<string>();
    public List<SkillSpellDash> allowedSpells = new List<SkillSpellDash>();
    public List<int> allowedTiers = new List<int>();
    public List<ItemData.Type> allowedTypes = new List<ItemData.Type>();
    public AnimationCurve hapticCurve = new AnimationCurve(new Keyframe[3]
    {
      new Keyframe(0.0f, 7f),
      new Keyframe(0.05f, 5f),
      new Keyframe(0.1f, 3f)
    });
    public Creature thisCreature;

    public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
    {
      base.OnSkillLoaded(skillData, creature);
      this.thisCreature = creature;
    }

    public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
    {
      base.OnSkillUnloaded(skillData, creature);
      this.thisCreature = (Creature) null;
      this.items.Clear();
    }

    public virtual void OnImbueLoad(SpellData spell, Imbue imbue)
    {
      base.OnImbueLoad(spell, imbue);
      Item key = imbue?.colliderGroup?.collisionHandler?.item;
      if (!(bool) (UnityEngine.Object) key || this.items.ContainsKey(key) || this.allowedSpells.Count <= 0)
        return;
      foreach (SkillSpellDash allowedSpell in this.allowedSpells)
      {
        if ((bool) (UnityEngine.Object) key && ((CatalogData) imbue?.spellCastBase).id == allowedSpell.spellId && this.IsItemAllowed(key) && (UnityEngine.Object) key.flyDirRef != (UnityEngine.Object) null && (string.IsNullOrEmpty(allowedSpell.skillId) || this.thisCreature.HasSkill(allowedSpell.skillId)))
        {
          this.items.Add(key, allowedSpell);
          if (!string.IsNullOrEmpty(allowedSpell.effectId))
            allowedSpell.dashEffectData = Catalog.GetData<EffectData>(allowedSpell.effectId, true);
          // ISSUE: method pointer
          key.OnHeldActionEvent += new Item.HeldActionDelegate((object) this, __methodptr(OnHeldActionEvent));
          break;
        }
      }
    }

    private bool IsItemAllowed(Item item)
    {
      return this.allowedItems.Count <= 0 && this.allowedCategories.Count <= 0 && this.allowedTypes.Count <= 0 && this.allowedTiers.Count <= 0 && this.allowedFlags.Count <= 0 && this.allowedSlots.Count <= 0 && this.allowedAuthors.Count <= 0 || this.allowedItems.Count > 0 && this.allowedItems.Contains(item.itemId) || this.allowedCategories.Count > 0 && this.allowedCategories.Contains(item.data.category) || this.allowedTypes.Count > 0 && this.allowedTypes.Contains(item.data.type) || this.allowedTiers.Count > 0 && this.allowedTiers.Contains(item.data.tier) || this.allowedFlags.Count > 0 && this.allowedFlags.Contains(item.data.flags) || this.allowedSlots.Count > 0 && this.allowedSlots.Contains(item.data.slot) || this.allowedAuthors.Count > 0 && this.allowedAuthors.Contains(item.data.author) && this.allowedAuthors.Contains(item.data.author);
    }

    public virtual void OnImbueUnload(SpellData spell, Imbue imbue)
    {
      base.OnImbueUnload(spell, imbue);
      Item key = imbue?.colliderGroup?.collisionHandler?.item;
      if (!(bool) (UnityEngine.Object) key || !this.items.ContainsKey(key))
        return;
      // ISSUE: method pointer
      key.OnHeldActionEvent -= new Item.HeldActionDelegate((object) this, __methodptr(OnHeldActionEvent));
      this.items.Remove(key);
    }

    private void OnHeldActionEvent(
      RagdollHand ragdollhand,
      Handle handle,
      Interactable.Action action)
    {
      if (action != this.action || !((UnityEngine.Object) ragdollhand.grabbedHandle == (UnityEngine.Object) handle) || !((UnityEngine.Object) handle.item != (UnityEngine.Object) null))
        return;
      ragdollhand.PlayHapticClipOver(this.hapticCurve, 0.4f);
      SkillSpellDash mix;
      if (this.items.TryGetValue(handle.item, out mix))
        this.Accelerate(handle.item.flyDirRef.forward, mix);
    }

    public void Accelerate(Vector3 direction, SkillSpellDash mix)
    {
      direction.Normalize();
      Player.local.locomotion.velocity = direction.normalized * Player.local.locomotion.velocity.magnitude;
      Player.local.locomotion.physicBody.AddForce(direction.normalized * mix.dashSpeed, (ForceMode) 2);
      Vector3 vector3 = Player.local.head.cam.transform.position + Player.local.head.cam.transform.forward * 0.3f;
      Quaternion rotation = Player.local.head.cam.transform.rotation;
      mix.dashEffectData?.Spawn(vector3, rotation, ((Component) Player.local.head).transform, (CollisionInstance) null, true, (ColliderGroup) null, false, 1f, 1f, Array.Empty<Type>())?.Play(0, false, false);
      if (string.IsNullOrEmpty(mix.statusId))
        return;
      Utils.RunAfter((MonoBehaviour) this.thisCreature, (Action) (() => ((ThunderEntity) this.thisCreature).Inflict(mix.statusId, (object) this, mix.statusDuration, (object) mix.statusParam, true)), mix.statusInflictDelay, false);
    }
  }
}
