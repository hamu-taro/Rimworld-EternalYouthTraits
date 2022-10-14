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
		private int MissingPartsRestoreIntervalTick = 600;
		private int InjuryPartsRestoreIntervalTick = 60;
		private int DiseaseRestoreIntervalTick = 60;

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
				Initialize();
			}

			if (Find.TickManager.TicksGame % InjuryPartsRestoreIntervalTick == 0)
			{
				EYTUtility.RegenerateInjuryPart(this.Pawn);
			}

			if (Find.TickManager.TicksGame % MissingPartsRestoreIntervalTick == 0)
			{
				EYTUtility.RegenerateMissingPartRandom(this.Pawn, 1);
			}

			if (Find.TickManager.TicksGame % DiseaseRestoreIntervalTick == 0)
			{
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
			Scribe_Values.Look<int>(ref this.MissingPartsRestoreIntervalTick, "intervalTick", 600, false);
			Scribe_Values.Look<int>(ref this.InjuryPartsRestoreIntervalTick, "intervalTick", 60, false);
			Scribe_Values.Look<int>(ref this.DiseaseRestoreIntervalTick, "intervalTick", 60, false);
		}
	}


}
