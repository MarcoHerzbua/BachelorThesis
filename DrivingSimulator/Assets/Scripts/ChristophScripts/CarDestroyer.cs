/*
 * Author: Christoph Birgmann
 * University of Applied Sciences Salzburg
 * MMP3
 */

using UnityEngine;

public class CarDestroyer : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<RCC_AICarController>() != null)
			GameObject.Destroy(other.gameObject);
	}
}
