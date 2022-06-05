using RimWorld;
using Verse;

namespace SydailyFox_Settingpack;

public class ThoughtWorker_FoxGoodThink : ThoughtWorker
{
    private const float Radius = 520f;

    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (!p.Spawned)
        {
            return false;
        }

        foreach (var item in p.Map.mapPawns.PawnsInFaction(Faction.OfPlayer))
        {
            if (item.kindDef == DFFerian_PawnKind.AF_SydailyFox)
            {
                return true;
            }
        }

        return false;
    }
}