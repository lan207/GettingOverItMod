using GOIWBF4.BasedOn;
using GOIWBF4.Buffs;
using GOIWBF4.Items;
using GOIWBF4.ModAndOther;
using GOIWBF4.ModConfigs;
using GOIWBF4.ModsAndOther;
using GOIWBF4.Mount;
using GOIWBF4.Proj;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GOIWBF4.Players
{
    
    public class MP:ModPlayer
    {
        Player p => Player;
        float headRot,targetRot,bodyRot,RotationOffsetScale;
        public Vector2 previewCenter,MyMouse;
        public HammerConfig config;
        public List<Vector2> pastVectors = new();
        int Timer;
        public override void Initialize()
        {
            On_Player.AddBuff += JarsHammerAdd;
            config = ModContent.GetInstance<HammerConfig>();
        }
        public override void UpdateDead()
        {
            if (pastVectors.Count>0) pastVectors.Clear();
            if (p.HasBuff<JarBuff2>()) p.ClearBuff(ModContent.BuffType<JarBuff2>());
        }
        public override void PreUpdate()
        {
            if (p.dead)
            {
                
                return;
            }
            KeyBindSystem.Press();
            p.fullRotationOrigin = 11.Int2V2X() + 22.Int2V2Y();
            if (Main.myPlayer == p.whoAmI)
            {
                MyMouse = Main.MouseWorld;
            }
            if(p.lavaWet||p.wet||p.honeyWet)
            {
                if (p.HasBuff<JarBuff2>()&&p.lavaWet) p.AddBuff(BuffID.Burning, 20);
                if(!p.controlDown&&p.HasBuff<JarBuff2>())
                {
                    p.velocity.Y += -1f;
                }
                
                
            }
        }
        public override void SyncPlayer(int toWho,int fromWho,bool newPlayer)
        {
            var packet = Mod.GetPacket();
            packet.Write((byte)MsgType.Player);
            packet.Write((byte)Player.whoAmI);
            packet.WriteVector2(MyMouse);
            //packet.Write(bodyRot);
            packet.Send(toWho,fromWho);
        }
        public void RecieveSync(BinaryReader reader)
        {
            MyMouse = reader.ReadVector2();
            //bodyRot = reader.ReadSingle();
        }
        public override void CopyClientState(ModPlayer targetCopy)
        {
            var clone = (MP)targetCopy;
            clone.MyMouse = MyMouse;
            //clone.bodyRot = bodyRot;
            
        }
        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            var clone = (MP)clientPlayer;
            if (MyMouse != clone.MyMouse)
                SyncPlayer(-1, Main.myPlayer, false);
        }
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {

            if (p.sleeping.isSleeping) headRot = 0;
            else
            {
                var Mouse = Main.gameMenu ? Main.MouseScreen : MyMouse;
                var Center = Main.gameMenu ?previewCenter : p.Center;
                var Head = Center - (p.height / 2 - 10).Int2V2Y();
                //0为一四象限分界，+为顺时针
                var dir = (Mouse - Head).SafeNormalize(Vector2.UnitX);
                var rad = (p.direction * dir).ToRotation();
                targetRot = Math.Clamp(rad, MathHelper.ToRadians(-60), MathHelper.ToRadians(60));
                headRot = MathHelper.Lerp(headRot, targetRot, .25f);
                var dirX = Math.Sign(dir.X);
                if (p.direction != dirX) p.direction = dirX;
            }
            if (p.HasBuff<JarBuff2>()&&!Main.gameMenu)drawInfo.Position -= 23.Int2V2Y();
            
            if(drawInfo.drawPlayer.headRotation!=headRot) drawInfo.drawPlayer.headRotation = headRot;
            
        }
        public override void PostUpdate()
        {
            if (p.HasBuff<JarBuff2>()&&!p.lavaWet) p.ClearBuff(BuffID.Burning);
            if (p.sleeping.isSleeping)
            {
                return;
            }
            
            if (Timer <= 0)
            {
                if (pastVectors.Count==0||p.position != pastVectors[pastVectors.Count - 1])
                {
                    Timer = config.PastPositionRecordCooldown * 60;
                    pastVectors.Add(p.position);
                }
                while (pastVectors.Count > config.PastPositionRecordMaxTimes&&pastVectors.Count>0&&config.PastPositionRecordMaxTimes>0)
                {
                    pastVectors.RemoveAt(0);
                }
            }
            else Timer--;
            if (RotationOffsetScale != 0f )
            {
                float movementRotation;

                if (p.velocity.Y==0)
                {
                    movementRotation = p.velocity.X * (p.velocity.X < Main.MouseWorld.X ? 1f : -1f) * 0.075f;
                }
                else
                {
                    var lim = 1f;
                    movementRotation = MathHelper.Clamp(p.velocity.Y * Math.Sign(p.velocity.X) * -0.015f, -lim, lim);
                }

                if (p.mount.Active)
                {
                    movementRotation *= 0.5f;
                }

                bodyRot += movementRotation;

                //TODO: If swimming, multiply by 4.
            }

            /*
            while (bodyRot >= MathHelper.TwoPi) {
                bodyRot -= MathHelper.TwoPi;
            }

            while (bodyRot <= -MathHelper.TwoPi) {
                bodyRot += MathHelper.TwoPi;
            }
            */

            p.fullRotation = bodyRot * p.gravDir;

            bodyRot = 0f;
            RotationOffsetScale = 1f;
        }
        public override void OnEnterWorld()
        {
            if (p.HasBuff<JarBuff2>()) p.ClearBuff(ModContent.BuffType<JarBuff2>());
            if (p.name == "Bannett Foddy" || p.name == "BannettFoddy")
            {
                var item = ModContent.ItemType<Jar2>();
                if (!p.HasItem(item)) p.QuickSpawnItem(null, item);
            }
        }
        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {

            if (p.name == "Bannnett Foddy" || p.name == "BannettFoddy") return new Item[]
            {
                new Item(ModContent.ItemType<Jar2>()),new Item(ModContent.ItemType<firstSword>())
            };
            return base.AddStartingItems(mediumCoreDeath);
        }
        public override void SetControls()
        {
           if(p.HasBuff<JarBuff2>())
           {
                p.controlMount = false;
                p.controlHook=
                p.controlLeft =
                p.controlUp   =
                p.controlRight=
                p.controlMount =
                p.controlJump = false;
               
           }
        }
        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
           if(cooldownCounter==0&& p.HasBuff<JarBuff2>())
           {
              return true;
           }
           return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
        }
        public void SpawnHammerAndJar(Player self)
        {
            Projectile.NewProjectile(self.GetSource_FromThis(), self.Center, Vector2.Zero,
                ModContent.ProjectileType<JarMount2>(), 0, 0, self.whoAmI);
            Projectile.NewProjectile(self.GetSource_FromThis(), self.Center, Vector2.Zero,
            ModContent.ProjectileType<Hammer>(), 0, 0, self.whoAmI);
        }
        private void JarsHammerAdd(On_Player.orig_AddBuff orig, Player self, int type, int timeToAdd, bool quiet, bool foodHack)
        {
            if(type!=BuffID.Bleeding||!self.HasBuff<JarBuff2>())orig?.Invoke(self,type,timeToAdd, quiet, foodHack);
            if (type == ModContent.BuffType<JarBuff2>())
            {
                SpawnHammerAndJar(self);
            }
        }
        public void SetBackPos()
        {
            Color co;
            if (pastVectors.Count > 0)
            {
                co = Color.Yellow;
                p.Teleport(pastVectors[pastVectors.Count - 1],TeleportationStyleID.DemonConch);
                Main.NewText(p.position,co);
                pastVectors.RemoveAt(pastVectors.Count - 1);
                Timer = config.PastPositionRecordCooldown * 60;
            }
            else co = Color.Red;
            Main.NewText(MyUtils.Translation("剩余存档点数量","Number of RemainingSavedPoint", 
                "Количество оставшихся точек архива") +":"+pastVectors.Count,co);
        }
    }
}
