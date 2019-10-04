/*
 * See Darra - 10/2019
 * Space Engineers - Payload
 * 1.0.0
 * 
 * Show the amount of cargo in percent and kilogram on a display of your cockpit.
 */

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