using GOIWBF4.BasedOn;
using GOIWBF4.Buffs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GOIWBF4.Players
{
    public class MPDL:PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Head);
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return Main.gameMenu;
        }
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {

            if (Main.gameMenu)
            {
                var p = drawInfo.drawPlayer;
                var Center = drawInfo.Center - Main.screenPosition;
                p.GetModPlayer<MP>().previewCenter = Center;

                
            }
        }
    }
}
