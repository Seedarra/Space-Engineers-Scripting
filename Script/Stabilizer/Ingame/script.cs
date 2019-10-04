/*
 * See Darra - 10/2019
 * Space Engineers - Stabilizer
 * 1.0.0
 * 
 * This script can be used to level out a ships pitch and roll rotations. 
 * It will utilize all gyroscopes found and needs a ship controller (cockpit 
 * or remote control) to work.
 * To use it, add the programmable block with the script to your ship's toolbar:
 * Run -> "stabilize" to toggle the Stabilizer on/off.
 */

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