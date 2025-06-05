// Decompiled with JetBrains decompiler
// Type: Arcana.Behaviors.LightningDragon
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using System.Collections;
using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Behaviors
{
  public class LightningDragon : ThunderBehaviour
  {
    public LightningDragon.DragonData dragonData;
    public EffectInstance effect;
    private SmoothFollowAndLookAt pd;

    public event LightningDragon.OnDespawn OnDespawnEvent;

    public void Init(LightningDragon.DragonData data)
    {
      this.dragonData = data;
      SphereCollider sphereCollider = ((Component) this).GetComponent<SphereCollider>();
      if ((Object) sphereCollider == (Object) null)
        sphereCollider = this.gameObject.AddComponent<SphereCollider>();
      ((Collider) sphereCollider).isTrigger = true;
      sphereCollider.radius = this.dragonData.hookRadius;
    }

    public void Form(Transform target, Mana mana)
    {
      this.pd = this.gameObject.AddComponent<SmoothFollowAndLookAt>();
      this.pd.positionSpeed = this.dragonData.positionSpeed;
      this.pd.rotationSpeed = this.dragonData.rotationSpeed;
      this.pd.target = target;
      this.transform.position = target.position;
      this.transform.rotation = target.rotation;
      this.effect = this.dragonData.effectData?.Spawn(this.transform, true, (ColliderGroup) null, false);
      ((MonoBehaviour) mana).StartCoroutine(PlayEffectDelay(0.5f));

      IEnumerator PlayEffectDelay(float delay)
      {
        yield return (object) new WaitForSeconds(delay);
        this.effect?.Play(0, false, false);
        if (this.effect != null)
        {
          // ISSUE: method pointer
          this.effect.onEffectFinished += new EffectInstance.EffectFinishEvent((object) this, __methodptr(\u003CForm\u003Eb__8_1));
        }
      }
    }

    public void Despawn()
    {
      this.effect?.End(false, -1f);
      this.CleanupDragon();
    }

    private void CleanupDragon()
    {
      Object.Destroy((Object) this.pd, 0.1f);
      LightningDragon.OnDespawn onDespawnEvent = this.OnDespawnEvent;
      if (onDespawnEvent != null)
        onDespawnEvent(this);
      Object.Destroy((Object) this);
    }

    private void OnTriggerEnter(Collider other)
    {
      if (!((Object) ((Component) other).GetComponentInParent<Creature>() == (Object) null))
        ;
    }

    private void OnTriggerExit(Collider other)
    {
      if (!((Object) ((Component) other).GetComponentInParent<Creature>() == (Object) null))
        ;
    }

    public delegate void OnDespawn(LightningDragon dragon);

    public class DragonData
    {
      public float positionSpeed = 5f;
      public float rotationSpeed = 15f;
      public float hookRadius = 10f;
      public float hookSpeed = 10f;
      public float hookDamper = 150f;
      public float hookSpring = 1000f;
      public string effectId;
      public EffectData effectData;

      public void LoadCatalogData()
      {
        this.effectData = Catalog.GetData<EffectData>(this.effectId, true);
      }
    }
  }
}
