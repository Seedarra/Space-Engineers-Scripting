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
		public class Payload
		{
			public Payload(Program parent, string container = "")
			{
				// Retrieve blocks:
				List<IMyTerminalBlock> block_list = new List<IMyTerminalBlock>();

				if(container != string.Empty) {
					IMyBlockGroup group = parent.GridTerminalSystem.GetBlockGroupWithName(container);

					if(group == null) {
						IMyTerminalBlock block = parent.GridTerminalSystem.GetBlockWithName(container) as IMyTerminalBlock;

						if(block != null && block.IsSameConstructAs(parent.Me))
							block_list.Add(block);
					}
					else group.GetBlocksOfType(block_list, x => x.IsSameConstructAs(parent.Me));

					if(block_list.Count == 0)
						throw new Exception($"No blocks found with argument: '{container}'");
				}
				else parent.GridTerminalSystem.GetBlocksOfType(block_list, x => x.IsSameConstructAs(parent.Me));

				// Retrieve inventories:
				Capacity = 0.0f;

				lInventory = new List<IMyInventory>();

				foreach(IMyTerminalBlock block in block_list) {
					for(int i = 0; i < block.InventoryCount; ++i) {
						lInventory.Add(block.GetInventory(i));
						Capacity += (float)block.GetInventory(i).MaxVolume;
					}
				}

				if(lInventory.Count == 0)
					throw new Exception("Selected blocks do not have inventories.");
				else parent.Echo($"Observing {lInventory.Count} inventories from {block_list.Count} block(s).");

				Update();
			}

			public float Capacity { get; }
			public float Mass { get; protected set; }
			public float Volume { get; protected set; }
			public float Percentage { get; protected set; }

			public void Update()
			{
				Mass = Volume = 0.0f;

				foreach(IMyInventory inv in lInventory) {
					Mass += (float)inv.CurrentMass;
					Volume += (float)inv.CurrentVolume;
				}

				Percentage = Volume / Capacity * 100.0f;
			}

			private readonly List<IMyInventory> lInventory;
		}
	}
}
