using GOIWBF4.BasedOn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GOIWBF4.ModsAndOther
{
    public class RecipeClass:ModSystem
    {
        public override void AddRecipeGroups()
        {
            var g = new RecipeGroup(() => MyUtils.Translation("任意贱金属锤子", "AnyBaseMetelHammer",
                 "Любой молоток из недрагоценного металла"), new int[] {
                ItemID.IronHammer,ItemID.LeadHammer,ItemID.TinHammer,ItemID.CopperHammer
                });
            var g2 = new RecipeGroup(() => MyUtils.Translation("任意邪恶石块", "AnyEvilStoneBlock",
                "Любое зло"), new int[] {ItemID.EbonstoneBlock, ItemID.CrimstoneBlock });
            var g3 = new RecipeGroup(() => MyUtils.Translation("任意邪恶沙块", "AnyEvilSandBlock",
               "любая злая глыба песка"), new int[] {ItemID.EbonsandBlock, ItemID.CrimsandBlock });
            var g4 = new RecipeGroup(() => MyUtils.Translation("任意邪恶冰块", "AnyEvilIceBlock",
               "любой злой кубик льда"), new int[] { ItemID.PurpleIceBlock,ItemID.RedIceBlock });
            RecipeGroup.RegisterGroup("anyBMEhammer", g);
            RecipeGroup.RegisterGroup("anyEvilStone", g2);
            RecipeGroup.RegisterGroup("anyEvilSand", g3);
            RecipeGroup.RegisterGroup("anyEvilIce", g4);

        }
    }
}
