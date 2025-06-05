// Decompiled with JetBrains decompiler
// Type: Crystallic.Options
// Assembly: Crystallic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 761B0351-0534-4830-8D43-C230E538461A
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa25472.41565.rartemp\Crystallic.dll

using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Crystallic
{
  public class Options : ThunderScript
  {
    [ModOption("Enemies Use Spell", "Enables/Disables enemies using the spell.")]
    [ModOptionCategory("Creatures", -1)]
    public static bool useSpell = true;
    [ModOption("Enemies Use Crystal Imbues", "Enables/Disables enemies using the Crystal Imbues.")]
    [ModOptionCategory("Creatures", -1)]
    public static bool useCrystalImbues = true;
    [ModOption("Enemies Use Crystallic Imbue", "Enables/Disables enemies using the Crystallic imbue.")]
    [ModOptionCategory("Creatures", -1)]
    public static bool useImbues = true;
    [ModOption("Enemies Use Misc Skills", "Enables/Disables enemies using misc Crystallic skills.")]
    [ModOptionCategory("Creatures", -1)]
    public static bool useMisc = true;
    public static Dictionary<Creature, List<string>> unloaded = new Dictionary<Creature, List<string>>();
    public static Dictionary<Creature, string> unloadedSpells = new Dictionary<Creature, string>();
    public static Settings settings;

    [ModOption("Refresh Enemies", "Refreshes the creature skills, if you've re-enabled a previously disabled option and don't want to restart the level, use this.")]
    [ModOptionCategory("Creatures", -1)]
    [ModOptionButton]
    public static void Refresh(bool _)
    {
      for (int index = 0; index < Creature.allActive.Count; ++index)
      {
        Creature key = Creature.allActive[index];
        if ((bool) (Object) key && !key.isKilled && !key.isPlayer && Options.unloaded.ContainsKey(key))
        {
          Debug.Log((object) (string.Format("Refreshing skills for {0}:\n - ", (object) key) + string.Join("\n - ", (IEnumerable<string>) Options.unloaded[key])));
          foreach (string str in Options.unloaded[key])
            key.ForceLoadSkill(str);
        }
        if (Options.unloadedSpells.ContainsKey(key))
          key.mana.AddSpell(Catalog.GetData<SpellData>(Options.unloadedSpells[key], true));
      }
    }

    public virtual void ScriptEnable()
    {
      base.ScriptEnable();
      Options.settings = Catalog.GetData<Settings>("Settings", true);
      // ISSUE: method pointer
      EventManager.onCreatureSpawn += new EventManager.CreatureSpawnedEvent((object) this, __methodptr(OnCreatureSpawn));
    }

    private void OnCreatureSpawn(Creature creature)
    {
      if (creature.isPlayer)
        return;
      if (!Options.useSpell && !string.IsNullOrEmpty(Options.settings.spellId) && creature.HasSkill(Options.settings.spellId))
      {
        creature.ForceUnloadSkill(Options.settings.spellId);
        creature.mana.RemoveSpell(Options.settings.spellId);
        Debug.Log((object) "Disabling spells");
        if (!Options.unloaded.ContainsKey(creature))
          Options.unloaded.Add(creature, new List<string>());
        Options.unloadedSpells.Add(creature, Options.settings.spellId);
      }
      if (!Options.useCrystalImbues && Options.settings.skills["crystalImbues"] != null)
      {
        List<string> skill = Options.settings.skills["crystalImbues"];
        for (int index = 0; index < skill.Count; ++index)
        {
          if (creature.HasSkill(skill[index]))
          {
            creature.ForceUnloadSkill(skill[index]);
            if (!Options.unloaded.ContainsKey(creature))
              Options.unloaded.Add(creature, new List<string>());
            Options.unloaded[creature].Add(skill[index]);
          }
        }
      }
      if (!Options.useImbues && Options.settings.skills["imbues"] != null)
      {
        List<string> skill = Options.settings.skills["imbues"];
        for (int index = 0; index < skill.Count; ++index)
        {
          if (creature.HasSkill(skill[index]))
          {
            creature.ForceUnloadSkill(skill[index]);
            if (!Options.unloaded.ContainsKey(creature))
              Options.unloaded.Add(creature, new List<string>());
            Options.unloaded[creature].Add(skill[index]);
          }
        }
      }
      if (Options.useMisc || Options.settings.skills["misc"] == null)
        return;
      List<string> skill1 = Options.settings.skills["misc"];
      for (int index = 0; index < skill1.Count; ++index)
      {
        if (creature.HasSkill(skill1[index]))
        {
          creature.ForceUnloadSkill(skill1[index]);
          if (!Options.unloaded.ContainsKey(creature))
            Options.unloaded.Add(creature, new List<string>());
          Options.unloaded[creature].Add(skill1[index]);
        }
      }
    }
  }
}
