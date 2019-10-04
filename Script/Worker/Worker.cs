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
		public class Worker
		{
			public Worker(Program parent, string cockpit = "", string container = "", float yaw_speed = 0.5f)
			{
				Reroute console = new Reroute((gParent = parent));

				try {
					console.Compile("Battery");
					gBattery = new Battery(gParent);

					console.Compile("Payload");
					gPayload = new Payload(gParent, container);

					console.Compile("Stabilizer");
					gStabilizer = new Stabilizer(gParent, yaw_speed);

					console.Compile("Worker");

					// Get a cockpit:
					if(cockpit == string.Empty) {
						List<IMyCockpit> cockpitblocks = new List<IMyCockpit>();
						gParent.GridTerminalSystem.GetBlocksOfType(cockpitblocks, x => x.IsSameConstructAs(gParent.Me));
						if(cockpitblocks.Count == 0)
							throw new Exception("No cockpit found.");
						else gCockpit = cockpitblocks[0];
					}
					else gCockpit = gParent.GridTerminalSystem.GetBlockWithName(cockpit) as IMyCockpit;

					if(gCockpit == null || !gCockpit.IsSameConstructAs(gParent.Me))
						throw new Exception($"No cockpit found with argument: '{cockpit}'");

					// Set up text surfaces:
					lSurface = null;

					switch(gCockpit.BlockDefinition.SubtypeName) {
						case "SmallBlockCockpit":
						case "LargeBlockCockpitSeat":
							lSurface = new List<IMyTextSurface> { gCockpit.GetSurface(0), gCockpit.GetSurface(1), gCockpit.GetSurface(2) };
							break;
						case "SmallBlockCockpitIndustrial":
							lSurface = new List<IMyTextSurface> { gCockpit.GetSurface(1), gCockpit.GetSurface(0), gCockpit.GetSurface(2) };
							break;
						case "LargeBlockCockpitIndustrial":
							lSurface = new List<IMyTextSurface> { gCockpit.GetSurface(2), gCockpit.GetSurface(1), gCockpit.GetSurface(3) };
							break;
						default:
							throw new Exception($"Cockpit '{gCockpit.CustomName}' is not a suitable cockpit.");
					}

					gParent.Echo($"Using cockpit: '{gCockpit.CustomName}'");

					foreach(IMyTextSurface s in lSurface) {
						s.Font = "Debug";
						s.FontSize = 3.0f;
						s.TextPadding = 20.0f;
						s.ContentType = ContentType.TEXT_AND_IMAGE;
						s.Alignment = TextAlignment.CENTER;
					}

					if(gCockpit.BlockDefinition.SubtypeName == "SmallBlockCockpit")
						lSurface[0].FontSize = 4.0f;

					// Done:
					SetCargoScreen();
					SetStabilizerScreen(false);
					SetBatteryScreen();

					console.Report();

					gParent.Runtime.UpdateFrequency = UpdateFrequency.Update100;
				}
				catch(Exception e) {
					console.Cancel(e.Message);
				}
			}

			public void Update(UpdateType updateType)
			{
				if((updateType & UpdateType.Update10) != 0) {
					if(gCockpit.IsUnderControl) {
						SetBatteryScreen();
						SetCargoScreen();
					}
					else gParent.Runtime.UpdateFrequency = UpdateFrequency.Update100;
				}
				else if(gCockpit.IsUnderControl)
					gParent.Runtime.UpdateFrequency = UpdateFrequency.Update10;

				gStabilizer.Update();
			}

			public void SwitchStabilizer()
			{ SetStabilizerScreen(gStabilizer.Switch()); }

			private readonly Battery gBattery;
			private readonly Payload gPayload;
			private readonly Stabilizer gStabilizer;

			private readonly Program gParent;
			private readonly IMyCockpit gCockpit;
			private readonly List<IMyTextSurface> lSurface;

			private int iStateCargoload = -1;
			private int iStateBattery = -1;

			private void SetCargoScreen()
			{
				gPayload.Update();

				SetDisplayColor(lSurface[0], (int)gPayload.Percentage, ref iStateCargoload);
				lSurface[0].WriteText(string.Format("Payload\n{0:0.00}%", gPayload.Percentage));
			}

			private void SetStabilizerScreen(bool state)
			{
				lSurface[1].WriteText("Stabilizer\n" + (state ? "<ON>" : "<OFF>"));
				lSurface[1].FontColor = state ? new Color(60, 210, 210) : new Color(210, 210, 210);
			}

			private void SetBatteryScreen()
			{
				gBattery.Update();

				SetDisplayColor(lSurface[2], 100 - (int)gBattery.Percentage, ref iStateBattery);
				lSurface[2].WriteText(string.Format("Battery\n{0:0.0}%", gBattery.Percentage));
			}

			private void SetDisplayColor(IMyTextSurface surface, int amount, ref int state)
			{
				if(amount > 90) {
					if(state != 3) {
						state = 3;

						surface.FontColor = new Color(210, 60, 60);
					}
				}
				else if(amount > 75) {
					if(state != 2) {
						state = 2;

						surface.FontColor = new Color(210, 120, 60);
					}
				}
				else if(amount > 50) {
					if(state != 1) {
						state = 1;

						surface.FontColor = new Color(210, 210, 60);
					}
				}
				else if(state != 0) {
					state = 0;

					surface.FontColor = new Color(60, 210, 60);
				}
			}
		}
	}
}
