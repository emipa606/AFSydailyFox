using RimWorld;
using Verse;

namespace SydailyFox_Settingpack;

internal class SydailyCrystalExplosionWhileDead : DeathActionWorker
{
    public override RulePackDef DeathRules => RulePackDefOf.Transition_DiedExplosive;

    public override bool DangerousInMelee => true;

    public override void PawnDied(Corpse corpse)
    {
        GenExplosion.DoExplosion(
            radius: corpse.InnerPawn.ageTracker.CurLifeStageIndex == 0 ? 3.6f :
            corpse.InnerPawn.ageTracker.CurLifeStageIndex != 120 ? 8.4f : 5.2f, center: corpse.Position,
            map: corpse.Map, damType: DFFerian_Damage.AF_DA_SydailyCrystalExplosion_Purple,
            instigator: corpse.InnerPawn);
    }
}