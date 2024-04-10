﻿using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using ReLogic.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.UI;
using Terraria.WorldBuilding;
using GOIWBF4.BasedOn;

namespace GOIWBF4.BasedOn
{
    internal static class MyUtils
    {
        #region System
        /// <summary>
        /// 将 Item 数组的信息写入指定路径的文件中
        /// </summary>
        /// <param name="items">要导出的 Item 数组</param>
        /// <param name="path">写入文件的路径，默认为 "D:\\模组资源\\AAModPrivate\\input.cs"</param>
        public static void ExportItemTypesToFile(Item[] items, string path = "D:\\模组资源\\AAModPrivate\\input.cs")
        {
            try
            {
                int columnIndex = 0;
                using StreamWriter sw = new(path);
                sw.Write("string[] fullItems = new string[] {");
                foreach (Item item in items)
                {
                    columnIndex++;
                    // 根据是否有 ModItem 决定写入的内容
                    string itemInfo = item.ModItem == null ? $"\"{item.type}\"" : $"\"{item.ModItem.FullName}\"";
                    sw.Write(itemInfo);
                    sw.Write(", ");
                    // 每行最多写入9个元素，然后换行
                    if (columnIndex >= 9)
                    {
                        sw.WriteLine();
                        columnIndex = 0;
                    }
                }
                sw.Write("};");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"UnauthorizedAccessException: 无法访问文件路径 '{path}'. 权限不足");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine($"DirectoryNotFoundException: 文件路径 '{path}' 中的目录不存在");
            }
            catch (PathTooLongException)
            {
                Console.WriteLine($"PathTooLongException: 文件路径 '{path}' 太长");
            }
            catch (IOException)
            {
                Console.WriteLine($"IOException: 无法打开文件 '{path}' 进行写入");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }
        }

        public static int GetTileDorp(Tile tile)
        {
            int stye = TileObjectData.GetTileStyle(tile);
            if (stye == -1)
            {
                stye = 0;
            }

            return TileLoader.GetItemDropFromTypeAndStyle(tile.TileType, stye);
        }

        public static Player InPosFindPlayer(Vector2 position, int maxRange = 3000)
        {
            foreach (Player player in Main.player)
            {
                if (!player.Alives())
                {
                    continue;
                }
                if (maxRange == -1)
                {
                    return player;
                }
                int distance = (int)player.position.To(position).Length();
                if (distance < maxRange)
                {
                    return player;
                }
            }
            return null;
        }

        public static Player TileFindPlayer(int i, int j)
        {
            return InPosFindPlayer(new Vector2(i, j) * 16, 9999);
        }

        public static Chest FindNearestChest(int x, int y)
        {
            int distance = 99999;
            Chest nearestChest = null;

            for (int c = 0; c < Main.chest.Length; c++)
            {
                Chest currentChest = Main.chest[c];
                if (currentChest != null)
                {
                    int length = (int)Math.Sqrt(Math.Pow(x - currentChest.x, 2) + Math.Pow(y - currentChest.y, 2));
                    if (length < distance)
                    {
                        nearestChest = currentChest;
                        distance = length;
                    }
                }
            }
            return nearestChest;
        }

        public static void AddItem(this Chest chest, Item item)
        {
            Item infoItem = item.Clone();
            for (int i = 0; i < chest.item.Length; i++)
            {
                if (chest.item[i] == null)
                {
                    chest.item[i] = new Item();
                }
                if (chest.item[i].type == ItemID.None)
                {
                    chest.item[i] = infoItem;
                    return;
                }
                if (chest.item[i].type == item.type)
                {
                    chest.item[i].stack += infoItem.stack;
                    return;
                }
            }
        }

        public static Color[] GetColorDate(Texture2D tex)
        {
            Color[] colors = new Color[tex.Width * tex.Height];
            tex.GetData(colors);
            List<Color> nonTransparentColors = new();
            foreach (Color color in colors)
            {
                if ((color.A > 0 || color.R > 0 || color.G > 0 || color.B > 0) && color != Color.White && color != Color.Black)
                {
                    nonTransparentColors.Add(color);
                }
            }
            return nonTransparentColors.ToArray();
        }

        public static List<Type> GetSubclasses(Type baseType)
        {
            List<Type> subclasses = new();

            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] allTypes = assembly.GetTypes();

            foreach (Type type in allTypes)
            {
                if (type.IsClass && !type.IsAbstract && baseType.IsAssignableFrom(type))
                {
                    subclasses.Add(type);
                }
            }

            return subclasses;
        }
        #endregion 

        #region AIUtils

        #region 工具部分

        public const float atoR = MathHelper.Pi / 180;

        public static float AtoR(this float num)
        {
            return num * atoR;
        }

        public static float RtoA(this float num)
        {
            return num / atoR;
        }

        /// <summary>
        /// 获取一个实体真正的中心位置,该结果被实体碰撞箱的长宽影响
        /// </summary>
        public static Vector2 GetEntityCenter(Entity entity)
        {
            Vector2 vector2 = new(entity.width * 0.5f, entity.height * 0.5f);
            return entity.position + vector2;
        }

        /// <summary>
        /// 获取生成源
        /// </summary>
        public static EntitySource_Parent parent(this Entity entity)
        {
            return new EntitySource_Parent(entity);
        }

        public static Vector2 InPosMoveTowards(this Vector2 currentPosition, Vector2 targetPosition, float maxAmountAllowedToMove)
        {
            Vector2 v = targetPosition - currentPosition;
            return v.Length() < maxAmountAllowedToMove
                ? targetPosition
                : currentPosition + v.SafeNormalize(Vector2.Zero) * maxAmountAllowedToMove;
        }

        /// <summary>
        /// 判断是否发生对视
        /// </summary>
        public static bool NPCVisualJudgement(Entity targetPlayer, Entity npc)
        {
            Vector2 Perspective = Main.MouseWorld - GetEntityCenter(targetPlayer);
            Vector2 Perspective2 = GetEntityCenter(npc) - GetEntityCenter(targetPlayer);
            Vector2 Perspective3 = Perspective - Perspective2;

            bool DistanceJudgment = Perspective2.LengthSquared() <= 1600 * 1600;
            bool PositioningJudgment = targetPlayer.position.X > npc.position.X;
            bool DirectionJudgment = targetPlayer.direction > npc.direction;
            bool FacingJudgment = (PositioningJudgment == true && DirectionJudgment == false || PositioningJudgment == false && DirectionJudgment == true) && targetPlayer.direction != npc.direction;
            bool PerspectiveJudgment = Perspective3.LengthSquared() <= Perspective2.LengthSquared() * 0.5f;
            return PerspectiveJudgment && FacingJudgment && DistanceJudgment;
        }

        /// <summary>
        /// 在指定位置施加吸引力或斥力，影响附近的NPC、投掷物、灰尘和物品
        /// </summary>
        /// <param name="position">施加效果的位置</param>
        /// <param name="range">影响范围的半径</param>
        /// <param name="strength">力的强度</param>
        /// <param name="projectiles">是否影响投掷物</param>
        /// <param name="magicOnly">是否仅影响魔法投掷物</param>
        /// <param name="npcs">是否影响NPC</param>
        /// <param name="items">是否影响物品</param>
        /// <param name="slow">力的影响程度，1 表示不受影响</param>
        public static void ForceFieldEffect(Vector2 position, int range, float strength, bool projectiles = true, bool magicOnly = false, bool npcs = true, bool items = true, float slow = 1.0f)
        {
            int rangeSquared = range * range;

            // 影响NPC
            if (npcs)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active)
                    {
                        int dist = (int)Vector2.DistanceSquared(npc.Center, position);
                        if (dist < rangeSquared)
                        {
                            npc.velocity *= slow;
                            npc.velocity += Vector2.Normalize(position - npc.Center) * strength;
                            _ = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Shadowflame, 0, 0, 0, default);
                        }
                    }
                }
            }

            // 影响投掷物
            if (projectiles)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    if (projectile.active && (!magicOnly || projectile.DamageType == DamageClass.Magic && projectile.friendly && !projectile.hostile))
                    {
                        int dist = (int)Vector2.DistanceSquared(projectile.Center, position);
                        if (dist < rangeSquared)
                        {
                            projectile.velocity *= slow;
                            projectile.velocity += Vector2.Normalize(position - projectile.Center) * strength;
                            _ = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Shadowflame, 0, 0, 0, default);
                        }
                    }
                }
            }

            // 影响灰尘
            for (int i = 0; i < Main.maxDust; i++)
            {
                Dust dust = Main.dust[i];
                if (dust.active)
                {
                    int dist = (int)Vector2.DistanceSquared(dust.position, position);
                    if (dist < rangeSquared)
                    {
                        dust.velocity *= slow;
                        dust.velocity += Vector2.Normalize(position - dust.position) * strength;
                    }
                }
            }

            // 影响物品
            if (items)
            {
                for (int i = 0; i < Main.maxItems; i++)
                {
                    Item item = Main.item[i];
                    if (item.active)
                    {
                        int dist = (int)Vector2.DistanceSquared(item.Center, position);
                        if (dist < rangeSquared)
                        {
                            item.velocity *= slow;
                            item.velocity += Vector2.Normalize(position - item.Center) * strength;
                            _ = Dust.NewDust(item.position, item.width, item.height, DustID.Shadowflame, 0, 0, 0, default);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 世界实体坐标转物块坐标
        /// </summary>
        /// <param name="wePos"></param>
        /// <returns></returns>
        public static Vector2 WEPosToTilePos(Vector2 wePos)
        {
            int tilePosX = (int)(wePos.X / 16f);
            int tilePosY = (int)(wePos.Y / 16f);
            Vector2 tilePos = new(tilePosX, tilePosY);
            tilePos = PTransgressionTile(tilePos);
            return tilePos;
        }

        /// <summary>
        /// 物块坐标转世界实体坐标
        /// </summary>
        /// <param name="tilePos"></param>
        /// <returns></returns>
        public static Vector2 TilePosToWEPos(Vector2 tilePos)
        {
            float wePosX = (float)(tilePos.X * 16f);
            float wePosY = (float)(tilePos.Y * 16f);

            return new Vector2(wePosX, wePosY);
        }

        /// <summary>
        /// 计算一个渐进速度值
        /// </summary>
        /// <param name="thisCenter">本体位置</param>
        /// <param name="targetCenter">目标位置</param>
        /// <param name="speed">速度</param>
        /// <param name="shutdownDistance">停摆范围</param>
        /// <returns></returns>
        public static float AsymptoticVelocity(Vector2 thisCenter, Vector2 targetCenter, float speed, float shutdownDistance)
        {
            Vector2 toMou = targetCenter - thisCenter;
            float thisSpeed = toMou.LengthSquared() > shutdownDistance * shutdownDistance ? speed : MathHelper.Min(speed, toMou.Length());
            return thisSpeed;
        }

        /// <summary>
        /// 进行圆形的碰撞检测
        /// </summary>
        /// <param name="centerPosition">中心点</param>
        /// <param name="radius">半径</param>
        /// <param name="targetHitbox">碰撞对象的箱体结构</param>
        /// <returns></returns>
        public static bool CircularHitboxCollision(Vector2 centerPosition, float radius, Rectangle targetHitbox)
        {
            if (new Rectangle((int)centerPosition.X, (int)centerPosition.Y, 1, 1).Intersects(targetHitbox))
            {
                return true;
            }

            float distanceToTopLeft = Vector2.Distance(centerPosition, targetHitbox.TopLeft());
            float distanceToTopRight = Vector2.Distance(centerPosition, targetHitbox.TopRight());
            float distanceToBottomLeft = Vector2.Distance(centerPosition, targetHitbox.BottomLeft());
            float distanceToBottomRight = Vector2.Distance(centerPosition, targetHitbox.BottomRight());
            float closestDistance = distanceToTopLeft;

            if (distanceToTopRight < closestDistance)
            {
                closestDistance = distanceToTopRight;
            }

            if (distanceToBottomLeft < closestDistance)
            {
                closestDistance = distanceToBottomLeft;
            }

            if (distanceToBottomRight < closestDistance)
            {
                closestDistance = distanceToBottomRight;
            }

            return closestDistance <= radius;
        }

        /// <summary>
        /// 检测玩家是否有效且正常存活
        /// </summary>
        /// <returns>返回 true 表示活跃，返回 false 表示为空或者已经死亡的非活跃状态</returns>
        public static bool Alives(this Player player)
        {
            return player != null && player.active && !player.dead;
        }


        /// <summary>
        /// 检测弹幕是否有效且正常存活
        /// </summary>
        /// <returns>返回 true 表示活跃，返回 false 表示为空或者已经死亡的非活跃状态</returns>
        public static bool Alives(this Projectile projectile)
        {
            return projectile != null && projectile.active && projectile.timeLeft > 0;
        }

        /// <summary>
        /// 检测NPC是否有效且正常存活
        /// </summary>
        /// <returns>返回 true 表示活跃，返回 false 表示为空或者已经死亡的非活跃状态</returns>
        public static bool Alives(this NPC npc)
        {
            return npc != null && npc.active && npc.timeLeft > 0;
        }

        public static bool AlivesByNPC<T>(this ModNPC npc) where T : ModNPC
        {
            if (npc == null)
            {
                return false;
            }

            return npc.NPC.Alives() && npc.NPC.type == ModContent.NPCType<T>();
        }

        /// <summary>
        /// 根据索引返回在player域中的player实例，同时考虑合法性校验
        /// </summary>
        /// <returns>当获取值非法时将返回 <see cref="null"/> </returns>
        public static Player GetPlayerInstance(int playerIndex)
        {
            if (playerIndex.ValidateIndex(Main.player))
            {
                Player player = Main.player[playerIndex];

                return player.Alives() ? player : null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据索引返回在npc域中的npc实例，同时考虑合法性校验
        /// </summary>
        /// <returns>当获取值非法时将返回 <see cref="null"/> </returns>
        public static NPC GetNPCInstance(int npcIndex)
        {
            if (npcIndex.ValidateIndex(Main.npc))
            {
                NPC npc = Main.npc[npcIndex];

                return npc.Alives() ? npc : null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据索引返回在projectile域中的Projectile实例，同时考虑合法性校验
        /// </summary>
        /// <returns>当获取值非法时将返回 <see cref="null"/> </returns>
        public static Projectile GetProjectileInstance(int projectileIndex)
        {
            if (projectileIndex.ValidateIndex(Main.projectile))
            {
                Projectile proj = Main.projectile[projectileIndex];

                return proj.Alives() ? proj : null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 返回该NPC的生命比例
        /// </summary>
        public static float NPCLifeRatio(NPC npc)
        {
            return npc.life / (float)npc.lifeMax;
        }

        /// <summary>
        /// 根据难度返回相应的血量数值
        /// </summary>
        public static int ConvenientBossHealth(int normalHealth, int expertHealth, int masterHealth)
        {
            if (Main.expertMode)
            {
                return expertHealth;
            }

            return Main.masterMode ? masterHealth : normalHealth;
        }
        /// <summary>
        /// 根据难度返回相应的伤害数值
        /// </summary>
        public static int ConvenientBossDamage(int normalDamage, int expertDamage, int masterDamage)
        {
            if (Main.expertMode)
            {
                return expertDamage;
            }

            return Main.masterMode ? masterDamage : normalDamage;
        }

        private static readonly object listLock = new();

        /// <summary>
        /// 用于处理NPC的局部集合加载问题
        /// </summary>
        /// <param name="Lists">这个NPC专属的局部集合</param>
        /// <param name="npc">NPC本身</param>
        /// <param name="NPCindexes">NPC的局部索引值</param>
        public static void LoadList(ref List<int> Lists, NPC npc, ref int NPCindexes)
        {
            ListUnNoAction(Lists, 0);//每次添加新元素时都将清理一次目标集合

            lock (listLock)
            {
                Lists.AddOrReplace(npc.whoAmI);
                NPCindexes = Lists.IndexOf(npc.whoAmI);
            }
        }

        /// <summary>
        /// 用于处理弹幕的局部集合加载问题
        /// </summary>
        /// <param name="Lists">这个弹幕专属的局部集合</param>
        /// <param name="projectile">弹幕本身</param>
        /// <param name="returnProJindex">弹幕的局部索引值</param>
        public static void LoadList(ref List<int> Lists, Projectile projectile, ref int returnProJindex)
        {
            ListUnNoAction(Lists, 1);

            lock (listLock)
            {
                Lists.AddOrReplace(projectile.whoAmI);
                returnProJindex = Lists.IndexOf(projectile.whoAmI);
            }
        }

        /// <summary>
        /// 用于处理NPC局部集合的善后工作，通常在NPC死亡或者无效化时调用，与 LoadList 配合使用
        /// </summary>
        public static void UnLoadList(ref List<int> Lists, NPC npc, ref int NPCindexes)
        {
            if (NPCindexes >= 0 && NPCindexes < Lists.Count)
            {
                Lists[NPCindexes] = -1;
            }
            else
            {
                npc.active = false;
                ListUnNoAction(Lists, 0);
            }
        }

        /// <summary>
        /// 用于处理弹幕局部集合的善后工作，通常在弹幕死亡或者无效化时调用，与 LoadList 配合使用
        /// </summary>
        public static void UnLoadList(ref List<int> Lists, Projectile projectile, ref int ProJindexes)
        {
            if (ProJindexes >= 0 && ProJindexes < Lists.Count)
            {
                Lists[ProJindexes] = -1;
            }
            else
            {
                projectile.active = false;
                ListUnNoAction(Lists, 1);
            }
        }

        /// <summary>
        /// 将非活跃的实体剔除出局部集合，该方法会影响到原集合
        /// </summary>
        /// <param name="Thislist">传入的局部集合</param>
        /// <param name="funcInt">处理对象，0将处理NPC，1将处理弹幕</param>
        public static void ListUnNoAction(List<int> Thislist, int funcInt)
        {
            List<int> list = Thislist.GetIntList();

            if (funcInt == 0)
            {
                foreach (int e in list)
                {
                    NPC npc = Main.npc[e];
                    int index = Thislist.IndexOf(e);

                    if (npc == null)
                    {
                        Thislist[index] = -1;
                        continue;
                    }

                    if (npc.active == false)
                    {
                        Thislist[index] = -1;
                    }
                }
            }
            if (funcInt == 1)
            {
                foreach (int e in list)
                {
                    Projectile proj = Main.projectile[e];
                    int index = Thislist.IndexOf(e);

                    if (proj == null)
                    {
                        Thislist[index] = -1;
                        continue;
                    }

                    if (proj.active == false)
                    {
                        Thislist[index] = -1;
                    }
                }
            }
        }

        /// <summary>
        /// 获取一个干净且无非活跃成员的集合，该方法不会直接影响原集合
        /// </summary>
        /// <param name="ThisList">传入的局部集合</param>
        /// <param name="funcInt">处理对象，0将处理NPC，非0值将处理弹幕</param>
        /// <param name="valueToReplace">决定排除对象，默认排除-1值元素</param>
        /// <returns></returns>
        public static List<int> GetListOnACtion(List<int> ThisList, int funcInt, int valueToReplace = -1)
        {
            List<int> list = ThisList.GetIntList();

            if (funcInt == 0)
            {
                foreach (int e in list)
                {
                    NPC npc = Main.npc[e];
                    int index = list.IndexOf(e);

                    if (npc == null)
                    {
                        list[index] = -1;
                        continue;
                    }

                    if (npc.active == false)
                    {
                        list[index] = -1;
                    }
                }

                return list.GetIntList();
            }
            else
            {
                foreach (int e in list)
                {
                    Projectile proj = Main.projectile[e];
                    int index = list.IndexOf(e);

                    if (proj == null)
                    {
                        list[index] = -1;
                        continue;
                    }

                    if (proj.active == false)
                    {
                        list[index] = -1;
                    }
                }

                return list.GetIntList();
            }
        }

        /// <summary>
        /// 获取鞭类弹幕的路径点集
        /// </summary>
        public static List<Vector2> GetWhipControlPoints(this Projectile projectile)
        {
            List<Vector2> list = new();
            Projectile.FillWhipControlPoints(projectile, list);
            return list;
        }

        #endregion

        #region 行为部分

        /// <summary>
        /// 让弹幕进行爆炸效果的操作
        /// </summary>
        /// <param name="projectile">要爆炸的投射物</param>
        /// <param name="blastRadius">爆炸效果的半径（默认为 120 单位）</param>
        /// <param name="explosionSound">爆炸声音的样式（默认为默认的爆炸声音）</param>
        public static void Explode(this Projectile projectile, int blastRadius = 120, SoundStyle explosionSound = default, bool spanSound = true)
        {
            Vector2 originalPosition = projectile.position;
            int originalWidth = projectile.width;
            int originalHeight = projectile.height;

            if (spanSound)
            {
                _ = SoundEngine.PlaySound(explosionSound == default ? SoundID.Item14 : explosionSound, projectile.Center);
            }

            projectile.position = projectile.Center;
            projectile.width = projectile.height = blastRadius * 2;
            projectile.position.X -= projectile.width / 2;
            projectile.position.Y -= projectile.height / 2;

            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;

            projectile.Damage();

            projectile.position = originalPosition;
            projectile.width = originalWidth;
            projectile.height = originalHeight;
        }

        /// <summary>
        /// 用于NPC的寻敌判断，会试图遍历玩家列表寻找最近的有效玩家
        /// </summary>
        /// <param name="NPC">寻找主体</param>
        /// <param name="maxFindingDg">最大搜寻范围，如果值为-1则不开启范围限制</param>
        /// <returns>返回一个玩家实例，如果返回的实例为null，则说明玩家无效或者范围内无有效玩家</returns>
        public static Player NPCFindingPlayerTarget(this Entity NPC, int maxFindingDg)
        {
            Player target = null;

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Player player = Main.player[Main.myPlayer];

                if (maxFindingDg == -1)
                {
                    return player;
                }

                return (NPC.position - player.position).LengthSquared() > maxFindingDg * maxFindingDg ? null : player;
            }
            else
            {
                float MaxFindingDgSquared = maxFindingDg * maxFindingDg;
                for (int i = 0; i < Main.player.Length; i++)
                {
                    Player player = Main.player[i];

                    if (!player.active || player.dead || player.ghost || player == null)
                    {
                        continue;
                    }

                    if (maxFindingDg == -1)
                    {
                        return player;
                    }

                    float TargetDg = (player.Center - NPC.Center).LengthSquared();

                    bool FindingBool = TargetDg < MaxFindingDgSquared;

                    if (!FindingBool)
                    {
                        continue;
                    }

                    MaxFindingDgSquared = TargetDg;
                    target = player;
                }
                return target;
            }
        }

        /// <summary>
        /// 用于弹幕寻找NPC目标的行为
        /// </summary>
        /// <param name="proj">寻找主体</param>
        /// <param name="maxFindingDg">最大搜寻范围，如果值为 <see cref="-1"/> 则不开启范围限制</param>
        /// <returns>返回一个NPC实例，如果返回的实例为 <see cref="null"/> ，则说明NPC无效或者范围内无有效NPC</returns>
        public static NPC ProjFindingNPCTarget(this Projectile proj, int maxFindingDg)
        {
            float MaxFindingDgSquared = maxFindingDg * maxFindingDg;
            NPC target = null;

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.Alives() == false || npc.friendly == true || npc.dontTakeDamage == true)
                {
                    continue;
                }

                float TargetDg = (npc.Center - proj.Center).LengthSquared();
                bool FindingBool = TargetDg < MaxFindingDgSquared;
                if (maxFindingDg == -1)
                {
                    FindingBool = true;
                }

                if (!FindingBool)
                {
                    continue;
                }

                MaxFindingDgSquared = TargetDg;
                target = npc;
            }
            return target;
        }

        /// <summary>
        /// 普通的追逐行为
        /// </summary>
        /// <param name="entity">需要操纵的实体</param>
        /// <param name="TargetCenter">目标地点</param>
        /// <param name="Speed">速度</param>
        /// <param name="ShutdownDistance">停摆距离</param>
        /// <returns></returns>
        public static Vector2 ChasingBehavior(this Entity entity, Vector2 TargetCenter, float Speed, float ShutdownDistance = 16)
        {
            if (entity == null)
            {
                return Vector2.Zero;
            }

            Vector2 ToTarget = TargetCenter - entity.Center;
            Vector2 ToTargetNormalize = ToTarget.SafeNormalize(Vector2.Zero);
            Vector2 speed = ToTargetNormalize * AsymptoticVelocity(entity.Center, TargetCenter, Speed, ShutdownDistance);
            entity.velocity = speed;
            return speed;
        }

        /// <summary>
        /// 更加缓和的追逐行为
        /// </summary>
        /// <param name="entity">需要操纵的实体</param>
        /// <param name="TargetCenter">目标地点</param>
        /// <param name="SpeedUpdates">速度的更新系数</param>
        /// <param name="HomingStrenght">追击力度</param>
        /// <returns></returns>
        public static Vector2 ChasingBehavior2(this Entity entity, Vector2 TargetCenter, float SpeedUpdates = 1, float HomingStrenght = 0.1f)
        {
            float targetAngle = entity.AngleTo(TargetCenter);
            float f = entity.velocity.ToRotation().RotTowards(targetAngle, HomingStrenght);
            Vector2 speed = f.ToRotationVector2() * entity.velocity.Length() * SpeedUpdates;
            entity.velocity = speed;
            return speed;
        }

        /// <summary>
        /// 寻找距离指定位置最近的NPC
        /// </summary>
        /// <param name="origin">开始搜索的位置</param>
        /// <param name="maxDistanceToCheck">搜索NPC的最大距离</param>
        /// <param name="ignoreTiles">在检查障碍物时是否忽略瓦片</param>
        /// <param name="bossPriority">是否优先选择Boss</param>
        /// <returns>距离最近的NPC。</returns>
        public static NPC InPosClosestNPC(this Vector2 origin, float maxDistanceToCheck, bool ignoreTiles = true, bool bossPriority = false)
        {
            NPC closestTarget = null;
            float distance = maxDistanceToCheck;
            if (bossPriority)
            {
                bool bossFound = false;
                for (int index2 = 0; index2 < Main.npc.Length; index2++)
                {
                    if (bossFound && !Main.npc[index2].boss && Main.npc[index2].type != NPCID.WallofFleshEye || !Main.npc[index2].CanBeChasedBy())
                    {
                        continue;
                    }
                    float extraDistance2 = Main.npc[index2].width / 2 + Main.npc[index2].height / 2;
                    bool canHit2 = true;
                    if (extraDistance2 < distance && !ignoreTiles)
                    {
                        canHit2 = Collision.CanHit(origin, 1, 1, Main.npc[index2].Center, 1, 1);
                    }
                    if (Vector2.Distance(origin, Main.npc[index2].Center) < distance + extraDistance2 && canHit2)
                    {
                        if (Main.npc[index2].boss || Main.npc[index2].type == NPCID.WallofFleshEye)
                        {
                            bossFound = true;
                        }
                        distance = Vector2.Distance(origin, Main.npc[index2].Center);
                        closestTarget = Main.npc[index2];
                    }
                }
            }
            else
            {
                for (int index = 0; index < Main.npc.Length; index++)
                {
                    if (Main.npc[index].CanBeChasedBy())
                    {
                        float extraDistance = Main.npc[index].width / 2 + Main.npc[index].height / 2;
                        bool canHit = true;
                        if (extraDistance < distance && !ignoreTiles)
                        {
                            canHit = Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1);
                        }
                        if (Vector2.Distance(origin, Main.npc[index].Center) < distance + extraDistance && canHit)
                        {
                            distance = Vector2.Distance(origin, Main.npc[index].Center);
                            closestTarget = Main.npc[index];
                        }
                    }
                }
            }
            return closestTarget;
        }

        /// <summary>
        /// 从一组NPC中寻找距离指定位置最近的NPC或者玩家拥有的召唤物攻击的目标NPC
        /// </summary>
        /// <param name="origin">开始搜索的位置</param>
        /// <param name="maxDistanceToCheck">搜索NPC的最大距离</param>
        /// <param name="owner">拥有这个召唤物的玩家</param>
        /// <param name="ignoreTiles">在检查障碍物时是否忽略瓦片</param>
        /// <param name="checksRange">是否检查召唤物的攻击范围</param>
        /// <returns>距离最近的NPC</returns>
        public static NPC MinionHoming(this Vector2 origin, float maxDistanceToCheck, Player owner, bool ignoreTiles = true, bool checksRange = false)
        {
            if (owner == null || !owner.whoAmI.ValidateIndex(Main.player.Length) || !owner.MinionAttackTargetNPC.ValidateIndex(Main.maxNPCs))
            {
                return origin.InPosClosestNPC(maxDistanceToCheck, ignoreTiles);
            }
            NPC npc = Main.npc[owner.MinionAttackTargetNPC];
            bool canHit = true;
            if (!ignoreTiles)
            {
                canHit = Collision.CanHit(origin, 1, 1, npc.Center, 1, 1);
            }
            float extraDistance = npc.width / 2 + npc.height / 2;
            bool distCheck = Vector2.Distance(origin, npc.Center) < maxDistanceToCheck + extraDistance || !checksRange;
            return owner.HasMinionAttackTargetNPC && canHit && distCheck ? npc : origin.InPosClosestNPC(maxDistanceToCheck, ignoreTiles);
        }

        /// <summary>
        /// 返回一个合适的渐进追击速度
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="TargetCenter"></param>
        /// <param name="Speed"></param>
        /// <param name="ShutdownDistance"></param>
        /// <returns></returns>
        public static Vector2 GetChasingVelocity(this Entity entity, Vector2 TargetCenter, float Speed, float ShutdownDistance)
        {
            if (entity == null)
            {
                return Vector2.Zero;
            }

            Vector2 ToTarget = TargetCenter - entity.Center;
            Vector2 ToTargetNormalize = ToTarget.SafeNormalize(Vector2.Zero);
            return ToTargetNormalize * AsymptoticVelocity(entity.Center, TargetCenter, Speed, ShutdownDistance);
        }

        /// <summary>
        /// 考虑加速度的追逐行为
        /// </summary>
        /// <param name="entity">需要操纵的实体</param>
        /// <param name="TargetCenter">目标地点</param>
        /// <param name="acceleration">加速度系数</param>
        /// <returns></returns>
        public static void AccelerationBehavior(this Entity entity, Vector2 TargetCenter, float acceleration)
        {
            if (entity.Center.X > TargetCenter.X)
            {
                entity.velocity.X -= acceleration;
            }

            if (entity.Center.X < TargetCenter.X)
            {
                entity.velocity.X += acceleration;
            }

            if (entity.Center.Y > TargetCenter.Y)
            {
                entity.velocity.Y -= acceleration;
            }

            if (entity.Center.Y < TargetCenter.Y)
            {
                entity.velocity.Y += acceleration;
            }
        }

        public static void EntityToRot(this NPC entity, float ToRot, float rotSpeed)
        {
            //entity.rotation = MathHelper.SmoothStep(entity.rotation, ToRot, rotSpeed);

            // 将角度限制在 -π 到 π 的范围内
            entity.rotation = MathHelper.WrapAngle(entity.rotation);

            // 计算差异角度
            float diff = MathHelper.WrapAngle(ToRot - entity.rotation);

            // 选择修改幅度小的方向进行旋转
            if (Math.Abs(diff) < MathHelper.Pi)
            {
                entity.rotation += diff * rotSpeed;
            }
            else
            {
                entity.rotation -= MathHelper.WrapAngle(-diff) * rotSpeed;
            }
        }

        /// <summary>
        /// 处理实体的旋转行为
        /// </summary>
        public static void EntityToRot(this Projectile entity, float ToRot, float rotSpeed)
        {
            //entity.rotation = MathHelper.SmoothStep(entity.rotation, ToRot, rotSpeed);

            // 将角度限制在 -π 到 π 的范围内
            entity.rotation = MathHelper.WrapAngle(entity.rotation);

            // 计算差异角度
            float diff = MathHelper.WrapAngle(ToRot - entity.rotation);

            // 选择修改幅度小的方向进行旋转
            if (Math.Abs(diff) < MathHelper.Pi)
            {
                entity.rotation += diff * rotSpeed;
            }
            else
            {
                entity.rotation -= MathHelper.WrapAngle(-diff) * rotSpeed;
            }
        }

        #endregion

        #endregion

        #region GameUtils
        /// <summary>
        /// 寻找距离指定位置最近的NPC
        /// </summary>
        /// <param name="origin">开始搜索的位置</param>
        /// <param name="maxDistanceToCheck">搜索NPC的最大距离</param>
        /// <param name="ignoreTiles">在检查障碍物时是否忽略瓦片</param>
        /// <param name="bossPriority">是否优先选择Boss</param>
        /// <returns>距离最近的NPC。</returns>
        public static NPC FindClosestNPC(this Vector2 origin, float maxDistanceToCheck, bool ignoreTiles = true, bool bossPriority = false)
        {
            NPC closestTarget = null;
            float distance = maxDistanceToCheck;
            if (bossPriority)
            {
                bool bossFound = false;
                for (int index2 = 0; index2 < Main.npc.Length; index2++)
                {
                    if ((bossFound && !Main.npc[index2].boss && Main.npc[index2].type != NPCID.WallofFleshEye) || !Main.npc[index2].CanBeChasedBy())
                    {
                        continue;
                    }
                    float extraDistance2 = (Main.npc[index2].width / 2) + (Main.npc[index2].height / 2);
                    bool canHit2 = true;
                    if (extraDistance2 < distance && !ignoreTiles)
                    {
                        canHit2 = Collision.CanHit(origin, 1, 1, Main.npc[index2].Center, 1, 1);
                    }
                    if (Vector2.Distance(origin, Main.npc[index2].Center) < distance + extraDistance2 && canHit2)
                    {
                        if (Main.npc[index2].boss || Main.npc[index2].type == NPCID.WallofFleshEye)
                        {
                            bossFound = true;
                        }
                        distance = Vector2.Distance(origin, Main.npc[index2].Center);
                        closestTarget = Main.npc[index2];
                    }
                }
            }
            else
            {
                for (int index = 0; index < Main.npc.Length; index++)
                {
                    if (Main.npc[index].CanBeChasedBy())
                    {
                        float extraDistance = (Main.npc[index].width / 2) + (Main.npc[index].height / 2);
                        bool canHit = true;
                        if (extraDistance < distance && !ignoreTiles)
                        {
                            canHit = Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1);
                        }
                        if (Vector2.Distance(origin, Main.npc[index].Center) < distance + extraDistance && canHit)
                        {
                            distance = Vector2.Distance(origin, Main.npc[index].Center);
                            closestTarget = Main.npc[index];
                        }
                    }
                }
            }
            return closestTarget;
        }

        /// <summary>
        /// 快速从模组本地化文件中设置对应物品的名称
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itemName"></param>
        public static void EasySetLocalTextNameOverride(this Item item, string itemName)
        {
            item.SetNameOverride(Language.GetText($"Mods.CalamityWeaponRemake.Items.{itemName}.DisplayName").Value);
        }

        /// <summary>
        /// 在游戏中发送文本消息
        /// </summary>
        /// <param name="message">要发送的消息文本</param>
        /// <param name="colour">（可选）消息的颜色,默认为 null</param>
        public static void Text(string message, Color? colour = null)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(message, colour);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(message), (Color)(colour == null ? Color.White : colour));
            }
        }

        /// <summary>
        /// 一个根据语言选项返回字符的方法
        /// </summary>
        public static string Translation(string Chinese = null, string English = null, string Russian = null)
        {
            string text;
            if (Language.ActiveCulture.Name == "zh-Hans")
            {
                text = Chinese;
            }
            else
            {
                text = Language.ActiveCulture.Name == "ru-RU" ? Russian : English;
            }

            if (text is null or default(string))
            {
                text = "Invalid Character";
            }

            return text;
        }

        public static Color MultiLerpColor(float percent, params Color[] colors)
        {
            float per = 1f / (colors.Length - 1f);
            float total = per;
            int currentID = 0;
            while (percent / total > 1f && currentID < colors.Length - 2)
            {
                total += per;
                currentID++;
            }
            return Color.Lerp(colors[currentID], colors[currentID + 1], (percent - per * currentID) / per);
        }

        /// <summary>
        /// 快速修改一个物品的简介文本，从模组本地化文本中拉取资源
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="item"></param>
        /// <param name="tooltips"></param>
        /// <param name="key"></param>
        /// <param name="leva"></param>
        public static void OnModifyTooltips(Mod mod, List<TooltipLine> tooltips, string key)
        {
            List<TooltipLine> newTooltips = new(tooltips);
            foreach (TooltipLine line in tooltips.ToList())
            {//复制 tooltips 集合，以便在遍历时修改
                for (int i = 0; i < 9; i++)
                {
                    if (line.Name == "Tooltip" + i)
                    {
                        line.Hide();
                    }
                }
            }

            TooltipLine newLine = new(mod, "CWRText"
                , Language.GetText($"Mods.CalamityWeaponRemake.Items.{key}.Tooltip").Value);
            newTooltips.Add(newLine);

            tooltips.Clear(); // 清空原 tooltips 集合
            tooltips.AddRange(newTooltips); // 添加修改后的 newTooltips 集合
        }

        /// <summary>
        /// 检查指定玩家是否按下了鼠标键
        /// </summary>
        /// <param name="player">要检查的玩家</param>
        /// <param name="leftCed">是否检查左鼠标键，否则检测右鼠标键</param>
        /// <param name="netCed">是否进行网络同步检查</param>
        /// <returns>如果按下了指定的鼠标键，则返回true，否则返回false</returns>
        public static bool PressKey(this Player player, bool leftCed = true, bool netCed = true)
        {
            return (!netCed || Main.myPlayer == player.whoAmI)
&& (leftCed ? PlayerInput.Triggers.Current.MouseLeft : PlayerInput.Triggers.Current.MouseRight);
        }

        /// <summary>
        /// 检查伤害类型是否与指定类型匹配或继承自指定类型
        /// </summary>
        /// <param name="damageClass">要检查的伤害类型</param>
        /// <param name="intendedClass">目标伤害类型</param>
        /// <returns>如果匹配或继承，则为 true；否则为 false</returns>
        public static bool CountsAsClass(this DamageClass damageClass, DamageClass intendedClass)
        {
            return damageClass == intendedClass || damageClass.GetEffectInheritance(intendedClass);
        }

        /// <summary>
        /// 通过玩家ID、弹幕标识和可选的弹幕类型来获取对应的弹幕索引
        /// </summary>
        /// <param name="player">玩家的ID</param>
        /// <param name="projectileIdentity">弹幕的标识</param>
        /// <param name="projectileType">可选的弹幕类型</param>
        /// <returns>找到的弹幕索引，如果未找到则返回 -1</returns>
        public static int GetProjectileByIdentity(int player, int projectileIdentity, params int[] projectileType)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].identity == projectileIdentity && Main.projectile[i].owner == player
                    && (projectileType.Length == 0 || projectileType.Contains(Main.projectile[i].type)))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 创建一个新的召唤物投射物，并返回其索引
        /// </summary>
        /// <param name="source">实体来源</param>
        /// <param name="spawn">生成位置</param>
        /// <param name="velocity">初始速度</param>
        /// <param name="type">投射物类型</param>
        /// <param name="rawBaseDamage">原始基础伤害</param>
        /// <param name="knockback">击退力度</param>
        /// <param name="owner">所有者ID（默认为255）</param>
        /// <param name="ai0">自定义AI参数0（默认为0）</param>
        /// <param name="ai1">自定义AI参数1（默认为0）</param>
        /// <returns>新投射物的索引</returns>
        public static int NewSummonProjectile(IEntitySource source, Vector2 spawn, Vector2 velocity, int type, int rawBaseDamage, float knockback, int owner = 255, float ai0 = 0, float ai1 = 0)
        {
            int projectileIndex = Projectile.NewProjectile(source, spawn, velocity, type, rawBaseDamage, knockback, owner, ai0, ai1);
            if (projectileIndex != Main.maxProjectiles)
            {
                Main.projectile[projectileIndex].originalDamage = rawBaseDamage;
                Main.projectile[projectileIndex].ContinuouslyUpdateDamageStats = true;
            }
            return projectileIndex;
        }

        #region NetUtils

        /// <summary>
        /// 判断是否处于客户端状态，如果是在单人或者服务端下将返回false
        /// </summary>
        public static bool isClient => Main.netMode == NetmodeID.MultiplayerClient;
        /// <summary>
        /// 判断是否处于服务端状态，如果是在单人或者客户端下将返回false
        /// </summary>
        public static bool isServer => Main.netMode == NetmodeID.Server;
        /// <summary>
        /// 仅判断是否处于单人状态，在单人模式下返回true
        /// </summary>
        public static bool isSinglePlayer => Main.netMode == NetmodeID.SinglePlayer;

        /// <summary>
        /// 检查一个 Projectile 对象是否属于当前客户端玩家拥有的，如果是，返回true
        /// </summary>
        public static bool IsOwnedByLocalPlayer(this Projectile projectile)
        {
            return projectile.owner == Main.myPlayer;
        }

        /// <summary>
        /// 同步整个世界状态
        /// </summary>
        public static void SyncWorld()
        {
            if (Main.dedServ)
            {
                NetMessage.SendData(MessageID.WorldData);
            }
        }

        /// <summary>
        /// 将指定数量的元素从二进制读取器读取到列表中，使用提供的读取函数
        /// </summary>
        /// <typeparam name="T">列表中的元素类型</typeparam>
        /// <param name="reader">要读取的二进制读取器</param>
        /// <param name="list">要填充读取元素的列表</param>
        /// <param name="count">要读取的元素数量</param>
        /// <param name="readFunction">用于从二进制读取器中读取类型 T 的元素的函数</param>
        public static void ReadToList<T>(this BinaryReader reader, List<T> list, int count, Func<BinaryReader, T> readFunction)
        {
            list.Clear();
            for (int i = 0; i < count; i++)
            {
                T value = readFunction(reader);
                list.Add(value);
            }
        }

        /// <summary>
        /// 从二进制读取器中读取指定数量的整数到列表中
        /// </summary>
        /// <param name="reader">要读取的二进制读取器</param>
        /// <param name="list">要填充读取整数的列表</param>
        /// <param name="count">要读取的整数数量</param>
        public static void ReadToList(this BinaryReader reader, List<int> list, int count)
        {
            list.Clear();
            for (int i = 0; i < count; i++)
            {
                int value = reader.ReadInt32();
                list.Add(value);
            }
        }

        /// <summary>
        /// 从二进制读取器中读取指定数量的字节到列表中
        /// </summary>
        /// <param name="reader">要读取的二进制读取器</param>
        /// <param name="list">要填充读取字节的列表</param>
        /// <param name="count">要读取的字节数量</param>
        public static void ReadToList(this BinaryReader reader, List<byte> list, int count)
        {
            list.Clear();
            for (int i = 0; i < count; i++)
            {
                byte value = reader.ReadByte();
                list.Add(value);
            }
        }

        /// <summary>
        /// 从二进制读取器中读取指定数量的单精度浮点数到列表中
        /// </summary>
        /// <param name="reader">要读取的二进制读取器</param>
        /// <param name="list">要填充读取单精度浮点数的列表</param>
        /// <param name="count">要读取的单精度浮点数数量</param>
        public static void ReadToList(this BinaryReader reader, List<float> list, int count)
        {
            list.Clear();
            for (int i = 0; i < count; i++)
            {
                float value = reader.ReadSingle();
                list.Add(value);
            }
        }

        /// <summary>
        /// 从二进制读取器中读取指定数量的二维向量到列表中
        /// </summary>
        /// <param name="reader">要读取的二进制读取器</param>
        /// <param name="list">要填充读取二维向量的列表</param>
        /// <param name="count">要读取的二维向量数量</param>
        public static void ReadToList(this BinaryReader reader, List<Vector2> list, int count)
        {
            list.Clear();
            for (int i = 0; i < count; i++)
            {
                float valueX = reader.ReadSingle();
                float valueY = reader.ReadSingle();
                list.Add(new Vector2(valueX, valueY));
            }
        }

        /// <summary>
        /// 将列表中的元素数量和元素内容写入二进制写入器，使用提供的写入函数
        /// </summary>
        /// <typeparam name="T">列表中的元素类型</typeparam>
        /// <param name="writer">要写入的二进制写入器</param>
        /// <param name="list">要写入的列表</param>
        /// <param name="writeFunction">用于将类型 T 的元素写入二进制写入器的函数</param>
        public static void WriteList<T>(this BinaryWriter writer, List<T> list, Action<BinaryWriter, T> writeFunction)
        {
            writer.Write(list.Count);
            foreach (T value in list)
            {
                writeFunction(writer, value);
            }
        }

        /// <summary>
        /// 将整数列表写入二进制写入器
        /// </summary>
        /// <param name="writer">要写入的二进制写入器</param>
        /// <param name="list">要写入的整数列表</param>
        public static void WriteList(this BinaryWriter writer, List<int> list)
        {
            writer.Write(list.Count);
            foreach (int value in list)
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// 将字节列表写入二进制写入器
        /// </summary>
        /// <param name="writer">要写入的二进制写入器</param>
        /// <param name="list">要写入的字节列表</param>
        public static void WriteList(this BinaryWriter writer, List<byte> list)
        {
            writer.Write(list.Count);
            foreach (byte value in list)
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// 将单精度浮点数列表写入二进制写入器
        /// </summary>
        /// <param name="writer">要写入的二进制写入器</param>
        /// <param name="list">要写入的单精度浮点数列表</param>
        public static void WriteList(this BinaryWriter writer, List<float> list)
        {
            writer.Write(list.Count);
            foreach (float value in list)
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// 将二维向量列表写入二进制写入器
        /// </summary>
        /// <param name="writer">要写入的二进制写入器</param>
        /// <param name="list">要写入的二维向量列表</param>
        public static void WriteList(this BinaryWriter writer, List<Vector2> list)
        {
            writer.Write(list.Count);
            foreach (Vector2 value in list)
            {
                writer.Write(value.X);
                writer.Write(value.Y);
            }
        }


        /// <summary>
        /// 生成Boss级实体，考虑网络状态
        /// </summary>
        /// <param name="player">触发生成的玩家实例</param>
        /// <param name="bossType">要生成的 Boss 的类型</param>
        /// <param name="obeyLocalPlayerCheck">是否要遵循本地玩家检查</param>
        public static void SpawnBossNetcoded(Player player, int bossType, bool obeyLocalPlayerCheck = true)
        {

            if (player.whoAmI == Main.myPlayer || !obeyLocalPlayerCheck)
            {
                // 如果使用物品的玩家是客户端
                // （在此明确排除了服务器端）

                _ = SoundEngine.PlaySound(SoundID.Roar, player.position);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // 如果玩家不在多人游戏中，直接生成 Boss
                    NPC.SpawnOnPlayer(player.whoAmI, bossType);
                }
                else
                {
                    // 如果玩家在多人游戏中，请求生成
                    // 仅当 NPCID.Sets.MPAllowedEnemies[type] 为真时才有效，需要在 NPC 代码中设置

                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: bossType);
                }
            }
        }

        /// <summary>
        /// 在易于使用的方式下生成一个新的 NPC，考虑网络状态
        /// </summary>
        /// <param name="source">生成 NPC 的实体源</param>
        /// <param name="spawnPos">生成的位置</param>
        /// <param name="type">NPC 的类型</param>
        /// <param name="start">NPC 的初始状态</param>
        /// <param name="ai0">NPC 的 AI 参数 0</param>
        /// <param name="ai1">NPC 的 AI 参数 1</param>
        /// <param name="ai2">NPC 的 AI 参数 2</param>
        /// <param name="ai3">NPC 的 AI 参数 3</param>
        /// <param name="target">NPC 的目标 ID</param>
        /// <param name="velocity">NPC 的初始速度</param>
        /// <returns>新生成的 NPC 的 ID</returns>
        public static int NewNPCEasy(IEntitySource source, Vector2 spawnPos, int type, int start = 0, float ai0 = 0, float ai1 = 0, float ai2 = 0, float ai3 = 0, int target = 255, Vector2 velocity = default)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return Main.maxNPCs;
            }

            int n = NPC.NewNPC(source, (int)spawnPos.X, (int)spawnPos.Y, type, start, ai0, ai1, ai2, ai3, target);
            if (n != Main.maxNPCs)
            {
                if (velocity != default)
                {
                    Main.npc[n].velocity = velocity;
                }

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                }
            }
            return n;
        }
        #endregion

        #endregion

        #region MathUtils

        public static Random rands = new();

        /// <summary>
        /// 生成一组不重复的随机数集合，数字的数量不能大于取值范围
        /// </summary>
        /// <param name="count">集合元素数量</param>
        /// <param name="minValue">元素最小值</param>
        /// <param name="maxValue">元素最大值</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static List<int> GenerateUniqueNumbers(int count, int minValue, int maxValue)
        {
            if (count > maxValue - minValue + 1)
            {
                throw new ArgumentException("Count of unique numbers cannot be greater than the range of values.");
            }

            List<int> uniqueNumbers = new();
            HashSet<int> usedNumbers = new();

            for (int i = minValue; i <= maxValue; i++)
            {
                _ = usedNumbers.Add(i);
            }

            for (int i = 0; i < count; i++)
            {
                int randomIndex = Main.rand.Next(usedNumbers.Count);
                int randomNumber = usedNumbers.ElementAt(randomIndex);
                _ = usedNumbers.Remove(randomNumber);
                uniqueNumbers.Add(randomNumber);
            }

            return uniqueNumbers;
        }

        public static float RotTowards(this float curAngle, float targetAngle, float maxChange)
        {
            curAngle = MathHelper.WrapAngle(curAngle);
            targetAngle = MathHelper.WrapAngle(targetAngle);
            if (curAngle < targetAngle)
            {
                if (targetAngle - curAngle > (float)Math.PI)
                {
                    curAngle += (float)Math.PI * 2f;
                }
            }
            else if (curAngle - targetAngle > (float)Math.PI)
            {
                curAngle -= (float)Math.PI * 2f;
            }

            curAngle += MathHelper.Clamp(targetAngle - curAngle, 0f - maxChange, maxChange);
            return MathHelper.WrapAngle(curAngle);
        }

        /// <summary>
        /// 比较两个角度之间的差异，将结果限制在 -π 到 π 的范围内
        /// </summary>
        /// <param name="baseAngle">基准角度（参考角度）</param>
        /// <param name="targetAngle">目标角度（待比较角度）</param>
        /// <returns>从基准角度到目标角度的差异，范围在 -π 到 π 之间</returns>
        public static float CompareAngle(float baseAngle, float targetAngle)
        {
            return (baseAngle - targetAngle + (float)Math.PI * 3) % MathHelper.TwoPi - (float)Math.PI;// 计算两个角度之间的差异并将结果限制在 -π 到 π 的范围内
        }

        /// <summary>
        /// 将给定的角度值转换为以 π 为中心的标准化角度
        /// </summary>
        /// <param name="angleIn">输入的角度值</param>
        /// <returns>标准化的角度，以 π 为中心，范围在 -π 到 π 之间</returns>
        public static float ConvertAngle(float angleIn)
        {
            // 将输入角度与零角度比较，以获得标准化的角度
            return CompareAngle(0, angleIn) + (float)Math.PI;
        }

        /// <summary>
        /// 色彩混合
        /// </summary>
        public static Color RecombinationColor(params (Color color, float weight)[] colorWeightPairs)
        {
            Vector4 result = Vector4.Zero;

            for (int i = 0; i < colorWeightPairs.Length; i++)
            {
                result += colorWeightPairs[i].color.ToVector4() * colorWeightPairs[i].weight;
            }

            return new Color(result);
        }

        public static Vector2 To(this Vector2 vr1, Vector2 vr2)
        {
            return vr2 - vr1;
        }

        /// <summary>
        /// 获取一个随机方向的向量
        /// </summary>
        /// <param name="startAngle">开始角度,请输入角度单位的值</param>
        /// <param name="targetAngle">目标角度,请输入角度单位的值</param>
        /// <param name="ModeLength">返回的向量的长度</param>
        /// <returns></returns>
        public static Vector2 GetRandomVevtor(float startAngle, float targetAngle, float ModeLength)
        {
            float angularSeparation = targetAngle - startAngle;
            float randomPosx = (angularSeparation * Main.rand.NextFloat() + startAngle) * (MathHelper.Pi / 180);
            float cosValue = MathF.Cos(randomPosx);
            float sinValue = MathF.Sin(randomPosx);

            return new Vector2(cosValue, sinValue) * ModeLength;
        }

        /// <summary>
        /// 获取一个垂直于该向量的单位向量
        /// </summary>
        public static Vector2 GetNormalVector(this Vector2 vr)
        {
            Vector2 nVr = new(vr.Y, -vr.X);
            return Vector2.Normalize(nVr);
        }

        /// <summary>
        /// 简单安全的获取一个单位向量，如果出现非法情况则会返回 <see cref="Vector2.Zero"/>
        /// </summary>
        public static Vector2 UnitVector(this Vector2 vr)
        {
            return vr.SafeNormalize(Vector2.Zero);
        }

        /// <summary>
        /// 计算两个向量的点积
        /// </summary>
        public static float DotProduct(this Vector2 vr1, Vector2 vr2)
        {
            return vr1.X * vr2.X + vr1.Y * vr2.Y;
        }

        /// <summary>
        /// 计算两个向量的叉积
        /// </summary>
        public static float CrossProduct(this Vector2 vr1, Vector2 vr2)
        {
            return vr1.X * vr2.Y - vr1.Y * vr2.X;
        }

        /// <summary>
        /// 获取向量与另一个向量的夹角
        /// </summary>
        /// <returns>返回劣弧角，即取值范围为 0 到 π 弧度之间</returns>
        public static float VetorialAngle(this Vector2 vr1, Vector2 vr2)
        {
            return (float)Math.Acos(vr1.DotProduct(vr2) / (vr1.Length() * vr2.Length()));
        }

        /// <summary>
        /// 一个随机布尔值获取方法
        /// </summary>
        /// <param name="ProbabilityDenominator">概率分母</param>
        /// <param name="ProbabilityExpectation">期望倾向</param>
        /// <param name="DesiredObject">期望对象</param>
        /// <returns></returns>
        public static bool RandomBooleanValue(int ProbabilityDenominator, int ProbabilityExpectation, bool DesiredObject)
        {
            int randomInt = rands.Next(0, ProbabilityDenominator);
            return randomInt == ProbabilityExpectation && DesiredObject;
        }

        /// <summary>
        /// 根据向量的Y值来进行比较
        /// </summary>
        public class VeYSort : IComparer<Vector2>
        {
            public int Compare(Vector2 v1, Vector2 v2)
            {
                // 比较两个向量的Y值，根据Y值大小进行排序
                if (v1.Y < v2.Y)
                {
                    return -1;
                }
                else
                {
                    return v1.Y > v2.Y ? 1 : 0;
                }
            }
        }

        /// <summary>
        /// 检测索引的合法性
        /// </summary>
        /// <returns>合法将返回 <see cref="true"/></returns>
        public static bool ValidateIndex(this int index, Array array)
        {
            return index >= 0 && index < array.Length;
        }

        /// <summary>
        /// 检测索引的合法性
        /// </summary>
        /// <returns>合法将返回 <see cref="true"/></returns>
        public static bool ValidateIndex(this List<int> ts, int index)
        {
            return index >= 0 && index < ts.Count;
        }

        /// <summary>
        /// 检测索引的合法性
        /// </summary>
        public static bool ValidateIndex(this int index, int cap)
        {
            return index >= 0 && index < cap;
        }

        /// <summary>
        /// 会自动替补-1元素
        /// </summary>
        /// <param name="list">目标集合</param>
        /// <param name="valueToAdd">替换为什么值</param>
        /// <param name="valueToReplace">替换的目标对象的值，不填则默认为-1</param>
        public static void AddOrReplace(this List<int> list, int valueToAdd, int valueToReplace = -1)
        {
            int index = list.IndexOf(valueToReplace);
            if (index >= 0)
            {
                list[index] = valueToAdd;
            }
            else
            {
                list.Add(valueToAdd);
            }
        }

        /// <summary>
        /// 返回一个集合的干净数量，排除数默认为-1，该扩展方法不会影响原集合
        /// </summary>
        public static int GetIntListCount(this List<int> list, int valueToReplace = -1)
        {
            List<int> result = new(list);
            _ = result.RemoveAll(item => item == -1);
            return result.Count;
        }

        /// <summary>
        /// 返回一个集合的筛选副本，排除数默认为-1，该扩展方法不会影响原集合
        /// </summary>
        public static List<int> GetIntList(this List<int> list, int valueToReplace = -1)
        {
            List<int> result = new(list);
            _ = result.RemoveAll(item => item == -1);
            return result;
        }

        /// <summary>
        /// 去除目标集合中所有-1元素
        /// </summary>
        /// <param name="list"></param>
        public static void SweepLoadLists(ref List<int> list)
        {
            int count = list.Count;
            int i = 0;
            while (i < count)
            {
                if (list[i] == -1)
                {
                    list.RemoveAt(i);
                    count--;
                }
                else
                {
                    i++;
                }
            }
        }

        /// <summary>
        /// 单独的重载集合方法
        /// </summary>
        public static void UnLoadList(ref List<int> Lists)
        {
            _ = new List<int>();
        }

        /// <summary>
        /// 将数组克隆出一份List类型
        /// </summary>
        public static List<T> ToList<T>(this T[] array)
        {
            return new List<T>(array);
        }

        /// <summary>
        /// 对float集合进行平滑插值，precision不应该输入0值或者负值
        /// </summary>
        public static List<float> InterpolateFloatList(List<float> originalList, float precision)
        {
            if (precision <= 0)
            {
                precision = 1;
            }

            int precisionCounter = (int)(1f / precision);
            if (precisionCounter < 1)
            {
                precisionCounter = 1;
            }

            List<float> interpolatedList = new();

            for (int i = 0; i < originalList.Count - 1; i++)
            {
                interpolatedList.Add(originalList[i]);

                float currentValue = originalList[i];
                float nextValue = originalList[i + 1];
                float dis = nextValue - currentValue;
                int absDis = (int)Math.Abs(dis);
                int numInterpolations = absDis * precisionCounter;

                for (int j = 1; j <= numInterpolations; j++)
                {
                    float t = j / (float)(numInterpolations + 1);
                    float interpolatedValue = MathHelper.Lerp(currentValue, nextValue, t);
                    interpolatedList.Add(interpolatedValue);
                }
            }

            interpolatedList.Add(originalList[^1]);

            return interpolatedList;
        }

        /// <summary>
        /// 对Vector2集合进行平滑插值，precision不应该输入0值或者负值
        /// </summary>
        public static List<Vector2> InterpolateVectorList(List<Vector2> originalList, float precision = 1)
        {
            if (precision <= 0)
            {
                precision = 1;
            }

            int precisionCounter = Math.Max(1, (int)(1f / precision));

            List<Vector2> interpolatedList = new(originalList.Count * (2 * precisionCounter + 1));

            for (int i = 0; i < originalList.Count - 1; i++)
            {
                interpolatedList.Add(originalList[i]);

                Vector2 currentValue = originalList[i];
                Vector2 nextValue = originalList[i + 1];
                float maxDis = Math.Max(Math.Abs(nextValue.X - currentValue.X), Math.Abs(nextValue.Y - currentValue.Y));
                int numInterpolations = Math.Max(1, (int)(maxDis * precisionCounter));

                for (int j = 1; j <= numInterpolations; j++)
                {
                    float t = j / (float)(numInterpolations + 1);
                    Vector2 interpolatedValue;
                    interpolatedValue.X = MathHelper.Lerp(currentValue.X, nextValue.X, t);
                    interpolatedValue.Y = MathHelper.Lerp(currentValue.Y, nextValue.Y, t);
                    interpolatedList.Add(interpolatedValue);
                }
            }

            interpolatedList.Add(originalList[^1]);

            return interpolatedList;
        }

        /// <summary>
        /// 在给定的 Vector2 列表之间使用贝塞尔曲线进行平滑插值
        /// </summary>
        /// <param name="originalList">原始的 Vector2 列表</param>
        /// <param name="precision">插值精度，不能为零或负数</param>
        /// <returns>包含平滑插值点的新 Vector2 列表</returns>
        public static List<Vector2> InterpolateVectorListWithBezier(List<Vector2> originalList, float precision = 1)
        {
            if (precision <= 0)
            {
                precision = 1;
            }

            int precisionCounter = (int)(1f / precision);
            if (precisionCounter < 1)
            {
                precisionCounter = 1;
            }

            List<Vector2> interpolatedList = new();

            Vector2[] controlPoints = new Vector2[4]; // 用于存储控制点

            for (int i = 0; i < originalList.Count - 1; i++)
            {
                Vector2 startPoint = originalList[i];
                Vector2 endPoint = originalList[i + 1];

                // 创建贝塞尔曲线的控制点，这里使用中点
                Vector2 midPoint = (startPoint + endPoint) / 2;

                controlPoints[0] = startPoint;
                controlPoints[1] = midPoint;
                controlPoints[2] = midPoint;
                controlPoints[3] = endPoint;

                for (int j = 0; j <= precisionCounter; j++)
                {
                    float t = j / (float)precisionCounter;

                    // 计算贝塞尔曲线点
                    Vector2 interpolatedValue = CalculateBezierPoint(controlPoints, t);
                    interpolatedList.Add(interpolatedValue);
                }
            }

            interpolatedList.Add(originalList[^1]);

            return interpolatedList;
        }

        // 计算贝塞尔曲线上的点
        private static Vector2 CalculateBezierPoint(Vector2[] controlPoints, float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            //float ttu = tt * u;
            float ttt = tt * t;

            Vector2 p = uuu * controlPoints[0];
            p += 3 * uu * t * controlPoints[1];
            p += 3 * u * tt * controlPoints[2];
            p += ttt * controlPoints[3];

            return p;
        }

        public const float TwoPi = MathF.PI * 2;
        public const float FourPi = MathF.PI * 4;
        public const float ThreePi = MathF.PI * 3;
        public const float PiOver3 = MathF.PI / 3f;
        public const float PiOver5 = MathF.PI / 5f;
        public const float PiOver6 = MathF.PI / 6f;

        #endregion

        #region MyUtils

        #region 普通绘制工具
        /// <summary>
        /// 获取与纹理大小对应的矩形框
        /// </summary>
        /// <param name="value">纹理对象</param>
        public static Rectangle GetRec(Texture2D value)
        {
            return new Rectangle(0, 0, value.Width, value.Height);
        }
        /// <summary>
        /// 获取与纹理大小对应的矩形框
        /// </summary>
        /// <param name="value">纹理对象</param>
        /// <param name="Dx">X起点</param>
        /// <param name="Dy">Y起点</param>
        /// <param name="Sx">宽度</param>
        /// <param name="Sy">高度</param>
        /// <returns></returns>
        public static Rectangle GetRec(Texture2D value, int Dx, int Dy, int Sx, int Sy)
        {
            return new Rectangle(Dx, Dy, Sx, Sy);
        }
        /// <summary>
        /// 获取与纹理大小对应的矩形框
        /// </summary>
        /// <param name="value">纹理对象</param>
        /// <param name="frameCounter">帧索引</param>
        /// <param name="frameCounterMax">总帧数，该值默认为1</param>
        /// <returns></returns>
        public static Rectangle GetRec(Texture2D value, int frameCounter, int frameCounterMax = 1)
        {
            int singleFrameY = value.Height / frameCounterMax;
            return new Rectangle(0, singleFrameY * frameCounter, value.Width, singleFrameY);
        }
        /// <summary>
        /// 获取与纹理大小对应的缩放中心
        /// </summary>
        /// <param name="value">纹理对象</param>
        /// <returns></returns>
        public static Vector2 GetOrig(Texture2D value)
        {
            return new Vector2(value.Width, value.Height) * 0.5f;
        }
        /// <summary>
        /// 获取与纹理大小对应的缩放中心
        /// </summary>
        /// <param name="value">纹理对象</param>
        /// <param name="ScaleOrig">整体缩放体积偏移</param>
        /// <returns></returns>
        public static Vector2 GetOrig(Texture2D value, float ScaleOrig)
        {
            return new Vector2(value.Width, value.Height) * ScaleOrig;
        }
        /// <summary>
        /// 获取与纹理大小对应的缩放中心
        /// </summary>
        /// <param name="value">纹理对象</param>
        /// <param name="ScaleX">X方向收缩系数</param>
        /// <param name="ScaleY">Y方向收缩系数</param>
        /// <returns></returns>
        public static Vector2 GetOrig(Texture2D value, float ScaleX, float ScaleY)
        {
            return new Vector2(value.Width * ScaleX, value.Height * ScaleY);
        }
        /// <summary>
        /// 获取与纹理大小对应的缩放中心
        /// </summary>
        /// <param name="value">纹理对象</param>
        /// <param name="frameCounter">帧索引</param>
        /// <param name="frameCounterMax">总帧数，该值默认为1</param>
        /// <returns></returns>
        public static Vector2 GetOrig(Texture2D value, int frameCounterMax = 1)
        {
            float singleFrameY = value.Height / frameCounterMax;
            return new Vector2(value.Width * 0.5f, singleFrameY / 2);
        }
        /// <summary>
        /// 对帧数索引进行走表
        /// </summary>
        /// <param name="frameCounter"></param>
        /// <param name="intervalFrame"></param>
        /// <param name="Maxframe"></param>
        public static void ClockFrame(ref int frameCounter, int intervalFrame, int maxFrame)
        {
            if (Main.GameUpdateCount % intervalFrame == 0)
            {
                frameCounter++;
            }

            if (frameCounter > maxFrame)
            {
                frameCounter = 0;
            }
        }
        /// <summary>
        /// 对帧数索引进行走表
        /// </summary>
        /// <param name="frameCounter"></param>
        /// <param name="intervalFrame"></param>
        /// <param name="Maxframe"></param>
        /// <param name="startCounter"></param>
        public static void ClockFrame(ref double frameCounter, int intervalFrame, int maxFrame, int startCounter = 0)
        {
            if (Main.fpsCount % intervalFrame == 0)
            {
                frameCounter++;
            }

            if (frameCounter > maxFrame)
            {
                frameCounter = startCounter;
            }
        }
        /// <summary>
        /// 将世界位置矫正为适应屏幕的画布位置
        /// </summary>
        /// <param name="entity">传入目标实体</param>
        /// <returns></returns>
        public static Vector2 WDEpos(Entity entity)
        {
            return GetEntityCenter(entity) - Main.screenPosition;
        }
        /// <summary>
        /// 将世界位置矫正为适应屏幕的画布位置
        /// </summary>
        /// <param name="pos">绘制目标的世界位置</param>
        /// <returns></returns>
        public static Vector2 WDEpos(Vector2 pos)
        {
            return pos - Main.screenPosition;
        }
        /// <summary>
        /// 获取纹理实例，类型为 Texture2D
        /// </summary>
        /// <param name="texture">纹理路径</param>
        /// <returns></returns>
        public static Texture2D GetT2DValue(string texture, bool immediateLoad = false)
        {
            return ModContent.Request<Texture2D>(texture, immediateLoad ? AssetRequestMode.AsyncLoad : AssetRequestMode.ImmediateLoad).Value;
        }

        /// <summary>
        /// 获取纹理实例，类型为 Asset<Texture2D>
        /// </summary>
        /// <param name="texture">纹理路径</param>
        /// <returns></returns>
        public static Asset<Texture2D> GetT2DAsset(string texture)
        {
            return ModContent.Request<Texture2D>(texture);
        }

        #endregion

        #region 高级绘制工具

        /// <summary>
        /// 任意设置 <see cref=" SpriteBatch "/> 的 <see cref=" BlendState "/>。
        /// </summary>
        /// <param name="spriteBatch">绘制模式</param>
        /// <param name="blendState">要使用的混合状态</param>
        public static void ModifyBlendState(this SpriteBatch spriteBatch, BlendState blendState)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, blendState, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        /// <summary>
        /// 将 <see cref="SpriteBatch"/> 的 <see cref="BlendState"/> 重置为典型的 <see cref="BlendState.AlphaBlend"/>。
        /// </summary>
        /// <param name="spriteBatch">绘制模式</param>
        public static void ResetBlendState(this SpriteBatch spriteBatch)
        {
            spriteBatch.ModifyBlendState(BlendState.AlphaBlend);
        }

        /// <summary>
        /// 将 <see cref="SpriteBatch"/> 的 <see cref="BlendState"/> 设置为 <see cref="BlendState.Additive"/>。
        /// </summary>
        /// <param name="spriteBatch">绘制模式</param>
        public static void SetAdditiveState(this SpriteBatch spriteBatch)
        {
            spriteBatch.ModifyBlendState(BlendState.Additive);
        }

        /// <summary>
        /// 将 <see cref="SpriteBatch"/> 重置为无效果的UI画布状态，在大多数情况下，这个适合结束一段在UI中的绘制
        /// </summary>
        /// <param name="spriteBatch">绘制模式</param>
        public static void ResetUICanvasState(this SpriteBatch spriteBatch)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);
        }

        #endregion

        #endregion

        #region TileUtils

        /// <summary>
        /// 检测是否为一个背景方块
        /// </summary>
        public static bool TopSlope(this Tile tile)
        {
            byte b = (byte)tile.Slope;
            return b is 1 or 2;
        }

        /// <summary>
        /// 将可能越界的方块坐标收值为非越界坐标
        /// </summary>
        public static Vector2 PTransgressionTile(Vector2 TileVr, int L = 0, int R = 0, int D = 0, int S = 0)
        {
            if (TileVr.X > Main.maxTilesX - R)
            {
                TileVr.X = Main.maxTilesX - R;
            }
            if (TileVr.X < 0 + L)
            {
                TileVr.X = 0 + L;
            }
            if (TileVr.Y > Main.maxTilesY - S)
            {
                TileVr.Y = Main.maxTilesY - S;
            }
            if (TileVr.Y < 0 + D)
            {
                TileVr.Y = 0 + D;
            }
            return new Vector2(TileVr.X, TileVr.Y);
        }

        /// <summary>
        /// 检测该位置是否存在一个实心的固体方块
        /// </summary>
        public static bool HasSolidTile(this Tile tile)
        {
            return tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType];
        }

        public static Vector2 FindTopLeft(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null)
            {
                return new Vector2(x, y);
            }

            TileObjectData data = TileObjectData.GetTileData(tile.TileType, 0);
            x -= tile.TileFrameX / 18 % data.Width;
            y -= tile.TileFrameY / 18 % data.Height;
            return new Vector2(x, y);
        }

        /// <summary>
        /// 检测方块的一个矩形区域内是否有实心物块
        /// </summary>
        /// <param name="tileVr">方块坐标</param>
        /// <param name="DetectionL">矩形左</param>
        /// <param name="DetectionR">矩形右</param>
        /// <param name="DetectionD">矩形上</param>
        /// <param name="DetectionS">矩形下</param>
        public static bool TileRectangleDetection(Vector2 tileVr, int DetectionL, int DetectionR, int DetectionD, int DetectionS)
        {
            Vector2 newTileVr;
            for (int x = 0; x < DetectionR - DetectionL; x++)
            {
                for (int y = 0; y < DetectionS - DetectionD; y++)
                {
                    newTileVr = PTransgressionTile(new Vector2(tileVr.X + x, tileVr.Y + y));
                    if (Main.tile[(int)newTileVr.X, (int)newTileVr.Y].HasSolidTile())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 获取一个物块目标，输入世界物块坐标，自动考虑收界情况
        /// </summary>
        public static Tile GetTile(int i, int j)
        {
            return GetTile(new Vector2(i, j));
        }

        /// <summary>
        /// 获取一个物块目标，输入世界物块坐标，自动考虑收界情况
        /// </summary>
        public static Tile GetTile(Vector2 pos)
        {
            pos = PTransgressionTile(pos);
            return Main.tile[(int)pos.X, (int)pos.Y];
        }

        /// <summary>
        /// 在世界中生成矿石
        /// </summary>
        /// <param name="tileID">矿石的瓦块ID</param>
        /// <param name="veinSize">矿脉的大小</param>
        /// <param name="chanceDenominator">生成机会的分母，值越小生成机会越大</param>
        public static void CreateOre(int tileID, int veinSize, int chanceDenominator)
        {
            // 根据机会分母循环尝试生成矿脉
            for (int i = 0; i < Main.maxTilesX * Main.maxTilesY / chanceDenominator; i++)
            {
                // 随机选择一个位置
                int x = Main.rand.Next(1, Main.maxTilesX - 1);
                int y = Main.rand.Next((int)GenVars.rockLayerLow, Main.maxTilesY - 1);

                // 检查位置的瓦块类型是否是可以生成矿脉的类型
                if (Main.tile[x, y].TileType is TileID.Stone or
                    TileID.Dirt or
                    TileID.Ebonstone or
                    TileID.Crimstone or
                    TileID.Pearlstone or
                    TileID.Sand or
                    TileID.Mud or
                    TileID.SnowBlock or
                    TileID.IceBlock)
                {
                    // 在符合条件的位置生成矿脉
                    WorldGen.TileRunner(x, y, veinSize, 15, tileID);
                }
            }
        }

        #endregion

        #region UIUtils

        /// <summary>
        /// 对UI版面的布局设置
        /// </summary>
        public static void SetRectangle(UIElement uiElement, float left, float top, float width, float height)
        {
            uiElement.Left.Set(left, 0f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }

        #endregion

        #region SoundUtils

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="pos">声音播放的位置</param>
        /// <param name="sound">要播放的声音样式（SoundStyle）</param>
        /// <param name="volume">声音的音量</param>
        /// <param name="pitch">声音的音调</param>
        /// <param name="pitchVariance">音调的变化范围</param>
        /// <param name="maxInstances">最大实例数，允许同时播放的声音实例数量</param>
        /// <param name="soundLimitBehavior">声音限制行为，用于控制当达到最大实例数时的行为</param>
        /// <returns>返回声音实例的索引</returns>
        public static SlotId SoundPlayer(
            Vector2 pos,
            SoundStyle sound,
            float volume = 1,
            float pitch = 1,
            float pitchVariance = 1,
            int maxInstances = 1,
            SoundLimitBehavior soundLimitBehavior = SoundLimitBehavior.ReplaceOldest
            )
        {
            sound = sound with
            {
                Volume = volume,
                Pitch = pitch,
                PitchVariance = pitchVariance,
                MaxInstances = maxInstances,
                SoundLimitBehavior = soundLimitBehavior
            };

            SlotId sid = SoundEngine.PlaySound(sound, pos);
            return sid;
        }

        /// <summary>
        /// 更新声音位置
        /// </summary>
        public static void PanningSound(Vector2 pos, SlotId sid)
        {
            if (!SoundEngine.TryGetActiveSound(sid, out ActiveSound activeSound))
            {
                return;
            }
            else
            {
                activeSound.Position = pos;
            }
        }

        #endregion

    }
}
