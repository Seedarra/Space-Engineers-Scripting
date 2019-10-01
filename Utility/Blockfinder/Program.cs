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
			Blockfinder finder = new Blockfinder(this);
			
			// By name:
			IMyTerminalBlock Block;
			
			if(!finder.ByName("Cockpit", out Block))
				Echo("ByName() returned false.");
			else Echo("ByName() has found : " + Block.CustomName);
			
			// By group name:
			List<IMyThrust> GroupBlocks;
			
			if(!finder.ByGroupName("Thrusters", out GroupBlocks))
				Echo("ByGroupName() returned false.");
			else Echo(string.Format("ByGroupName() has found a group with {0} matching block(s).", GroupBlocks.Count));

			// First by type:
			IMyBatteryBlock FirstBattery;
			
			if(!finder.ByType(out FirstBattery))
				Echo("ByType(BLOCK) returned false.");
			else Echo("ByType(BLOCK) has found a block named '" + FirstBattery.CustomName + "'");
			
			// All by type:
			List<IMyBatteryBlock> AllBatteries;
			
			if(!finder.ByType(out AllBatteries))
				Echo("ByType(LIST) returned false.");
			else Echo(string.Format("ByType(LIST) has found {0} block(s).", AllBatteries.Count));
			
			// Search first:
			IMyThrust SearchBlock;
		
			if(!finder.BySearch("Thrus", out SearchBlock))
				Echo("BySearch(BLOCK) returned false.");
			else Echo(string.Format("BySearch(BLOCK) has found the block '" + SearchBlock.CustomName + "'"));
			
			// Search all:
			List<IMyThrust> SearchList;
			
			if(!finder.BySearch("Thrus", out SearchList))
				Echo("BySearch(LIST) returned false.");
			else Echo(string.Format("BySearch(LIST) has found {0} block(s).", SearchList.Count));
			
			// Default:
			List<IMyTerminalBlock> DefaultList;
			
			if(!finder.ByDefault("", out DefaultList))
				Echo("ByDefault() returned false.");
			else Echo(string.Format("ByDefault() has found {0} block(s).", DefaultList.Count));
        }

        public void Main()
        {

        }
    }
}
