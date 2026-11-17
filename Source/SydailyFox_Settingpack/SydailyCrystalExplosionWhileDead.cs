using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SydailyFox_Settingpack;

internal class SydailyCrystalExplosionWhileDead : DeathActionWorker
{
    public override RulePackDef DeathRules => RulePackDefOf.Transition_DiedExplosive;

    public override bool DangerousInMelee => true;

    public override void PawnDied(Corpse corpse, Lord prevLord)
    {
        if (corpse.InnerPawn.ageTracker.CurLifeStageIndex == 0)
        {
            GenExplosion.DoExplosion(
                radius: 3.6f, center: corpse.Position,
                map: corpse.Map, damType: DFFerian_Damage.AF_DA_SydailyCrystalExplosion_Purple,
                instigator: corpse.InnerPawn);
        }
        else
        {
            GenExplosion.DoExplosion(
                radius: corpse.InnerPawn.ageTracker.CurLifeStageIndex != 120 ? 8.4f : 5.2f, center: corpse.Position,
                map: corpse.Map, damType: DFFerian_Damage.AF_DA_SydailyCrystalExplosion_Purple,
                instigator: corpse.InnerPawn);
        }
    }
}