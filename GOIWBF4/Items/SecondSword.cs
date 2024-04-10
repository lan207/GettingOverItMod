using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GOIWBF4.BasedOn;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace GOIWBF4.Items
{
    public class SecondSword:firstSword
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 2;
            RandomColor = 0f;
            Item.rare = 2;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
             .AddIngredient(ModContent.ItemType<firstSword>())
             .AddIngredient(ItemID.Cloud, 10)
             .AddIngredient(ItemID.SandBlock, 10)
             .AddIngredient(ItemID.IceBlock, 10)
             .AddRecipeGroup("anyEvilStone", 10)
             .AddTile(TileID.Anvils)
             .AddTile(TileID.Furnaces)
             .AddTile(TileID.SkyMill)
             .Register();
        }
    }
    
   
}
