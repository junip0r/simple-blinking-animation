using System.Collections.Generic;
using Verse;

namespace BlinkingAnimation;

public class PawnRenderNodeProperties_Eyelids : PawnRenderNodeProperties
{
	public PawnRenderNodeProperties_Eyelids()
	{
		visibleFacing = new List<Rot4>
		{
			Rot4.East,
			Rot4.South,
			Rot4.West
		};
	}
}
