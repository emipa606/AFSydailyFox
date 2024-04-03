using RimWorld;
using UnityEngine;
using Verse;

namespace SydailyFox_Settingpack;

public class IncidentWorker_StrangeCapsule : IncidentWorker
{
    private static readonly Pair<int, float>[] CountChance;

    static IncidentWorker_StrangeCapsule()
    {
        CountChance =
        [
            new Pair<int, float>(1, 1f)
        ];
    }

    protected override bool CanFireNowSub(IncidentParms parms)
    {
        if (!base.CanFireNowSub(parms))
        {
            return false;
        }

        var map = (Map)parms.target;
        return TryFindShipChunkDropCell(map.Center, map, 999999, out _);
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        var map = (Map)parms.target;
        if (!TryFindShipChunkDropCell(map.Center, map, 999999, out var pos))
        {
            return false;
        }

        SpawnShipChunks(pos, map, get_RandomCountToDrop());
        var threatBig = LetterDefOf.ThreatBig;
        var text = string.Format(def.letterText).CapitalizeFirst();
        SendStandardLetter(def.LabelCap, text, threatBig, parms, new TargetInfo(pos, map));
        return true;
    }

    private void SpawnShipChunks(IntVec3 firstChunkPos, Map map, int count)
    {
        SpawnChunk(firstChunkPos, map);
        for (var i = 0; i < count - 1; i++)
        {
            if (TryFindShipChunkDropCell(firstChunkPos, map, 5, out var pos))
            {
                SpawnChunk(pos, map);
            }
        }
    }

    private void SpawnChunk(IntVec3 pos, Map map)
    {
        SkyfallerMaker.SpawnSkyfaller(DFFerian_Thing.AF_IC_StrangeCapsule, DFFerian_Thing.AF_StrangeCapsule, pos, map);
    }

    private bool TryFindShipChunkDropCell(IntVec3 nearLoc, Map map, int maxDist, out IntVec3 pos)
    {
        return CellFinderLoose.TryFindSkyfallerCell(DFFerian_Thing.AF_IC_StrangeCapsule, map, out pos, 10, nearLoc,
            maxDist);
    }

    private int get_RandomCountToDrop()
    {
        var x2 = Find.TickManager.TicksGame / 3600000f;
        var timePassedFactor = Mathf.Clamp(GenMath.LerpDouble(0f, 1.2f, 1f, 0.1f, x2), 0.1f, 1f);
        return CountChance.RandomElementByWeight(x => x.First == 1 ? x.Second : x.Second * timePassedFactor).First;
    }
}