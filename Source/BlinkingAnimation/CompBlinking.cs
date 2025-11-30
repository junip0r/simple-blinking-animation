using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BlinkingAnimation;

public class CompBlinking : ThingComp
{
	const int blinkDurationTicks = 12;

	const int blinkIntervalMin = 200;

	const int blinkIntervalMax = 400;

	private static readonly HashSet<string> warnedMissing = [];

	private static readonly Lazy<PawnRenderNodeTagDef> Eyelids = new(delegate
	{
		return DefDatabase<PawnRenderNodeTagDef>.GetNamedSilentFail("Eyelids");
	});

	private Pawn pawn;
	private HeadTypeDef prevHeadType;
	private string eyelidTexturePathBase;

	private bool spawned;

	private int nextBlinkTick;

	private bool isBlinkingNow;

	internal bool hasEyeNode;

	internal bool EyelidsClosed
	{
		get
		{
			if (pawn.Spawned && pawn.Awake())
			{
				return Find.TickManager.TicksGame < nextBlinkTick && isBlinkingNow;
			}
			return true;
		}
	}

	internal string EyelidTexturePathBase
	{
		get
		{
			if (pawn.story is null) return null;
			var headType = pawn.story.headType;
			if (prevHeadType != headType)
			{
				prevHeadType = headType;
				var defName = headType.defName;
				var pathBase = $"Things/Pawn/Humanlike/Eyelids/{defName}_Closed";
				var pathSouth = $"{pathBase}_south";
				if (!ContentFinder<Texture2D>.Get(pathSouth, reportFailure: false))
				{
					if (Prefs.DevMode && warnedMissing.Add(defName))
					{
						Log.Warning($"[BlinkingAnimation] Missing eyelid texture for headType '{defName}' at {pathSouth}. Using fallback.");
					}
					pathBase = "Things/Pawn/Humanlike/Eyelids/Fallback_Blank";
				}
				eyelidTexturePathBase = pathBase;
			}
			return eyelidTexturePathBase;
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
		spawned = true;
		UpdateHasEyeNode();
	}

	public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
	{
		spawned = false;
	}

	public override void CompTick()
	{
		if (spawned)
		{
			var ticksGame = Find.TickManager.TicksGame;
			if (ticksGame >= nextBlinkTick)
			{
				if (!isBlinkingNow)
				{
					isBlinkingNow = true;
					nextBlinkTick = ticksGame + blinkDurationTicks;
				}
				else
				{
					isBlinkingNow = false;
					nextBlinkTick = ticksGame + Rand.RangeInclusive(blinkIntervalMin, blinkIntervalMax);
				}
				InvalidateEyelidNode();
			}
		}
	}

	internal void InvalidateEyelidNode()
	{
		if (pawn.Drawer?.renderer?.renderTree is not PawnRenderTree renderTree)
		{
			return;
		}
		if (!renderTree.Resolved)
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
