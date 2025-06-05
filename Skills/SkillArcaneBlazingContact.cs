// Decompiled with JetBrains decompiler
// Type: Arcana.Skills.SkillArcaneBlazingContact
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using Arcana.Misc;
using System;
using System.Collections;
using System.Linq;
using ThunderRoad;
using ThunderRoad.Skill.Spell;
using UnityEngine;

#nullable disable
namespace Arcana.Skills
{
  public class SkillArcaneBlazingContact : SpellSkillArcaneChromaticProjectile
  {
    public float chancePercentage = 1f;
    public float fireballSpeed = 10f;
    public int fireballCount = 3;
    public string spellCastProjectileId = "Fire";
    private SpellCastProjectile spellCastProjectile;

    public override void OnCatalogRefresh()
    {
      base.OnCatalogRefresh();
      this.spellCastProjectile = Catalog.GetData<SpellCastProjectile>(this.spellCastProjectileId, true);
    }

    protected override void OnProjectileHit(
      SpellCastCharge spell,
      ItemMagicProjectile projectile,
      CollisionInstance collision,
      SpellCaster caster)
    {
      if (!this.projectileLookup.ContainsKey(projectile))
      {
        base.OnProjectileHit(spell, projectile, collision, caster);
      }
      else
      {
        if ((double) UnityEngine.Random.Range(0.0f, 1f) > (double) Mathf.Clamp01(this.chancePercentage))
          return;
        Vector3 shootStartAxis = (double) Mathf.Abs(Vector3.Dot(collision.contactNormal.normalized, Vector3.up)) > 0.99000000953674316 ? Vector3.Cross(collision.contactNormal, Vector3.right).normalized : Vector3.Cross(collision.contactNormal, Vector3.up).normalized;
        ((MonoBehaviour) caster).StartCoroutine(this.SpawnFireSparks(collision.contactNormal, shootStartAxis, collision.contactPoint - collision.contactNormal * 0.1f));
        base.OnProjectileHit(spell, projectile, collision, caster);
      }
    }

    private IEnumerator SpawnFireSparks(
      Vector3 direction,
      Vector3 shootStartAxis,
      Vector3 spawnPoint)
    {
      float radius = 20f;
      Creature[] targets = Creature.allActive.Where<Creature>((Func<Creature, bool>) (creature => !creature.isKilled && !creature.isPlayer && !creature.isCulled && (double) (((ThunderBehaviour) creature.ragdoll.targetPart).transform.position - spawnPoint).sqrMagnitude < (double) radius * (double) radius)).Take<Creature>(this.spellCastProjectile.staffSlamNumFireballs).ToArray<Creature>();
      int targetIndex = 0;
      for (int i = 1; i < this.fireballCount + 1; ++i)
      {
        Vector3 vector3 = Utilities.GetRandomVelocityInCone(direction, 90f, 1f, 10f);
        this.spellCastProjectile.ShootFireSpark(this.spellCastProjectile.imbueHitProjectileEffectData, spawnPoint + vector3.normalized * 0.2f, vector3 * this.fireballSpeed, true, targets.Length != 0 ? targets[targetIndex++ % targets.Length] : (Creature) null, 1f, (SpellCastProjectile.ProjectileSpawnEvent) null);
        yield return (object) 0;
        vector3 = new Vector3();
      }
    }
  }
}
