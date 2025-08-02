using RimWorld;
using UnityEngine;
using Verse;

namespace SydailyFox_Settingpack;

public class AF_PlaySpecialAnimation : ThingComp
{
    private bool emittedBefore;

    private int ticksSinceLastEmitted;

    private Vector3 EmissionOffset => new Vector3(Rand.Range(Props.StartRange.x, Props.EndRange.x),
        Rand.Range(Props.StartRange.y, Props.EndRange.y), Rand.Range(Props.StartRange.z, Props.EndRange.z));

    private Color EmissionColor => Color.Lerp(Props.FirstColor, Props.SecondColor, Rand.Value);

    private bool IsOn
    {
        get
        {
            if (!parent.Spawned)
            {
                return false;
            }

            var comp = parent.GetComp<CompPowerTrader>();
            if (comp is { PowerOn: false })
            {
                return false;
            }

            var comp2 = parent.GetComp<CompSendSignalOnCountdown>();
            if (comp2 is { ticksLeft: <= 0 })
            {
                return false;
            }

            if (parent is Building_MusicalInstrument { IsBeingPlayed: false })
            {
                return false;
            }

            return parent.GetComp<CompInitiatable>()?.Initiated ?? true;
        }
    }

    private AF_PlaySpecialAnimation_ON Props => (AF_PlaySpecialAnimation_ON)props;

    public override void CompTick()
    {
        if (!IsOn)
        {
            return;
        }

        if (Props.TicktoSpawn == -1)
        {
            if (emittedBefore)
            {
                return;
            }

            emit();
            emittedBefore = true;
        }
        else if (ticksSinceLastEmitted >= Props.TicktoSpawn)
        {
            emit();
            ticksSinceLastEmitted = 0;
        }
        else
        {
            ticksSinceLastEmitted++;
        }
    }

    private void emit()
    {
        for (var i = 0; i < Props.QuantitytoSpawn; i++)
        {
            var moteThrown = (MoteThrown)ThingMaker.MakeThing(Props.mote);
            moteThrown.Scale = Props.PlaySize.RandomInRange;
            moteThrown.rotationRate = Props.RotationRate.RandomInRange;
            moteThrown.exactPosition = parent.DrawPos + EmissionOffset;
            moteThrown.instanceColor = EmissionColor;
            moteThrown.SetVelocity(Props.SpeedrangeX.RandomInRange, Props.SpeedrangeX.RandomInRange);
            GenSpawn.Spawn(moteThrown, moteThrown.exactPosition.ToIntVec3(), parent.Map);
        }
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref ticksSinceLastEmitted, "ticksSinceLastEmitted");
        Scribe_Values.Look(ref emittedBefore, "emittedBefore");
    }
}