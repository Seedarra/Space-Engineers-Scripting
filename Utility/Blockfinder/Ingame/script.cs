/*
 * See Darra - 09/2019
 * Space Engineers - Blockfinder
 * 1.0.0
 * 
 * This serves as a demonstration on how to use Blockfinder.
 */

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