/*
 * See Darra - 10/2019
 * Space Engineers - Worker
 * 1.0.0
 * 
 * This script is made for atmospheric worker ships (drilling, mining, 
 * grinding ships). It includes Battery, Payload and Stabilizer and
 * requires either the Standard or the Industrial Cockpit.
 * Add the programmable block with the script to your toolbar:
 * Run -> "stabilize" to use the Stabilizer.
 */

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

public class Reroute
{
	public Reroute(Program parent)
	{
		gParent = parent;
		gEcho = gParent.Echo;
		gScreen = gParent.Me.GetSurface(0);

		lBuffer = new List<string>();

		// Set up display:
		gScreen.Font = "Debug";
		gScreen.FontSize = gParent.Me.CubeGrid.GridSize == 2.5 ? 0.62f : 1.0f;
		gScreen.FontColor = new Color(179, 237, 255);
		gScreen.BackgroundColor = new Color(20, 20, 20);
		gScreen.TextPadding = 2.0f;
		gScreen.Alignment = TextAlignment.LEFT;
		gScreen.ContentType = ContentType.TEXT_AND_IMAGE;

		if(gScreen.GetText() != string.Empty && gScreen.GetText()[0] != '#')
			gScreen.WriteText("", false);

		// Determine screen size limits:
		Vector2 screensize = gScreen.SurfaceSize;

		if(gParent.Me.CubeGrid.GridSize != 2.5)
			screensize = new Vector2(472.0f, 512.0f);

		StringBuilder sb = new StringBuilder("|");

		float padding = 1.0f - (2.0f * gScreen.TextPadding / 100.0f);
		float height = gScreen.MeasureStringInPixels(sb, gScreen.Font, gScreen.FontSize).Y;

		vTextLimit = new Vector2(padding * screensize.X, (padding * screensize.Y) / height);
	}

	public void Compile(string user)
	{
		if(sUser != string.Empty)
			Report();

		sUser = user != string.Empty ? user : "<UNKNOWN>";

		lBuffer.Clear();
		lBuffer.Add($"# ./compile {(sUser.Contains(' ') ? $"\"{sUser}\"" : sUser)}");

		gParent.Echo = Echo;
	}

	public void Report()
	{
		if(sUser == string.Empty)
			return;

		lBuffer.Add($"# {sUser} ready.");

		gParent.Echo = gEcho;
		gParent.Echo($"{sUser} ready.");

		sUser = "";

		WriteBuffer();
	}

	public void Cancel(string message)
	{
		if(sUser != string.Empty) {
			sUser = "";

			gParent.Echo = gEcho;

			AppendToBuffer($"# !!! {message}");

			WriteBuffer();
		}
		gParent.Echo(message);
	}

	private readonly Program gParent;
	private readonly Action<string> gEcho;
	private readonly IMyTextSurface gScreen;
	private readonly Vector2 vTextLimit;
	private readonly List<string> lBuffer;

	private string sUser = "";

	private void Echo(string message)
	{
		if(sUser != string.Empty)
			AppendToBuffer($"# -> {message}");
	}

	private void AppendToBuffer(string message)
	{
		if(message[message.Length - 1] == '\n')
			message = message.Remove(message.Length - 1);
		message = message.Replace("\n", "\n# + ");

		List<string> text = new List<string>(message.Split('\n'));

		foreach(string line in text) {
			StringBuilder sb = new StringBuilder(line);

			float width = gScreen.MeasureStringInPixels(sb, gScreen.Font, gScreen.FontSize).X;

			if(width >= vTextLimit.X) {
				StringBuilder newline = new StringBuilder();

				List<string> words = new List<string>(line.Split(' '));

				float linewidth = 0.0f;

				foreach(string w in words) {
					sb.Clear();
					sb.Append(w + ' ');

					width = gScreen.MeasureStringInPixels(sb, gScreen.Font, gScreen.FontSize).X;

					if(linewidth + width >= vTextLimit.X) {
						lBuffer.Add(newline.ToString());

						newline.Clear();
						newline.Append("# + " + w + ' ');

						linewidth = gScreen.MeasureStringInPixels(sb, gScreen.Font, gScreen.FontSize).X;
					}
					else {
						newline.Append(w + ' ');

						linewidth += width;
					}
				}
				lBuffer.Add(newline.ToString());
			}
			else lBuffer.Add(line);
		}
	}

	private void WriteBuffer()
	{
		lBuffer.Add("#");

		int height = (int)vTextLimit.Y;

		if(lBuffer.Count < height) {
			string content = gScreen.GetText();

			if(content != string.Empty) {
				if(content.Length > 2)
					content = content.Substring(0, content.Length - 3);

				List<string> previous = new List<string>(content.Split('\n'));

				lBuffer.InsertRange(0, previous);
			}
		}

		if(lBuffer.Count > height)
			lBuffer.RemoveRange(0, lBuffer.Count - height);

		gScreen.WriteText("");

		foreach(string line in lBuffer)
			gScreen.WriteText(line + '\n', true);

		lBuffer.Clear();
	}
}

public class Stabilizer
{
	public Stabilizer(Program parent, float yaw_speed = 0.5f)
	{
		// Get ship controller:
		lController = new List<IMyShipController>();
		parent.GridTerminalSystem.GetBlocksOfType(lController, x => x.IsSameConstructAs(parent.Me));

		if((gActiveController = lController.Count > 0 ? lController[0] : null) == null)
			throw new Exception("No ship controller found.");

		// Get the gyroscope(s):
		lGyro = new List<IMyGyro>();
		parent.GridTerminalSystem.GetBlocksOfType(lGyro, x => x.IsSameConstructAs(parent.Me));

		if(lGyro.Count == 0)
			throw new Exception("No gyroscope(s) found.");
		else parent.Echo($"Number of gyroscopes: {lGyro.Count}");

		bActive = false;

		foreach(IMyGyro gyro in lGyro) {
			gyro.GyroOverride = bActive;
			gyro.Pitch = 0.0f;
			gyro.Roll = 0.0f;
			gyro.Yaw = 0.0f;
		}

		fSpeed = yaw_speed / 10.0f;
	}

	public bool Switch()
	{
		if((bActive = !bActive) && !gActiveController.IsUnderControl) {
			foreach(IMyShipController ctrl in lController)
				if(ctrl.IsUnderControl)
					gActiveController = ctrl;
		}

		foreach(IMyGyro gyro in lGyro) {
			gyro.GyroOverride = bActive;
			gyro.Pitch = 0.0f;
			gyro.Roll = 0.0f;
			gyro.Yaw = 0.0f;
		}
		return bActive;
	}

	public void Update()
	{
		if(bActive) {
			Vector3D gravity = Vector3D.Normalize(gActiveController.GetNaturalGravity());

			float pitch = (float)Vector3D.Dot(-gravity, gActiveController.WorldMatrix.Forward);
			float roll = (float)Vector3D.Dot(gravity, gActiveController.WorldMatrix.Left);
			float yaw = fSpeed * gActiveController.RotationIndicator.Y;

			Matrix ctrlOrientation;
			gActiveController.Orientation.GetMatrix(out ctrlOrientation);

			Vector3 pyr = new Vector3(pitch, yaw, roll);
			pyr = Vector3.Transform(pyr, ctrlOrientation);

			foreach(IMyGyro gyro in lGyro) {
				Matrix gyroOrientation;
				gyro.Orientation.GetMatrix(out gyroOrientation);
				Vector3 localRot = Vector3.Transform(pyr, MatrixD.Transpose(gyroOrientation));

				gyro.Pitch = localRot.X;
				gyro.Yaw = localRot.Y;
				gyro.Roll = localRot.Z;
			}
		}
	}

	private bool bActive;
	private readonly float fSpeed;
	private readonly List<IMyGyro> lGyro;
	private readonly List<IMyShipController> lController;
	private IMyShipController gActiveController;
}

public class Worker
{
	public Worker(Program parent, string cockpit = "", string container = "", float yaw_speed = 0.5f)
	{
		Reroute console = new Reroute((gParent = parent));

		try {
			console.Compile("Battery");
			gBattery = new Battery(gParent);

			console.Compile("Payload");
			gPayload = new Payload(gParent, container);

			console.Compile("Stabilizer");
			gStabilizer = new Stabilizer(gParent, yaw_speed);

			console.Compile("Worker");

			// Get a cockpit:
			if(cockpit == string.Empty) {
				List<IMyCockpit> cockpitblocks = new List<IMyCockpit>();
				gParent.GridTerminalSystem.GetBlocksOfType(cockpitblocks, x => x.IsSameConstructAs(gParent.Me));
				if(cockpitblocks.Count == 0)
					throw new Exception("No cockpit found.");
				else gCockpit = cockpitblocks[0];
			}
			else gCockpit = gParent.GridTerminalSystem.GetBlockWithName(cockpit) as IMyCockpit;

			if(gCockpit == null || !gCockpit.IsSameConstructAs(gParent.Me))
				throw new Exception($"No cockpit found with argument: '{cockpit}'");

			// Set up text surfaces:
			lSurface = null;

			switch(gCockpit.BlockDefinition.SubtypeName) {
				case "SmallBlockCockpit":
				case "LargeBlockCockpitSeat":
					lSurface = new List<IMyTextSurface> { gCockpit.GetSurface(0), gCockpit.GetSurface(1), gCockpit.GetSurface(2) };
					break;
				case "SmallBlockCockpitIndustrial":
					lSurface = new List<IMyTextSurface> { gCockpit.GetSurface(1), gCockpit.GetSurface(0), gCockpit.GetSurface(2) };
					break;
				case "LargeBlockCockpitIndustrial":
					lSurface = new List<IMyTextSurface> { gCockpit.GetSurface(2), gCockpit.GetSurface(1), gCockpit.GetSurface(3) };
					break;
				default:
					throw new Exception($"Cockpit '{gCockpit.CustomName}' is not a suitable cockpit.");
			}

			gParent.Echo($"Using cockpit: '{gCockpit.CustomName}'");

			foreach(IMyTextSurface s in lSurface) {
				s.Font = "Debug";
				s.FontSize = 3.0f;
				s.TextPadding = 20.0f;
				s.ContentType = ContentType.TEXT_AND_IMAGE;
				s.Alignment = TextAlignment.CENTER;
			}

			if(gCockpit.BlockDefinition.SubtypeName == "SmallBlockCockpit")
				lSurface[0].FontSize = 4.0f;

			// Done:
			SetCargoScreen();
			SetStabilizerScreen(false);
			SetBatteryScreen();

			console.Report();

			gParent.Runtime.UpdateFrequency = UpdateFrequency.Update100;
		}
		catch(Exception e) {
			console.Cancel(e.Message);
		}
	}

	public void Update(UpdateType updateType)
	{
		if((updateType & UpdateType.Update10) != 0) {
			if(gCockpit.IsUnderControl) {
				SetBatteryScreen();
				SetCargoScreen();
			}
			else gParent.Runtime.UpdateFrequency = UpdateFrequency.Update100;
		}
		else if(gCockpit.IsUnderControl)
			gParent.Runtime.UpdateFrequency = UpdateFrequency.Update10;

		gStabilizer.Update();
	}

	public void SwitchStabilizer()
	{ SetStabilizerScreen(gStabilizer.Switch()); }

	private readonly Battery gBattery;
	private readonly Payload gPayload;
	private readonly Stabilizer gStabilizer;

	private readonly Program gParent;
	private readonly IMyCockpit gCockpit;
	private readonly List<IMyTextSurface> lSurface;

	private int iStateCargoload = -1;
	private int iStateBattery = -1;

	private void SetCargoScreen()
	{
		gPayload.Update();

		SetDisplayColor(lSurface[0], (int)gPayload.Percentage, ref iStateCargoload);
		lSurface[0].WriteText(string.Format("Payload\n{0:0.00}%", gPayload.Percentage));
	}

	private void SetStabilizerScreen(bool state)
	{
		lSurface[1].WriteText("Stabilizer\n" + (state ? "<ON>" : "<OFF>"));
		lSurface[1].FontColor = state ? new Color(60, 210, 210) : new Color(210, 210, 210);
	}

	private void SetBatteryScreen()
	{
		gBattery.Update();

		SetDisplayColor(lSurface[2], 100 - (int)gBattery.Percentage, ref iStateBattery);
		lSurface[2].WriteText(string.Format("Battery\n{0:0.0}%", gBattery.Percentage));
	}

	private void SetDisplayColor(IMyTextSurface surface, int amount, ref int state)
	{
		if(amount > 90) {
			if(state != 3) {
				state = 3;

				surface.FontColor = new Color(210, 60, 60);
			}
		}
		else if(amount > 75) {
			if(state != 2) {
				state = 2;

				surface.FontColor = new Color(210, 120, 60);
			}
		}
		else if(amount > 50) {
			if(state != 1) {
				state = 1;

				surface.FontColor = new Color(210, 210, 60);
			}
		}
		else if(state != 0) {
			state = 0;

			surface.FontColor = new Color(60, 210, 60);
		}
	}
}