using GOIWBF4.BasedOn;
using GOIWBF4.Mount;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GOIWBF4.Proj
{
    public class Class1:ModProjectile
    {
        Player p => Main.player[Projectile.owner];
        public override string Texture => "GOIWBF4/Proj/Hammer";
        bool Has,Has2,Has3;
        int dir,timer,MaxTimer=150;
        public override void SetDefaults()
        {
            Projectile.timeLeft = 90;
            Projectile.width = Projectile.height = 48;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
        }
        private void SetSpeedAndHas(string a = "", bool b = false)
        {
            if (!Has)
            {
                Main.NewText(a+"!!!");
                Has = true;
                Has2 = false;
                if(!b)Projectile.velocity = Vector2.Zero;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            SetSpeedAndHas(MyUtils.Translation("物块","Tile", "блокировать"));
            return false;
        }
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SetSpeedAndHas("NPC");
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ScalingArmorPenetration+=1;
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.ScalingArmorPenetration += 1;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            SetSpeedAndHas(MyUtils.Translation("玩家","Player", "Игрок"));
        }
        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(3*dir);
            if(Has)
            {
                if(!Has2)
                {
                    if(Has3)
                    {
                        if (Projectile.velocity.Length() < .01f) Has3 = false;
                        Projectile.velocity *= .999f;
                        Projectile.timeLeft = 150;
                    }
                    else
                    {
                        if (timer < MaxTimer)
                        {
                            timer++;
                            Projectile.timeLeft = 150;
                        }
                        else Has2 = true;
                    }
                   
                }
                else
                {
                    Projectile.velocity = (p.Center - Projectile.Center) / 4;
                }
            }
            else
            {
                Projectile.timeLeft = 150;
            }
            foreach(var p in Main.projectile)
            {
                if(p.whoAmI!=Projectile.whoAmI&&p.Hitbox.Intersects(Projectile.Hitbox)&&p.friendly&&p.ModProjectile is not JarMount2&&p.ModProjectile is not TestSwordProj)
                {
                    Has3 = true;
                    var b = p.ModProjectile is Hammer||p.ModProjectile is Class1;
                    SetSpeedAndHas(b?MyUtils.Translation("锤子","Hammer", "молоток") :MyUtils.Translation("射弹","Projectile", "Стрельба из пули"),true);
                    var a = b? 12f : 8f;
                    Projectile.velocity = (Projectile.Center - p.Center).SafeNormalize(Vector2.UnitX) * a;
                }
            }
        }
        
        public override void OnSpawn(IEntitySource source)
        {
            dir = p.direction;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var t = MyUtils.GetT2DValue(Texture);
            Main.spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, MyUtils.GetRec(t), lightColor, Projectile.rotation.JudgeSwordRot(),
            MyUtils.GetOrig(t), 1, SpriteEffects.None, 0);
            return false ;
        }
    }
}
