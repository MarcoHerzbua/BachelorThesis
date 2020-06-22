/*
 * Author: Christoph Birgmann
 * University of Applied Sciences Salzburg
 * MMP3
 */

using UnityEngine;

public class LightSign : MonoBehaviour
{
	[SerializeField]
	private Light redLight;
	[SerializeField]
	private Light yellowLight;
	[SerializeField]
	private Light greenLight;

	private const float YELLOW_LIGHT_TIME = 1.0f;

	private float timeToSwitch = 0.0f;

	private bool switchLights;

	private void Update()
	{
		if (switchLights)
		{
			timeToSwitch += Time.deltaTime;

			if (timeToSwitch >= YELLOW_LIGHT_TIME)
			{
				yellowLight.gameObject.SetActive(false);
				greenLight.gameObject.SetActive(true);
				switchLights = false;
				timeToSwitch = 0.0f;
			}
		}
	}

	public void SwitchToGreen()
	{
		redLight.gameObject.SetActive(false);
		yellowLight.gameObject.SetActive(true);
		switchLights = true;
	}

}
