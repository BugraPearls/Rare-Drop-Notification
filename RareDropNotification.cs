using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Audio;
using System.Threading.Channels;
using System.IO;
using System.Runtime.InteropServices.JavaScript;

namespace RareDropNotification
{
    public class RareDropNotification : Mod
    {
        enum MessageType
        {
            ReceiveNotification
        }
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType msgType = (MessageType)reader.ReadByte();
            switch (msgType)
            {
                case MessageType.ReceiveNotification: //received by server
                    int itemId = reader.ReadInt32();
                    float chance = reader.ReadSingle();
                    NotificationEffects(itemId, chance);
                    break;
            }
        }
        public static SoundStyle ModifiedSound(SoundStyle style)
        {
            return style with { Pitch = Options.SoundEffectPitch, PitchVariance = Options.SoundEffectPitchVariation, Volume = Options.SoundEffectVolume, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, Type = SoundType.Sound, PlayOnlyIfFocused = false };
        }
        public static ConfigOptions Options => ModContent.GetInstance<ConfigOptions>();
        public override void Load()
        {
            On_ItemDropResolver.ResolveRule += NotifyDrop;
        }
        public static void NotificationEffects(int itemID, float chance)
        {
                Main.NewText(Language.GetTextValue("Mods.RareDropNotification.RareDrop")
                            .Replace("<color>", Options.TextColor.Hex3())
                            .Replace("<itemName>", ContentSamples.ItemsByType[itemID].Name)
                            .Replace("<item>", itemID.ToString())
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
        private static ItemDropAttemptResult NotifyDrop(On_ItemDropResolver.orig_ResolveRule orig, ItemDropResolver self, IItemDropRule rule, DropAttemptInfo info)
        {
            ItemDropAttemptResult result = orig(self, rule, info);
            if (result.State is ItemDropAttemptResultState.Success && rule is CommonDrop drop)
            {
                double chance = Math.Round((double)Math.Max(drop.chanceNumerator, 1) / Math.Max(drop.chanceDenominator, 1) * 100, 3);
                if (chance <= Options.TriggerThreshold)
                {
                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = ModContent.GetInstance<RareDropNotification>().GetPacket();
                        packet.Write((byte)MessageType.ReceiveNotification);
                        packet.Write(drop.itemId);
                        packet.Write((float)chance);
                        packet.Send(toClient: info.player.whoAmI);
                    }
                    else
                    {
                        NotificationEffects(drop.itemId, (float)chance);
                    }
                }
            }
            return result;
        }
    }
}
