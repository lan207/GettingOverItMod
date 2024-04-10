using GOIWBF4.BasedOn;
using GOIWBF4.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GOIWBF4.Buffs
{
    public class JarBuff2 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.buffTime[buffIndex] = 10;
            player.noFallDmg = true;
            player.lavaImmune = true;
            if(!ClassHelp.IsThereAHammer())
            {
                player.buffTime[buffIndex] = 0;
            }
        }
      
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            var st = MyUtils.Translation("罐与锤", "Jar and Hammer", "Банка и молоток");
            buffName = st;
            rare = ItemRarityID.Expert;
            var str = MyUtils.Translation("一罐一锤之间，受苦之路之上，仅一点需铭记:\n束缚你的从来不是手脚，而是对超我的认知",
                "Between a jars and a hammer,on the path of suffering,there is only one thing to remember:\n " +
                "“It is never your hands and feet that shackle you,but your understanding of the super-ego”",
                "Между банкой и молотком, на дороге страданий, нужно помнить только одно:\n" +
                "вас связывают никогда не ваши руки и ноги, а ваше понимание суперэго."
                );
            tip = str;
        }
    }
}
