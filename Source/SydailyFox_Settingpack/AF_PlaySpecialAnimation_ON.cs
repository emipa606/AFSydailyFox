using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SydailyFox_Settingpack;

public class AF_PlaySpecialAnimation_ON : CompProperties
{
    public readonly int QuantitytoSpawn;

    public readonly int TicktoSpawn;
    public Vector3 EndRange;

    public Color FirstColor;
    public ThingDef mote;

    public FloatRange PlaySize;

    public FloatRange RotationRate;

    public Color SecondColor;

    public FloatRange SpeedrangeX;

    public FloatRange SpeedrangeY;

    public Vector3 StartRange;

    public AF_PlaySpecialAnimation_ON()
    {
        QuantitytoSpawn = 1;
        TicktoSpawn = 1;
        FirstColor = Color.white;
        SecondColor = Color.white;
        compClass = typeof(AF_PlaySpecialAnimation);
    }

    public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
    {
        if (mote == null)
        {
            yield return "CompThrownMoteEmitter must have a mote assigned.";
        }
    }
}