using CorruptMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using static Terraria.ModLoader.ModContent;

namespace ExampleMod
{
	public class ExampleWorld : ModWorld
	{
		public static bool downedAbomination;
		public static bool downedPuritySpirit;
		public const int VolcanoProjectiles = 30;
		public const float VolcanoAngleSpread = 170;
		public const int DefaultVolcanoTremorTime = 200; // ~ 3 seconds
		public const int DefaultVolcanoCountdown = 300; // 5 seconds
		public const int DefaultVolcanoCooldown = 10000; // At least 3 min of daytime between volcanoes
		public const int VolcanoChance = 10000; // Chance each tick of Volcano if cooldown exhausted.
		public int VolcanoCountdown;
		public int VolcanoCooldown = DefaultVolcanoCooldown;
		public int VolcanoTremorTime;
		public static int exampleTiles;

		public override void Initialize() {
			downedAbomination = false;
			downedPuritySpirit = false;
			VolcanoCountdown = 0;
			VolcanoTremorTime = 0;
		//	00ExampleTravelingMerchant.spawnTime = double.MaxValue;
		}

		public override TagCompound Save() {
			var downed = new List<string>();

			return new TagCompound {
				["downed"] = downed,
			//	00["traveler"] = ExampleTravelingMerchant.Save()
			};
		}

		public override void Load(TagCompound tag) {
			var downed = tag.GetList<string>("downed");
		//	ExampleTravelingMerchant.Load(tag.GetCompound("traveler"));
		}

		public override void LoadLegacy(BinaryReader reader) {
			int loadVersion = reader.ReadInt32();
			if (loadVersion == 0) {
				BitsByte flags = reader.ReadByte();
				downedAbomination = flags[0];
				downedPuritySpirit = flags[1];
			}
			else {
				mod.Logger.WarnFormat("ExampleMod: Unknown loadVersion: {0}", loadVersion);
			}
		}

		public override void NetSend(BinaryWriter writer) {
			var flags = new BitsByte();
			flags[0] = downedAbomination;
			flags[1] = downedPuritySpirit;
			writer.Write(flags);

			/*
			Remember that Bytes/BitsByte only have 8 entries. If you have more than 8 flags you want to sync, use multiple BitsByte:

				This is wrong:
			flags[8] = downed9thBoss; // an index of 8 is nonsense. 

				This is correct:
			flags[7] = downed8thBoss;
			writer.Write(flags);
			BitsByte flags2 = new BitsByte(); // create another BitsByte
			flags2[0] = downed9thBoss; // start again from 0
			// up to 7 more flags here
			writer.Write(flags2); // write this byte
			*/

			//If you prefer, you can use the BitsByte constructor approach as well.
			//writer.Write(saveVersion);
			//BitsByte flags = new BitsByte(downedAbomination, downedPuritySpirit);
			//writer.Write(flags);

			// This is another way to do the same thing, but with bitmasks and the bitwise OR assignment operator (the |=)
			// Note that 1 and 2 here are bit masks. The next values in the pattern are 4,8,16,32,64,128. If you require more than 8 flags, make another byte.
			//writer.Write(saveVersion);
			//byte flags = 0;
			//if (downedAbomination)
			//{
			//	flags |= 1;
			//}
			//if (downedPuritySpirit)
			//{
			//	flags |= 2;
			//}
			//writer.Write(flags);
		}

		public override void NetReceive(BinaryReader reader) {
			BitsByte flags = reader.ReadByte();
			downedAbomination = flags[0];
			downedPuritySpirit = flags[1];
			// As mentioned in NetSend, BitBytes can contain 8 values. If you have more, be sure to read the additional data:
			// BitsByte flags2 = reader.ReadByte();
			// downed9thBoss = flags[0];
		}

		// We use this hook to add 3 steps to world generation at various points. 
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight) {
			// Because world generation is like layering several images ontop of each other, we need to do some steps between the original world generation steps.

			// The first step is an Ore. Most vanilla ores are generated in a step called "Shinies", so for maximum compatibility, we will also do this.
			// First, we find out which step "Shinies" is.
			int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
			if (ShiniesIndex != -1) {
				// Next, we insert our step directly after the original "Shinies" step. 
				// ExampleModOres is a method seen below.
				tasks.Insert(ShiniesIndex + 1, new PassLegacy("C Ores", COres));
			}


			int LivingTreesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Living Trees"));
			if (LivingTreesIndex != -1) {
				tasks.Insert(LivingTreesIndex + 1, new PassLegacy("Post Terrain", delegate (GenerationProgress progress) {
					// We can inline the world generation code like this, but if exceptions happen within this code 
					// the error messages are difficult to read, so making methods is better. This is called an anonymous method.
					progress.Message = "What is it Lassie, did Timmy fall down a well?";
		////////////	\\\	--	MakeWells();
				}));
			}
		}

		private void COres(GenerationProgress progress) {
			// progress.Message is the message shown to the user while the following code is running. Try to make your message clear. You can be a little bit clever, but make sure it is descriptive enough for troubleshooting purposes. 
			progress.Message = "C Ores";

			// Ores are quite simple, we simply use a for loop and the WorldGen.TileRunner to place splotches of the specified Tile in the world.
			// "6E-05" is "scientific notation". It simply means 0.00006 but in some ways is easier to read.
			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++) {
				// The inside of this for loop corresponds to one single splotch of our Ore.
				// First, we randomly choose any coordinate in the world by choosing a random x and y value.
				int x = WorldGen.genRand.Next(0, Main.maxTilesX);
				int y = WorldGen.genRand.Next((int)WorldGen.worldSurfaceLow, Main.maxTilesY); // WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use WorldGen.rockLayer or other WorldGen values.

				// Then, we call WorldGen.TileRunner with random "strength" and random "steps", as well as the Tile we wish to place. Feel free to experiment with strength and step to see the shape they generate.
				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(5, 25), WorldGen.genRand.Next(4, 25), TileType<DeepslateOre>());

				// Alternately, we could check the tile already present in the coordinate we are interested. Wrapping WorldGen.TileRunner in the following condition would make the ore only generate in Snow.
				// Tile tile = Framing.GetTileSafely(x, y);
				// if (tile.active() && tile.type == TileID.SnowBlock)
				// {
				// 	WorldGen.TileRunner(.....);
				// }
			}
		}

		public override void ResetNearbyTileEffects() {
		}

		public override void TileCountsAvailable(int[] tileCounts) {

		}

		public override void PreUpdate() {
			// Update everything about spawning the traveling merchant from the methods we have in the Traveling Merchant's class
			// ExampleTravelingMerchant.UpdateTravelingMerchant();
		}

		public override void PostUpdate() {
		}

		// In ExampleMod, we use PostDrawTiles to draw the TEScoreBoard area. PostDrawTiles draws before players, npc, and projectiles, so it works well.
		public override void PostDrawTiles() {

		}

		// A helper method that draws a bordered rectangle. 
		public static void DrawBorderedRect(SpriteBatch spriteBatch, Color color, Color borderColor, Vector2 position, Vector2 size, int borderWidth) {
			spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), color);
			spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X - borderWidth, (int)position.Y - borderWidth, (int)size.X + borderWidth * 2, borderWidth), borderColor);
			spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X - borderWidth, (int)position.Y + (int)size.Y, (int)size.X + borderWidth * 2, borderWidth), borderColor);
			spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X - borderWidth, (int)position.Y, (int)borderWidth, (int)size.Y), borderColor);
			spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X + (int)size.X, (int)position.Y, (int)borderWidth, (int)size.Y), borderColor);
		}
	}
}
