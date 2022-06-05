using UnityEngine;
using Verse;

namespace SydailyFox_Settingpack;

public class TrytoMakeSydailyFoxWarningEffect : Verb_Shoot
{
    protected override bool TryCastShot()
    {
        if (!base.TryCastShot())
        {
            return false;
        }

        var loc = caster.PositionHeld.ToVector3();
        var mapHeld = caster.MapHeld;
        var size = Mathf.Clamp01(this.GetProjectile()?.projectile?.GetDamageAmount(caster) / 1f ?? 1f);
        AF_MakeSydailyFox_Warning.ShootSydailyFlash(loc, mapHeld, size);
        AF_MakeSydailyFox_Warning.AF_MakeSydailyBomb_SpawnFlash.ShootSydailyFlash(loc, mapHeld, size);
        return true;
    }

    [StaticConstructorOnStartup]
    public class AF_MakeSydailyFox_Warning
    {
        public TrytoMakeSydailyFoxWarningEffect caster;

        public static void ShootSydailyFlash(Vector3 loc, Map map, float size)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }

            var obj = (MoteThrown)ThingMaker.MakeThing(DFFerian_Thing.SydailyFox_Warning);
            obj.Scale = Rand.Range(1f, 1f) * size;
            obj.exactPosition = loc;
            obj.rotationRate = Rand.Range(0f, 0f);
            obj.SetVelocity(Rand.Range(0, 0), Rand.Range(0f, 0f));
            GenSpawn.Spawn(obj, loc.ToIntVec3(), map);
        }

        [StaticConstructorOnStartup]
        public class AF_MakeSydailyBomb_SpawnFlash
        {
            public TrytoMakeSydailyFoxWarningEffect caster;

            public static void ShootSydailyFlash(Vector3 loc, Map map, float size)
            {
                if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
                {
                    return;
                }

                var obj = (MoteThrown)ThingMaker.MakeThing(DFFerian_Thing.AF_SydailyBomb_SpawnFlash);
                obj.Scale = Rand.Range(1f, 1f) * size;
                obj.exactPosition = loc;
                obj.rotationRate = Rand.Range(0f, 0f);
                obj.SetVelocity(Rand.Range(0, 0), Rand.Range(0f, 0f));
                GenSpawn.Spawn(obj, loc.ToIntVec3(), map);
            }
        }
    }
}