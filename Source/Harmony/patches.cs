using System;
using System.Collections.Generic;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI.Group;
using System.Reflection;

namespace EternalYouthTraits
{
	[HarmonyPatch(typeof(Pawn_AgeTracker))]
	[HarmonyPatch(nameof(Pawn_AgeTracker.AgeTick))]
	static class Patches_AgeTracker_AgeTick_aliceDoll
	{
		[HarmonyPrefix]
		static bool prefix_AgeTick(Pawn_AgeTracker __instance)
		{
			Pawn p = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
			if(core.has_eternalYouth(p)) {
				return false; //return false to skip execution of the original.
			}
			return true;
		}
	}

	[StaticConstructorOnStartup]
	internal static class First
	{
		static First()
		{
			var har = HarmonyInstance.Create("EternalYouthTraits");
			har.PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}