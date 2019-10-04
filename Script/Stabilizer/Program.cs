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
			// Speed of yaw rotation. Use 0.0f to lock.
			float YawSpeed = 0.5f;

			// ======== ======== ======== ======== ======== ======== ======== ========
			// END OF SETUP
			// ======== ======== ======== ======== ======== ======== ======== ========

			try {
				gStabilizer = new Stabilizer(this, YawSpeed);

				Runtime.UpdateFrequency = UpdateFrequency.Update100;
			}
			catch(Exception e) {
				Echo(e.Message);
			}
		}

		private readonly Stabilizer gStabilizer;

		public void Main(string argument, UpdateType updateSource)
		{
			if(argument == "stabilize")
				Runtime.UpdateFrequency = gStabilizer.Switch() ? UpdateFrequency.Update10 : UpdateFrequency.Update100;
			gStabilizer.Update();
		}
	}
}
