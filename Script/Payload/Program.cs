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
	partial class Program : MyGridProgram
	{
		public Program()
		{
			// ======== ======== ======== ======== ======== ======== ======== ========
			// SETUP
			// ======== ======== ======== ======== ======== ======== ======== ========
			// Blocks with inventory to observe (group name, single block, or "" for all):
			string Container = "";

			// Name of a cockpit, or "" to use the first one found:
			string Cockpit = "Cockpit";

			// Display number to use (0 for center panel):
			int DisplayNum = 0;

			// Display text:
			string Format =
				"Payload:\n" +
				"{0:0.0}%\n" +
				"Mass:\n" +
				"{1:0.00}kg\n";

			// ======== ======== ======== ======== ======== ======== ======== ========
			// END OF SETUP
			// ======== ======== ======== ======== ======== ======== ======== ========

			try {
				sFormat = Format;

				gPayload = new Payload(this, Container);

				if(Cockpit == string.Empty) {
					List<IMyCockpit> block_list = new List<IMyCockpit>();

					GridTerminalSystem.GetBlocksOfType(block_list, x => x.IsSameConstructAs(Me));

					if(block_list.Count > 0)
						gCockpit = block_list[0];
					else throw new Exception("No cockpit found.");
				}
				else if((gCockpit = GridTerminalSystem.GetBlockWithName(Cockpit) as IMyCockpit) == null || !gCockpit.IsSameConstructAs(Me))
					throw new Exception($"No blocks found with argument: '{Cockpit}'");

				if(DisplayNum >= 0 && DisplayNum < gCockpit.SurfaceCount) {
					gSurface = gCockpit.GetSurface(DisplayNum);
					gSurface.ContentType = ContentType.TEXT_AND_IMAGE;
				}
				else throw new Exception($"Variable 'DisplayNum' is out of range: 0 - {gCockpit.SurfaceCount - 1}");

				Runtime.UpdateFrequency = UpdateFrequency.Update100;
			}
			catch(Exception e) {
				Echo(e.Message);

				if(gPayload != null)
					Echo(string.Format(sFormat, gPayload.Percentage, gPayload.Mass));
			}
		}

		private readonly string sFormat = "";
		private readonly Payload gPayload;
		private readonly IMyCockpit gCockpit;
		private readonly IMyTextSurface gSurface;

		public void Main(string _, UpdateType updateSource)
		{
			if((updateSource & UpdateType.Update10) != 0) {
				if(gCockpit.IsUnderControl) {
					gPayload.Update();
					gSurface.WriteText(string.Format(sFormat, gPayload.Percentage, gPayload.Mass));
				}
				else Runtime.UpdateFrequency = UpdateFrequency.Update100;
			}
			else if(gCockpit.IsUnderControl)
				Runtime.UpdateFrequency = UpdateFrequency.Update10;
		}
	}
}
