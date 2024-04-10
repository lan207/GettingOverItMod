using GOIWBF4.BasedOn;
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
    public class SixthSword : firstSword
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 8;
            RandomColor = .8f;
            Item.rare = 8;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
             .AddIngredient(ModContent.ItemType<FifthSword>())
             .AddIngredient(ItemID.PinkIceBlock, 10)
             .AddIngredient(ItemID.PearlsandBlock, 10)
             .AddTile(TileID.WoodenSpikes)
             .AddTile(TileID.LunarCraftingStation)
             .Register();
        }
        public override void HoldItem(Player player)
        {
            if (Main.mouseMiddleRelease && Main.mouseMiddle)
            {
                var or = MyUtils.Translation("或", "or", "или");
                Main.NewText("1)[i:3536]" + or + "[i:3537]" + or + "[i:3538]" +
                     or + "[i:3539]\n2)[i:4054] 3)[i:4318] 4)[i:5347] 5)[i:5345]");
            }
        }
    }
}
