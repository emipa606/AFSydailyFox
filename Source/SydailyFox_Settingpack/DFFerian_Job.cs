using RimWorld;
using Verse;

namespace SydailyFox_Settingpack;

[DefOf]
public static class DFFerian_Job
{
    public static JobDef SydailyFoxNuzzleJob;

    static DFFerian_Job()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(JobDefOf));
    }
}