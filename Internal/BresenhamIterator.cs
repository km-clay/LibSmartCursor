using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace LibSmartCursor.Internal {
	internal class BresenhamIterator {
		private int x;
		private int y;

		private int endX;
		private int endY;

		private int dx;
		private int dy;
		private int sx;
		private int sy;
		private int err;

		private bool finished;

		public BresenhamIterator(Point start, Point end) {
			x = start.X;
			y = start.Y;
			endX = end.X;
			endY = end.Y;

			dx = Math.Abs(endX - x);
			dy = Math.Abs(endY - y);
			sx = x < endX ? 1 : -1;
			sy = y < endY ? 1 : -1;
			err = dx - dy;

			finished = false;
		}

		public bool HasNext() {
			return !finished;
		}

		public Point Next() {
			Point current = new(x,y);

			if (x == endX && y == endY) {
				finished = true;
				return current;
			}

			int e2 = 2 * err;
			if (e2 > -dy) {
				err -= dy;
				x += sx;
			}
			if (e2 < dx) {
				err += dx;
				y += sy;
			}

			return current;
		}
	}
}
