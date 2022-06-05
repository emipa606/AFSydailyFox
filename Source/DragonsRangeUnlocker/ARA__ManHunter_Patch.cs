using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace DragonsRangedAttack;

[HarmonyPatch(typeof(JobGiver_Manhunter), "TryGiveJob")]
public static class ARA__ManHunter_Patch
{
    private static bool Prefix(ref JobGiver_Manhunter __instance, ref Job __result, ref Pawn pawn)
    {
        var rangedVerb = false;
        var allVerbs = pawn.verbTracker.AllVerbs;
        var list = new List<Verb>();
        foreach (var verb in allVerbs)
        {
            if (!(verb.verbProps.range > 1.1f))
            {
                continue;
            }

            list.Add(verb);
            rangedVerb = true;
        }

        bool result;
        if (!rangedVerb)
        {
            result = true;
        }
        else
        {
            var verb = list.RandomElementByWeight(rangeItem => rangeItem.verbProps.commonality);
            if (verb == null)
            {
                Log.Warning("Can't get random range verb");
                result = true;
            }
            else
            {
                var thing = (Thing)ARA_AttackTargetFinder.BestAttackTarget(pawn,
                    TargetScanFlags.NeedReachable | TargetScanFlags.NeedThreat, x => x is Pawn || x is Building);
                if (thing == null)
                {
                    thing = (Thing)ARA_AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedThreat,
                        x => x is Pawn || x is Building);
                }

                var foundTarget = false;
                Thing thing2 = null;
                if (thing == null)
                {
                    thing2 = (Thing)ARA_AttackTargetFinder.BestShootTargetFromCurrentPosition(pawn,
                        x => x is Pawn || x is Building, verb.verbProps.range, verb.verbProps.minRange,
                        TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedThreat |
                        TargetScanFlags.LOSBlockableByGas);
                    if (thing2 == null)
                    {
                        return true;
                    }

                    foundTarget = true;
                }
                else if (thing.Position.DistanceTo(pawn.Position) < verb.verbProps.minRange ||
                         thing.Position.AdjacentTo8Way(pawn.Position))
                {
                    if (!pawn.CanReach((LocalTargetInfo)thing, PathEndMode.Touch, Danger.Deadly))
                    {
                        return true;
                    }

                    __result = new Job(JobDefOf.AttackMelee, thing)
                    {
                        maxNumMeleeAttacks = 1,
                        expiryInterval = Rand.Range(420, 900),
                        attackDoorIfTargetLost = false
                    };
                    return false;
                }

                if (!foundTarget)
                {
                    thing2 = (Thing)ARA_AttackTargetFinder.BestShootTargetFromCurrentPosition(pawn,
                        x => x is Pawn || x is Building, verb.verbProps.range, verb.verbProps.minRange,
                        TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedThreat |
                        TargetScanFlags.LOSBlockableByGas);
                }

                if (thing2 != null)
                {
                    if (thing != null && (thing.Position.DistanceTo(pawn.Position) < verb.verbProps.minRange ||
                                          thing.Position.AdjacentTo8Way(pawn.Position)))
                    {
                        if (pawn.CanReach((LocalTargetInfo)thing, PathEndMode.Touch, Danger.Deadly))
                        {
                            __result = new Job(JobDefOf.AttackMelee, thing)
                            {
                                maxNumMeleeAttacks = 1,
                                expiryInterval = Rand.Range(420, 900),
                                attackDoorIfTargetLost = false
                            };
                            result = false;
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        var named = DefDatabase<JobDef>.GetNamed("AA_DragonAnimalRangeAttack");
                        LocalTargetInfo targetA = thing2;
                        __result = new Job(named, targetA,
                            JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, true)
                        {
                            verbToUse = verb
                        };
                        result = false;
                    }
                }
                else
                {
                    var newReq = default(CastPositionRequest);
                    newReq.caster = pawn;
                    newReq.target = thing;
                    newReq.verb = verb;
                    newReq.maxRangeFromTarget = 9999f;
                    newReq.wantCoverFromTarget = false;
                    if (!CastPositionFinder.TryFindCastPosition(newReq, out var dest))
                    {
                        result = true;
                    }
                    else if (pawn.Position == dest)
                    {
                        __result = new Job(JobDefOf.Wait, 100);
                        result = false;
                    }
                    else
                    {
                        var job = new Job(JobDefOf.Goto, dest)
                        {
                            expiryInterval = JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange,
                            checkOverrideOnExpire = true
                        };
                        __result = job;
                        result = false;
                    }
                }
            }
        }

        return result;
    }
}