/*
 * Author: Christoph Birgmann
 * University of Applied Sciences Salzburg
 * MMP3
 */

using UnityEngine;

public class BusEventTrigger : HazardEventTrigger
{
	[SerializeField]
	private float indicatorTime = 1.0f;

	private bool displayIndicator = false;

	void Update()
	{
		if (displayIndicator)
		{
			indicatorTime -= Time.deltaTime;

			if (indicatorTime <= 0.0f)
			{
				carController.indicatorsOn = RCC_CarControllerV3.IndicatorsOn.Off;
				displayIndicator = false;
				aiController.enabled = true;
			}
		}
	}

	protected override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);

		carController.indicatorsOn = RCC_CarControllerV3.IndicatorsOn.Right;
		aiController.enabled = false;

		displayIndicator = true;
	}
}
