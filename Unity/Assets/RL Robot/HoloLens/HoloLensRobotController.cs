using System;
using UnityEngine;
#if NETFX_CORE
using UnityEngine.VR.WSA.Input;
#endif
namespace Jake.RL.Robot.HoloLens
{
	public class HoloLensRobotController : MonoBehaviour
	{
		public Cycle cycle;
		public Target target;

		private bool started;
		private bool selecting;
#if NETFX_CORE
		private GestureRecognizer recognizer;
#endif
		void Start()
		{
#if NETFX_CORE
			recognizer = new GestureRecognizer();
			recognizer.SetRecognizableGestures(GestureSettings.Tap);
			recognizer.TappedEvent += (InteractionSourceKind source, int tapCount, Ray headRay) =>
			{
				if (!started)
				{
					started = true;
					cycle.Begin();
				}
				else
				{
					StopSelecting();
				}
			};

			recognizer.StartCapturingGestures();
#endif
		}

		void Update()
		{
			if (selecting)
			{
				try
				{
					var pos = GetHitPoint();
					target.transform.position = pos;
				}
				catch
				{
					target.transform.position = new Vector3(1000, 1000, 1000);
				}
			}
		}

		public void StartSelecting(bool start)
		{
			if (start)
			{
				cycle.Wait = selecting = true;
			}
		}

		public void StopSelecting()
		{
			if (selecting)
			{
				cycle.Wait = selecting = false;
			}
		}

		public Vector3 GetHitPoint()
		{
			var cam = Camera.main.transform;
			var hit = default(RaycastHit);
			if (Physics.Raycast(cam.position, cam.forward, out hit))
			{
				return hit.point;
			}
			else
			{
				throw new Exception();
			}
		}
	}
}
