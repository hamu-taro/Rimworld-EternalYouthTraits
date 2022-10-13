using System;
using System.Collections.Generic;
using System.Linq;
using Verse;


namespace EternalYouthTraits
{
	[StaticConstructorOnStartup]
	public class HediffComp_ImmortalRegeneration : HediffComp
	{
		// なってもすぐに治るリスト
		// Cataract : 白内障
		// HearingLoss : 難聴
		// Cirrhosis : 肝硬変
		// HeartArteryBlockage : 心筋梗塞
		// Dementia : 認知症
		// Asthma : 喘息
		// Frail : 衰弱
		// BadBack : 腰痛
		// Carcinoma : 癌
		// ChemicalDamageModerate : 化学的損傷中度
		// ChemicalDamageSevere : 化学的損傷重度
		// Alzheimers : アルツハイマー
		static string[] restoreHediffArray = {
			"Cataract", "HearingLoss", "Cirrhosis", "HeartArteryBlockage",
			"Dementia", "Asthma", "ChemicalDamageModerate", "Scaria", "Frail",
			"BadBack", "Carcinoma", "ChemicalDamageModerate", "ChemicalDamageSevere", "Alzheimers",
		};

		private bool initialized = false;
		private int intervalTick = 600;

		private void Initialize()
		{
			this.initialized = true;
		}


		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);

			// Check to 
			if (base.Pawn != null) return;
			if (base.Pawn.Map != null) return;
			if (this.Pawn.Dead) return;
			if (this.Pawn.DestroyedOrNull()) return;
			if (!core.has_eternalImmortal(base.Pawn) && !core.has_eternalImmortary(base.Pawn)) return;

			if (!this.initialized)
			{
				Initialize();
			}

			if (Find.TickManager.TicksGame % intervalTick == 0)
			{
				Pawn pawn = base.Pawn;

				using (IEnumerator<BodyPartRecord> enumerator = pawn.health.hediffSet.GetInjuredParts().GetEnumerator())
				{
					bool isRegened = false;

					while (enumerator.MoveNext() && !isRegened)
					{
						BodyPartRecord rec = enumerator.Current;
						IEnumerable<Hediff_Injury> arg_BB_0 = pawn.health.hediffSet.GetHediffs<Hediff_Injury>();
						Func<Hediff_Injury, bool> arg_BB_1;

						arg_BB_1 = ((Hediff_Injury injury) => injury.Part == rec);

						foreach (Hediff_Injury current in arg_BB_0.Where(arg_BB_1))
						{
							if (current.CanHealNaturally() && !current.IsPermanent())
							{
								current.Heal(0.2f);
							}
							else
							{
								current.Heal(0.1f);
							}
							isRegened = true;
							break;
						}
					}
				}

				using (IEnumerator<Hediff> enumerator = pawn.health.hediffSet.GetHediffs<Hediff>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Hediff rec = enumerator.Current;
						if (!rec.IsPermanent())
						{
							if(Array.IndexOf(restoreHediffArray, rec.def.defName) >= 0)
							{
								pawn.health.RemoveHediff(rec);
							}
						}
					}
				}
			}
		}


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

	}

}
