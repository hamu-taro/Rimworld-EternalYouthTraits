using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;


namespace EternalYouthTraits
{


	public static partial class core
	{
		public static readonly TraitDef TraifDefEternalYouth = TraitDef.Named("EternalYouth");
		public static readonly TraitDef TraifDefEternalImmortal = TraitDef.Named("EternalImmortal");
		public static readonly TraitDef TraifDefEternalImmortary = TraitDef.Named("EternalImmortary");

		//		public static HediffDef hediffDefEternalYouth = HediffDef.Named("hediffOfEternalYouth");
		//		public static HediffDef hediffDefEternalImortary = HediffDef.Named("hediffOfEternalImortary");

		public static bool has_traits(Pawn pawn)
		{
			return pawn?.story?.traits != null;
		}

		public static bool is_human(Pawn pawn)
		{
			if (pawn == null) return false;

			return pawn.RaceProps.Humanlike;
		}

		public static bool has_eternalYouth(Pawn pawn)
		{
			if (!has_traits(pawn)) { return false; }
			return is_human(pawn) && pawn.story.traits.HasTrait(TraifDefEternalYouth);
		}

		public static bool has_eternalImmortal(Pawn pawn)
		{
			if (!has_traits(pawn)) { return false; }
			return is_human(pawn) && pawn.story.traits.HasTrait(TraifDefEternalImmortal);
		}

		public static bool has_eternalImmortary(Pawn pawn)
		{
			if (!has_traits(pawn)) { return false; }
			return is_human(pawn) && pawn.story.traits.HasTrait(TraifDefEternalImmortary);
		}
		

	}
}