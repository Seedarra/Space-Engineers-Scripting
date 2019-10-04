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
			// Name of a single rotor, a group of rotors, or "" for all.
			string Rotors = "";

			// Angle to increment/decrement in degree.
			float Angle = 15.0f;

			// ======== ======== ======== ======== ======== ======== ======== ========
			// END OF SETUP
			// ======== ======== ======== ======== ======== ======== ======== ========

			try {
				gRotor = new Rotor(this, Rotors);
				fAngle = Angle;
			}
			catch(Exception e) {
				Echo(e.Message);
			}
		}

		private readonly Rotor gRotor;
		private readonly float fAngle;

		public void Main(string argument)
		{
			switch(argument) {
				case "+":
					gRotor.AddAngle(fAngle);
					break;
				case "-":
					gRotor.AddAngle(-fAngle);
					break;
				case "reset":
					gRotor.SetAngle(0.0f);
					break;
				default:
					if(argument != string.Empty) {
						float angle;

						if(float.TryParse(argument, out angle))
							gRotor.SetAngle(angle);
					}
					break;
			}
		}
	}
}
