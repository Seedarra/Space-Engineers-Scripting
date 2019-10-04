using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
	partial class Program
	{
		public class Battery
		{
			public float CurrentPower { get; protected set; }
			public float MaxPower { get; }
			public float Percentage { get; protected set; }

			public Battery(Program parent, string batteries = "")
			{
				// Find batteries:
				lBatteries = new List<IMyBatteryBlock>();

				if(batteries != string.Empty) {
					IMyBlockGroup group = parent.GridTerminalSystem.GetBlockGroupWithName(batteries);

					if(group == null) {
						IMyBatteryBlock block = parent.GridTerminalSystem.GetBlockWithName(batteries) as IMyBatteryBlock;

						if(block != null && block.IsSameConstructAs(parent.Me))
							lBatteries.Add(block);
					}
					else group.GetBlocksOfType(lBatteries, x => x.IsSameConstructAs(parent.Me));
				}
				else parent.GridTerminalSystem.GetBlocksOfType(lBatteries, x => x.IsSameConstructAs(parent.Me));

				MaxPower = 0.0f;

				if(lBatteries.Count == 0)
					throw new Exception($"No blocks found with argument: '{batteries}'");
				else foreach(IMyBatteryBlock block in lBatteries)
					MaxPower += block.MaxStoredPower;

				parent.Echo($"Batteries found: {lBatteries.Count}");

				Update();
			}

			public void Update()
			{
				CurrentPower = 0.0f;

				foreach(IMyBatteryBlock block in lBatteries)
					CurrentPower += block.CurrentStoredPower;

				Percentage = CurrentPower / MaxPower * 100.0f;
			}

			private readonly List<IMyBatteryBlock> lBatteries;
		}
	}
}
