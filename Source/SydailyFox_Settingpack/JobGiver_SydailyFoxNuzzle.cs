using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace SydailyFox_Settingpack;

public class JobGiver_SydailyFoxNuzzle : ThinkNode_JobGiver
{
    private const float MaxNuzzleDistance = 40f;

    protected override Job TryGiveJob(Pawn pawn)
    {
        if (pawn.RaceProps.nuzzleMtbHours <= 0f)
        {
            return null;
        }

        if (!(from p in pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction)
                where !p.NonHumanlikeOrWildMan() && p != pawn && p.Position.InHorDistOf(pawn.Position, 40f) &&
                      pawn.GetRoom(RegionType.Normal | RegionType.Portal) ==
                      p.GetRoom(RegionType.Normal | RegionType.Portal) && !p.Position.IsForbidden(pawn) &&
                      p.CanCasuallyInteractNow()
                select p).TryRandomElement(out var result))
        {
            return null;
        }

        var job = JobMaker.MakeJob(DFFerian_Job.SydailyFoxNuzzleJob, result);
        job.locomotionUrgency = LocomotionUrgency.Walk;
        job.expiryInterval = 1200;
        return job;
    }
}