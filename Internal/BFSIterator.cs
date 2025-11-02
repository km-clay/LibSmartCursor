using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;

namespace LibSmartCursor.Internal {
	internal class BFSIterator {
		private PriorityQueue<Point, float> toVisit;
		private HashSet<Point> visited;
		private Point center;
		private Rectangle bounds;
		private int maxRadius;
		private bool square;

		public BFSIterator(Point start, Rectangle bounds, int maxRadius, bool square = false) {
			this.center = start;
			this.bounds = bounds;
			this.maxRadius = maxRadius;
			this.square = square;
			toVisit = new PriorityQueue<Point, float>();
			visited = new HashSet<Point>();
			if (bounds.Contains(start)) {
				toVisit.Enqueue(start, 0);
			}
		}

		public bool HasNext() {
			return toVisit.Count > 0;
		}

		private bool NeighborIsValid(Point neighbor) {
			return !visited.Contains(neighbor) && bounds.Contains(neighbor);
		}

		private Point[] GetNeighbors(Point p) {
			if (square) {
				return new Point[] {
					new Point(p.X + 1, p.Y),
					new Point(p.X - 1, p.Y),
					new Point(p.X, p.Y + 1),
					new Point(p.X, p.Y - 1),
					new Point(p.X + 1, p.Y + 1),
					new Point(p.X - 1, p.Y - 1),
					new Point(p.X + 1, p.Y - 1),
					new Point(p.X - 1, p.Y + 1)
				};
			} else {
				return new Point[] {
					new Point(p.X + 1, p.Y),
					new Point(p.X - 1, p.Y),
					new Point(p.X, p.Y + 1),
					new Point(p.X, p.Y - 1),
				};
			}
		}

		public Point Next() {
			Point current = toVisit.Dequeue();

			foreach(Point neighbor in GetNeighbors(current)) {
				if (NeighborIsValid(neighbor)) {
					float nDist = Vector2.Distance(neighbor.ToVector2(), center.ToVector2());
					toVisit.Enqueue(neighbor, nDist);
					visited.Add(neighbor);
				}
			}

			return current;
		}
	}
}
