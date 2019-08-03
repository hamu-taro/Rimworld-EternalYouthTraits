using System;
using System.Collections.Generic;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI.Group;
using System.Reflection;

namespace EternalYouthTraits
{
	[StaticConstructorOnStartup]
	static partial class HarmonyPatches
	{
		private static readonly Type patchType = typeof(HarmonyPatches);

		static HarmonyPatches()
		{
			HarmonyInstance harmonyInstance = HarmonyInstance.Create(id: "EternalYouthTraits");

			#region EternalYouthTraits            
//			Log.Message("EternalYouthTraits");

			harmonyInstance.Patch(original: AccessTools.Method(type: typeof(Pawn_AgeTracker), name: "AgeTick"),
					prefix: new HarmonyMethod(type: patchType, name: nameof(prefix_AgeTick_EternalYouthTraits)));

			#endregion

			#region EternalImortaly
//			Log.Message("EternalImortaly");

#if __EY_UNUSE__
			harmonyInstance.Patch(original: AccessTools.Method(type: typeof(Pawn), name: "Kill"),
					prefix: new HarmonyMethod(typeof(HarmonyPatches), name: nameof(prefix_Kill_EternalImmortalityTraits)));
#endif

			harmonyInstance.Patch(original: AccessTools.Method(type: typeof(Pawn_HealthTracker), name: "ShouldBeDead"),
					prefix: new HarmonyMethod(type: patchType, name: nameof(prefix_ShouldBeDead_EternalImmortalityTraits)));
			#endregion
		}

		// -------------------------------------------------------------------------
		// 不老制御
		public static bool prefix_AgeTick_EternalYouthTraits(Pawn_AgeTracker __instance)
		{
			Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();

			if (core.has_eternalYouth(pawn) || core.has_eternalImmortary(pawn)) {
				return false; //return false to skip execution of the original.
			}
			return true;
		}

#if __EY_UNUSE__
		// Kill 関数ではなく、ShouldBeDead()でやるほうが合理的か？
		// -------------------------------------------------------------------------
		// 不死制御
		// 判定優先度は死亡判定、ダウン判定なので
		// ダウンを通り越して即死判定がでた場合、ダウンしなくなる
		static bool prefix_Kill_EternalImmortalityTraits(Pawn __instance, ref bool __result, DamageInfo? dinfo, Hediff hediff)
		{
			// -------------------------------------------------------------------------
			// 処理除外判定群
			// プレイヤーファクション以外は対象外
			if (__instance == null) return true;
			if (!__instance.Faction.IsPlayer) return true;
			if (!core.has_eternalImmortal(__instance)) return true;


			if (!__instance.Downed)
			{
				Traverse traverse = Traverse.Create(root: __instance);

				if (traverse.Method("ShouldBeDowned", new object[0]).GetValue<bool>())
				{
					__instance.health.forceIncap = false;
					traverse.Method("MakeDowned", new object[0]);
				}

			}
			return false; //return false to skip execution of the original.
		}
#endif
		// -------------------------------------------------------------------------
		// 不死制御
		// 死ぬ判定のHOOK
		public static bool prefix_ShouldBeDead_EternalImmortalityTraits(Pawn_HealthTracker __instance, ref bool __result)
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
