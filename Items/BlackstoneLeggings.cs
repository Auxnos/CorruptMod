
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;

namespace CorruptMod.Items
{
	[AutoloadEquip(EquipType.Legs)]
	public class BlackstoneLeggings : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("Blackstone"
				+ "\n5% increased movement speed");
		}

		public override void SetDefaults() {
			item.width = 18;
			item.height = 18;
			item.value = 10000;
			item.rare = ItemRarityID.Green;
			item.defense = 15;
		}

		public override void UpdateEquip(Player player) {
			player.moveSpeed += 0.5f;
		}

		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<DeepslateBar>(), 65);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
