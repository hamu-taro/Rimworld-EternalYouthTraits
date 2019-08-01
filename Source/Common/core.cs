using System;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace EternalYouthTraits
{

	public static partial class core
	{
		public static readonly TraitDef EternalYouth = TraitDef.Named("EternalYouth");
		public static readonly TraitDef EternalImmortal = TraitDef.Named("EternalImmortal");

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
			return is_human(pawn) && pawn.story.traits.HasTrait(EternalYouth);
		}

		public static bool has_eternalImmortal(Pawn pawn)
		{
			if (!has_traits(pawn)) { return false; }
			return is_human(pawn) && pawn.story.traits.HasTrait(EternalImmortal);
		}
	}
}