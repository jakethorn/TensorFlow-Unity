using System.Collections.Generic;
using UnityEngine;

namespace Jake.RL.Robot
{
	/// <summary>
	/// Debug robot target
	/// </summary>
	[ExecuteInEditMode]
	public class TargetDebug : MonoBehaviour
	{
		enum Frustum { Origin, LowerLeft, LowerRight, UpperLeft, UpperRight };

		public Target target;

		[Space]

		public Transform	innerLimit;
		public Transform	outerLimit;
		public LineRenderer	rotationLimit;
		
		void Update()
		{
			UpdateSpheres();
			UpdateLines();
		}
		
		private void UpdateSpheres()
		{
			innerLimit.localScale = new Vector3(
				target.minRadius * 2,
				target.minRadius * 2,
				target.minRadius * 2
			);

			outerLimit.localScale = new Vector3(
				target.maxRadius * 2,
				target.maxRadius * 2,
				target.maxRadius * 2
			);
		}

		private void UpdateLines()
		{
			var positions = new List<Vector3>
			{
				GetPosition(Frustum.Origin),
				GetPosition(Frustum.LowerLeft),
				GetPosition(Frustum.LowerRight),

				GetPosition(Frustum.Origin),
				GetPosition(Frustum.LowerRight),
				GetPosition(Frustum.UpperRight),

				GetPosition(Frustum.Origin),
				GetPosition(Frustum.UpperRight),
				GetPosition(Frustum.UpperLeft),

				GetPosition(Frustum.Origin),
				GetPosition(Frustum.UpperLeft),
				GetPosition(Frustum.LowerLeft),

				GetPosition(Frustum.Origin)
			};

			rotationLimit.positionCount = positions.Count;
			rotationLimit.SetPositions(positions.ToArray());
		}

		private Vector3 GetPosition(Frustum frustum)
		{
			var transform = rotationLimit.transform;

			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;

			var x = 0f;
			var y = 0f;
			switch (frustum)
			{
				case Frustum.Origin:
					return transform.position;
				case Frustum.LowerLeft:
					x = 360f - target.lowerLimit;
					y = target.leftLimit;
					break;
				case Frustum.LowerRight:
					x = 360f - target.lowerLimit;
					y = target.rightLimit;
					break;
				case Frustum.UpperLeft:
					x = 360f - target.upperLimit;
					y = target.leftLimit;
					break;
				case Frustum.UpperRight:
					x = 360f - target.upperLimit;
					y = target.rightLimit;
					break;
			}

			transform.Rotate(x, y, 0);
			transform.Translate(0, 0, target.maxRadius);
			return transform.position;
		}
	}
}
