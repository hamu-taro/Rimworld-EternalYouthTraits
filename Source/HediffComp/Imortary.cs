using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace EternalYouthTraits
{
	[StaticConstructorOnStartup]
	class HediffComp_Imortary : HediffComp
	{

		public string labelCap
		{
			get
			{
				return base.Def.LabelCap;
			}
		}

		public string label
		{
			get
			{
				return base.Def.label;
			}
		}

		public void restorePart()
		{
			Pawn pawn = base.Pawn as Pawn;

			using (IEnumerator<BodyPartRecord> enumerator = pawn.health.hediffSet.GetInjuredParts().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BodyPartRecord rec = enumerator.Current;

					IEnumerable<Hediff_Injury> arg_BB_0 = pawn.health.hediffSet.GetHediffs<Hediff_Injury>();
					Func<Hediff_Injury, bool> arg_BB_1;

					arg_BB_1 = ((Hediff_Injury injury) => injury.Part == rec);

					foreach (Hediff_Injury current in arg_BB_0.Where(arg_BB_1))
					{
						if (current.CanHealNaturally() && !current.IsPermanent())
						{
							current.Heal(2.0f);
						}
						else
						{
							current.Heal(1.0f);
						}
					}
				}
			}
		}

		public void restoreBlood()
		{
			Pawn pawn = base.Pawn as Pawn;

			using (IEnumerator<BodyPartRecord> enumerator = pawn.health.hediffSet.GetInjuredParts().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BodyPartRecord rec = enumerator.Current;

					IEnumerable<Hediff_Injury> arg_BB_0 = pawn.health.hediffSet.GetHediffs<Hediff_Injury>();
					Func<Hediff_Injury, bool> arg_BB_1;

					arg_BB_1 = ((Hediff_Injury injury) => injury.Part == rec);

					foreach (Hediff_Injury current in arg_BB_0.Where(arg_BB_1))
					{
						if (current.CanHealNaturally() && !current.IsPermanent())
						{
							current.Heal(2.0f);
						}
						else
						{
							current.Heal(1.0f);
						}
					}
				}
			}
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);

			if (this.Pawn.DestroyedOrNull() || this.Pawn.Dead) return;

			if (Find.TickManager.TicksGame % 600 != 0)
				restorePart();

			return;
		}

		public override void CompExposeData()
		{
			base.CompExposeData();
		}

	}
}
