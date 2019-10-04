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
	}
}
