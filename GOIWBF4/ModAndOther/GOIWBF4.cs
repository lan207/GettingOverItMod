using GOIWBF4.Players;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace GOIWBF4.ModsAndOther
{
    public enum MsgType
    {
        Player,
    }
    public class GOIWBF4 :Mod
    {

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MsgType type = (MsgType)reader.ReadByte();
            if (type == MsgType.Player)
            {
                var index = reader.ReadByte();
                var player = Main.player[index];
                var p = player.GetModPlayer<MP>();
                if (index >= 0 && index < Main.player.Length)
                {
                    p.RecieveSync(reader);
                }
                if(Main.netMode==NetmodeID.Server)
                {
                    p.SyncPlayer(-1, whoAmI, false);
                }
            }
        }
    }
}