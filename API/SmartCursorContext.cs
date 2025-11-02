using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;

namespace LibSmartCursor.API {
	/// <summary>
	/// Context data passed to smart cursor appliances during targeting.
	/// Contains information about the player, item, cursor position, reachable area, and tile restrictions.
	/// This struct is created fresh each frame and is automatically disposed.
	/// </summary>
	public struct SmartCursorContext {
		/// <summary>
		/// The player performing the smart cursor lookup.
		/// </summary>
		public Player Player;

		/// <summary>
		/// The currently selected item in the player's hotbar.
		/// </summary>
		public Item Item;

		/// <summary>
		/// The world position of the mouse cursor.
		/// </summary>
		public Vector2 MouseWorld;

		/// <summary>
		/// The rectangular bounds of tiles the player can reach with their current item.
		/// Takes into account tileRangeX, tileRangeY, and item.tileBoost.
		/// </summary>
		public Rectangle ReachBounds;

		/// <summary>
		/// Set of tiles that have been explicitly restricted by earlier appliances in the execution chain.
		/// Add tiles here to prevent other appliances and vanilla logic from targeting them.
		/// This set is cleared each frame.
		/// </summary>
		public HashSet<Point> RestrictedTiles { get; internal set; }

		/// <summary>
		/// Creates a SmartCursorContext from the given player.
		/// Calculates reach bounds based on player position, tile range, and item tileBoost.
		/// </summary>
		/// <param name="player">The player to create context for</param>
		/// <returns>A new SmartCursorContext with initialized values</returns>
		public static SmartCursorContext FromPlayer(Player player) {
			Item item = player.inventory[player.selectedItem];

			int tileRangeX = Player.tileRangeX;
			int tileRangeY = Player.tileRangeY;
			int tileBoost = item.tileBoost;

			int x1 = (int)(player.position.X / 16f) - tileRangeX - tileBoost + 1;
			int x2 = (int)((player.position.X + player.width) / 16f) + tileRangeX + tileBoost - 1;
			int y1 = (int)(player.position.Y / 16f) - tileRangeY - tileBoost + 1;
			int y2 = (int)((player.position.Y + player.height) / 16f) + tileRangeY + tileBoost - 2;

			int startX = Math.Clamp(x1, 10, Main.maxTilesX - 10);
			int endX = Math.Clamp(x2, 10, Main.maxTilesX - 10);
			int startY = Math.Clamp(y1, 10, Main.maxTilesY - 10);
			int endY = Math.Clamp(y2, 10, Main.maxTilesY - 10);

			return new SmartCursorContext {
				Player = player,
				Item = item,
				MouseWorld = Main.MouseWorld,
				ReachBounds = new Rectangle(startX, startY, endX - startX + 1, endY - startY + 1),
				RestrictedTiles = new HashSet<Point>()
			};
		}
	}
}
