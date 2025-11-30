using HarmonyLib;
using RimWorld;

namespace BlinkingAnimation;

[HarmonyPatch(typeof(Pawn_GeneTracker), nameof(Pawn_GeneTracker.Notify_GenesChanged))]
internal class Pawn_GeneTracker_Notify_GenesChanged
{
	private void Postfix(Pawn_GeneTracker __instance)
	{
		if (__instance.pawn.GetComp<CompBlinking>() is CompBlinking comp)
		{
			comp.Notify_GenesChanged();
		}
	}
}
