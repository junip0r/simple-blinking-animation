using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using Verse;

namespace BlinkingAnimation;

public class CompBlinking : ThingComp
{
	private int nextBlinkTick;

	private int blinkDurationTicks = 12;

	private int blinkIntervalMin = 200;

	private int blinkIntervalMax = 400;

	private bool isBlinkingCached;

	private bool IsBlinkingNow
	{
		get
		{
			if (Find.TickManager.TicksGame < nextBlinkTick)
			{
				return isBlinkingCached;
			}
			return false;
		}
	}

	public bool EyelidsClosed
	{
		get
		{
			if (parent.Spawned && Pawn.Awake())
			{
				return IsBlinkingNow;
			}
			return true;
		}
	}

	public Pawn Pawn => parent as Pawn;

	public override void CompTick()
	{
		base.CompTick();
		if (Pawn.Spawned && Pawn.RaceProps.Humanlike)
		{
			int ticksGame = Find.TickManager.TicksGame;
			if (!isBlinkingCached && ticksGame >= nextBlinkTick)
			{
				isBlinkingCached = true;
				nextBlinkTick = ticksGame + blinkDurationTicks;
				InvalidateEyelidNode();
			}
			else if (isBlinkingCached && ticksGame >= nextBlinkTick)
			{
				isBlinkingCached = false;
				nextBlinkTick = ticksGame + Rand.RangeInclusive(blinkIntervalMin, blinkIntervalMax);
				InvalidateEyelidNode();
			}
		}
	}

	public void InvalidateEyelidNode()
	{
		if (Pawn.Drawer == null || Pawn.Drawer.renderer == null)
		{
			return;
		}
		PawnRenderTree renderTree = Pawn.Drawer.renderer.renderTree;
		if (renderTree == null || !renderTree.Resolved)
		{
			return;
		}
		PawnRenderNodeTagDef namedSilentFail = DefDatabase<PawnRenderNodeTagDef>.GetNamedSilentFail("Eyelids");
		if (namedSilentFail != null)
		{
			FieldInfo field = typeof(PawnRenderTree).GetField("nodesByTag", BindingFlags.Instance | BindingFlags.NonPublic);
			Dictionary<PawnRenderNodeTagDef, PawnRenderNode> dictionary = ((field != null) ? (field.GetValue(renderTree) as Dictionary<PawnRenderNodeTagDef, PawnRenderNode>) : null);
			if (dictionary != null && dictionary.TryGetValue(namedSilentFail, out var value))
			{
				value.requestRecache = true;
			}
		}
	}
}
