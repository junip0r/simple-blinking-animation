using Verse;

namespace BlinkingAnimation;

public class BlinkingAnimationSettings : ModSettings
{
	public bool eyelidsOnCorpses = true;

	public override void ExposeData()
	{
		Scribe_Values.Look(ref eyelidsOnCorpses, "eyelidsOnCorpses", defaultValue: true);
	}
}
