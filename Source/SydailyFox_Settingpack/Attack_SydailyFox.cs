using RimWorld;
using Verse;

namespace SydailyFox_Settingpack;

public class Attack_SydailyFox : ThingComp
{
    private int age = -1;

    private Attack_SydailyFox_ON Props => (Attack_SydailyFox_ON)props;

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref age, "age");
    }

    public override void CompTick()
    {
        age++;
        if (age >= Props.lifespanTicks)
        {
            parent.TakeDamage(new DamageInfo(DamageDefOf.Crush, 5000f));
        }
    }

    public override void CompTickRare()
    {
        age += 250;
        if (age >= Props.lifespanTicks)
        {
            parent.TakeDamage(new DamageInfo(DamageDefOf.Crush, 5000f));
        }
    }
}