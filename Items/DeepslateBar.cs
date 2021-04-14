using CorruptMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CorruptMod.Items
{
	public class DeepslateBar : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("epic"
				+"\nThis is a modded material/instance.");
		}

		public override void SetDefaults() {
			item.width = 30;
			item.height = 24;
			item.maxStack = 999;
		}
				public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Wood, 9999);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
