using Verse;

namespace BlinkingAnimation;

public class PawnRenderNodeProperties_EyeOverlay : PawnRenderNodeProperties_Eye
{
	public PawnRenderNodeProperties_EyeOverlay()
	{
		workerClass = typeof(PawnRenderNodeWorker_EyeOverlay);
		debugLabel = "EyeOverlay";
	}
}
