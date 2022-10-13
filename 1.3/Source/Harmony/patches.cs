using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;


namespace EternalYouthTraits
{
	[StaticConstructorOnStartup]
	static partial class HarmonyPatches
	{
		private static readonly Type patchType = typeof(HarmonyPatches);

		static HarmonyPatches()
		{
			Harmony harmonyInstance = new Harmony( "rimworld.hamutaro.EternalYouthTraits");

			harmonyInstance.Patch(original: AccessTools.Method(type: typeof(Pawn_AgeTracker), name: "AgeTick"),
					prefix: new HarmonyMethod(patchType, nameof(prefix_AgeTick_EternalYouthTraits)));

			harmonyInstance.Patch(original: AccessTools.Method(type: typeof(Pawn_HealthTracker), name: "ShouldBeDead"),
					prefix: new HarmonyMethod(patchType, nameof(prefix_ShouldBeDead_EternalImmortalityTraits)));

			harmonyInstance.Patch(AccessTools.Method(typeof(IncidentWorker_DiseaseHuman), "PotentialVictimCandidates"),
					prefix: new HarmonyMethod(patchType, nameof(prefix_PotentialVictimCandidates_EternalImmortalTraits)));

//			harmonyInstance.Patch(AccessTools.Method(typeof(GenSpawn), "Spawn"), 
//					postfix: new HarmonyMethod(patchType, nameof(postfix_Spawn_EternalImmortalTraits)));
		}

		// -------------------------------------------------------------------------
		// 不老制御
		static bool prefix_AgeTick_EternalYouthTraits(Pawn_AgeTracker __instance)
		{
			Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
			if (core.has_eternalYouth(pawn) || core.has_eternalImmortary(pawn))
			{
				return false; //return false to skip execution of the original.
			}
			return true;
		}

		// -------------------------------------------------------------------------
		// 不死制御
		// 死ぬ判定のHOOK
		static bool prefix_ShouldBeDead_EternalImmortalityTraits(Pawn_HealthTracker __instance, ref bool __result)
		{
			Pawn pawn = Traverse.Create(root: __instance).Field(name: "pawn").GetValue<Pawn>();

			// -------------------------------------------------------------------------
			// 処理除外判定群
			// プレイヤーファクション以外は対象外
			if (!pawn?.Faction?.IsPlayer ?? true) return true;
			if (!core.has_eternalImmortal(pawn) && !core.has_eternalImmortary(pawn)) return true;

			__result = false;
			return false; //return false to skip execution of the original.
		}

		// 不死者は病気にならない
		static void prefix_PotentialVictimCandidates_EternalImmortalTraits(ref IEnumerable<Pawn> __result)
		{
			List<Pawn> tempList = __result.ToList();
			List<Pawn> removalList = new List<Pawn>();
			removalList.Clear();

			foreach(Pawn pawn in tempList)
			{
				if (core.has_eternalImmortal(pawn) || core.has_eternalImmortary(pawn))
				{
					removalList.Add(pawn);
				}
			}
			__result = tempList.Except(removalList);
		}


		[HarmonyPatch(typeof(GenSpawn), "Spawn", new Type[] { typeof(Thing), typeof(IntVec3), typeof(Map), typeof(Rot4), typeof(WipeMode), typeof(bool) })]
		static class Patch_GenSpawn_Spawn
		{
			[HarmonyPostfix]
			static void postfix_Spawn_EternalImmortalTraits(ref Thing __result)
			{
				if (__result != null && __result is Pawn)
				{
					Pawn pawn = __result as Pawn;

					if (core.has_eternalImmortal(pawn) || core.has_eternalImmortary(pawn))
					{
						HealthUtility.AdjustSeverity(pawn, EYTDefOf.EYT_ImmortalRegeneration, 1.0f);
					}
				}
			}
		}
	}

}
