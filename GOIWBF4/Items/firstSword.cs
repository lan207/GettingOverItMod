using GOIWBF4.BasedOn;
using GOIWBF4.Proj;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GOIWBF4.Items
{
	public class firstSword : ModItem
	{
        public override string Texture => "GOIWBF4/Items/firstSword";
        protected float RandomColor=-1;
		protected bool CanShootHammer=true ;
		public override void SetDefaults()
		{
			Item.damage = 1;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = 1;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 1;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<TestSwordProj>();
			Item.shootSpeed = 0;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.channel = true;
			Item.crit = 10;
		}
        public override bool AltFunctionUse(Player player)
        {
			if(player.itemTime==0)
			{
                CanShootHammer = !CanShootHammer;
                var str = CanShootHammer ? MyUtils.Translation("开启","ON", "включать") :MyUtils.Translation("关闭","OFF", "закрытие");
                Main.NewText(MyUtils.Translation("发射锤子模式","Shoot Hammer Mode", "Режим пожарного молота") +":"+ str);
            }
           
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

			if (player.altFunctionUse == 0)Projectile.NewProjectile(source,position,Item.scale.Float2V2X()+Item.useTime.Int2V2Y(),type,damage,knockback,player.whoAmI,CanShootHammer?0:1,RandomColor);
            return false;
        }
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe()
			.AddIngredient(ItemID.DirtBlock, 10)
			.AddIngredient(ItemID.StoneBlock, 10)
			.AddTile(TileID.WorkBenches)
			.Register();
		}
	}
}