using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace RareDropNotification
{
    public enum SoundEffect
    {
        HypixelSkyblock, //Hypixel Skyblock Rare drop sound effect
        CSGO, //Counter Strike: Global Offensive Weapon Case Legendary Item drop sound effect
        PSO2, //Phantasy Star Online 2 Rare drop sound effect
        PokemonRBY, //Pokemon RBY Finding a Item sound effect
        POE, //Path of Exile Divine Drop sound effect
        Item35, //Using Bell
        Item129, //Golf Ball sunk into a Golf Cup
        Item150, //Mimic projectile reflect
        Zombie15, //Rat
    }
    public class ConfigOptions : ModConfig
    {
        public const float MaxPercent = 20f;
        public override LocalizedText DisplayName => Language.GetText("Mods.RareDropNotification.ConfigName");
        public override ConfigScope Mode => ConfigScope.ClientSide;
        [Header("$Mods.RareDropNotification.Mechanics")]
        [Slider()]
        [Range(0.5f, MaxPercent)]
        [DefaultValue(5f)]
        [LabelKey("$Mods.RareDropNotification.TriggerThreshold")]
        [TooltipKey("$Mods.RareDropNotification.TriggerThresholdTip")]
        public float TriggerThreshold { get; set; }

        [LabelKey("$Mods.RareDropNotification.TextColor")]
        [TooltipKey("$Mods.RareDropNotification.TextColorTip")]
        [DefaultValue(typeof(Color), "84, 252, 252, 255"), ColorNoAlpha]
        public Color TextColor { get; set; }

        [LabelKey("$Mods.RareDropNotification.ShowResearch")]
        [TooltipKey("$Mods.RareDropNotification.ShowResearchTip")]
        [DefaultValue(true)]
        public bool EnableNotShowingResearched { get; set; }

        [LabelKey("$Mods.RareDropNotification.EnableSuperRare")]
        [TooltipKey("$Mods.RareDropNotification.EnableSuperRareTip")]
        [DefaultValue(true)]
        public bool EnableSuperRare { get; set; }

        [Slider()]
        [Range(0.01f, 5f)]
        [DefaultValue(0.5f)]
        [LabelKey("$Mods.RareDropNotification.SuperTriggerThreshold")]
        [TooltipKey("$Mods.RareDropNotification.SuperTriggerThresholdTip")]
        public float SuperTriggerThreshold { get; set; }

        [LabelKey("$Mods.RareDropNotification.SuperTextColor")]
        [TooltipKey("$Mods.RareDropNotification.SuperTextColorTip")]
        [DefaultValue(typeof(Color), "255, 125, 115, 255"), ColorNoAlpha]
        public Color SuperTextColor { get; set; }

        [LabelKey("$Mods.RareDropNotification.EnableAnnouncements")]
        [TooltipKey("$Mods.RareDropNotification.EnableAnnouncementsTip")]
        [DefaultValue(true)]
        public bool EnableAnnouncements { get; set; }

        [LabelKey("$Mods.RareDropNotification.BlacklistedItems")]
        [TooltipKey("$Mods.RareDropNotification.BlacklistedItemsTip")]
        public List<ItemDefinition> BlacklistedItems = new();

        [Header("$Mods.RareDropNotification.Sound")]

        [Slider()]
        [Range(0f, 1f)]
        [DefaultValue(1f)]
        [LabelKey("$Mods.RareDropNotification.SoundVolume")]
        [TooltipKey("$Mods.RareDropNotification.SoundVolumeTip")]
        public float SoundEffectVolume { get; set; }

        [Slider()]
        [Range(-1f, 1f)]
        [DefaultValue(0)]
        [LabelKey("$Mods.RareDropNotification.SoundPitch")]
        [TooltipKey("$Mods.RareDropNotification.SoundPitchTip")]
        public float SoundEffectPitch { get; set; }

        [Slider()]
        [Range(0, 2f)]
        [DefaultValue(0.5f)]
        [LabelKey("$Mods.RareDropNotification.SoundPitchVariation")]
        [TooltipKey("$Mods.RareDropNotification.SoundPitchVariationTip")]
        public float SoundEffectPitchVariation { get; set; }

        [DefaultValue(SoundEffect.HypixelSkyblock)]
        [LabelKey("$Mods.RareDropNotification.CurrentSound")]
        [TooltipKey("$Mods.RareDropNotification.CurrentSoundTip")]
        public SoundEffect CurrentSound { get; set; }

        [DefaultValue(SoundEffect.PSO2)]
        [LabelKey("$Mods.RareDropNotification.SuperCurrentSound")]
        [TooltipKey("$Mods.RareDropNotification.SuperCurrentSoundTip")]
        public SoundEffect SuperCurrentSound { get; set; }

        [Header("$Mods.RareDropNotification.Experimental")]

        [DefaultValue(false)]
        [LabelKey("$Mods.RareDropNotification.EnableCustom")]
        [TooltipKey("$Mods.RareDropNotification.EnableCustomTip")]
        public bool EnableCustom { get; set; }

        [LabelKey("$Mods.RareDropNotification.CustomSound")]
        [TooltipKey("$Mods.RareDropNotification.CustomSoundTip")]
        public string CustomSound { get; set; }

        [DefaultValue(false)]
        [LabelKey("$Mods.RareDropNotification.EnableSuperCustom")]
        [TooltipKey("$Mods.RareDropNotification.EnableSuperCustomTip")]
        public bool EnableSuperCustom { get; set; }

        [LabelKey("$Mods.RareDropNotification.SuperCustomSound")]
        [TooltipKey("$Mods.RareDropNotification.SuperCustomSoundTip")]
        public string SuperCustomSound { get; set; }
    }
}
