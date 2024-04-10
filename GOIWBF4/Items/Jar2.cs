using GOIWBF4.Buffs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace GOIWBF4.Items
{
    public class Jar2: ModItem
    {
        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.value = 100;
            Item.rare = ItemRarityID.Gray;
            Item.useTime =
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.shoot = 10;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("anyBMEhammer")
                .AddRecipeGroup(RecipeGroupID.IronBar, 8)
                .AddTile(ItemID.IronAnvil)
                .Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.HasBuff<JarBuff2>())
            {
                player.ClearBuff(ModContent.BuffType<JarBuff2>());
            }
            else
            {
                player.AddBuff(ModContent.BuffType<JarBuff2>(), 20);
            }
            return false;
        }
    }
}
