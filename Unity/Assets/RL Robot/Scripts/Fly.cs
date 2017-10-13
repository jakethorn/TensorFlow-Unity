using UnityEngine;

namespace Jake
{
	/// <summary>
	/// Attach to the main camera to fly around the game.
	/// </summary>
	public class Fly : MonoBehaviour
	{
		[Range(0, 10)]
		public float	lookSpeed = 1,
						moveSpeed = 1;

		private bool paused;

		void Update()
		{
			if (paused = paused ^ Input.GetKeyDown(KeyCode.Escape))
				return;

			var x = Input.GetAxis("Mouse X")	* lookSpeed;
			var y = Input.GetAxis("Mouse Y")	* lookSpeed;
			var h = Input.GetAxis("Horizontal")	* moveSpeed;
			var v = Input.GetAxis("Vertical")	* moveSpeed;

			transform.Rotate(0, x, 0, Space.World);
			transform.Rotate(-y, 0, 0, Space.Self);

			transform.Translate(h, 0, v);
		}
	}
}
