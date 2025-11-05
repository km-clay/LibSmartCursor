using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace LibSmartCursor {
	public class LibSmartCursorConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[Header("Debug")]
		[Label("Show Debug Overlay")]
		[Tooltip("Display which appliance and mod is handling the smart cursor target")]
		[DefaultValue(false)]
		public bool ShowDebugOverlay { get; set; }

		public override void OnChanged() {
			LibSmartCursor.DebugMode = ShowDebugOverlay;
		}
	}
}
