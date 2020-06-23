/*
 * Author: Christoph Birgmann
 * University of Applied Sciences Salzburg
 * MMP3
 */

using UnityEngine;

public class LightSignSwitcher : MonoBehaviour
{
	[SerializeField]
	private LightSign lightSign;

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<RCC_AICarController>() != null)
			lightSign.SwitchToGreen();
	}
}
