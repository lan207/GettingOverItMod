using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace GOIWBF4.Items
{

    public class FifthSword : firstSword
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 6;
            RandomColor = .6f;
            Item.rare = 6;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
             .AddIngredient(ModContent.ItemType<FourthSword>())
             .AddIngredient(ItemID.PearlstoneBlock, 10)
             .AddTile(TileID.AdamantiteForge)
             .Register();
        }
    }
}
