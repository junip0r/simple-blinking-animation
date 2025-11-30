using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BlinkingAnimation;

using Key = (string, float, float, float, float, float, float);

public class PawnRenderNode_Eyelids(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : PawnRenderNode(pawn, props, tree)
{
	private static readonly Dictionary<Key, Graphic_Multi> cachedGraphics = [];

    public override GraphicMeshSet MeshSetFor(Pawn pawn)
	{
		return HumanlikeMeshPoolUtility.GetHumanlikeHeadSetForPawn(pawn);
	}

	public override Graphic GraphicFor(Pawn pawn)
	{
		if (!pawn.RaceProps.Humanlike || pawn.story?.headType == null)
		{
			return null;
		}
		if (pawn.Dead && pawn.Corpse is Corpse corpse)
		{
			if (!BlinkingAnimationMod.Settings.eyelidsOnCorpses || corpse.CurRotDrawMode == RotDrawMode.Rotting)
			{
				return null;
			}
		}
		if (pawn.GetComp<CompBlinking>() is not CompBlinking comp)
		{
			return null;
		}
		string pathBase = comp.EyelidTexturePathBase;
		Color color = ColorFor(pawn);
		Vector2 drawSize = Vector2.one;
		if (props != null && props.drawData?.scaleOffsetByBodySize == true && pawn.ageTracker?.CurLifeStage != null)
		{
			drawSize *= pawn.ageTracker.CurLifeStage.headSizeFactor ?? 1f;
		}
		var key = (pathBase, color.r, color.g, color.b, color.a, drawSize.x, drawSize.y);
		if (!cachedGraphics.TryGetValue(key, out var value))
		{
			value = (Graphic_Multi)GraphicDatabase.Get<Graphic_Multi>(pathBase, ShaderFor(pawn), drawSize, color);
			cachedGraphics[key] = value;
		}
		return value;
	}
}
