using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace LibSmartCursor.API {
	/// <summary>
	/// Handle representing a registered smart cursor appliance.
	/// Can be used to unregister appliances in the future.
	/// </summary>
	public class ApplianceHandle {
		internal int id;
	}
	/// <summary>
	/// Registry for smart cursor appliances.
	/// Manages registration and lookup of appliances based on item predicates and priority.
	/// Accessed via LibSmartCursor.LibSmartCursor.Registry.
	/// </summary>
	public class SmartCursorRegistry {
		private static int nextHandleId = 0;
		/// <summary>
		/// High priority (25). Best used for highly specific or situational smart cursor behaviors.
		/// Lower numbers execute first.
		/// </summary>
		public const int PRIORITY_HIGH = 25;

		/// <summary>
		/// Normal priority (50). Default for most appliances.
		/// </summary>
		public const int PRIORITY_NORMAL = 50;

		/// <summary>
		/// Low priority (75). Use for broad/generic behavior that should be overridable.
		/// Higher numbers execute last.
		/// </summary>
		public const int PRIORITY_LOW = 75;


		private SortedList<int, List<(Func<Item,bool>, SmartCursorAppliance, int)>> rulesByPriority;

		/// <summary>
		/// Registers a smart cursor appliance with the given item predicate and priority.
		/// Appliances are executed in priority order (lower priority numbers first).
		/// For Appliances of equal priority, registration order determines execution order.
		/// Multiple appliances can match the same item - the first to find a target wins.
		/// </summary>
		/// <param name="itemPredicate">Function that returns true for items this appliance should handle</param>
		/// <param name="appliance">The appliance instance to register</param>
		/// <param name="priority">Priority value (lower executes first). Use PRIORITY_* constants or custom values. Expects something between 0 and 100.</param>
		/// <returns>A handle that can be used to unregister this appliance later</returns>
		/// <example>
		/// <code>
		/// // Register and save the handle for later unregistration
		/// var handle = LibSmartCursor.LibSmartCursor.Registry.RegisterAppliance(
		///     item => item.type == ItemID.StaffofRegrowth,
		///     new MyAppliance(),
		///     SmartCursorRegistry.PRIORITY_NORMAL
		/// );
		///
		/// // Later, conditionally unregister
		/// LibSmartCursor.LibSmartCursor.Registry.UnregisterAppliance(handle);
		/// </code>
		/// </example>
		public ApplianceHandle RegisterAppliance(Func<Item,bool> itemPredicate, SmartCursorAppliance appliance, int priority = PRIORITY_NORMAL) {
			int id = nextHandleId++;
			if (rulesByPriority == null) {
				rulesByPriority = new SortedList<int, List<(Func<Item,bool>, SmartCursorAppliance, int)>>();
			}

			if (!rulesByPriority.ContainsKey(priority)) {
				rulesByPriority[priority] = new List<(Func<Item,bool>, SmartCursorAppliance, int)>();
			}

			rulesByPriority[priority].Add((itemPredicate, appliance, id));
			return new ApplianceHandle { id = id };
		}

		/// <summary>
		/// Unregisters a previously registered appliance using its handle.
		/// This allows dynamic enabling/disabling of smart cursor behaviors without
		/// requiring conditional logic within the appliance itself.
		/// </summary>
		/// <param name="handle">The handle returned from RegisterAppliance</param>
		/// <example>
		/// <code>
		/// // Register in Load()
		/// ApplianceHandle myHandle;
		/// public override void Load() {
		///     myHandle = LibSmartCursor.LibSmartCursor.Registry.RegisterAppliance(
		///         item => item.type == ItemID.Pickaxe,
		///         new MyAppliance(),
		///         SmartCursorRegistry.PRIORITY_NORMAL
		///     );
		/// }
		///
		/// // Conditionally unregister based on some game state
		/// if (shouldDisableFeature) {
		///     LibSmartCursor.LibSmartCursor.Registry.UnregisterAppliance(myHandle);
		/// }
		/// </code>
		/// </example>
		public void UnregisterAppliance(ApplianceHandle handle) {
			foreach (var kvp in rulesByPriority) {
				foreach (var (itemPredicate, app, id) in kvp.Value) {
					if (id == handle.id) {
						kvp.Value.Remove((itemPredicate, app, id));
						return;
					}
				}
			}
		}

		internal void FillPreVanillaAppliances(Item item, ref List<SmartCursorAppliance> apps) {
			if (rulesByPriority == null) {
				return;
			}

			foreach (var kvp in rulesByPriority) {
				foreach (var (itemPredicate, app, id) in kvp.Value) {
					if (app.Behavior == ApplianceBehavior.PreVanilla && itemPredicate(item)) {
						apps.Add(app);
					}
				}
			}
		}

		internal void FillPostVanillaAppliances(Item item, ref List<SmartCursorAppliance> apps) {
			if (rulesByPriority == null) {
				return;
			}

			foreach (var kvp in rulesByPriority) {
				foreach (var (itemPredicate, app, id) in kvp.Value) {
					if (app.Behavior == ApplianceBehavior.PostVanilla && itemPredicate(item)) {
						apps.Add(app);
					}
				}
			}
		}

		internal void FillAppliances(Item item, ref List<SmartCursorAppliance> apps) {
			if (rulesByPriority == null) {
				return;
			}

			foreach (var kvp in rulesByPriority) {
				foreach (var (itemPredicate, app, id) in kvp.Value) {
					if (itemPredicate(item)) {
						apps.Add(app);
					}
				}
			}
		}
	}
}
