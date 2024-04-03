using RimWorld;
using Verse;

namespace SydailyFox_Settingpack;

public class ThoughtWorker_BloodFog_B : ThoughtWorker
{
    private const float Radius = 2520f;

    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (!p.Spawned)
        {
            return false;
        }

        var list = p.Map.listerThings.ThingsOfDef(DFFerian_Thing.AF_FailedExperiments);
        foreach (var thing in list)
        {
            var compPowerTrader = thing.TryGetComp<CompPowerTrader>();
            if ((compPowerTrader == null || compPowerTrader.PowerOn) && p.Position.InHorDistOf(thing.Position, Radius))
            {
                return true;
            }
        }

        return false;
    }
}