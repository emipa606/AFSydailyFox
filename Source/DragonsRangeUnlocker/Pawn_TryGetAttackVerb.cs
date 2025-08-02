using HarmonyLib;
using Verse;

namespace DragonsRangedAttack;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.TryGetAttackVerb))]
public static class Pawn_TryGetAttackVerb
{
    private static bool Prefix(ref Pawn __instance, ref Verb __result)
    {
        if (!__instance.AnimalOrWildMan())
        {
            return true;
        }

        var allVerbs = __instance.verbTracker.AllVerbs;
        foreach (var verb in allVerbs)
        {
            if (!(verb.verbProps.range > 1.1f))
            {
                continue;
            }

            __result = verb;
            return false;
        }

        return true;
    }
}