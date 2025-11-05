using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace LibSmartCursor {
	internal class DebugOverlay : ModSystem {
		public override void PostDrawTiles() {
			if (LibSmartCursor.DebugMode && Main.SmartCursorIsUsed && Main.SmartCursorShowing && LibSmartCursor.CurrentDebugInfo != null) {
				DrawDebugText();
			}
		}

		private void DrawDebugText() {
			// Get the world position of the smart cursor target
			int tileX = Main.SmartCursorX;
			int tileY = Main.SmartCursorY;

			// Convert tile position to screen position
			Vector2 worldPos = new Vector2(tileX * 16 + 8, tileY * 16 + 8);
			Vector2 screenPos = worldPos - Main.screenPosition;

			// Offset to the right of the cursor
			screenPos.X += 24;
			screenPos.Y -= 8;

			// Build the debug text
			string applianceName = LibSmartCursor.CurrentDebugInfo.ApplianceName;
			string modName = LibSmartCursor.CurrentDebugInfo.ModName;
			string debugText = $"{applianceName}\n(Mod: {modName})";

			// Draw with a slight shadow for readability
			Color shadowColor = Color.Black * 0.8f;
			Color textColor = Color.Yellow;

			Main.spriteBatch.Begin();

			Utils.DrawBorderString(Main.spriteBatch, debugText, screenPos + new Vector2(1, 1), shadowColor, 0.7f);
			Utils.DrawBorderString(Main.spriteBatch, debugText, screenPos, textColor, 0.7f);

			Main.spriteBatch.End();
		}
	}
}
