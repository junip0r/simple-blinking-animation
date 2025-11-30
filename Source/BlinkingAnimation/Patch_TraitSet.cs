using HarmonyLib;
using RimWorld;

namespace BlinkingAnimation;

[HarmonyPatch(typeof(TraitSet), nameof(TraitSet.GainTrait))]
internal class TraitSet_GainTrait
{
	private void Postfix(TraitSet __instance)
	{
		if (__instance.pawn.GetComp<CompBlinking>() is CompBlinking comp)
		{
			comp.Notify_TraitsChanged();
		}
	}
}


[HarmonyPatch(typeof(TraitSet), nameof(TraitSet.RemoveTrait))]
internal class TraitSet_RemoveTrait
{
	private void Postfix(TraitSet __instance)
	{
		if (__instance.pawn.GetComp<CompBlinking>() is CompBlinking comp)
		{
			comp.Notify_TraitsChanged();
		}
	}
}
