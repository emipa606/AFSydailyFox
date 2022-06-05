using RimWorld;
using Verse;

namespace SydailyFox_Settingpack;

[DefOf]
public static class DFFerian_Damage
{
    public static DamageDef AF_DA_SydailyCrystalExplosion_Purple;

    public static DamageDef SydailyCrystalAttack_Purple;

    static DFFerian_Damage()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(DamageDefOf));
    }
}