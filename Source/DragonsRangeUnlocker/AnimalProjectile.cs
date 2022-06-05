using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace DragonsRangedAttack;

public class AnimalProjectile : Projectile
{
    protected override void Impact(Thing hitThing)
    {
        var map = Map;
        base.Impact(hitThing);
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
                pawn.stances.StaggerFor(95);
            }

            if (def.defName == "AA_FrostWeb")
            {
                var dinfo2 = new DamageInfo(DamageDefOf.Frostbite, num / 2f, armorPenetration, y, thing, null, null,
                    DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
                hitThing.TakeDamage(dinfo2).AssociateWithLog(battleLogEntry_RangedImpact);
            }

            if (def.defName == "AA_FireWeb")
            {
                var dinfo3 = new DamageInfo(DamageDefOf.Burn, num / 2f, armorPenetration, y, thing, null, null,
                    DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
                hitThing.TakeDamage(dinfo3).AssociateWithLog(battleLogEntry_RangedImpact);
            }

            if (def.defName == "AA_AcidicWeb")
            {
                var dinfo4 = new DamageInfo(DefDatabase<DamageDef>.GetNamed("AA_AcidSpit"), num / 2f, armorPenetration,
                    y, thing, null, null, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
                hitThing.TakeDamage(dinfo4).AssociateWithLog(battleLogEntry_RangedImpact);
            }

            if (def.defName != "AA_ExplodingWeb")
            {
                return;
            }

            var dinfo5 = new DamageInfo(DamageDefOf.Bomb, num / 2f, armorPenetration, y, thing, null, null,
                DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
            hitThing.TakeDamage(dinfo5).AssociateWithLog(battleLogEntry_RangedImpact);
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