# Quick Start

### 1. Add LibSmartCursor as a dependency

In your mod's `build.txt`, add this line:
`modReferences = LibSmartCursor`

And in your mod's `csproj` file, add this inbetween the ItemGroup tags:
```
<Reference Include="LibSmartCursor">
	<HintPath>/path/to/LibSmartCursor.dll</HintPath>
</Reference>
```
The path referenced in the HintPath tags must point to the LibSmartCursor dll file. This can be obtained through tModLoader by extracting it.

### 2. Create a Smart Cursor Appliance

Let's make a `SmartCursorAppliance`. We'll make it find and target dirt blocks using Smart Cursor.
```csharp
using LibSmartCursor.API;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace YourMod.SmartCursor {
	public class DirtFinder : SmartCursorAppliance {
		protected override bool IsValidTile(SmartCursorContext ctx, Point tile) {
			// Check if this tile is dirt
			Tile t = Main.tile[tile.X, tile.Y];
			return t.HasTile && t.TileType == TileID.Dirt;
		}
	}
}
```

### 3. Register your appliance

Next, you'll need to call `LibSmartCursor.Registry.RegisterAppliance()` to hook your appliance into the registry. This is best performed in a place early in a mod's loading, like `ModSystem.PostSetupContent()` for instance.
```csharp
using Terraria.ModLoader;
using LibSmartCursor.API;
using Terraria.ID;

namespace YourMod.SmartCursor {
	public class DirtFinderSystem : ModSystem {
		public override void PostSetupContent() {
			LibSmartCursor.LibSmartCursor.Registry.RegisterAppliance(
				item => item.pick > 0, // When this returns true for the item the player is holding, your logic will run.
				new DirtFinder(), // Your appliance
				SmartCursorRegistry.PRIORITY_NORMAL // Execution order. Use PRIORITY_HIGH for specialized, situational behavior, and PRIORITY_LOW for general, widely used behavior.
			);
		}
	}
}
```

That's it. Your appliance will now cause Smart Cursor to prefer targeting dirt blocks when the player is holding a pickaxe. I say "prefer" here, because if your appliance does not find a valid tile, the execution will fall back to the vanilla Smart Cursor logic.

## Advanced Usage

### Custom Search Algorithm

You can override `FindTarget()` in `SmartCursorAppliance` to provide your own method of searching for tiles around the player. By default, `FindTarget()` uses a simple BFS that radiates outward from the player's position.

```csharp
protected override Point? FindTarget(SmartCursorContext ctx) {
	// Your custom search logic here
	// Returns the coordinates of a valid tile
	// The default implementation calls IsValidTile() in here.
}
```

### Restricting Tiles

Add tiles to `ctx.RestrictedTiles` to prevent other appliances from targeting them.
```csharp
protected override bool IsValidTile(SmartCursorContext ctx, Point tile) {
	if (IsDangerousTile(tile)) {
		ctx.RestrictedTiles.Add(tile);
		return false;
	}
	return true;
}
```

These restrictions can be ignored if you so choose, like so:
```csharp
protected override bool IsValidTile(SmartCursorContext ctx, Point tile) {
	// ...logic...
	ctx.RestrictedTiles.Remove(tile);
	return tile;
}
```

### Stateful Behavior

Use ModPlayer to track state across frames:

```csharp
public class MyModPlayer : ModPlayer {
	public Point? LastTargetedOre = null;
}

public class VeinMiner : SmartCursorAppliance {
	protected override bool IsValidTile(SmartCursorContext ctx, Point tile) {
		var modPlayer = ctx.Player.GetModPlayer<MyModPlayer>();
		// Use modPlayer.LastTargetedOre to track vein mining state
	}
}
```

### Priority Levels

You can specify different priority levels when registering your appliance to control the order of execution:
- `SmartCursorRegistry.PRIORITY_HIGH`: For specialized, situational behavior.
- `SmartCursorRegistry.PRIORITY_NORMAL`: For standard behavior.
- `SmartCursorRegistry.PRIORITY_LOW`: For general, widely used behavior.

### Execution Order

1. All `PreVanilla` appliances run in priority order.
2. Vanilla smart cursor logic runs
3. All `PostVanilla` appliances run in priority order.

Override the `Behavior` property in your appliance to change whether it runs before or after vanilla logic:
```csharp
public override ApplianceBehavior Behavior => ApplianceBehavior.PostVanilla;
```

Appliances will run one after another until one of them finds a valid target.
