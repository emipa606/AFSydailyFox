using Verse;

namespace SydailyFox_Settingpack;

public class PlaceWorker_MegaStatue : PlaceWorker
{
    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map,
        Thing thingToIgnore = null, Thing thing = null)
    {
        var thing2 = map.thingGrid.ThingAt(loc, DFFerian_Thing.FAN_Statue);
        if (thing2 == null || thing2.Position != loc)
        {
            return "OwO".Translate();
        }

        return true;
    }

    public override bool ForceAllowPlaceOver(BuildableDef otherDef)
    {
        return otherDef == DFFerian_Thing.FAN_Statue;
    }
}