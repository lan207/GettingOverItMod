using GOIWBF4.BasedOn;
using GOIWBF4.Buffs;
using GOIWBF4.ModConfigs;
using GOIWBF4.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GOIWBF4.Proj
{
    public class Hammer:ModProjectile
    {
        public int maxDis = 48;
        Player p;
        HammerConfig config;
        int LiquidType,timer,MaxTimer=11;
        Vector2 v0,v,v2,last,TilePos,TileSize,HammerCenter,HammerRot,Destination,PlayCenter,LiquidCenter;
        bool CollideTile,SolidTop,signal,Slope;
        Rectangle TileRect;
        float dis, distance;
        public override void SetDefaults()
        {
            Projectile.timeLeft = 20;
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide =true;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.hide = true;
            Projectile.netUpdate = true;
        }
       
        public override void AI()
        {
            PlayCenter = p.Center - 23.Int2V2Y();

            v0 = p.GetModPlayer<MP>().MyMouse - PlayCenter;
            // v为玩家指向鼠标向量
            v = v0.SafeNormalize(Vector2.UnitX);
            //Main.NewText(velocity.Length());

            //限制一下v2大小
            v2 = (v0 -v*24.5f).LimitV2(maxDis);
            //《有buff者活，无buff者死》
            if (p.HasBuff<JarBuff2>()&&p.Alives()) Projectile.timeLeft = 20;
            else Projectile.Kill();
            //设置上一帧射弹位置
            last = Projectile.Center;
            var FurtherCenter = (v2 + PlayCenter + v * 24.5f);
            var ProjDirOri = (FurtherCenter - last)/5;
            Projectile.velocity = ProjDirOri.LimitV2(10);
            //手动检测非半砖物块碰撞
            if (Projectile.position.SolidCollision(Projectile.width, Projectile.height, out TilePos, out TileSize, out SolidTop, out _, out _))
            {
                var TileC = TilePos + TileSize / 2;
                if (SolidTop )
                {
                    if(Projectile.Center.Y < TileC.Y) CollideTile = true;

                }
            }
            else
            {
                //划水逻辑
                if(Projectile.position.HasHalfBlockLiquid(Projectile.width,Projectile.height,out LiquidType,out LiquidCenter))
                {
                    var AddVelX = Math.Clamp(-Projectile.velocity.X/40, -.05f, .05f);
                    p.velocity.X += AddVelX;
                   
                }
            }
            TileRect = new Rectangle(TilePos.X.RoundInt(), TilePos.Y.RoundInt(), TileSize.X.RoundInt(), TileSize.Y.RoundInt());
            var d = (FurtherCenter - PlayCenter).Length();
            dis = d/24f;
            Player.CompositeArmStretchAmount LittleArmRot;
            //135-180
            if (d > 44.35f) LittleArmRot = Player.CompositeArmStretchAmount.Full;
            //0-45
            else if (d <= 18.37f) LittleArmRot=Player.CompositeArmStretchAmount.None;
            //45-90
            else if (d > 18.37f && d <= 19.8f) LittleArmRot=Player.CompositeArmStretchAmount.Quarter;
            //90 -135
            else LittleArmRot=Player.CompositeArmStretchAmount.ThreeQuarters;
            p.SetCompositeArmFront(true, LittleArmRot, Projectile.rotation - MathHelper.ToRadians(90));
            //检测到物块距离
            distance = 0f;
            Collision.CheckAABBvLineCollision(TilePos, TileSize, PlayCenter, PlayCenter + v * 48.Int2Hypotenuse(), 4, ref distance);
            //if (Projectile.velocity != Vector2.Zero) Projectile.velocity = Vector2.Zero;
            var TileCenter = TilePos + TileSize / 2;
            var AntiTileDir = (Projectile.Center - TileCenter).SafeNormalize(Vector2.UnitX);
            if(Keyboard.GetState().GetPressedKeys().Contains(Keys.C))Projectile.Center = FurtherCenter;

            if(CollideTile)
            {
                DoSthInCollideTile();
            }
            if(signal)
            {
                if (timer >= MaxTimer)
                {
                    signal = false;
                    timer = 0;
                }
                else timer++;
            }
            //Main.NewText(signal+timer.ToString());
            var l = 0;
            while (Projectile.Hitbox.Intersects(TileRect))
            {
                if (!signal) signal = true;
                if (l > 5) break;
                Projectile.Center += AntiTileDir * 16;
                l++;

            }
        }
        void DoSthInCollideTile()
        {
            if(!signal)
            {
                SoundEngine.PlaySound(SoundID.Dig);
                signal = true;
                //核心-人物移动代码
                Destination = Projectile.Center - v * distance;
                var playerDir = (Destination - PlayCenter).SafeNormalize(Vector2.UnitX);
                var Round = (dis).RoundByAnyStep(8);
                var RoundDes = 3f;
                var SimilarValue = Math.Abs(Round - RoundDes) < .25f ? RoundDes : Round;
                var originVel = playerDir * (SimilarValue + (Projectile.velocity.Length() / 4).RoundByAnyStep(8));
                
                var SpeedBaseX = .1f*config.GivenPlayerVelocityXBase;
                var SpeedBaseY = .15f*config.GivenPlayerVelocityYBase;
                p.velocity += (originVel.X * SpeedBaseX).Float2V2X() + (originVel.Y * SpeedBaseY).Float2V2Y();
                if(Main.netMode==NetmodeID.SinglePlayer)Main.NewText(dis + ">" + Round + ">" + originVel.Length() + ">" + (originVel * SpeedBaseX).Length() + ">" + p.velocity.Length());
            }
            
            

        }
        public override void OnSpawn(IEntitySource source)
        {
            p = Main.player[Projectile.owner];
            config = p.GetModPlayer<MP>().config;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //检测碰撞
            //Main.NewText("Colllide tiles");
            DoSthInCollideTile();
            //Projectile.TryGetGlobalProjectile(out MyGlobalProj hammer);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles,
            List<int> overPlayers, List<int> overWiresUI)
        {
            //画在玩家图层之上
            overPlayers.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var hammer = MyUtils.GetT2DValue(Texture);

            v0 = Main.MouseWorld - PlayCenter;
            //绘制锤身与锤头
            HammerCenter = Projectile.Center - v * 24.5f;
            HammerRot = (HammerCenter - PlayCenter).SafeNormalize(Vector2.UnitX);
            Projectile.rotation = HammerRot.ToRotation();
           
            Main.spriteBatch.Draw(hammer, HammerCenter - Main.screenPosition, MyUtils.GetRec(hammer), lightColor,
                Projectile.rotation.JudgeSwordRot(), MyUtils.GetOrig(hammer), 1, SpriteEffects.None, 0);
            return false;
        }
        public override void PostAI()
        {
            
        }
        public override void PostDraw(Color lightColor)
        {
            //每帧清除bool
            CollideTile =
            SolidTop = false;
            distance = 0;
        }
    }
}
