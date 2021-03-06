﻿using System;
using System.Collections.Generic;
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
	}




}
