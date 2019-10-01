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
		public class Blockfinder
		{
			public Blockfinder(Program parent) { gParent = parent; }

			public bool ByName<T>(string name, out T block) where T : class
			{
				block = gParent.GridTerminalSystem.GetBlockWithName(name) as T;
				
				return block != null && (block as IMyTerminalBlock).IsSameConstructAs(gParent.Me);
			}
			
			public bool ByGroupName<T>(string name, out List<T> block_list) where T : class
			{
				block_list = null;
				
				IMyBlockGroup group = gParent.GridTerminalSystem.GetBlockGroupWithName(name);
				
				if(group != null) {
					block_list = new List<T>();
				
					group.GetBlocksOfType(block_list, x => (x as IMyTerminalBlock).IsSameConstructAs(gParent.Me));
				
					return block_list.Count > 0;
				}
				else return false;
			}
			
			public bool ByType<T>(out T block) where T : class
			{
				block = null;
				
				List<T> block_list = new List<T>();
				
				gParent.GridTerminalSystem.GetBlocksOfType(block_list, x => (x as IMyTerminalBlock).IsSameConstructAs(gParent.Me));
				
				if(block_list.Count > 0) {
					block = block_list[0];
					return true;
				}
				else return false;
			}
			
			public bool ByType<T>(out List<T> block_list) where T : class
			{
				block_list = new List<T>();
				
				gParent.GridTerminalSystem.GetBlocksOfType(block_list, x => (x as IMyTerminalBlock).IsSameConstructAs(gParent.Me));
				
				return block_list.Count > 0;
			}
			
			public bool BySearch<T>(string term, out T block) where T : class
			{
				block = null;
				
				List<IMyTerminalBlock> block_list = new List<IMyTerminalBlock>();
				gParent.GridTerminalSystem.SearchBlocksOfName(term, block_list, x => x is T && x.IsSameConstructAs(gParent.Me));
				
				if(block_list.Count > 0) {
					block = block_list[0] as T;
					return true;
				}
				else return false;
			}
			
			public bool BySearch<T>(string term, out List<T> block_list) where T : class
			{
				List<IMyTerminalBlock> buffer = new List<IMyTerminalBlock>();
				gParent.GridTerminalSystem.SearchBlocksOfName(term, buffer, x => x is T && x.IsSameConstructAs(gParent.Me));
				
				block_list = buffer.Cast<T>().ToList();
				
				return block_list.Count > 0;
			}
			
			public bool ByCombination<T>(string name, out List<T> block_list) where T : class
			{
				block_list = new List<T>();
				
				IMyBlockGroup group = gParent.GridTerminalSystem.GetBlockGroupWithName(name);
				
				if(group == null) {
					T block = gParent.GridTerminalSystem.GetBlockWithName(name) as T;
					
					if(block != null && (block as IMyTerminalBlock).IsSameConstructAs(gParent.Me))
						block_list.Add(block);
				}
				else group.GetBlocksOfType(block_list, x => (x as IMyTerminalBlock).IsSameConstructAs(gParent.Me));

				return block_list.Count > 0;
			}
			
			public bool ByDefault<T>(string name, out List<T> block_list) where T : class
			{
				block_list = new List<T>();
				
				if(name != string.Empty) {
					IMyBlockGroup group = gParent.GridTerminalSystem.GetBlockGroupWithName(name);
					
					if(group == null) {
						T block = gParent.GridTerminalSystem.GetBlockWithName(name) as T;
						
						if(block != null && (block as IMyTerminalBlock).IsSameConstructAs(gParent.Me))
							block_list.Add(block);
					}
					else group.GetBlocksOfType(block_list, x => (x as IMyTerminalBlock).IsSameConstructAs(gParent.Me));
				}
				else gParent.GridTerminalSystem.GetBlocksOfType(block_list, x => (x as IMyTerminalBlock).IsSameConstructAs(gParent.Me));

				return block_list.Count > 0;
			}

			private readonly Program gParent;
		}
    }
}
