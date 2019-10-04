/*
 * See Darra - 10/2019
 * Space Engineers - Rotor
 * 1.0.0
 * 
 * You can use this script to control one or more rotors.
 * 
 * 1.Place down your rotor(s).
 * 2.Change the velocity setting(s) of the rotor(s) to your liking.
 * 3.Give the rotor a name/assign the rotors to a group.
 * 4.Change the 'Rotors' variable to this name (or leave it with "" for all).
 * 5.Add the programmable block to your ship/station toolbar with Run -> ARGUMENT.
 * 
 * ARGUMENT can be:
 * +		Increment angle (rotate clockwise).
 * -		Decrement angle (rotate counter-clockwise).
 * reset	Rotate back to 0°.
 * 4.20	Any number of your choice.
 */

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

public class Rotor
{
	public Rotor(Program parent, string name = "")
	{
		lRotor = new List<IMyMotorStator>();

		if(name != string.Empty) {
			IMyBlockGroup group = parent.GridTerminalSystem.GetBlockGroupWithName(name);

			if(group == null) {
				IMyMotorStator block = parent.GridTerminalSystem.GetBlockWithName(name) as IMyMotorStator;

				if(block != null && block.IsSameConstructAs(parent.Me))
					lRotor.Add(block);
			}
			else group.GetBlocksOfType(lRotor, x => x.IsSameConstructAs(parent.Me));

			if(lRotor.Count == 0)
				throw new Exception($"No blocks found with argument: '{name}'");
		}
		else parent.GridTerminalSystem.GetBlocksOfType(lRotor, x => x.IsSameConstructAs(parent.Me));

		if(lRotor.Count == 0)
			throw new Exception($"No rotors found.");
	}

	public void SetAngle(float angle)
	{
		fAngle = MathHelper.ToRadians(angle);
		Rotate();
	}

	public void AddAngle(float angle)
	{
		fAngle += MathHelper.ToRadians(angle);
		Rotate();
	}

	public float CurrentAngle
	{ get { return MathHelper.ToDegrees(fAngle); } }

	private readonly List<IMyMotorStator> lRotor;
	private float fAngle = 0.0f;

	private void Rotate()
	{
		MathHelper.LimitRadians(ref fAngle);

		foreach(IMyMotorStator motor in lRotor) {
			float currentAngle = motor.Angle;
			MathHelper.LimitRadians(ref currentAngle);

			float currentVelocity = Math.Abs(motor.TargetVelocityRad);

			if((fAngle > currentAngle && fAngle - currentAngle < MathHelper.Pi) ||
			   (fAngle < currentAngle && currentAngle - fAngle > MathHelper.Pi)) {
				motor.LowerLimitRad = 0.0f;
				motor.UpperLimitRad = fAngle;
				motor.TargetVelocityRad = currentVelocity;
			}
			else {
				motor.UpperLimitRad = MathHelper.TwoPi;
				motor.LowerLimitRad = fAngle;
				motor.TargetVelocityRad = -currentVelocity;
			}
		}
	}
}