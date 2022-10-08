using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace DragonsRangedAttack;

public class SiegeProjectile : Projectile
{
    protected override void Impact(Thing hitThing, bool blockedByShield = false)
    {
        var map = Map;
        base.Impact(hitThing, blockedByShield);
        var battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(launcher, hitThing, intendedTarget.Thing,
            ThingDef.Named("Gun_Autopistol"), def, targetCoverDef);
        Find.BattleLog.Add(battleLogEntry_RangedImpact);
        if (hitThing != null)
        {
            var damageDef = def.projectile.damageDef;
            float num = DamageAmount;
            var armorPenetration = ArmorPenetration;
            var y = ExactRotation.eulerAngles.y;
            var thing = launcher;
            var dinfo = new DamageInfo(damageDef, num, armorPenetration, y, thing, null, null,
                DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
            hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
            if (hitThing is Pawn { stances: { } } pawn && pawn.BodySize <= def.projectile.StoppingPower + 0.001f)
            {
                pawn.stances.stagger.StaggerFor(95);
            }

            if (hitThing is not Building building ||
                !building.def.building.isNaturalRock && building.def != ThingDefOf.Wall)
            {
                return;
            }

            var dinfo2 = new DamageInfo(damageDef, num * 5f, armorPenetration * 3f, y, thing, null, null,
                DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
            hitThing.TakeDamage(dinfo2).AssociateWithLog(battleLogEntry_RangedImpact);
        }
        else
        {
            SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(Position, map));
            FleckMaker.Static(ExactPosition, map, FleckDefOf.ShotHit_Dirt);
            if (Position.GetTerrain(map).takeSplashes)
            {
                FleckMaker.WaterSplash(ExactPosition, map, Mathf.Sqrt(DamageAmount) * 1f, 4f);
            }
        }
    }
}