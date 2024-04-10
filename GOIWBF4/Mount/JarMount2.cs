using GOIWBF4.BasedOn;
using GOIWBF4.Buffs;
using GOIWBF4.Proj;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GOIWBF4.Mount
{
    public class JarMount2:ModProjectile
    {
        Player p;
        int dir;
        public override void SetDefaults()
        {
            Projectile.timeLeft = 20;
            Projectile.width = 50;
            Projectile.height = 46;
            Projectile.friendly =
            Projectile.hostile=
            Projectile.tileCollide = false;
            
        }

        public override void AI()
        {
            if (p.HasBuff<JarBuff2>()&&p.Alives()) Projectile.timeLeft = 20;
            else Projectile.Kill();
            Projectile.Center = p.Center + 4.Int2V2Y();
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation = 0;
            dir = p.direction;
            Projectile.rotation =p.fullRotation;
        }
        public override void OnSpawn(IEntitySource source)
        {
            p = Main.player[Projectile.owner];
            //SetPlayerControls();
        }
        public override void OnKill(int timeLeft)
        {
            //SetPlayerControls(true);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs,
            List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var j = MyUtils.GetT2DValue(Texture);
            Main.spriteBatch.Draw(j, Projectile.Center - Main.screenPosition, MyUtils.GetRec(j), lightColor,
                Projectile.rotation, MyUtils.GetOrig(j), 1, dir==1?SpriteEffects.None:
                SpriteEffects.FlipHorizontally, 0);
            return false ;
        }
    }
}
