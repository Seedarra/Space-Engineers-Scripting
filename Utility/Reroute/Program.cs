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
		public class GoodClass
		{
			public GoodClass(Program parent)
			{
				parent.Echo("Reroute says hi!");
				parent.Echo("I support linebreaks with \"\\n\" but i also try to split up lines automatically that are to long to fit the screen.");
			}
		}

		public class BadClass
		{
			public BadClass(Program parent)
			{
				parent.Echo("Beware the bad class!");
				throw new Exception("BadClass class has failed (according to plan).");
			}
		}

		public Program()
		{
			Reroute console = new Reroute(this);

			try {
				console.Compile("Good Class");

				GoodClass gc = new GoodClass(this);

				console.Compile("Bad Class");

				BadClass bc = new BadClass(this);

				console.Report();
			}
			catch(Exception e) {
				console.Cancel(e.Message);
			}
		}

		public void Main()
		{
			
		}
	}
}
