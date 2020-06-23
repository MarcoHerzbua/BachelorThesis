/*
 * Author: Christoph Birgmann
 * University of Applied Sciences Salzburg
 * MMP3
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingCarHazardManager : MonoBehaviour
{
	[SerializeField]
	private List<HazardEventTrigger> parkingHazards;

	void Start ()
	{
		foreach (HazardEventTrigger parkingHazard in parkingHazards)
			parkingHazard.TryTriggerEvent += OnTryTriggerEvent;
	}

	private void OnTryTriggerEvent(HazardEventTrigger eventToTrigger)
	{
		// 50:50 Chance to Trigger Event
		if (Random.Range(0, 2) == 1)
			eventToTrigger.TriggerEvent();
	}

	private void OnDestroy()
	{
		foreach (HazardEventTrigger parkingHazard in parkingHazards)
			parkingHazard.TryTriggerEvent -= OnTryTriggerEvent;
	}
}
