using RimWorld;
using System;
using Verse;

namespace BlinkingAnimation;

public class CompBlinking : ThingComp
{
	private static readonly Lazy<PawnRenderNodeTagDef> Eyelids = new(delegate
	{
		return DefDatabase<PawnRenderNodeTagDef>.GetNamedSilentFail("Eyelids");
	});

	private Pawn pawn;

	internal bool hasEyeNode;

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
			if (parent.Spawned && pawn.Awake())
			{
				return IsBlinkingNow;
			}
			return true;
		}
	}

	public override void Initialize(CompProperties props)
	{
		pawn = parent as Pawn;
		if (pawn is null || !pawn.RaceProps.Humanlike)
		{
			parent.AllComps.Remove(this);
			return;
		}
		base.Initialize(props);
	}

public override void PostSpawnSetup(bool respawningAfterLoad)
{
base.PostSpawnSetup(respawningAfterLoad);
		UpdateHasEyeNode();
}

	public override void CompTick()
	{
		base.CompTick();
		if (pawn.Spawned)
		{
			int ticksGame = Find.TickManager.TicksGame;
			if (ticksGame >= nextBlinkTick)
			{
				if (!isBlinkingCached)
				{
					isBlinkingCached = true;
					nextBlinkTick = ticksGame + blinkDurationTicks;
				}
				else
				{
					isBlinkingCached = false;
					nextBlinkTick = ticksGame + Rand.RangeInclusive(blinkIntervalMin, blinkIntervalMax);
				}
				InvalidateEyelidNode();
			}
		}
	}

	public void InvalidateEyelidNode()
	{
		var renderTree = pawn.Drawer?.renderer?.renderTree;
		if (renderTree is null || !renderTree.Resolved)
		{
			return;
		}
		if (Eyelids.Value is not PawnRenderNodeTagDef eyelids)
		{
			return;
		}
		if (renderTree.nodesByTag.TryGetValue(eyelids, out var value))
		{
			value.requestRecache = true;
		}
	}

	internal void Notify_TraitsChanged()
	{
		UpdateHasEyeNode();
	}

	internal void Notify_GenesChanged()
	{
		UpdateHasEyeNode();
	}

	private void UpdateHasEyeNode()
	{
		hasEyeNode = false;
		if (pawn.genes != null)
		{
			var genes = pawn.genes.GenesListForReading;
			for (var i = 0; i < genes.Count; i++)
			{
				if (!genes[i].Active) continue;
				var props = genes[i].def.renderNodeProperties;
				if (props is null) continue;
				for (var j = 0; j < props.Count; j++)
				{
					if (props[j] is PawnRenderNodeProperties_Eye)
					{
						hasEyeNode = true;
						return;
					}
				}
			}
		}
		var traitSet = pawn.story?.traits;
		if (traitSet != null)
		{
			var traits = traitSet.allTraits;
			for (var i = 0; i < traits.Count; i++)
			{
				var degreeDatas = traits[i].def.degreeDatas;
				for (var j = 0; j < degreeDatas.Count; j++)
				{
					var props = degreeDatas[j].RenderNodeProperties;
					if (props is null) continue;
					for (var k = 0; k < props.Count; k++)
					{
						if (props[k] is PawnRenderNodeProperties_Eye)
						{
							hasEyeNode = true;
							return;
						}
					}
				}
			}
		}
	}
}
