using System;
using System.Collections.Generic;
using System.Linq;
using Verse;


namespace EternalYouthTraits
{
	[StaticConstructorOnStartup]
	public class HediffComp_ImmortalRegeneration : HediffComp
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
