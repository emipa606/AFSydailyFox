using RimWorld;
using Verse;

namespace SydailyFox_Settingpack;

public class ThoughtWorker_BloodFog_A : ThoughtWorker
{
    private const float Radius = 2520f;

    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (!p.Spawned)
        {
            return false;
        }

        var list = p.Map.listerThings.ThingsOfDef(DFFerian_Thing.AF_StrangeCapsule);
        foreach (var thing in list)
        {
            var compPowerTrader = thing.TryGetComp<CompPowerTrader>();
            if ((compPowerTrader == null || compPowerTrader.PowerOn) && p.Position.InHorDistOf(thing.Position, 2520f))
            {
                return true;
            }
        }

        return false;
    }
}