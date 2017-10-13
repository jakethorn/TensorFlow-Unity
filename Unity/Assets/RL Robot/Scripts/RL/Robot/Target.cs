using UnityEngine;

namespace Jake.RL.Robot
{
	/// <summary>
	/// Robots target.
	/// </summary>
	public class Target : MonoBehaviour
	{
		[Range(20, 40)] public float minRadius;
		[Range(20, 40)] public float maxRadius;
		
		[Space]
		[Range(0, 360)] public float leftLimit;
		[Range(0, 360)] public float rightLimit;

		[Space]
		[Range(0, 90)] public float	lowerLimit;
		[Range(0, 90)] public float	upperLimit;
		
		public void MoveToRandomPosition()
		{
			// place target at random position on sphere surface

			var left = Math.ReduceAngle(leftLimit);
			var right = Math.ReduceAngle(rightLimit);
			if (left > right)
			{
				left -= 360;
			}
			
			// set random rotation
			transform.localEulerAngles = new Vector3(
				Random.Range(-lowerLimit, -upperLimit),
				Random.Range(left, right),
				0
			);

			// place at zero and forward to spheres surface
			transform.localPosition = Vector3.zero;
			transform.Translate(0, 0, Random.Range(minRadius, maxRadius));
		}
	}
}
