using System;
using System.Collections.Generic;
using System.Linq;
using Verse;


namespace EternalYouthTraits
{
	[StaticConstructorOnStartup]
	class HediffComp_ImmortalRegeneration : HediffComp
	{
		// �Ȃ��Ă������Ɏ��郊�X�g
		// Cataract : ������
		// HearingLoss : �
		// Cirrhosis : �̍d��
		// HeartArteryBlockage : �S�؍[��
		// Dementia : �F�m��
		// Asthma : �b��
		// Frail : ����
		// BadBack : ����
		// Carcinoma : ��
		// ChemicalDamageModerate : ���w�I�������x
		// ChemicalDamageSevere : ���w�I�����d�x
		// Alzheimers : �A���c�n�C�}�[
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
