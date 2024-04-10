using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace GOIWBF4.Items
{

    public class SeventhSword : firstSword
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 16;
            Item.rare = -12;
            RandomColor = 0;
            Item.useTime =
            Item.useAnimation = 15;
        }
        public override bool CanUseItem(Player player)
        {
            RandomColor = Main.rand.NextFloat();
            return base.CanUseItem(player);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
             .AddIngredient(ModContent.ItemType<SixthSword>())
             .AddRecipeGroup(RecipeGroupID.Wood, 5000)
             .AddTile(TileID.LunarMonolith)
             .AddTile(TileID.BloodMoonMonolith)
             .AddTile(TileID.EchoMonolith)
             .AddTile(TileID.ShimmerMonolith)
             .AddTile(TileID.VoidMonolith)
             .Register();
        }
    }
}
