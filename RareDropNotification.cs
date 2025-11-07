using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

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
        public static void PlayASound(SoundEffect setting)
        {
            switch (setting)
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
                case SoundEffect.POE:
                    SoundEngine.PlaySound(ModifiedSound(new SoundStyle("RareDropNotification/Sounds/POE")));
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
        public static SoundStyle ModifiedSound(SoundStyle style)
        {
            return style with { Pitch = Options.SoundEffectPitch, PitchVariance = Options.SoundEffectPitchVariation, Volume = Options.SoundEffectVolume, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, Type = SoundType.Sound, PlayOnlyIfFocused = false };
        }
        public static ConfigOptions Options => ModContent.GetInstance<ConfigOptions>();
        public static void NotificationEffects(int itemID, float chance)
        {
            if (chance <= Options.TriggerThreshold && Options.BlacklistedItems.Exists(x => x.Type == itemID) == false) //a if check on the chance is done beforehand, re-checked here because; server checks its own %, with this it uses the sent client's set threshold.
            {
                if (Options.EnableNotShowingResearched && CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(itemID, out int count) && Main.LocalPlayerCreativeTracker.ItemSacrifices.GetSacrificeCount(itemID) >= count)
                {
                    return;
                }

                if (Options.EnableNotShowingAutoTrashed && ModLoader.TryGetMod("AutoTrash", out Mod autoTrash)) //Checking if Auto Trash mod exists
                {
                    foreach (var trashPlayer in Main.LocalPlayer.ModPlayers) //Going through Mod Player instances
                    {
                        if (trashPlayer.Mod != autoTrash || trashPlayer.Name != "AutoTrashPlayer") //Eliminating all that isn't AutoTrashPlayer from Auto Trash Mod
                        {
                            continue;
                        }
                        
                        Type trashPlayerTypeInfo = autoTrash.GetType().Assembly.GetType("AutoTrash.AutoTrashPlayer"); //Getting 'template' of the instance, not actually getting the instance itself here
                        FieldInfo isItEnabled = trashPlayerTypeInfo.GetField("AutoTrashEnabled"); //We get the field info through the template
                        if (isItEnabled is not null && isItEnabled.FieldType == typeof(bool)) //We make sure its a bool for no errors
                        {
                            bool enable = (bool)isItEnabled.GetValue(trashPlayer); //We get the value from isItEnabled (the AutoTrashEnabled field) inside the trashPlayer (ModPlayer instance we got from Main.LocalPlayer.ModPlayers) 
                            if (enable) //if Auto Trash is enabled
                            {
                                FieldInfo listOfItems = trashPlayerTypeInfo.GetField("AutoTrashItems"); //Same as 'enable', we take the List's information from 'template'.
                                if (listOfItems is not null && listOfItems.FieldType == typeof(List<Item>)) //we make sure its List<Item> for no errors
                                {
                                    List<Item> AutoTrashItems = (List<Item>)listOfItems.GetValue(trashPlayer); //We take the actual item list off of trashPlayer, same as above
                                    if (AutoTrashItems.Exists(x => x.type == itemID)) //checking if our dropped item is inside this list
                                    {
                                        return; //This method will stop running from here ofc, if both Auto Trash is enabled, and item id exists within the list of items of Auto Trash.
                                    }
                                }
                            }
                        }
                    }
                }
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
                            SoundEngine.PlaySound(ModifiedSound(new SoundStyle(Options.SuperCustomSound)));
                        }
                        else
                        {
                            PlayASound(Options.SuperCurrentSound);
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
                            PlayASound(Options.CurrentSound);
                        }
                    }
                }
            }
        }
        public override void Load()
        {
            On_ItemDropResolver.ResolveRule += NotifyDrop;
            On_OneFromOptionsDropRule.TryDroppingItem += NotifyDropForOneFromOptions;
            //On_OneFromRulesRule.TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction += NotifyDropForOneFromRules; //straight up appears to be 100% chance, doesn't work zz
        }
        public static void HandleNotifEffects(double chance, int itemID, int playerWhoAmI)
        {
            if (chance <= ConfigOptions.MaxPercent //we check if chance is below 20% (Max in config) here to prevent unnecessary messages sent as Netcode since otherwise it would send messages for all drops.
                && NPCLoader.blockLoot.Contains(itemID) == false) //checking if the given item drop is blocked from all NPCs. Fix for Calamity preventing food drops.
            {
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = ModContent.GetInstance<RareDropNotification>().GetPacket();
                    packet.Write((byte)MessageType.ReceiveNotification);
                    packet.Write(itemID);
                    packet.Write((float)chance);
                    packet.Send(toClient: playerWhoAmI);
                }
                else
                {
                    NotificationEffects(itemID, (float)chance);
                }
            }
        }
        private static ItemDropAttemptResult NotifyDrop(On_ItemDropResolver.orig_ResolveRule orig, ItemDropResolver self, IItemDropRule rule, DropAttemptInfo info)
        {
            ItemDropAttemptResult result = orig(self, rule, info);
            if (result.State is ItemDropAttemptResultState.Success && rule is CommonDrop drop)
            {
                double chance = Math.Round((double)Math.Max(drop.chanceNumerator, 1) / Math.Max(drop.chanceDenominator, 1) * 100, 3);
                HandleNotifEffects(chance, drop.itemId, info.player.whoAmI);
            }
            return result;
        }
        private static ItemDropAttemptResult NotifyDropForOneFromOptions(On_OneFromOptionsDropRule.orig_TryDroppingItem orig, OneFromOptionsDropRule self, DropAttemptInfo info)
        {
            ItemDropAttemptResult result; //This here is vanilla code + HandleNotifEffects in it. This may cause disturbance if other mods also use same method detour, an IL edit may be looked into in the future.
            if (info.player.RollLuck(self.chanceDenominator) < self.chanceNumerator)
            {
                int itemId = self.dropIds[info.rng.Next(self.dropIds.Length)];
                //Dividing the 'chance' to be notified with the length of drop id's, bc for some reason the chance appears as the cumulative chance of all drops in this rule combined,
                //I THINK despite still being 'cumulative chance / item count'. Ex. Ancient Cobalt Armor drop chance from Hornets seems to be 0.33%, but when notif is received, it says 1%.
                HandleNotifEffects(Math.Round((double)Math.Max(self.chanceNumerator, 1) / Math.Max(self.chanceDenominator, 1) * 100 / self.dropIds.Length, 3), itemId, info.player.whoAmI);

                CommonCode.DropItem(info, itemId, 1);
                result = default(ItemDropAttemptResult);
                result.State = ItemDropAttemptResultState.Success;
                return result;
            }

            result = default(ItemDropAttemptResult);
            result.State = ItemDropAttemptResultState.FailedRandomRoll;
            return result;
        }



        //This was a attempt to show loot drops such as from Pumpkin Moon bosses, but for whatever reason, both self. and drop. ones has 100% chance (despite of course not being 100%)
        //And truthfully I don't know where to find the info to check the current chance etc. for it. I am shelving this for now.


        //private static ItemDropAttemptResult NotifyDropForOneFromRules(On_OneFromRulesRule.orig_TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction orig, OneFromRulesRule self, DropAttemptInfo info, ItemDropRuleResolveAction resolveAction)
        //{
        //    ItemDropAttemptResult result; //This here is vanilla code + HandleNotifEffects in it. This may cause disturbance if other mods also use same method detour, an IL edit may be looked into in the future.
        //    /*
        //    if (info.rng.Next(chanceDenominator) == 0) {
        //    */

        //    if (info.rng.Next(self.chanceDenominator) < self.chanceNumerator)
        //    {
        //        int chosenOption = info.rng.Next(self.options.Length);
        //        if (self.options[chosenOption] is CommonDrop drop)
        //        {
        //            HandleNotifEffects(Math.Round((double)Math.Max(self.chanceNumerator, 1) / Math.Max(self.chanceDenominator, 1) * 100, 3), drop.itemId, info.player.whoAmI);
        //        }
        //        resolveAction(self.options[chosenOption], info);
        //        result = default(ItemDropAttemptResult);
        //        result.State = ItemDropAttemptResultState.Success;
        //        return result;
        //    }

        //    result = default(ItemDropAttemptResult);
        //    result.State = ItemDropAttemptResultState.FailedRandomRoll;
        //    return result;
        //}
    }
}
