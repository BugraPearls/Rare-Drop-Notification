using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Audio;

namespace RareDropNotification
{
    public class RareDropNotification : Mod
    {
        public static SoundStyle ModifiedSound(SoundStyle style)
        {
            return style with { Pitch = Options.SoundEffectPitch, PitchVariance = Options.SoundEffectPitchVariation, Volume = Options.SoundEffectVolume, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, Type = SoundType.Sound, PlayOnlyIfFocused = false};
        }
        public static ConfigOptions Options => ModContent.GetInstance<ConfigOptions>();
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
                if (chance <= Options.TriggerThreshold)
                {
                    Main.NewText(Language.GetTextValue("Mods.RareDropNotification.RareDrop")
                        .Replace("<color>",Options.TextColor.Hex3())
                        .Replace("<itemName>", ContentSamples.ItemsByType[drop.itemId].Name)
                        .Replace("<item>", drop.itemId.ToString())
                        .Replace("<chance>", chance.ToString()));
                    if (Options.SoundEffectVolume > 0)
                    {
                        if (Options.EnableCustom)
                        {
                            SoundEngine.PlaySound(ModifiedSound(new SoundStyle(Options.CustomSound))); //This is risky, but still fun
                        }
                        else
                        {
                            switch (Options.CurrentSound)
                            {
                                case SoundEffect.HypixelSkyblock:
                                    SoundEngine.PlaySound(ModifiedSound(new SoundStyle("RareDropNotification/Sounds/HypixelSkyblock")));
                                    break;
                                case SoundEffect.Item35:
                                    SoundEngine.PlaySound(ModifiedSound(new SoundStyle("Terraria/Sounds/Item_35")));
                                    break;
                                case SoundEffect.Item150:
                                    SoundEngine.PlaySound(ModifiedSound(new SoundStyle("Terraria/Sounds/Item_150")));
                                    break;
                                case SoundEffect.Item129:
                                    SoundEngine.PlaySound(ModifiedSound(new SoundStyle("Terraria/Sounds/Item_129")));
                                    break;
                                case SoundEffect.Zombie15:
                                    SoundEngine.PlaySound(ModifiedSound(new SoundStyle("Terraria/Sounds/Zombie_15")));
                                    break;
                                default:
                                    SoundEngine.PlaySound(ModifiedSound(new SoundStyle("RareDropNotification/Sounds/HypixelSkyblock")));
                                    break;
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
