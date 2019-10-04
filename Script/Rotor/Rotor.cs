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
	}
}
