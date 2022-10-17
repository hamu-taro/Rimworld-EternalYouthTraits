using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;


namespace EternalYouthTraits
{
	public static class EYTUtility
	{


		/// <summary>
		/// 傷の再生
		/// </summary>
		/// <param name="target"></param>
		/// <param name="maxInjuries"></param>
		/// <param name="maxInjuriesPerBodyPartInit"></param>
		public static void RegenerateInjuryPart(Pawn pawn)
		{
			foreach (var rec in pawn.health.hediffSet.GetInjuredParts())
			{
				foreach (var current in from injury in pawn.health.hediffSet.GetHediffs<Hediff_Injury>() where injury.Part == rec select injury)
				{
					if (current.CanHealNaturally() && !current.IsPermanent())
					{
						current.Heal(0.5f);
					}
					else
					{
						current.Heal(0.1f);
					}
				}
			}
		}

		/// <summary>
		/// 欠損部位の再生
		/// </summary>
		/// <param name="pawn"></param>
		/// <param name="count"></param>
		public static void RegenerateMissingPartRandom(Pawn pawn, int count = 1)
		{
			List<Hediff_MissingPart> missingParts = new List<Hediff_MissingPart>()
				.Concat(pawn?.health?.hediffSet?.GetMissingPartsCommonAncestors()).ToList();
			if (missingParts.NullOrEmpty()) return;

			for (int i = 0; i < count; i++)
			{
				if (missingParts.Count() <= 0)
				{
					break;
				}
				
				Hediff_MissingPart RestorePart = missingParts.RandomElement();
				BodyPartRecord part = RestorePart.Part;
				if (pawn.health != null)
				{
					pawn.health.RestorePart(part);

					// 再生後、欠損寸前までダメージを与える
					DamageInfo dinfo = new DamageInfo(DamageDefOf.Crush, (float)part.def.hitPoints - 0.1f, 9999f, -1f, hitPart:part, instigatorGuilty: false, spawnFilth:false);
					dinfo.SetAllowDamagePropagation(false);
					pawn.TakeDamage(dinfo);
				}
				missingParts.Remove(RestorePart);
			}
		}
	}
}