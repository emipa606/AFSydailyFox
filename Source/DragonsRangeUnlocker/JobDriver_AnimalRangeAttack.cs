using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace DragonsRangedAttack;

internal class JobDriver_AnimalRangeAttack : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        if (job.targetA.Thing is IAttackTarget target)
        {
            pawn.Map.attackTargetReservationManager.Reserve(pawn, job, target);
        }

        return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Fire(TargetThingA).FailOnDespawnedNullOrForbidden(TargetIndex.A);
    }

    private Toil Fire(Thing target)
    {
        if (target == null)
        {
            return null;
        }

        return new Toil
        {
            initAction = delegate { GetActor().CurJob.verbToUse.TryStartCastOn((LocalTargetInfo)target); },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }
}