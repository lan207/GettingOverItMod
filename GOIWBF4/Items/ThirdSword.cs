using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace GOIWBF4.Items
{
    public class ThirdSword : firstSword
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 3;
            RandomColor = .2f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
             .AddIngredient(ModContent.ItemType<SecondSword>())
             .AddIngredient(ItemID.AshBlock, 10)
             .AddIngredient(ItemID.HoneyBlock, 10)
             .AddTile(TileID.Hellforge)
             .AddTile(TileID.Spikes)
             .Register();
        }
    }
}
