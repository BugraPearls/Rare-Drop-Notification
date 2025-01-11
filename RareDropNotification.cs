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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Terraria.ModLoader.IO;

namespace RareDropNotification
{
    public class RareDropNotification : Mod
    {
        enum MessageType
        {
            ReceiveNotification,
            SendAnnouncement,
            ReceiveAnnouncement
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
                    case MessageType.SendAnnouncement:
                    int ID = reader.ReadInt32();
                    float chancc = reader.ReadSingle();
                    ModPacket packet = GetPacket();
                    packet.Write((byte)MessageType.ReceiveAnnouncement);
                    packet.Write(ID);
                    packet.Write(chancc);
                    packet.Write((byte)whoAmI);
                    packet.Send(ignoreClient: whoAmI);
                    break;
                    case MessageType.ReceiveAnnouncement:
                    int itemsId = reader.ReadInt32();
                    float chanceee = reader.ReadSingle();
                    int playerwho = reader.ReadByte();
                    Main.NewText(Language.GetTextValue("Mods.RareDropNotification.SuperRareAnnouncement")
                        .Replace("<name>", Main.player[playerwho].name)
                        .Replace("<color>", Options.SuperTextColor.Hex3())
                        .Replace("<itemName>", ContentSamples.ItemsByType[itemsId].Name)
                        .Replace("<item>", itemsId.ToString())
                        .Replace("<chance>", chanceee.ToString()));
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
            if (chance <= Options.TriggerThreshold) //a if check on the chance is done beforehand, re-checked here because; server checks its own %, with this it uses the sent client's set threshold.
            {
                if (Options.EnableSuperRare && chance <= Options.SuperTriggerThreshold)
                {
                    Main.NewText(Language.GetTextValue("Mods.RareDropNotification.SuperRareDrop")
                        .Replace("<color>", Options.SuperTextColor.Hex3())
                        .Replace("<itemName>", ContentSamples.ItemsByType[itemID].Name)
                        .Replace("<item>", itemID.ToString())
                        .Replace("<chance>", chance.ToString()));
                    if (Options.SoundEffectVolume > 0)
                    {
                        if (Options.EnableSuperCustom)
                        {
                            SoundEngine.PlaySound(ModifiedSound(new SoundStyle(Options.SuperCustomSound))); //This is risky, but still fun
                        }
                        else
                        {
                            switch (Options.SuperCurrentSound)
                            {
                                case SoundEffect.HypixelSkyblock:
                                    SoundEngine.PlaySound(ModifiedSound(new SoundStyle("RareDropNotification/Sounds/HypixelSkyblock")));
                                    break;
                                case SoundEffect.CSGO:
                                    SoundEngine.PlaySound(ModifiedSound(new SoundStyle("RareDropNotification/Sounds/CSGO")));
                                    break;
                                case SoundEffect.PSO2:
                                    SoundEngine.PlaySound(ModifiedSound(new SoundStyle("RareDropNotification/Sounds/PSO2")));
                                    break;
                                case SoundEffect.PokemonRBY:
                                    SoundEngine.PlaySound(ModifiedSound(new SoundStyle("RareDropNotification/Sounds/PokemonRBYitem")));
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
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        ModPacket packet = ModContent.GetInstance<RareDropNotification>().GetPacket();
                        packet.Write((byte)MessageType.SendAnnouncement);
                        packet.Write(itemID);
                        packet.Write((float)chance);
                        packet.Send();
                    }
                }
                else
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
                                case SoundEffect.CSGO:
                                    SoundEngine.PlaySound(ModifiedSound(new SoundStyle("RareDropNotification/Sounds/CSGO")));
                                    break;
                                case SoundEffect.PSO2:
                                    SoundEngine.PlaySound(ModifiedSound(new SoundStyle("RareDropNotification/Sounds/PSO2")));
                                    break;
                                case SoundEffect.PokemonRBY:
                                    SoundEngine.PlaySound(ModifiedSound(new SoundStyle("RareDropNotification/Sounds/PokemonRBYitem")));
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
        }
        private static ItemDropAttemptResult NotifyDrop(On_ItemDropResolver.orig_ResolveRule orig, ItemDropResolver self, IItemDropRule rule, DropAttemptInfo info)
        {
            ItemDropAttemptResult result = orig(self, rule, info);
            if (result.State is ItemDropAttemptResultState.Success && rule is CommonDrop drop)
            {
                double chance = Math.Round((double)Math.Max(drop.chanceNumerator, 1) / Math.Max(drop.chanceDenominator, 1) * 100, 3);
                if (chance <= ConfigOptions.MaxPercent) //we check if chance is below 20% (Max in config) here to prevent unnecessary messages sent as Netcode since otherwise it would send messages for all drops.
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
