using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace GOIWBF4.Items
{

    public class FourthSword : firstSword
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 4;
            RandomColor = .4f;
            Item.rare = 4;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
             .AddIngredient(ModContent.ItemType<ThirdSword>())
             .AddRecipeGroup("anyEvilIce", 10)
             .AddRecipeGroup("anyEvilSand", 10)
             .AddTile(TileID.MythrilAnvil)
             .Register();
        }
    }
}
