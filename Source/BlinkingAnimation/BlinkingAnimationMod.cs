using UnityEngine;
using Verse;

namespace BlinkingAnimation;

public class BlinkingAnimationMod : Mod
{
	public static BlinkingAnimationSettings Settings;

	public BlinkingAnimationMod(ModContentPack content)
		: base(content)
	{
		Settings = GetSettings<BlinkingAnimationSettings>();
	}

	public override void DoSettingsWindowContents(Rect inRect)
	{
		Listing_Standard listing_Standard = new Listing_Standard();
		listing_Standard.Begin(inRect);
		bool eyelidsOnCorpses = Settings.eyelidsOnCorpses;
		listing_Standard.CheckboxLabeled("Corpses have closed eyes", ref Settings.eyelidsOnCorpses, "If disabled, eyelids will not draw on corpses.");
		if (Settings.eyelidsOnCorpses != eyelidsOnCorpses)
		{
			RefreshAllCorpses();
		}
		listing_Standard.End();
	}

	public override string SettingsCategory()
	{
		return "Blinking Animation";
	}

	private void RefreshAllCorpses()
	{
		foreach (Map map in Find.Maps)
		{
			foreach (Thing item in map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse))
			{
				if (item is Corpse corpse)
				{
					Pawn innerPawn = corpse.InnerPawn;
					if (innerPawn != null && innerPawn.RaceProps?.Humanlike == true)
					{
						corpse.InnerPawn.TryGetComp<CompBlinking>()?.InvalidateEyelidNode();
					}
				}
			}
		}
	}
}
