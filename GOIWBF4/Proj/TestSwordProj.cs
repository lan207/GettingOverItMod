using GOIWBF4.BasedOn;
using GOIWBF4.Items;
using GOIWBF4.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GOIWBF4.Proj
{
    public class TestSwordProj:ModProjectile
    {
        Player p => Main.player[Projectile.owner];
        int dir => p.direction;
        int type => (int)Projectile.ai[0];
        float rotSpeed=10,MutiplySpeed,HasRot,Scale,ZeroDeg,RotDir,StabTimer, kb;
        int StabTiees,StabTimersMax=3,RotTimer, RotTimerMax=300,dirNOW,dmg;
        bool IsRoting,HasChange,CanShoot;
        Vector2 HandCenter,ProjFlyDir;
        Vector2 p2Mouse => (p.GetModPlayer<MP>().MyMouse - p.Center).SafeNormalize(Vector2.UnitX);
        List<int> deg2Swing = new List<int>()
        {
            270,225
        };
        List<int> degIntitial = new List<int>()
        {
            -135,45,0,0
        };
        Color c;
        public override string Texture => "GOIWBF4/Items/firstSword";
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ScalingArmorPenetration += 1;
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.ScalingArmorPenetration += 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 20;
            Projectile.netUpdate = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = Projectile.velocity.X;
            MutiplySpeed =60/ Projectile.velocity.Y;
            Projectile.velocity = Vector2.Zero;
            c =Projectile.ai[1]>=0&&Projectile.ai[1]<=1? Main.hslToRgb(Projectile.ai[1], 1, .5f):Color.White;
            dmg = Projectile.damage;
            kb = Projectile.knockBack;
            CanShoot = Projectile.ai[0] == 0;
            Projectile.ai[0] = 0;
            DoSth(true);
            
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            var Half = (Scale * 20).Float2Hypotenuse().Float2V2X().RotatedBy(Projectile.rotation);
            var start = Projectile.Center - Half;
            var end = Projectile.Center + Half;
            var p = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(),targetHitbox.Size(),start,
                end,(8*Scale).Float2Hypotenuse(),ref p);
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, c.R/255, c.G/255, c.B/255);
            Projectile.timeLeft = 20;
            p.heldProj = Projectile.whoAmI;
            p.itemTime = p.itemAnimation = 2;
            Projectile.width = Projectile.height =
            (40 * Scale).RoundInt();
            if (Projectile.ai[0]==0)
            {
                RotateSword(true);
                SetProjCenter(2);
            }
            else if (Projectile.ai[0]==1)
            {
                RotateSword();
                SetProjCenter(1,2);
            }
            else if (Projectile.ai[0]==2)
            {
                if (StabTiees < StabTimersMax)
                {
                    if (Vector2.Distance(Projectile.Center, p.Center) < 60.Int2Hypotenuse())
                    {
                        StabTimer += rotSpeed / 2;
                    }
                    else
                    {
                        StabTiees++;
                        StabTimer = 0;
                        ProjFlyDir = p2Mouse;
                        Projectile.rotation = ProjFlyDir.ToRotation();
                    }
                }
                else ContinueOrSuicide();
                SetProjCenter(1.5f,1.5f,true);
            }
            else
            {
                SetProjFly();
                if ( !IsRoting&&Vector2.Distance(Projectile.Center
                    , p.Center) < 16)ContinueOrSuicide();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var A = lightColor.A;
            c.A = A;
            var t = MyUtils.GetT2DValue(Texture);
            Main.EntitySpriteDraw(t, Projectile.Center - Main.screenPosition, MyUtils.GetRec(t),
              c , Projectile.rotation.JudgeSwordRot(), MyUtils.GetOrig(t), Scale,SpriteEffects.None);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            //var HI = p.HeldItem;
            //if (HI.ModItem is firstSword fs)
            //{
            //    fs.ProjType = type;
            //}
        }
        void RotateSword(bool One =false)
        {
            if (HasRot < Projectile.ai[1])
            {
                p.direction = dirNOW;
                HasRot += rotSpeed;
                var ADDdirection = One ? 1 : -1;
                Projectile.rotation += ADDdirection * dir * rotSpeed.Mutiply1Deg();
            }
            else ContinueOrSuicide();
            
        }
        void SetProjCenter(float XScale = 1, float YScale = 1,bool Stab=false)
        {
            Projectile.velocity = Vector2.Zero;
            p.SetCompositeArmFront(true,Player.CompositeArmStretchAmount.Full, Projectile.rotation - (90).Int2Rad());
            HandCenter = p.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2);
            HandCenter.Y += p.gfxOffY;
            Scale =(Projectile.rotation-ZeroDeg).Rot2Oval(XScale,YScale,ZeroDeg).Length()*Projectile.scale;
            var Center = HandCenter + (16*Scale).Float2Hypotenuse().Float2V2X().RotatedBy(
                Projectile.rotation);
            if (Stab)
            {
                Projectile.Center = Center + StabTimer * ProjFlyDir * .25f*MutiplySpeed;
            }
            else Projectile.Center = Center;
        }
        void SetProjFly()
        {
            IsRoting = true;
            if (!Projectile.velocity.Is2_V2Contrary(ProjFlyDir)&&!HasChange) Projectile.velocity -= .02f * ProjFlyDir;
            else
            {
                HasChange = true;
                if (RotTimer < RotTimerMax)
                {
                    RotTimer++;
                    Projectile.velocity = (p.GetModPlayer<MP>().MyMouse - Projectile.Center).SafeNormalize(Vector2.UnitX) * 9;
                }
                else
                {
                    IsRoting = false;
                    Projectile.velocity = Vector2.Zero;

                    Projectile.Center = Vector2.Lerp(Projectile.Center, p.Center, .15f);
                }
            }
            p.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.Center-p.Center).SafeNormalize
                (Vector2.UnitX).ToRotation() -(90).Int2Rad());
            Projectile.rotation += RotDir * rotSpeed/5;
            Scale = Projectile.scale*1.5f;
        }
        void SetIntialDeg()
        {
            ZeroDeg = p2Mouse.ToRotation();
            Projectile.rotation =  ZeroDeg+(dir * degIntitial[type]).Int2Rad();
            HasRot = 
            StabTimer = 
            RotTimer = 0;
            StabTiees = 0;
            HasChange = false;
        }
        void DoSth(bool newone=false)
        {
            SetIntialDeg();
            if(!newone)SoundEngine.PlaySound(SoundID.Item1);
            if (type == 0 || type == 1)
            {
                Projectile.ai[1] = deg2Swing[type];
                dirNOW = dir;
                if (type == 0) Projectile.damage = 3 * dmg;
                else
                {
                    Projectile.damage = dmg;
                    Projectile.knockBack = 4 * kb;
                }
            }
            else
            {
                ProjFlyDir = Projectile.rotation.ToRotationVector2();
                if (type == 2)
                {
                    Projectile.velocity = ProjFlyDir * 15;
                    RotDir =Math.Sign(p2Mouse.X);
                    Projectile.knockBack = kb;
                    Projectile.damage = 5 * dmg;
                }
                else
                {
                    Projectile.damage = dmg;
                }

            }
        }
        void ContinueOrSuicide()
        {
            Projectile.ai[0]++;
            Projectile.ai[0] %= 4;
            if(CanShoot)Projectile.NewProjectile(p.GetSource_ItemUse(p.HeldItem), p.Center, p2Mouse * 5, ModContent.ProjectileType<Class1>(), dmg, 1, p.whoAmI);
            if (!p.channel) Projectile.Kill();
            else
            {
                DoSth();
            }
        }
    }
}
