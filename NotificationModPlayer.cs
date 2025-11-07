using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace RareDropNotification
{
    public class NotificationModPlayer : ModPlayer
    {
        public bool AutoTrashEnabled = false;
        public List<Item> AutoTrashItems = new();
        public override void Load()
        {
            if (ModLoader.TryGetMod("AutoTrash", out Mod autoTrash) && autoTrash.TryFind("AutoTrashPlayer", out ModPlayer trashPlayer))
            {
                FieldInfo isItEnabled = trashPlayer.GetType().GetField("AutoTrashEnabled");
                FieldInfo listOfItems = trashPlayer.GetType().GetField("AutoTrashItems");
                if (isItEnabled is not null && isItEnabled.FieldType == typeof(bool))
                {
                    AutoTrashEnabled = (bool)isItEnabled.GetValue(trashPlayer);
                }
                if (listOfItems is not null && listOfItems.FieldType == typeof(List<Item>))
                {
                    AutoTrashItems = (List<Item>)listOfItems.GetValue(trashPlayer);
                }
            }
        }
    }
}