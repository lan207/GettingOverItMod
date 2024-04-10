using GOIWBF4.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace GOIWBF4.ModAndOther
{
    public class KeyBindSystem:ModSystem
    {
        public static ModKeybind key1;
        private static Player p => Main.LocalPlayer;
        public override void Load()
        {
            key1 = KeybindLoader.RegisterKeybind(Mod, "BackInTimeButton", "None");
        }
        public override void Unload()
        {
            key1 = null;
        }
        public static void Press()
        {
            if(key1.JustPressed)
            {
                p.GetModPlayer<MP>().SetBackPos();
            }
        }
    }
}
