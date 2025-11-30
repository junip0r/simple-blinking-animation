using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BlinkingAnimation;

public class PawnRenderNodeWorker_EyeOverlay : PawnRenderNodeWorker_Eye
{
	public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
	{
		if (!base.CanDrawNow(node, parms))
		{
			return false;
		}
		if (parms.skipFlags.HasFlag(RenderSkipFlagDefOf.Eyes))
		{
			return false;
		}
		CompBlinking compBlinking = parms.pawn.TryGetComp<CompBlinking>();
		if (compBlinking == null)
		{
			return false;
		}
		if (!HasAnyEyeNode(parms.pawn))
		{
			return false;
		}
		if (!parms.pawn.Awake())
		{
			return true;
		}
		if (!parms.Portrait)
		{
			return compBlinking.EyelidsClosed;
		}
		return false;
	}

	private bool HasAnyEyeNode(Pawn pawn)
	{
		if (pawn.genes != null)
		{
			foreach (Gene item in pawn.genes.GenesListForReading)
			{
				if (!item.Active)
				{
					continue;
				}
				List<PawnRenderNodeProperties> renderNodeProperties = item.def.renderNodeProperties;
				if (renderNodeProperties == null)
				{
					continue;
				}
				foreach (PawnRenderNodeProperties item2 in renderNodeProperties)
				{
					if (item2 is PawnRenderNodeProperties_Eye)
					{
						return true;
					}
				}
			}
		}
		if (pawn.story?.traits != null)
		{
			foreach (Trait allTrait in pawn.story.traits.allTraits)
			{
				foreach (TraitDegreeData degreeData in allTrait.def.degreeDatas)
				{
					List<PawnRenderNodeProperties> renderNodeProperties2 = degreeData.RenderNodeProperties;
					if (renderNodeProperties2 == null)
					{
						continue;
					}
					foreach (PawnRenderNodeProperties item3 in renderNodeProperties2)
					{
						if (item3 is PawnRenderNodeProperties_Eye)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}
}
