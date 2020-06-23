/*
 * Author: Christoph Birgmann
 * University of Applied Sciences Salzburg
 * MMP3
 */

using UnityEngine;
using System;

public class NavigationTrigger : MonoBehaviour
{
	public enum NavigationDirection
	{
		None,
		Straight,
		Left,
		Right
	}

	public Action<NavigationDirection> OnNavigationTriggerEnter;
	public Action<NavigationDirection> OnNavigationTriggerExit;

	[SerializeField]
	private NavigationDirection navigationDirection;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Player")
			return;

		if (OnNavigationTriggerEnter != null)
			OnNavigationTriggerEnter(navigationDirection);
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag != "Player")
			return;

		if (OnNavigationTriggerExit != null)
			OnNavigationTriggerExit(NavigationDirection.None);
	}
}
