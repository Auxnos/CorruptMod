
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CorruptMod.Items
{
	[AutoloadEquip(EquipType.Head)]
	public class BlackstoneHood : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("This is a modded helmet.");
		}

		public override void SetDefaults() {
			item.width = 18;
			item.height = 18;
			item.value = 10000;
			item.rare = ItemRarityID.Green;
			item.defense = 30;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ItemType<BlackstoneBreastplate>();// && legs.type == ItemType<ExampleLeggings>();
		}

		public override void UpdateArmorSet(Player player) {
			player.AddBuff(BuffID.Shine, 5);
			player.AddBuff(BuffID.Regeneration, 2);
		}

		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<DeepslateBar>(), 95);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
