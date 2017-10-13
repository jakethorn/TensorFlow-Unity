using UnityEngine;
using UnityEngine.UI;

public class BaseSlider : MonoBehaviour
{
	public Transform RobotBase;
	public Slider sliderTheta1;

	private float theta;

	// sets robot base slider to the appropriate starting position 
	void Start()
	{
		theta = RobotBase.rotation.y;
		sliderTheta1.value = theta;
	}
	
	//during an automated process, update the slider
	void Update() 
	{
		if (DHParameters.getMoveSlider())
		{
			sliderTheta1.value = DHParameters.getTheta1();
		}
	}

	//called when theta1 slider is moved.  rotates base
	public void SliderJoint0(float angle)
	{
		if (!DHParameters.getMoveSlider())
		{
			RobotBase.rotation = Quaternion.Euler(0f, -angle , 0f);
		}
	}
}
