# LibSmartCursor

A [tModLoader](https://github.com/tModLoader/tModLoader) library that provides a clean, extensible API for customizing Terraria's Smart Cursor behavior.

tModLoader provides no built-in API for extending smart cursor behavior for tools and tile-based items. Modders are forced to detour behavior for vanilla's `SmartCursorHelper.SmartCursorLookup` method directly, which is:
* Complex - Requires understanding vanilla's messy 500+ line implementation.
* Error-prone - Manual handling of `ref int` in many places can prove to be unwieldy.
* Incompatible - Multiple mods detouring the same method is never ideal.

LibSmartCursor provides a **Smart Cursor Registry** that allows any number of mods to register custom **Smart Cursor Appliances** simultaneously through a simple, composable API.

## Features

- **Priority-based execution** - Control when your logic runs relative to other registered smart cursor appliances.
- **Tile Restriction** - Ability to signal to other registered appliances that specific tiles are off limits (overridable).
- **Stateful behaviors** - Allows for easy two-way communication between smart cursor logic and the rest of the tModLoader API. For instance, you can track state using a ModPlayer instance, and then refer to that state in the smart cursor appliance.
- **Simple API** - Implementing new smart cursor appliances is trivial. Override a single method to have a working appliance ready to be registered. You get the tile search algorithm for free.

## Examples
For some example implementations, check out my other mod [Smart Cursor Rewritten](https://github.com/km-clay/SmartCursorRewritten), which includes:
- **Rope Placement** - Targets ropes in the world while you have rope equipped.
- **Hellevator Guide** - Ensures that your hellevator is aligned as you dig it, so you no longer have to eyeball alignment.
- **Vein Mining** - Uses a stateful implementation to find all tiles in an ore vein and heavily prioritize them.

## Quick Start
