using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BlinkingAnimation;

public class PawnRenderNode_Eyelids : PawnRenderNode
{
	private static readonly Dictionary<string, Graphic_Multi> cachedGraphics = new Dictionary<string, Graphic_Multi>();

	private static readonly HashSet<string> warnedMissing = new HashSet<string>();

	public PawnRenderNode_Eyelids(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
		: base(pawn, props, tree)
	{
	}

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
		if (pawn.Corpse != null && pawn.Dead)
		{
			if (!BlinkingAnimationMod.Settings.eyelidsOnCorpses)
			{
				return null;
			}
			Corpse corpse = pawn.Corpse;
			if (corpse != null && corpse.CurRotDrawMode == RotDrawMode.Rotting)
			{
				return null;
			}
		}
		string defName = pawn.story.headType.defName;
		string text = "Things/Pawn/Humanlike/Eyelids/" + defName + "_Closed";
		if (!ContentFinder<Texture2D>.Get(text + "_south", reportFailure: false))
		{
			if (Prefs.DevMode && warnedMissing.Add(defName))
			{
				Log.Warning("[BlinkingAnimation] Missing eyelid texture for headType '" + defName + "' at " + text + "_south. Using fallback.");
			}
			text = "Things/Pawn/Humanlike/Eyelids/Fallback_Blank";
		}
		Color color = ColorFor(pawn);
		Vector2 one = Vector2.one;
		PawnRenderNodeProperties pawnRenderNodeProperties = props;
		if (pawnRenderNodeProperties != null && pawnRenderNodeProperties.drawData?.scaleOffsetByBodySize == true && pawn.ageTracker?.CurLifeStage != null)
		{
			one *= pawn.ageTracker.CurLifeStage.headSizeFactor ?? 1f;
		}
		string key = $"{text}_{color.r:F3}_{color.g:F3}_{color.b:F3}_{color.a:F3}_{one.x:F2}_{one.y:F2}";
		if (!cachedGraphics.TryGetValue(key, out var value))
		{
			value = (Graphic_Multi)GraphicDatabase.Get<Graphic_Multi>(text, ShaderFor(pawn), one, color);
			cachedGraphics[key] = value;
		}
		return value;
	}
}
