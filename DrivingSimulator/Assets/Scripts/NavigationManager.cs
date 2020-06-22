/*
 * Author: Christoph Birgmann
 * University of Applied Sciences Salzburg
 * MMP3
 */

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class NavigationManager : MonoBehaviour
{
	[SerializeField]
	private List<NavigationTrigger> navigationTriggers;

	[SerializeField]
	private Text navigationText;

	void Start()
	{
		foreach (NavigationTrigger navigationTrigger in navigationTriggers)
		{
			navigationTrigger.OnNavigationTriggerEnter += ShowNavigationMessage;
			navigationTrigger.OnNavigationTriggerExit += ShowNavigationMessage;
		}
	}

	private void ShowNavigationMessage(NavigationTrigger.NavigationDirection direction)
	{
		if (direction == NavigationTrigger.NavigationDirection.None)
			navigationText.gameObject.SetActive(false);
		else
		{
			switch (direction)
			{
				case NavigationTrigger.NavigationDirection.Left:
					navigationText.text = "<-- TURN LEFT";
					break;
				case NavigationTrigger.NavigationDirection.Right:
					navigationText.text = "TURN RIGHT -->";
					break;
				case NavigationTrigger.NavigationDirection.Straight:
					navigationText.text = "GO STRAIGHT";
					break;
				default:
					Debug.LogError("Unknown direction.");
					return;
			}

			navigationText.gameObject.SetActive(true);
		}
	}

	private void OnDestroy()
	{
		foreach (NavigationTrigger navigationTrigger in navigationTriggers)
		{
			navigationTrigger.OnNavigationTriggerEnter -= ShowNavigationMessage;
			navigationTrigger.OnNavigationTriggerExit -= ShowNavigationMessage;
		}
	}
}
