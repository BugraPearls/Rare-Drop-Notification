using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RareDropNotification
{
    public class RareDropNotification : Mod
    {
        public override void Load()
        {
            On_ItemDropResolver.ResolveRule += NotifyDrop;
        }

        private static ItemDropAttemptResult NotifyDrop(On_ItemDropResolver.orig_ResolveRule orig, ItemDropResolver self, IItemDropRule rule, DropAttemptInfo info)
        {
            ItemDropAttemptResult result = orig(self, rule, info);
            if (result.State is ItemDropAttemptResultState.Success && rule is CommonDrop drop)
            {
                double chance = Math.Round((double)Math.Max(drop.chanceNumerator, 1) / Math.Max(drop.chanceDenominator, 1) * 100, 3);
                if (chance <= 10)
                {
                    Main.NewText(Language.GetTextValue("Mods.RareDropNotification.RareDrop").Replace("<itemName>", ContentSamples.ItemsByType[drop.itemId].Name).Replace("<item>", drop.itemId.ToString()).Replace("<chance>", chance.ToString()));
                }
            }
            return result;
        }
    }
}
