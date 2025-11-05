using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using AlgoLib.Algo.Graph;
using AlgoLib.Geometry;

namespace LibSmartCursor.API {
	/// <summary>
	/// Determines when a smart cursor appliance executes relative to vanilla smart cursor logic.
	/// </summary>
	public enum ApplianceBehavior {
		/// <summary>
		/// Appliance runs before vanilla smart cursor logic.
		/// Use this for replacing or enhancing vanilla behavior (default).
		/// Multiple PreVanilla appliances run in priority order until one finds a target.
		/// </summary>
		PreVanilla,

		/// <summary>
		/// Appliance runs after vanilla smart cursor logic.
		/// Use this for fallback behavior when vanilla finds nothing.
		/// Multiple PostVanilla appliances run in priority order until one finds a target.
		/// </summary>
		PostVanilla
	}

	/// <summary>
	/// Base class for implementing custom smart cursor behavior for items.
	/// Subclass this and override IsValidTile to define which tiles can be targeted,
	/// or override FindTarget for complete control over the targeting algorithm.
	/// </summary>
	/// <example>
	/// <code>
	/// public class DirtFinder : SmartCursorAppliance {
	///     protected override bool IsValidTile(SmartCursorContext ctx, Point tile) {
	///         Tile t = Main.tile[tile.X, tile.Y];
	///         return t.HasTile &amp;&amp; t.TileType == TileID.Dirt;
	///     }
	/// }
	/// </code>
	/// </example>
	public abstract class SmartCursorAppliance {
		/// <summary>
		/// Determines when this appliance runs relative to vanilla smart cursor logic.
		/// Defaults to PreVanilla (runs before vanilla).
		/// Override to return PostVanilla to run only after vanilla fails to find a target.
		/// </summary>
		public virtual ApplianceBehavior Behavior => ApplianceBehavior.PreVanilla;

		/// <summary>
		/// Determines whether the specified tile is a valid target for smart cursor.
		/// Called during the search process for each tile within reach.
		/// You can also add tiles to ctx.RestrictedTiles to prevent other appliances/vanilla from targeting them.
		/// </summary>
		/// <param name="ctx">Context containing player, item, mouse position, reach bounds, and restricted tiles</param>
		/// <param name="tile">Tile coordinates to validate</param>
		/// <returns>True if this tile should be considered a valid target, false otherwise</returns>
		protected abstract bool IsValidTile(SmartCursorContext ctx, Point tile);

		/// <summary>
		/// Finds the best tile to target with smart cursor.
		/// The default implementation uses BFS to search in expanding square rings from the player,
		/// returning the closest tile to the cursor within the first ring that has valid tiles.
		/// Override this for complete control over the targeting algorithm.
		/// </summary>
		/// <param name="ctx">Context containing player, item, mouse position, reach bounds, and restricted tiles</param>
		/// <returns>The tile coordinates to target, or null if no valid target found</returns>
		protected virtual Point? FindTarget(SmartCursorContext ctx) {
			Point playerTile = GridUtils.WorldToTile(ctx.Player.Center);
			Point mouseTile = GridUtils.WorldToTile(ctx.MouseWorld);

			Point? bestTile = null;

			int maxRadius = ctx.ReachBounds.Width + ctx.ReachBounds.Height;

			List<Point> tilesInRing = new();
			int curRingDist = 0;

			var bfs = new BFSIterator(playerTile, validator: (pnt) => ctx.ReachBounds.Contains(pnt), square: true);
			while (bfs.HasNext()) {
				Point tile = bfs.Next();

				int tileChebyshev = GridUtils.ChebyshevDistance(playerTile, tile);

				if (tileChebyshev > curRingDist) {
					if (tilesInRing.Count > 0) {
						float closestToMouse = float.MaxValue;

						foreach (Point ringTile in tilesInRing) {
							float distToMouse = Vector2.Distance(ringTile.ToVector2(), mouseTile.ToVector2());
							if (distToMouse < closestToMouse) {
								closestToMouse = distToMouse;
								bestTile = ringTile;
							}
						}
						return bestTile;
					}

					curRingDist = tileChebyshev;
					tilesInRing.Clear();
				}

				if (IsValidTile(ctx, tile)) {
					tilesInRing.Add(tile);
				}
			}

			return bestTile;
		}

		internal void ApplySmartCursor(SmartCursorContext ctx, ref int focusedX, ref int focusedY) {
			Point? target = FindTarget(ctx);
			if (target.HasValue) {
				focusedX = target.Value.X;
				focusedY = target.Value.Y;
			}
		}
	}
}
