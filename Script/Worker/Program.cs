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
			// Name of the cockpit to use (or "" for the first one found).
			string Cockpit = "Cockpit";

			// Blocks to observe for payload (name of a group, a single block, or "" for all.
			string Container = "";

			// Speed of yaw rotation for the stabilizer (use 0.0f to lock).
			float YawSpeed = 0.5f;

			// ======== ======== ======== ======== ======== ======== ======== ========
			// END OF SETUP
			// ======== ======== ======== ======== ======== ======== ======== ========

			gWorker = new Worker(this, Cockpit, Container, YawSpeed);
		}

		private readonly Worker gWorker;

		public void Main(string argument, UpdateType updateSource)
		{
			if(argument == "stabilize")
				gWorker.SwitchStabilizer();
			gWorker.Update(updateSource);
		}
	}
}
