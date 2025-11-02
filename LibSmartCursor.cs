using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using LibSmartCursor.API;
using Terraria;
using Terraria.GameContent;
using Microsoft.Xna.Framework;

namespace LibSmartCursor
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class LibSmartCursor : Mod
	{
		public static bool DebugMode { get; set; } = true; // Set to false for release

		public static SmartCursorRegistry Registry { get; private set; }

		public override void Load() {
			Registry = new SmartCursorRegistry();

			On_SmartCursorHelper.SmartCursorLookup += SmartCursorLookup_Override;
		}

		private void SetFocus(SmartCursorContext ctx, int x, int y) {
			Main.SmartCursorX = Player.tileTargetX = x;
			Main.SmartCursorY = Player.tileTargetY = y;
			Main.SmartCursorShowing = true;
		}

		private void SmartCursorLookup_Override(On_SmartCursorHelper.orig_SmartCursorLookup orig, Player player) {
			if (!Main.SmartCursorIsUsed) {
				orig(player);
				return;
			}

			SmartCursorContext ctx = SmartCursorContext.FromPlayer(player);
			List<SmartCursorAppliance> apps = new();

			Main.SmartCursorShowing = false;

			int focusedX = -1, focusedY = -1;

			Registry.FillPreVanillaAppliances(ctx.Item, ref apps);
			foreach (var preVanillaApp in apps) {
				preVanillaApp.ApplySmartCursor(ctx, ref focusedX, ref focusedY);
				if (focusedX != -1 && focusedY != -1) {
					if (ctx.RestrictedTiles.Contains(new Point(focusedX, focusedY))) {
						continue;
					}
					SetFocus(ctx, focusedX, focusedY);
					return;
				}
			}

			orig(player);
			if (Main.SmartCursorShowing) {
				if (ctx.RestrictedTiles.Contains(new Point(Main.SmartCursorX, Main.SmartCursorY))) {
					Main.SmartCursorShowing = false;
					Main.SmartCursorX = -1;
					Main.SmartCursorY = -1;
					Player.tileTargetX = (int)(((float)Main.mouseX + Main.screenPosition.X) / 16f);
					Player.tileTargetY = (int)(((float)Main.mouseY + Main.screenPosition.Y) / 16f);
					focusedX = -1;
					focusedY = -1;
				} else {
					return;
				}
			}

			apps.Clear();
			Registry.FillPostVanillaAppliances(ctx.Item, ref apps);
			foreach(var postVanillaApp in apps) {
				postVanillaApp.ApplySmartCursor(ctx, ref focusedX, ref focusedY);
				if (focusedX != -1 && focusedY != -1) {
					if (ctx.RestrictedTiles.Contains(new Point(focusedX, focusedY))) {
						continue;
					}
					SetFocus(ctx, focusedX, focusedY);
					return;
				}
			}
		}
	}
}
