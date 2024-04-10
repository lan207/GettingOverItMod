using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using GOIWBF4.Proj;
using GOIWBF4.Mount;

namespace GOIWBF4.BasedOn
{
    public static class ClassHelp
    {
        #region #TileIDs#
        //(蛛网)Cobweb
        public static List<int>
            workStation = new() { TileID.LunarCraftingStation, TileID.HeavyWorkBench, TileID.Furnaces, TileID.Hellforge, TileID.AdamantiteForge,TileID.Autohammer,TileID.Sawmill, TileID.GlassKiln,TileID.BoneWelder,TileID.IceMachine,
            TileID.LivingLoom,TileID.SkyMill,TileID.LihzahrdFurnace,TileID.SteampunkBoiler,TileID.HoneyDispenser,TileID.SharpeningStation,TileID.Dressers,TileID.Kegs,TileID.CookingPots,TileID.CrystalBall,
            TileID.BewitchingTable,TileID.AlchemyTable,TileID.LesionStation,TileID.FogMachine,TileID.SliceOfCake,TileID.TeaKettle,TileID.Campfire },
            workStation2 = new() { TileID.WorkBenches, TileID.Anvils, TileID.MythrilAnvil, TileID.TinkerersWorkbench },
            platforms = new() { TileID.Platforms, TileID.TeamBlockBluePlatform, TileID.TeamBlockGreenPlatform, TileID.TeamBlockPinkPlatform, TileID.TeamBlockRedPlatform, TileID.TeamBlockWhitePlatform
        ,TileID.TeamBlockYellowPlatform,TileID.PlanterBox},
            lightSource = new() { TileID.Torches, TileID.Lampposts, TileID.Candles, TileID.PeaceCandle, TileID.PlatinumCandle, TileID.ShadowCandle,TileID.Candelabras,TileID.Chandeliers,TileID.ShimmerflyinaBottle,
                TileID.WaterCandle,TileID.HangingLanterns,TileID.ChineseLanterns,TileID.SkullLanterns,TileID.Jackolanterns,TileID.Lamps,TileID.LavaLamp,TileID.PlasmaLamp,TileID.SoulBottles},//Lampposts=>路灯
            furniture = new() { TileID.Chairs,  TileID.Beds,TileID.Bathtubs,TileID.Banners, TileID.GrandfatherClocks, TileID.DiscoBall,TileID.GardenGnome,
            TileID.Mannequin,TileID.Womannequin,TileID.LunarMonolith,TileID.DisplayDoll,TileID.Toilets,TileID.VoidMonolith,TileID.EchoMonolith,TileID.ShimmerMonolith},
            furniture2 = new() { TileID.Tables, TileID.Tables2 },
            sign = new() { TileID.ArrowSign, TileID.PaintedArrowSign, TileID.Signs, TileID.TatteredWoodSign },
            openDoors = new() { TileID.TallGateOpen, TileID.OpenDoor, TileID.TrapdoorOpen },
            trees = new() { TileID.Trees, TileID.MushroomTrees, TileID.TreeSapphire, TileID.TreeAmber, TileID.TreeAmethyst, TileID.TreeAsh, TileID.TreeDiamond, TileID.TreeEmerald , TileID.TreeRuby,
            TileID.TreeTopaz ,TileID.VanityTreeSakura,TileID.VanityTreeYellowWillow,TileID.Cactus },//Cacus=>仙人掌
            containers = new() { TileID.Containers, TileID.Containers2, TileID.FakeContainers, TileID.FakeContainers2 },
            rope = new() { TileID.Rope, TileID.Chain, TileID.MysticSnakeRope, TileID.SilkRope, TileID.VineRope, TileID.WebRope, TileID.SillyStreamerBlue, TileID.SillyStreamerGreen, TileID.SillyStreamerPink },
            partyUse = new() { TileID.SillyBalloonGreen, TileID.SillyBalloonPink, TileID.SillyBalloonPurple, TileID.SillyStreamerBlue, TileID.SillyBalloonMachine, TileID.PartyPresent, TileID.PartyMonolith
            , TileID.PartyBundleOfBalloonTile,TileID.Pigronata,TileID.PinWheel,},
            beamAndColumn = new() { TileID.BorealBeam, TileID.RichMahoganyBeam, TileID.MarbleColumn, TileID.GraniteColumn, TileID.PalladiumColumn, TileID.SandstoneColumn },
            oldSun = new() { TileID.DefendersForge, TileID.WarTable, TileID.WarTableBanner, TileID.ElderCrystalStand },
            golfUse = new() { TileID.GolfCupFlag, TileID.GolfTee, TileID.GolfTrophies },
            saplings = new() { TileID.Saplings, TileID.GemSaplings, TileID.VanityTreeSakuraSaplings, TileID.VanityTreeWillowSaplings },//树苗
            placeable = new() { TileID.PiggyBank, TileID.Bottles, TileID.Bowls, TileID.DjinnLamp },
            plants = new() { TileID.Plants, TileID.Plants2, TileID.CorruptPlants, TileID.HallowedPlants, TileID.HallowedPlants2, TileID.CrimsonPlants, TileID.AshPlants, TileID.JunglePlants,TileID.AbigailsFlower,
            TileID.JunglePlants2,TileID.CrimsonPlants,TileID.DyePlants,TileID.MushroomPlants,TileID.OasisPlants,TileID.PottedCrystalPlants,TileID.PottedLavaPlants,TileID.PottedPlants1,TileID.PottedPlants2
            ,TileID.MatureHerbs, TileID.ImmatureHerbs, TileID.BloomingHerbs,TileID.Pumpkins,TileID.LilyPad,TileID.Cattail,TileID.SeaOats,TileID.OasisPlants ,TileID.LongMoss,TileID.GlowTulip},
            pottedPlant = new() { TileID.PottedPlants1, TileID.PottedPlants2, TileID.PottedLavaPlants, TileID.PottedCrystalPlants, TileID.Seaweed, TileID.SeaweedPlanter,TileID.PottedLavaPlantTendrils
            ,TileID.PottedLavaPlants,TileID.PottedLavaPlantTendrils,TileID.PotsSuspended},
            fragileProducts = new() { TileID.CorruptThorns, TileID.CrimsonThorns, TileID.PlanteraThorns, TileID.JungleThorns, TileID.Heart, TileID.ShadowOrbs, TileID.LifeFruit,TileID.Larva,TileID.ManaCrystal
            ,TileID.PlanteraBulb},
            piles = new() { TileID.SmallPiles, TileID.LargePiles, TileID.LargePiles2, TileID.LargePilesEcho, TileID.LargePiles2Echo, TileID.CopperCoinPile, TileID.SilverCoinPile,TileID.PlantDetritus2x2Echo,
            TileID.PlantDetritus3x2Echo,TileID.GoldCoinPile,TileID.PotsEcho,TileID.PlatinumCoinPile,TileID.BeachPiles,TileID.SmallPiles1x1Echo,TileID.SmallPiles2x1Echo,TileID.LargePilesEcho,TileID.LargePiles2Echo,TileID.Coral},//Coral=>珊瑚，pile=>物品堆
            boulders = new() { TileID.Boulder, TileID.LifeCrystalBoulder, TileID.BouncyBoulder, TileID.RollingCactus },
            vines = new() { TileID.Vines, TileID.AshVines, TileID.CorruptVines, TileID.CrimsonVines, TileID.HallowedVines, TileID.MushroomVines, TileID.JungleVines, TileID.VineFlowers },//藤蔓
            statues = new() { TileID.Statues, TileID.AlphabetStatues, TileID.BoulderStatue, TileID.MushroomStatue },
            racksAndPaint = new() { TileID.WeaponsRack, TileID.WeaponsRack2,TileID.ItemFrame,TileID.HatRack, TileID.Painting2X3, TileID.Painting3X2, TileID.Painting3X3,TileID.Painting4X3,TileID.Painting6X4,
              },
            livingFire = new() { TileID.LivingFire, TileID.LivingCursedFire, TileID.LivingDemonFire, TileID.LivingFrostFire, TileID.LivingUltrabrightFire, TileID.LivingIchor },
            cages = new() { TileID.BirdCage, TileID.GoldBirdCage, TileID.BunnyCage, TileID.GoldBunnyCage, TileID.GoldButterflyCage, TileID.FrogCage, TileID.GoldFrogCage, TileID.GrasshopperCage,
            TileID.GoldGrasshopperCage,TileID.MouseCage,TileID.GoldMouseCage,TileID.WormCage,TileID.GoldWormCage,TileID.TruffleWormCage,TileID.CageEnchantedNightcrawler,TileID.CageBuggy
            ,TileID.CageGrubby,TileID.CageSluggy,TileID.SquirrelCage,TileID.SquirrelOrangeCage,TileID.SquirrelGoldCage,TileID.LadybugCage,TileID.GoldLadybugCage,TileID.OwlCage,TileID.TurtleCage,
            TileID.TurtleJungleCage,TileID.GrebeCage,TileID.SeagullCage,TileID.WaterStriderCage,TileID.GoldWaterStriderCage,TileID.AmberBunnyCage,TileID.AmethystBunnyCage,TileID.DiamondBunnyCage,TileID.EmeraldBunnyCage
            ,TileID.RubyBunnyCage,TileID.SapphireBunnyCage,TileID.TopazBunnyCage,TileID.AmberSquirrelCage,TileID.AmethystSquirrelCage,TileID.DiamondSquirrelCage,TileID.EmeraldSquirrelCage,TileID.RubySquirrelCage,
            TileID.SapphireSquirrelCage,TileID.TopazSquirrelCage,TileID.StinkbugCage,TileID.ScarletMacawCage,TileID.BlueMacawCage},
            jar = new() { TileID.GreenDragonflyJar, TileID.OrangeDragonflyJar, TileID.RedDragonflyJar, TileID.YellowDragonflyJar, TileID.BlackDragonflyJar, TileID.BlueDragonflyJar, TileID.GoldDragonflyJar,
            TileID.BlueFairyJar,TileID.GreenFairyJar,TileID.PinkFairyJar,TileID.EmpressButterflyJar},
            otherBottles = new() { TileID.BlueJellyfishBowl, TileID.FishBowl, TileID.GreenJellyfishBowl, TileID.PinkJellyfishBowl, TileID.ShipInABottle,TileID.GoldGoldfishBowl,
            TileID.LavafishBowl},
            trap = new() { TileID.Detonator, TileID.GeyserTrap, TileID.BeeHive, TileID.AntlionLarva },
            other = new() {TileID.TargetDummy, TileID.Cobweb, TileID.Books,  TileID.Tombstones,  TileID.Crystals,  TileID.FireworksBox,TileID.Bubble, TileID.Sandcastles, TileID.TeleportationPylon,
            TileID.VolcanoSmall,TileID.VolcanoLarge,TileID.MasterTrophyBase,TileID.TNTBarrel},
            other2 = new() { TileID.Dirt, TileID.Stone, TileID.Bookcases, TileID.Safes, TileID.MetalBars, TileID.Presents, TileID.FishingCrate, TileID.Explosives, TileID.TrapdoorClosed }
            ;
        public static List<List<int>> AirCheck = new List<List<int>>() {workStation,workStation2,platforms,lightSource,furniture,furniture2,sign,openDoors,trees,containers,rope,partyUse,beamAndColumn,oldSun,golfUse,
         saplings,placeable,plants,pottedPlant,fragileProducts,piles,boulders,vines,statues,racksAndPaint,livingFire,cages,jar,otherBottles,other,other2 },
        StandableCheck = new() { workStation2, platforms, boulders, furniture2, other2 },
        StandableSolid=new () { workStation2, platforms, boulders, furniture2, other2 };
        #endregion
        
        public static double Pow(this double a, int b)
        {
            return Math.Pow(a, b);
        }
        public static float Pow(this float a, int b)
        {
            return (float)Math.Pow(a, b);
        }
        public static float Pow(this int a, int b)
        {
            return (float)Math.Pow(a, b);
        }
        public static float Sqrt(this float a)
        {
            return (float)Math.Sqrt(a);
        }
        public static Color RtnAlphaBlendTransparentT2D(this float a)
        {
            return new Color(a, a, a, a);
        }
        public static bool Is2_V2Contrary(this Vector2 v,Vector2 v2)
        {
            if ((v.SafeNormalize(Vector2.UnitX) - v2.SafeNormalize(Vector2.UnitX)).Length() < .1f) return false;
            return true;
        }
        public static Vector2 Rot2Oval(this float rad, float x = 1, float y = 1,float OriginRot=0)
        {
            if (x < 1) x = 1;
            if (y < 1) y = 1;
            var v2 = rad.ToRotationVector2();
            var X = x*v2.X;
            var Y = y*v2.Y;
            return new Vector2(X, Y).RotatedBy(OriginRot);
        }
        public static Vector2 Float2V2X(this float x)
        {
            return new Vector2(x, 0);
        }
        public static Vector2 Float2V2Y(this float y)
        {
            return new Vector2(0, y);
        }
        public static Vector2 Float2V2(this float i, bool xInvert = false, bool yInvert = false)
        {
            return new Vector2(!xInvert ? i : -i, !yInvert ? i : -i);
        }
        public static Vector2 Int2V2X(this int x)
        {
            return new Vector2(x, 0);
        }
        public static Vector2 Int2V2Y(this int y)
        {
            return new Vector2(0, y);
        }
        public static Vector2 Int2V2(this int i, bool xInvert = false, bool yInvert = false)
        {
            return new Vector2(!xInvert ? i : -i, !yInvert ? i : -i);
        }
        public static float Int2Hypotenuse(this int a)
        {
            return a * 1.414f;
        }
        public static float Int2Hypotenuse(this int a, int b)
        {
            return (a.Pow(2) + b.Pow(2)).Sqrt();
        }
        public static float Hypotenuse2Float(this int a)
        {
            return a / 1.414f;
        }
        public static float Float2Hypotenuse(this float a)
        {
            return a * 1.414f;
        }
        public static float Hypotenuse2Float(this float a)
        {
            return a / 1.414f;
        }
        public static float Int2Rad(this int a)=>MathHelper.ToRadians(a);

        public static float JudgeSwordRot(this float rotRad)
        {
            return rotRad + MathHelper.ToRadians(45);
        }
        public static float JudgeSwordRotBack(this float rotRad)
        {
            return rotRad - MathHelper.ToRadians(45);
        }
        public static bool RtnNpercentIsTrue(this int num)
        {
            if (num >= 1) return Main.rand.Next(1, 101) <= num;
            return false;

        }
        public static int GetDirFrom2_V2(this Vector2 v, Vector2 v2)
        {
            return v.X == v2.X ? 1 : Math.Sign(v.X - v2.X);
        }
        public static float Mutiply1Deg(this float a)
        {
            return a * (float)Math.PI / 180f;
        }
        public static float Mutiply1Deg(this int a)
        {
            return a * (float)Math.PI / 180f;
        }
        public static float RotHalf(this float a)
        {
            return a + (float)Math.PI;
        }

        public static float RotQuarter(this float a, bool minus = false)
        {
            var b = 1;
            if (minus) b = -1;
            return a + (float)Math.PI * b / 2f;
        }
        public static int RoundInt(this float a) => (int)Math.Round(a, 0);
        public static bool IsThereAHammer()
        {
            foreach (var p in Main.projectile)
            {
                if (p.Alives() && (p.ModProjectile is Hammer || p.ModProjectile is JarMount2)) return true;
            }
            return false;
        }
        public static Player player => Main.LocalPlayer;
        public static void Draw2TestByRect(this Texture2D t2d, Rectangle rect, Color c = default)
        {
            Main.spriteBatch.Draw(t2d, rect.X.Int2V2X() + rect.Y.Int2V2Y(), MyUtils.GetRec(t2d), c,
                0, Vector2.Zero, (rect.Width.Int2V2X() + rect.Height.Int2V2Y()) / (float)t2d.Width, SpriteEffects.None, 0);
        }
        public static bool CheckSolidTile(this int i,int j,out bool platform, bool CheckTable = true)
        {
            Tile t = Main.tile[i,j];
            platform= false;
            if (CheckTable)
            {
                foreach (var type in workStation2)
                {
                    if (t.TileType == type)
                    {
                        return true;
                    }
                }
                foreach (var type in platforms)
                {
                    if (t.TileType == type)
                    {
                        platform = true;
                        return true;
                    }
                }
                return false;
            }
            
            return false;
        }
        public static bool CheckSolidTile(this Vector2 v,  out Vector2 v2,bool DetectStandable = false,bool CheckPlatform=false,bool CheckTable=false)
        {
            Tile t = Main.tile[(v.X / 16).RoundInt(), (v.Y / 16).RoundInt()];
            v2 = new Vector2((v.X ).RoundInt(), (v.Y ).RoundInt());
            if(CheckTable)
            {
                foreach(var i in workStation2)
                {
                    if (t.TileType == i) return true;
                }
                foreach (var i in platforms)
                {
                    if (t.TileType == i) return true;
                }
                return false;
            }
            if(CheckPlatform)
            {
                foreach (var i in platforms)
                {
                    if(t.TileType == i)return true;
                }
                return false;
            }
            
            if (!DetectStandable)
            {
                foreach (var list in StandableSolid)
                {
                    foreach (var i in list)
                    {
                        if (t.TileType == i)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                foreach (var list in StandableCheck)
                {
                    foreach (var i in list)
                    {
                        if (t.TileType == i)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        //public bool inActive(this Tile t)
        //{
        //    return (sTileHeader & 0x40) == 64;
        //}
        public static bool HasHalfBlockLiquid(this Vector2 Pos,int w,int h,out int type,out Vector2 LCenter)
        {
            type = -1;
            LCenter = default;
            if (w==16&&h==16)
            {
                int i = (Pos.X / 16).RoundInt();
                int j = (Pos.Y / 16).RoundInt();
                var tile = Main.tile[i, j];
                if (tile.LiquidType != -1 && tile.LiquidAmount >= 127)
                {
                    type = tile.LiquidType;
                    LCenter = (i * 16).Int2V2X() + (j * 16).Int2V2Y() + 8.Int2V2();
                    return true;
                }
                return false;
            }
            else
            {
               
                int X = (int)(Pos.X / 16f) - 1;
                int XMax = (int)((Pos.X + w) / 16f) + 1;
                int TrueY = (int)(Pos.Y / 16f) - 1;
                int YMax = (int)((Pos.Y + h) / 16f) + 1;
                int TrueX = Utils.Clamp(X, 0, Main.maxTilesX - 1);
                XMax = Utils.Clamp(XMax, 0, Main.maxTilesX - 1);
                TrueY = Utils.Clamp(TrueY, 0, Main.maxTilesY - 1);
                YMax = Utils.Clamp(YMax, 0, Main.maxTilesY - 1);
                for (int i = TrueX; i < XMax; i++)
                {
                    for (int j = TrueY; j < YMax; j++)
                    {
                        var tile = Main.tile[i, j];
                        if (tile.LiquidType != -1 && tile.LiquidAmount >= 127)
                        {
                            type = tile.LiquidType;
                            LCenter = (i * 16).Int2V2X() + (j * 16).Int2V2Y() + 8.Int2V2();
                            return true;
                        }
                    }
                }
                return false;
            }
           
        }
        public static int GetKeyByIndex(Dictionary<int, bool> dictionary, int index)
        {
            int currentIndex = 0;
            foreach (var key in dictionary.Keys)
            {
                if (currentIndex == index)
                {
                    return key;
                }
                currentIndex++;
            }
            throw new IndexOutOfRangeException("Index is out of range.");
        }
        public static Vector2 GetKeyByIndex(Dictionary<Vector2, Vector2> dictionary, int index)
        {
            int currentIndex = 0;
            foreach (var key in dictionary.Keys)
            {
                if (currentIndex == index)
                {
                    return key;
                }
                currentIndex++;
            }
            throw new IndexOutOfRangeException("Index is out of range.");
        }
        public static bool SolidCollision(this Vector2 Position, int Width, int Height, out Vector2 vector, out Vector2 WAndH, out bool SolidTop,out bool Slope,out bool Plat)
        {
            int X = (int)(Position.X / 16f) - 1;
            int XMax = (int)((Position.X + Width) / 16f) + 1;
            int TrueY = (int)(Position.Y / 16f) - 1;
            int YMax = (int)((Position.Y +Height) / 16f) + 1;
            int TrueX = Utils.Clamp(X, 0, Main.maxTilesX - 1);
            XMax = Utils.Clamp(XMax, 0, Main.maxTilesX - 1);
            TrueY = Utils.Clamp(TrueY, 0, Main.maxTilesY - 1);
            YMax = Utils.Clamp(YMax, 0, Main.maxTilesY - 1);
            vector = default;
            SolidTop = false;
            Slope = true;
            Plat = false;
            WAndH = Vector2.One * 16;
            Dictionary<int ,bool> dict = new();
            Dictionary<Vector2, Vector2> dic2 = new();
            Dictionary<int, bool> dic3 = new();
            int I = 0;
            for (int i = TrueX; i < XMax; i++)
            {
                for (int j = TrueY; j < YMax; j++)
                {
                    vector =(i*10+j).Int2V2X();
                    SolidTop = false;
                    var isTile = false;
                    WAndH = Vector2.One * 16;
                    var tile = Main.tile[i, j];
                    if ( tile!= null &&tile.HasUnactuatedTile && Main.tileSolid[tile.TileType])
                    {
                        isTile = true;
                        
                        vector.X = i * 16;
                        vector.Y = j * 16;
                        int Ycorrect = 16;
                        int XCorrect = 16;
                        if(tile.Slope!=SlopeType.Solid)
                        {
                            if (tile.IsHalfBlock)
                            {
                                vector.Y += 8f;
                                Ycorrect -= 8;
                            }
                        }
                        else Slope = false;
                        if (i.CheckSolidTile(j,out bool plat) && Main.tileSolidTop[tile.TileType])
                        {
                            SolidTop = true;
                            if(plat)
                            {
                                if(!Slope)Ycorrect -= 8;
                                Plat = true;
                            }
                            else
                            {
                                XCorrect += 16;
                            }
                        }
                        if (Position.X + Width > vector.X && Position.X < vector.X + XCorrect && Position.Y + Height > vector.Y && Position.Y < vector.Y + Ycorrect)
                        {
                            WAndH = XCorrect.Int2V2X() + Ycorrect.Int2V2Y();
                        }
                        
                    }
                    dict.Add(I, isTile);
                    dic2.Add(vector, WAndH);
                    dic3.Add(I, SolidTop);
                    I++;
                }
            }
            var Center = Position+(Width/2).Int2V2X()+(Height/2).Int2V2Y();
            var mininum = (float)int.MaxValue;
            int TrueI = 0;
            for(int j=0;j<dict.Count;j++)
            {
                var v = GetKeyByIndex(dic2, j);
                var b = dict[j];
                if (!b) continue;
                var v2 =v+ 8.Int2V2();
                var dis = Vector2.Distance(v2, Center);
                if (dis<mininum)
                {
                    mininum = dis;
                    TrueI = j;
                }
            }
            if(mininum != int.MaxValue && dict[TrueI])
            {
                vector = GetKeyByIndex(dic2, TrueI);
                WAndH = dic2[vector];
                var columnNum = YMax - TrueY;
                var Truei = TrueI - columnNum;
                if (WAndH != Vector2.One * 16&& Truei >= 0)
                {
                    
                    var v2 = GetKeyByIndex(dic2, Truei);
                    var wah = dic2[v2];
                    if (wah==WAndH)
                    {
                        vector = v2;
                        SolidTop = true;
                        return true;
                    }

                }
                SolidTop = dic3[ TrueI];
                return true;
            }
            Slope = false;
            return false;
        }
        public static float RoundByAnyStep(this float a, int stepCount)
        {
            if (stepCount < 1) stepCount = 1;
            var step = 1 / (float)stepCount;
            var low = (int)a;
            for (int i = -1; i <= stepCount; i++)
            {
                var now = low + step * i;
                var fur = now + step;
                var Ndis = Math.Abs(now - a);
                var FDis = Math.Abs(fur - a);
                if (Ndis < step)
                {
                    if (FDis <= Ndis) return fur;
                    return now;
                }
            }
            return -1;
        }
        public static Vector2 LimitV2(this Vector2 v2, float len = 1)
        {
            if (len < 0) len = 0;
            var v = v2.SafeNormalize(Vector2.UnitX);
            var Length = v2.Length();
            return Length > len ? v * len : v2;
        }
        public static Vector2 RtnMiddleV2(this Vector2 a, Vector2 b) => (a + b) / 2;
        public static bool CheckClockWise(this Vector2 a, bool up = false, bool left = false)
        {
            //if(up&&a==(1).Int2V2X())return true;
            //if (!up && a == (-1).Int2V2X()) return true;
            if (left && a == 1.Int2V2Y()) return true;
            if (!left && a == (-1).Int2V2Y()) return true;
            if (up && a.X > 0) return true;
            if (!up && a.X < 0) return true;
            return false;
        }
        public static bool CheckClockWise(this Vector2 a, Vector2 origin,Vector2 middle)
        {
            //if(up&&a==(1).Int2V2X())return true;
            //if (!up && a == (-1).Int2V2X()) return true;
            if (origin == middle) return false;
            var up = middle.Y - origin.Y <= 0;
            var left = middle.X-origin.X <= 0;
            if (left && a == 1.Int2V2Y()) return true;
            if (!left && a == (-1).Int2V2Y()) return true;
            if (up && a.X > 0) return true;
            if (!up && a.X < 0) return true;
            return false;
        }
    }
    
}
