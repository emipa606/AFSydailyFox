using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace SydailyFox_Settingpack;

public class JobDriver_SydailyFoxNuzzleJob : JobDriver
{
    private const int NuzzleDuration = 100;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnNotCasualInterruptible(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        yield return Toils_Interpersonal.WaitToBeAbleToInteract(pawn);
        Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).socialMode = RandomSocialMode.Off;
        Toils_General.WaitWith(TargetIndex.A, NuzzleDuration, false, true).socialMode = RandomSocialMode.Off;
        yield return Toils_General.Do(delegate
        {
            var recipient = (Pawn)pawn.CurJob.targetA.Thing;
            pawn.interactions.TryInteractWith(recipient,
                DefDatabase<InteractionDef>.GetNamed("InteractionThatSydailyFoxNuzzle"));
        });
    }
}