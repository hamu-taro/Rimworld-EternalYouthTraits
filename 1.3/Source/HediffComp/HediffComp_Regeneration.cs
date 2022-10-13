using System;
using System.Collections.Generic;
using System.Linq;
using Verse;


namespace EternalYouthTraits
{
	[StaticConstructorOnStartup]
	class HediffComp_ImmortalRegeneration : HediffComp
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
			if (this.Pawn == null) return;
			if (this.Pawn.Map == null) return;
			if (this.Pawn.Dead) return;
			if (this.Pawn.DestroyedOrNull()) return;
			if (!core.has_eternalImmortal(this.Pawn) && !core.has_eternalImmortary(this.Pawn)) return;

			if (!this.initialized)
			{
				Log.Message("rimworld.hamutaro.EternalYouthTraits HediffComp_ImmortalRegeneration:Initialize.");
				Initialize();
			}

			if (Find.TickManager.TicksGame % intervalTick == 0)
			{
				using (IEnumerator<BodyPartRecord> enumerator = this.Pawn.health.hediffSet.GetInjuredParts().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BodyPartRecord rec = enumerator.Current;
						IEnumerable<Hediff_Injury> arg_BB_0 = this.Pawn.health.hediffSet.GetHediffs<Hediff_Injury>();
						Func<Hediff_Injury, bool> arg_BB_1;

						arg_BB_1 = ((Hediff_Injury injury) => injury.Part == rec);

						foreach (Hediff_Injury current in arg_BB_0.Where(arg_BB_1))
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
#if false
				using (IEnumerator<Hediff_MissingPart> enumerator = this.Pawn.health.hediffSet.GetMissingPartsCommonAncestors().GetEnumerator())
				{
					bool isRegened = false;

					while (enumerator.MoveNext() && !isRegened)
					{
						Hediff_MissingPart rec = enumerator.Current;

						IEnumerable<Hediff_MissingPart> arg_BB_0 = this.Pawn.health.hediffSet.GetHediffs<Hediff_MissingPart>();
						Func<Hediff_MissingPart, bool> arg_BB_1;

						arg_BB_1 = ((Hediff_MissingPart missing) => missing.Part == rec);

						foreach (Hediff_MissingPart current in arg_BB_0.Where(arg_BB_1))
						{
							current.Heal(0.1f);
							isRegened = true;
							break;
						}
					}
				}
#endif
				using (IEnumerator<Hediff> enumerator = this.Pawn.health.hediffSet.GetHediffs<Hediff>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Hediff rec = enumerator.Current;
						if (!rec.IsPermanent())
						{
							if(Array.IndexOf(restoreHediffArray, rec.def.defName) >= 0)
							{
								this.Pawn.health.RemoveHediff(rec);
							}
						}
					}
				}
			}
		}

		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Values.Look<bool>(ref this.initialized, "initialized", false, false);
			Scribe_Values.Look<int>(ref this.intervalTick, "intervalTick", 600, false);
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
