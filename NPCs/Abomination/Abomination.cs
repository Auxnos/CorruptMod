using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
namespace CorruptMod.NPCs.Abomination
{
	[AutoloadBossHead]
    public class Abomination : ModNPC
    {
							public override void SetStaticDefaults() {
			DisplayName.SetDefault("The Abomination");
			Main.npcFrameCount[npc.type] = 2;
		}
		//	[AutoloadBossHead]
							private static int hellLayer => Main.maxTilesY - 200;
							private const int sphereRadius = 300;
		public override void SetDefaults() {
			npc.aiStyle = -1;
			npc.lifeMax = 40000;
			npc.damage = 100;
			npc.defense = 55;
			npc.knockBackResist = 0f;
			npc.width = 100;
			npc.height = 100;
			npc.value = Item.buyPrice(0, 20, 0, 0);
			npc.npcSlots = 15f;
			npc.boss = true;
			npc.lavaImmune = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.buffImmune[24] = true;
			music = MusicID.Boss2;
		}
		
		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor) {
			return true;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) {
			scale = 1.5f;
			return null;
		}
        public override void AI() {//this is where you program your AI
								npc.netUpdate = true;
			Player player = Main.player[npc.target];
			if (!player.active || player.dead || player.position.Y < hellLayer * 16) {
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead || player.position.Y < hellLayer * 16) {
					npc.velocity = new Vector2(0f, 10f);
					if (npc.timeLeft > 10) {
						npc.timeLeft = 10;
					}
					return;
				}
			}
				}
		}
}
