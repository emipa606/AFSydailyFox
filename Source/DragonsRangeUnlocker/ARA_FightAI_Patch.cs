using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace DragonsRangedAttack;

[HarmonyPatch(typeof(JobGiver_AIDefendPawn), "TryGiveJob")]
internal static class ARA_FightAI_Patch
{
    private static bool Prefix(ref JobGiver_AIFightEnemy __instance, ref Job __result, ref Pawn pawn)
    {
        bool result;
        if (!pawn.RaceProps.Animal)
        {
            result = true;
        }
        else
        {
            var rangedAttack = false;
            var allVerbs = pawn.verbTracker.AllVerbs;
            var list = new List<Verb>();
            foreach (var verb in allVerbs)
            {
                if (!(verb.verbProps.range > 1.1f))
                {
                    continue;
                }

                list.Add(verb);
                rangedAttack = true;
            }

            if (!rangedAttack)
            {
                result = true;
            }
            else
            {
                var verb = list.RandomElementByWeight(rangeItem => rangeItem.verbProps.commonality);
                if (verb == null)
                {
                    result = true;
                }
                else
                {
                    var thing = (Thing)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedThreat,
                        x => x is Pawn || x is Building, 0f, verb.verbProps.range);
                    if (thing == null)
                    {
                        result = true;
                    }
                    else
                    {
                        if (thing.Position.DistanceTo(pawn.Position) < verb.verbProps.minRange)
                        {
                            if (thing.Position.AdjacentTo8Way(pawn.Position))
                            {
                                __result = new Job(JobDefOf.AttackMelee, thing)
                                {
                                    maxNumMeleeAttacks = 1,
                                    expiryInterval = Rand.Range(420, 900),
                                    attackDoorIfTargetLost = false
                                };
                                return false;
                            }

                            if (pawn.Faction == null || pawn.Faction.def.isPlayer)
                            {
                                if (!pawn.CanReach((LocalTargetInfo)thing, PathEndMode.Touch, Danger.Deadly) ||
                                    !pawn.playerSettings.Master.playerSettings.animalsReleased)
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
                        }

                        if (CoverUtility.CalculateOverallBlockChance(pawn.Position, thing.Position, pawn.Map) >
                            0.00999999977648258 && pawn.Position.Standable(pawn.Map) && verb.CanHitTarget(thing) ||
                            (pawn.Position - thing.Position).LengthHorizontalSquared < 25 && verb.CanHitTarget(thing))
                        {
                            var named = DefDatabase<JobDef>.GetNamed("AA_DragonAnimalRangeAttack");
                            LocalTargetInfo targetA = thing;
                            __result = new Job(named, targetA,
                                JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, true)
                            {
                                verbToUse = verb
                            };
                            result = false;
                        }
                        else
                        {
                            var newReq = default(CastPositionRequest);
                            newReq.caster = pawn;
                            newReq.target = thing;
                            newReq.verb = verb;
                            newReq.maxRangeFromTarget = verb.verbProps.range;
                            newReq.wantCoverFromTarget = pawn.training.HasLearned(TrainableDefOf.Release) &&
                                                         verb.verbProps.range > 7.0;
                            newReq.locus = pawn.playerSettings.Master.Position;
                            newReq.maxRangeFromLocus = Traverse.Create(__instance).Method("GetFlagRadius", pawn)
                                .GetValue<float>();
                            newReq.maxRegions = 50;
                            if (!CastPositionFinder.TryFindCastPosition(newReq, out var dest))
                            {
                                result = true;
                            }
                            else if (dest == pawn.Position)
                            {
                                var named2 = DefDatabase<JobDef>.GetNamed("AA_DragonAnimalRangeAttack");
                                LocalTargetInfo targetA2 = thing;
                                __result = new Job(named2, targetA2,
                                    JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, true)
                                {
                                    verbToUse = verb
                                };
                                result = false;
                            }
                            else
                            {
                                var job = new Job(JobDefOf.Goto, dest)
                                {
                                    expiryInterval =
                                        JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange,
                                    checkOverrideOnExpire = true
                                };
                                __result = job;
                                result = false;
                            }
                        }
                    }
                }
            }
        }

        return result;
    }
}