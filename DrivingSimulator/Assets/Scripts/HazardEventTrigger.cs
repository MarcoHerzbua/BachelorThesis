/*
 * Author: Christoph Birgmann
 * University of Applied Sciences Salzburg
 * MMP3
 */

using System;
using UnityEngine;

public class HazardEventTrigger : MonoBehaviour
{
	[SerializeField]
	protected RCC_CarControllerV3 carController;
	[SerializeField]
	protected RCC_AICarController aiController;

	public Action<HazardEventTrigger> TryTriggerEvent;

	public void TriggerEvent()
	{
		carController.enabled = true;
		aiController.enabled = true;
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		if (carController == null || aiController == null)
			return;

		if (other.tag != "Player")
			return;

		if (TryTriggerEvent != null)
			TryTriggerEvent(this);
		else
			TriggerEvent();
	}

}
