using Verse;

namespace SydailyFox_Settingpack;

public class Attack_SydailyFox_ON : CompProperties
{
    public readonly int lifespanTicks = 100;

    public Attack_SydailyFox_ON()
    {
        compClass = typeof(Attack_SydailyFox);
    }
}