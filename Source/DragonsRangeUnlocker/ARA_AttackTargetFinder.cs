using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DragonsRangedAttack;

public static class ARA_AttackTargetFinder
{
    private static readonly List<IAttackTarget> tmpTargets = [];

    private static readonly List<Pair<IAttackTarget, float>> availableShootingTargets = [];

    private static readonly List<float> tmpTargetScores = [];

    private static readonly List<bool> tmpCanShootAtTarget = [];

    private static readonly List<IntVec3> tempDestList = [];

    private static readonly List<IntVec3> tempSourceList = [];

    public static IAttackTarget BestAttackTarget(IAttackTargetSearcher searcher, TargetScanFlags flags,
        Predicate<Thing> validator = null, float minDist = 0f, float maxDist = 9999f, IntVec3 locus = default,
        float maxTravelRadiusFromLocus = float.MaxValue, bool canBash = false)
    {
        var searcherThing = searcher.Thing;
        var verb = searcher.CurrentEffectiveVerb;
        if (verb == null)
        {
            return null;
        }

        var onlyTargetMachines = verb.IsEMP();
        var minDistanceSquared = minDist * minDist;
        var num = maxTravelRadiusFromLocus + verb.verbProps.range;
        var maxLocusDistSquared = num * num;
        Func<IntVec3, bool> losValidator = null;
        if ((flags & TargetScanFlags.LOSBlockableByGas) > TargetScanFlags.None)
        {
            losValidator = vec3 => !vec3.AnyGas(searcherThing.Map, GasType.BlindSmoke);
        }

        Predicate<IAttackTarget> innerValidator = delegate(IAttackTarget t)
        {
            var thing = t.Thing;
            bool result2;
            if (t == searcher)
            {
                result2 = false;
            }
            else if (minDistanceSquared > 0f &&
                     (searcherThing.Position - thing.Position).LengthHorizontalSquared < minDistanceSquared)
            {
                result2 = false;
            }
            else if (maxTravelRadiusFromLocus < 9999f &&
                     (thing.Position - locus).LengthHorizontalSquared > maxLocusDistSquared)
            {
                result2 = false;
            }
            else if (!searcherThing.HostileTo(thing))
            {
                result2 = false;
            }
            else if (validator != null && !validator(thing))
            {
                result2 = false;
            }
            else
            {
                if ((flags & TargetScanFlags.NeedLOSToAll) != 0 && !searcherThing.CanSee(thing, losValidator))
                {
                    if (t is Pawn)
                    {
                        if ((flags & TargetScanFlags.NeedLOSToPawns) > TargetScanFlags.None)
                        {
                            return false;
                        }
                    }
                    else if ((flags & TargetScanFlags.NeedLOSToNonPawns) > TargetScanFlags.None)
                    {
                        return false;
                    }
                }

                if ((flags & TargetScanFlags.NeedThreat) != 0 && t.ThreatDisabled(searcher))
                {
                    result2 = false;
                }
                else
                {
                    if (onlyTargetMachines && t is Pawn pawn2 && pawn2.RaceProps.IsFlesh)
                    {
                        result2 = false;
                    }
                    else if ((flags & TargetScanFlags.NeedNonBurning) != 0 && thing.IsBurning())
                    {
                        result2 = false;
                    }
                    else
                    {
                        if (searcherThing.def.race != null && (int)searcherThing.def.race.intelligence >= 2)
                        {
                            var compExplosive = thing.TryGetComp<CompExplosive>();
                            if (compExplosive is { wickStarted: true })
                            {
                                return false;
                            }
                        }

                        if (thing.def.size is { x: 1, z: 1 })
                        {
                            if (thing.Position.Fogged(thing.Map))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            var visible = false;
                            foreach (var intVec3 in thing.OccupiedRect())
                            {
                                if (intVec3.Fogged(thing.Map))
                                {
                                    continue;
                                }

                                visible = true;
                                break;
                            }

                            if (!visible)
                            {
                                return false;
                            }
                        }

                        result2 = true;
                    }
                }
            }

            return result2;
        };
        if (!HasRangedAttack(searcher))
        {
            return null;
        }

        tmpTargets.Clear();
        var req = ThingRequest.ForGroup(ThingRequestGroup.AttackTarget);
        IEnumerable<Thing> enumerable = searcherThing.Map.listerThings.ThingsMatching(req);
        foreach (var item2 in enumerable)
        {
            var item = (IAttackTarget)item2;
            tmpTargets.Add(item);
        }

        if ((flags & TargetScanFlags.NeedReachable) > TargetScanFlags.None)
        {
            var oldValidator = innerValidator;
            innerValidator = t => oldValidator(t) && CanReach(searcherThing, t.Thing, canBash);
        }

        var validTarget = false;
        if (searcherThing.Faction != Faction.OfPlayer)
        {
            foreach (var attackTarget in tmpTargets)
            {
                if (!attackTarget.Thing.Position.InHorDistOf(searcherThing.Position, maxDist) ||
                    !innerValidator(attackTarget) || !CanShootAtFromCurrentPosition(attackTarget, searcher, verb))
                {
                    continue;
                }

                validTarget = true;
                break;
            }
        }

        IAttackTarget result;
        if (validTarget)
        {
            tmpTargets.RemoveAll(x =>
                !x.Thing.Position.InHorDistOf(searcherThing.Position, maxDist) || !innerValidator(x));
            result = GetRandomShootingTargetByScore(tmpTargets, searcher, verb);
        }
        else
        {
            result = (IAttackTarget)GenClosest.ClosestThing_Global(
                validator: (flags & TargetScanFlags.NeedReachableIfCantHitFromMyPos) == 0 ||
                           (flags & TargetScanFlags.NeedReachable) != 0
                    ? t => innerValidator((IAttackTarget)t)
                    : t => innerValidator((IAttackTarget)t) && (CanReach(searcherThing, t, canBash) ||
                                                                CanShootAtFromCurrentPosition((IAttackTarget)t,
                                                                    searcher, verb)),
                center: searcherThing.Position, searchSet: tmpTargets, maxDistance: maxDist);
        }

        tmpTargets.Clear();
        return result;
    }

    private static bool CanReach(Thing searcher, Thing target, bool canBash)
    {
        if (searcher is Pawn pawn)
        {
            if (!pawn.CanReach((LocalTargetInfo)target, PathEndMode.Touch, Danger.Some, canBash, canBash))
            {
                return false;
            }
        }
        else
        {
            var traverseMode = (TraverseMode)(canBash ? 1u : 2u);
            if (!searcher.Map.reachability.CanReach(searcher.Position, target, PathEndMode.Touch,
                    TraverseParms.For(null, Danger.Deadly, traverseMode)))
            {
                return false;
            }
        }

        return true;
    }

    private static bool HasRangedAttack(IAttackTargetSearcher t)
    {
        var currentEffectiveVerb = t.CurrentEffectiveVerb;
        return currentEffectiveVerb != null && !currentEffectiveVerb.verbProps.IsMeleeAttack;
    }

    private static bool CanShootAtFromCurrentPosition(IAttackTarget target, IAttackTargetSearcher searcher, Verb verb)
    {
        return verb?.CanHitTargetFrom(searcher.Thing.Position, target.Thing) ?? false;
    }

    private static IAttackTarget GetRandomShootingTargetByScore(List<IAttackTarget> targets,
        IAttackTargetSearcher searcher, Verb verb)
    {
        return GetAvailableShootingTargetsByScore(targets, searcher, verb)
            .TryRandomElementByWeight(x => x.Second, out var result)
            ? result.First
            : null;
    }

    private static List<Pair<IAttackTarget, float>> GetAvailableShootingTargetsByScore(List<IAttackTarget> rawTargets,
        IAttackTargetSearcher searcher, Verb verb)
    {
        availableShootingTargets.Clear();
        if (rawTargets.Count == 0)
        {
            return availableShootingTargets;
        }

        tmpTargetScores.Clear();
        tmpCanShootAtTarget.Clear();
        var num = 0f;
        IAttackTarget attackTarget = null;
        for (var i = 0; i < rawTargets.Count; i++)
        {
            tmpTargetScores.Add(float.MinValue);
            tmpCanShootAtTarget.Add(false);
            if (rawTargets[i] == searcher)
            {
                continue;
            }

            tmpCanShootAtTarget[i] = CanShootAtFromCurrentPosition(rawTargets[i], searcher, verb);
            if (!tmpCanShootAtTarget[i])
            {
                continue;
            }

            var shootingTargetScore = GetShootingTargetScore(rawTargets[i], searcher, verb);
            tmpTargetScores[i] = shootingTargetScore;
            if (attackTarget != null && !(shootingTargetScore > num))
            {
                continue;
            }

            attackTarget = rawTargets[i];
            num = shootingTargetScore;
        }

        if (num < 1f)
        {
            if (attackTarget != null)
            {
                availableShootingTargets.Add(new Pair<IAttackTarget, float>(attackTarget, 1f));
            }
        }
        else
        {
            var num2 = num - 30f;
            for (var j = 0; j < rawTargets.Count; j++)
            {
                if (rawTargets[j] == searcher || !tmpCanShootAtTarget[j])
                {
                    continue;
                }

                var num3 = tmpTargetScores[j];
                if (!(num3 >= num2))
                {
                    continue;
                }

                var second = Mathf.InverseLerp(num - 30f, num, num3);
                availableShootingTargets.Add(new Pair<IAttackTarget, float>(rawTargets[j], second));
            }
        }

        return availableShootingTargets;
    }

    private static float GetShootingTargetScore(IAttackTarget target, IAttackTargetSearcher searcher, Verb verb)
    {
        var num = 60f;
        num -= Mathf.Min((target.Thing.Position - searcher.Thing.Position).LengthHorizontal, 40f);
        if (target.TargetCurrentlyAimingAt == searcher.Thing)
        {
            num += 10f;
        }

        if (searcher.LastAttackedTarget == target.Thing &&
            Find.TickManager.TicksGame - searcher.LastAttackTargetTick <= 300)
        {
            num += 40f;
        }

        num -= CoverUtility.CalculateOverallBlockChance(target.Thing.Position, searcher.Thing.Position,
            searcher.Thing.Map) * 10f;
        if (target is Pawn pawn && pawn.RaceProps.Animal && pawn.Faction != null && !pawn.IsFighting())
        {
            num -= 50f;
        }

        return num + FriendlyFireShootingTargetScoreOffset(target, searcher, verb);
    }

    private static float FriendlyFireShootingTargetScoreOffset(IAttackTarget target, IAttackTargetSearcher searcher,
        Verb verb)
    {
        if (verb.verbProps.ai_AvoidFriendlyFireRadius <= 0f)
        {
            return 0f;
        }

        var map = target.Thing.Map;
        var position = target.Thing.Position;
        var num = GenRadial.NumCellsInRadius(verb.verbProps.ai_AvoidFriendlyFireRadius);
        var num2 = 0f;
        for (var i = 0; i < num; i++)
        {
            var intVec = position + GenRadial.RadialPattern[i];
            if (!intVec.InBounds(map))
            {
                continue;
            }

            var searchForTarget = true;
            var thingList = intVec.GetThingList(map);
            foreach (var thing in thingList)
            {
                if (thing is not IAttackTarget || thing == target)
                {
                    continue;
                }

                if (searchForTarget)
                {
                    if (!GenSight.LineOfSight(position, intVec, map, true))
                    {
                        break;
                    }

                    searchForTarget = false;
                }

                var num3 = thing == searcher ? 40f :
                    thing is not Pawn ? 10f :
                    !thing.def.race.Animal ? 18f : 7f;
                num2 = !searcher.Thing.HostileTo(thing) ? num2 - num3 : num2 + (num3 * 0.6f);
            }
        }

        return Mathf.Min(num2, 0f);
    }

    public static IAttackTarget BestShootTargetFromCurrentPosition(IAttackTargetSearcher searcher,
        Predicate<Thing> validator, float maxDistance, float minDistance, TargetScanFlags flags)
    {
        return BestAttackTarget(searcher, flags, validator, minDistance, maxDistance);
    }

    public static bool CanSee(this Thing seer, Thing target, Func<IntVec3, bool> validator = null)
    {
        ShootLeanUtility.CalcShootableCellsOf(tempDestList, target, seer.Position);
        foreach (var intVec3 in tempDestList)
        {
            if (GenSight.LineOfSight(seer.Position, intVec3, seer.Map, true, validator))
            {
                return true;
            }
        }

        ShootLeanUtility.LeanShootingSourcesFromTo(seer.Position, target.Position, seer.Map, tempSourceList);
        foreach (var intVec3 in tempSourceList)
        {
            foreach (var vec3 in tempDestList)
            {
                if (GenSight.LineOfSight(intVec3, vec3, seer.Map, true, validator))
                {
                    return true;
                }
            }
        }

        return false;
    }
}