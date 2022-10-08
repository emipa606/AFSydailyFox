using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace DragonsRangedAttack;

public class Projectile_Explode : Projectile
{
    public override Quaternion ExactRotation
    {
        get
        {
            var forward = destination - origin;
            forward.y = 0f;
            return Quaternion.LookRotation(forward);
        }
    }

    protected override void Impact(Thing hitThing, bool blockedByShield = false)
    {
        Ignite();
    }

    protected virtual void Ignite()
    {
        var map = Map;
        Destroy();
        var explosionRadius = def.projectile.explosionRadius;
        var list = SimplePool<List<IntVec3>>.Get();
        list.Clear();
        list.AddRange(def.projectile.damageDef.Worker.ExplosionCellsToHit(Position, map, explosionRadius));
        FleckMaker.Static(Position, map, FleckDefOf.ExplosionFlash, explosionRadius * 4f);
        for (var i = 0; i < 4; i++)
        {
            FleckMaker.ThrowSmoke(Position.ToVector3Shifted() + Gen.RandomHorizontalVector(explosionRadius * 0.7f), map,
                explosionRadius * 0.6f);
        }

        if (def.projectile.explosionEffect != null)
        {
            var effecter = def.projectile.explosionEffect.Spawn();
            effecter.Trigger(new TargetInfo(Position, map), new TargetInfo(Position, map));
            effecter.Cleanup();
        }

        GenExplosion.DoExplosion(Position, map, def.projectile.explosionRadius, def.projectile.damageDef, launcher,
            def.projectile.GetDamageAmount(1f), def.projectile.GetArmorPenetration(1f), def.projectile.soundExplode,
            equipmentDef, def, null, def.projectile.postExplosionSpawnThingDef, def.projectile.postExplosionSpawnChance,
            def.projectile.postExplosionSpawnThingCount, def.projectile.postExplosionGasType,
            def.projectile.applyDamageToExplosionCellsNeighbors,
            def.projectile.preExplosionSpawnThingDef, def.projectile.preExplosionSpawnChance,
            def.projectile.preExplosionSpawnThingCount, def.projectile.explosionChanceToStartFire,
            def.projectile.explosionDamageFalloff);
    }
}