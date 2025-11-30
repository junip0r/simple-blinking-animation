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
		if (parms.pawn.GetComp<CompBlinking>() is not CompBlinking comp)
		{
			return false;
		}
		if (!comp.hasEyeNode)
		{
			return false;
		}
		if (!parms.pawn.Awake())
		{
			return true;
		}
		if (!parms.Portrait)
		{
			return comp.EyelidsClosed;
		}
		return false;
	}
}
