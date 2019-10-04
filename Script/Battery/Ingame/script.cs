/*
 * See Darra - 10/2019
 * Space Engineers - Battery
 * 1.0.0
 * 
 * Get power status of batteries on a display of your cockpit.
 */

public Program()
{
	// ======== ======== ======== ======== ======== ======== ======== ========
	// SETUP
	// ======== ======== ======== ======== ======== ======== ======== ========
	// Batteries to observe (group name, single block, or "" for all):
	string Batteries = "";

	// Name of a cockpit, or "" to use the first one found:
	string Cockpit = "Cockpit";

	// Display number to use (0 for center panel):
	int DisplayNum = 0;

	// Display text:
	string Format =
		"MWh:\n" +
		"{0:0.00} of {1:0.00}\n" +
		"Charge:\n" +
		"{2:0.0}%\n";

	// ======== ======== ======== ======== ======== ======== ======== ========
	// END OF SETUP
	// ======== ======== ======== ======== ======== ======== ======== ========

	try {
		sFormat = Format;

		gBattery = new Battery(this, Batteries);

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

		if(gBattery != null)
			Echo(string.Format(sFormat, gBattery.CurrentPower, gBattery.MaxPower, gBattery.Percentage));
	}
}

private readonly string sFormat = "";
private readonly Battery gBattery;
private readonly IMyCockpit gCockpit;
private readonly IMyTextSurface gSurface;

public void Main(string _, UpdateType updateSource)
{
	if((updateSource & UpdateType.Update10) != 0) {
		if(gCockpit.IsUnderControl) {
			gBattery.Update();
			gSurface.WriteText(string.Format(sFormat, gBattery.CurrentPower, gBattery.MaxPower, gBattery.Percentage));
		}
		else Runtime.UpdateFrequency = UpdateFrequency.Update100;
	}
	else if(gCockpit.IsUnderControl)
		Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public class Battery
{
	public float CurrentPower { get; protected set; }
	public float MaxPower { get; }
	public float Percentage { get; protected set; }

	public Battery(Program parent, string batteries = "")
	{
		// Find batteries:
		lBatteries = new List<IMyBatteryBlock>();

		if(batteries != string.Empty) {
			IMyBlockGroup group = parent.GridTerminalSystem.GetBlockGroupWithName(batteries);

			if(group == null) {
				IMyBatteryBlock block = parent.GridTerminalSystem.GetBlockWithName(batteries) as IMyBatteryBlock;

				if(block != null && block.IsSameConstructAs(parent.Me))
					lBatteries.Add(block);
			}
			else group.GetBlocksOfType(lBatteries, x => x.IsSameConstructAs(parent.Me));
		}
		else parent.GridTerminalSystem.GetBlocksOfType(lBatteries, x => x.IsSameConstructAs(parent.Me));

		MaxPower = 0.0f;

		if(lBatteries.Count == 0)
			throw new Exception($"No blocks found with argument: '{batteries}'");
		else foreach(IMyBatteryBlock block in lBatteries)
			MaxPower += block.MaxStoredPower;

		parent.Echo($"Batteries found: {lBatteries.Count}");

		Update();
	}

	public void Update()
	{
		CurrentPower = 0.0f;

		foreach(IMyBatteryBlock block in lBatteries)
			CurrentPower += block.CurrentStoredPower;

		Percentage = CurrentPower / MaxPower * 100.0f;
	}

	private readonly List<IMyBatteryBlock> lBatteries;
}