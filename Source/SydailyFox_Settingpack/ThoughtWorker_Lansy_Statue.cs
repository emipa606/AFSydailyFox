using RimWorld;
using Verse;

namespace SydailyFox_Settingpack;

public class ThoughtWorker_Lansy_Statue : ThoughtWorker
{
    private const float Radius = 42f;

    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (!p.Spawned)
        {
            return false;
        }

        var list = p.Map.listerThings.ThingsOfDef(DFFerian_Thing.Lansy_Statue);
        foreach (var thing in list)
        {
            var compPowerTrader = thing.TryGetComp<CompPowerTrader>();
            if ((compPowerTrader == null || compPowerTrader.PowerOn) && p.Position.InHorDistOf(thing.Position, 42f))
            {
                return true;
            }
        }

        return false;
    }
}