using System.Collections.Generic;
using RimWorld;
using Verse;

namespace SydailyFox_Settingpack;

public class InteractionWorker_SydailyFoxNuzzle : InteractionWorker
{
    public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks,
        out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
    {
        AddNuzzledThought(initiator, recipient);
        //TryGiveName(initiator, recipient);
        letterText = null;
        letterLabel = null;
        letterDef = null;
        lookTargets = null;
    }

    private void AddNuzzledThought(Pawn initiator, Pawn recipient)
    {
        var newThought = (Thought_Memory)ThoughtMaker.MakeThought(ThoughtDef.Named("SydailyFoxNuzzleThought"));
        recipient.needs.mood.thoughts.memories.TryGainMemory(newThought);
    }

    //private void TryGiveName(Pawn initiator, Pawn recipient)
    //{
    //	if ((initiator.Name == null || initiator.Name.Numerical) && Rand.Value < initiator.RaceProps.nuzzleMtbHours)
    //	{
    //		PawnUtility.GiveNameBecauseOfNuzzle(recipient, initiator);
    //	}
    //}
}