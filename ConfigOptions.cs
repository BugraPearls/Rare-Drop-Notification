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
        HypixelSkyblock, //Rare drop sound effect
        Item35, //Using Bell
        Item129, //Golf Ball sunk into a Golf Cup
        Item150, //Mimic projectile reflect
        Zombie15, //Rat
    }
    public class ConfigOptions : ModConfig
    {
        public override LocalizedText DisplayName => Language.GetText("Mods.RareDropNotification.ConfigName");
        public override ConfigScope Mode => ConfigScope.ClientSide;
        [Slider()]
        [Range(0.5f, 20f)]
        [DefaultValue(5)]
        [LabelKey("$Mods.RareDropNotification.TriggerThreshold")]
        [TooltipKey("$Mods.RareDropNotification.TriggerThresholdTip")]
        public float TriggerThreshold { get; set; }

        [LabelKey("$Mods.RareDropNotification.TextColor")]
        [TooltipKey("$Mods.RareDropNotification.TextColorTip")]
        [DefaultValue(typeof(Color), "84, 252, 252, 255"), ColorNoAlpha]
        public Color TextColor { get; set; }

        [Header("$Mods.RareDropNotification.Sound")]

        [Slider()]
        [Range(0f, 1f)]
        [DefaultValue(1f)]
        [LabelKey("$Mods.RareDropNotification.SoundVolume")]
        [TooltipKey("$Mods.RareDropNotification.SoundVolumeTip")]
        public float SoundEffectVolume { get; set; }

        [DefaultValue(SoundEffect.HypixelSkyblock)]
        [LabelKey("$Mods.RareDropNotification.CurrentSound")]
        [TooltipKey("$Mods.RareDropNotification.CurrentSoundTip")]
        public SoundEffect CurrentSound {  get; set; }

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

        [Header("$Mods.RareDropNotification.Experimental")]

        [DefaultValue(false)]
        [LabelKey("$Mods.RareDropNotification.EnableCustom")]
        [TooltipKey("$Mods.RareDropNotification.EnableCustomTip")]
        public bool EnableCustom { get; set; }

        [LabelKey("$Mods.RareDropNotification.CustomSound")]
        [TooltipKey("$Mods.RareDropNotification.CustomSoundTip")]
        public string CustomSound { get; set; }

    }
}
