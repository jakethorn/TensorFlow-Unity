using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace Jake.RL.Robot
{
	/// <summary>
	/// Robot controller.
	/// </summary>
	public class RobotController : MonoBehaviour
	{
		public Cycle	cycle;
		public Target	target;

		[Space]
		public Slider rot1Slider;
		public Slider rot2Slider;
		public Slider rot3Slider;
		
		[NonSerialized]
		public float	time,
						rot1,
						rot2,
						rot3;
		
		public bool Reset
		{
			set
			{
				if (value)
				{
					MoveToZero();
					target.MoveToRandomPosition();
				}
			}
		}

		public void MoveToZero()
		{
			rot1Slider.value = 0;
			rot2Slider.value = 0;
			rot3Slider.value = 0;
		}

		public void SetRotationsOverTime()
		{
			// stop cycle
			if (time > 0)
			{
				cycle.Wait = true;
			}

			// rotate robot
			StartCoroutine(SetRotationsOverTime_Coroutine());
		}

		IEnumerator SetRotationsOverTime_Coroutine()
		{
			rot1 = Math.ReduceAngle(rot1);
			
			// initial values
			var rot1Initial = rot1Slider.value;
			var rot2Initial = rot2Slider.value;
			var rot3Initial = rot3Slider.value;

			// change in values
			var rot1Delta = rot1 - rot1Initial;
			var rot2Delta = rot2 - rot2Initial;
			var rot3Delta = rot3 - rot3Initial;

			// go the shortest distance to the target
			if (rot1Delta > 180)
			{
				rot1Delta -= 360;
			}
			else if (rot1Delta < -180)
			{
				rot1Delta += 360;
			}
			
			// rotate
			var timeElapsed = 0f;
			var endTime = time;
			while (endTime > 0 && timeElapsed <= endTime)
			{
				var ratio = timeElapsed / endTime;

				rot1Slider.value = Math.ReduceAngle(rot1Initial + rot1Delta * ratio);
				rot2Slider.value = rot2Initial + rot2Delta * ratio;
				rot3Slider.value = rot3Initial + rot3Delta * ratio;

				yield return null;
				timeElapsed += Time.deltaTime;
			}

			// make sure they got to their target
			rot1Slider.value = Math.ReduceAngle(rot1);
			rot2Slider.value = rot2;
			rot3Slider.value = rot3;

			// continue cycle
			if (time > 0)
			{
				cycle.Wait = false;
			}
		}
	}
}
