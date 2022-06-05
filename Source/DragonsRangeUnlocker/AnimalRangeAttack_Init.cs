using System.Reflection;
using HarmonyLib;
using Verse;

namespace DragonsRangedAttack;

[StaticConstructorOnStartup]
internal static class AnimalRangeAttack_Init
{
    static AnimalRangeAttack_Init()
    {
        var harmony = new Harmony("com.github.rimworld.mod.DragonAnimalRangeAttack");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}