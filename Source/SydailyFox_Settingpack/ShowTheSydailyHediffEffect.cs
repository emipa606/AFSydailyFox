using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SydailyFox_Settingpack;

public class ShowTheSydailyHediffEffect : HediffCompProperties
{
    public readonly int filthCount = 4;

    public readonly int moteCount = 3;
    public ThingDef filth;

    public ThingDef mote;

    public FloatRange moteOffsetRange = new FloatRange(0.2f, 0.4f);

    public SoundDef sound;

    public ShowTheSydailyHediffEffect()
    {
        compClass = typeof(PlaySydailyHediffPost);
    }

    public class PlaySydailyHediffPost : HediffComp
    {
        public ShowTheSydailyHediffEffect Props => (ShowTheSydailyHediffEffect)props;

        public override void Notify_PawnKilled()
        {
            if (!Pawn.Spawned)
            {
                return;
            }

            if (Props.mote != null)
            {
                var drawPos = Pawn.DrawPos;
                for (var i = 0; i < Props.moteCount; i++)
                {
                    var vector = Rand.InsideUnitCircle * Props.moteOffsetRange.RandomInRange * Rand.Sign;
                    MoteMaker.MakeStaticMote(new Vector3(drawPos.x + vector.x, drawPos.y, drawPos.z + vector.y),
                        Pawn.Map, Props.mote);
                }
            }

            if (Props.filth != null)
            {
                FilthMaker.TryMakeFilth(Pawn.Position, Pawn.Map, Props.filth, Props.filthCount);
            }

            Props.sound?.PlayOneShot(SoundInfo.InMap(Pawn));
        }
    }
}