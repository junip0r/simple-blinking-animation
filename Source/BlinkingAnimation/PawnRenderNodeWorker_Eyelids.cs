using RimWorld;
using Verse;

namespace BlinkingAnimation;

public class PawnRenderNodeWorker_Eyelids : PawnRenderNodeWorker
{
	public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
	{
		if (!base.CanDrawNow(node, parms))
		{
			return false;
		}
		CompBlinking compBlinking = parms.pawn.TryGetComp<CompBlinking>();
		if (compBlinking == null)
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
}
