// Decompiled with JetBrains decompiler
// Type: Arcana.Misc.MaterialSwapManager
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ThunderRoad;
using ThunderRoad.Manikin;
using UnityEngine;
using UnityEngine.Rendering;

#nullable disable
namespace Arcana.Misc
{
  internal class MaterialSwapManager
  {
    public Dictionary<Creature, CreatureMaterialContainer> managedCreatures;
    private string targetShaderName = "ThunderRoad/LitMoss - ASshader";

    public MaterialSwapManager()
    {
      this.managedCreatures = new Dictionary<Creature, CreatureMaterialContainer>();
    }

    public void SwapCreature(Creature creature, Shader shader)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      MaterialSwapManager.\u003C\u003Ec__DisplayClass3_0 cDisplayClass30 = new MaterialSwapManager.\u003C\u003Ec__DisplayClass3_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass30.\u003C\u003E4__this = this;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass30.creature = creature;
      // ISSUE: reference to a compiler-generated field
      this.print(cDisplayClass30.creature);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (cDisplayClass30.creature.isPlayer || cDisplayClass30.creature.isKilled)
        return;
      // ISSUE: reference to a compiler-generated field
      if (this.managedCreatures.ContainsKey(cDisplayClass30.creature))
      {
        // ISSUE: reference to a compiler-generated field
        this.ResetCreature(cDisplayClass30.creature);
      }
      // ISSUE: reference to a compiler-generated field
      CreatureMaterialContainer materialContainer = new CreatureMaterialContainer(cDisplayClass30.creature);
      // ISSUE: reference to a compiler-generated field
      this.managedCreatures[cDisplayClass30.creature] = materialContainer;
      List<ManikinProperties> manikinPropertiesList;
      // ISSUE: reference to a compiler-generated field
      if (cDisplayClass30.creature.manikinProperties.TryGetPrivate<List<ManikinProperties>>("childrenProperties", out manikinPropertiesList))
      {
        foreach (ManikinProperties manikinProperties in manikinPropertiesList)
        {
          MaterialInstance materialInstance;
          if (manikinProperties.TryGetPrivate<MaterialInstance>("materialInstance", out materialInstance))
          {
            Material[] materialArray = materialInstance.AcquireMaterials();
            Material[] materials = new Material[materialArray.Length];
            for (int index = 0; index < materialArray.Length; ++index)
              materials[index] = !(materialArray[index].shader.name == this.targetShaderName) ? materialArray[index] : this.CopyAllMaterialProperties(materialArray[index], shader);
            materialInstance.materials = materials;
            if (materialArray[materialArray.Length - 1].name.IndexOf("chest", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
              // ISSUE: reference to a compiler-generated field
              cDisplayClass30.creature.SetBodyMaterials(sortMaterialsByLOD(materials));
            }
            if (materialArray[materialArray.Length - 1].name.IndexOf("hands", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
              // ISSUE: reference to a compiler-generated field
              cDisplayClass30.creature.SetHandsMaterials(sortMaterialsByLOD(materials));
            }
            manikinProperties.ApplyOcclusionBitmask();
          }
        }
      }
      // ISSUE: reference to a compiler-generated field
      cDisplayClass30.creature.manikinLocations.UpdateParts();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass30.creature.manikinProperties.UpdateProperties();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass30.creature.UpdateRenderers();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      cDisplayClass30.creature.OnDespawnEvent += new Creature.DespawnEvent((object) cDisplayClass30, __methodptr(\u003CSwapCreature\u003Eb__1));
      // ISSUE: reference to a compiler-generated field
      this.print(cDisplayClass30.creature);

      static Material[] sortMaterialsByLOD(Material[] materials)
      {
        int result;
        return ((IEnumerable<Material>) materials).OrderBy<Material, int>((Func<Material, int>) (material => !int.TryParse(Regex.Match(material.name, "LOD(\\d+)").Groups[1].Value, out result) ? -1 : result)).ToArray<Material>();
      }
    }

    private void print(Creature creature)
    {
      string message = "";
      foreach (Creature.RendererData renderer in creature.renderers)
      {
        foreach (Material material in renderer.renderer.materials)
        {
          message = message + "\n" + renderer.renderer.gameObject.name + ": " + material.shader.name;
          message = message + "\n \t" + string.Format("BitMask: {0}", (object) material.GetFloat("_Bitmask"));
        }
      }
      Debug.LogWarning((object) message);
    }

    public void ResetCreature(Creature creature)
    {
      if (!this.managedCreatures.ContainsKey(creature))
        return;
      creature.SetHandsMaterials(new Material[2]
      {
        this.managedCreatures[creature].handMaterialLOD0,
        this.managedCreatures[creature].handMaterialLOD1
      });
      creature.SetBodyMaterials(new Material[2]
      {
        this.managedCreatures[creature].bodyMaterialLOD0,
        this.managedCreatures[creature].bodyMaterialLOD1
      });
      creature.manikinProperties.UpdateProperties();
      foreach (Renderer key in this.managedCreatures[creature].renderers.Keys.ToArray<Renderer>())
      {
        if ((UnityEngine.Object) key != (UnityEngine.Object) null)
        {
          key.materials = this.managedCreatures[creature].renderers[key];
          PlaneClipManager component;
          if (!key.gameObject.TryGetComponent<PlaneClipManager>(out component))
            ;
          UnityEngine.Object.Destroy((UnityEngine.Object) component);
        }
        this.managedCreatures[creature].renderers.Remove(key);
      }
      this.managedCreatures.Remove(creature);
    }

    public void ConfigPlaneClip(Creature creature, Vector3 planePoint, Vector3 planeNormal)
    {
      if (!this.managedCreatures.ContainsKey(creature))
        return;
      foreach (PlaneClipManager componentsInChild in ((Component) creature).GetComponentsInChildren<PlaneClipManager>())
      {
        componentsInChild.planeNormal = planeNormal;
        componentsInChild.planePoint = planePoint;
      }
    }

    private void OnDespawn(Creature creature, EventTime time)
    {
      if (time != null || !this.managedCreatures.ContainsKey(creature))
        return;
      this.ResetCreature(creature);
    }

    private Material CopyAllMaterialProperties(Material material, Shader shader)
    {
      Material material1 = new Material(shader);
      int propertyCount = material.shader.GetPropertyCount();
      for (int propertyIndex = 0; propertyIndex < propertyCount; ++propertyIndex)
      {
        string propertyName = material.shader.GetPropertyName(propertyIndex);
        ShaderPropertyType propertyType = material.shader.GetPropertyType(propertyIndex);
        switch (propertyType)
        {
          case ShaderPropertyType.Color:
            if (material.HasProperty(propertyName))
            {
              Color color = material.GetColor(propertyName);
              material1.SetColor(propertyName, color);
              break;
            }
            break;
          case ShaderPropertyType.Vector:
            if (material.HasProperty(propertyName))
            {
              Vector4 vector = material.GetVector(propertyName);
              material1.SetVector(propertyName, vector);
              break;
            }
            break;
          case ShaderPropertyType.Float:
          case ShaderPropertyType.Range:
            if (material.HasProperty(propertyName))
            {
              float num = material.GetFloat(propertyName);
              material1.SetFloat(propertyName, num);
              break;
            }
            break;
          case ShaderPropertyType.Texture:
            if (material.HasProperty(propertyName))
            {
              Texture texture = material.GetTexture(propertyName);
              material1.SetTexture(propertyName, texture);
              break;
            }
            break;
          case ShaderPropertyType.Int:
            if (material.HasProperty(propertyName))
            {
              int num = material.GetInt(propertyName);
              material1.SetInt(propertyName, num);
              break;
            }
            break;
          default:
            Debug.LogWarning((object) ("Unknown property type: " + propertyType.ToString()));
            break;
        }
      }
      return material1;
    }
  }
}
