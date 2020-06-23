/*
 * Author: Christoph Birgmann
 * University of Applied Sciences Salzburg
 * MMP3
 */

using UnityEngine;
using UnityEngine.UI;

public class CellPhoneTrigger : MonoBehaviour
{
	[SerializeField]
	private AudioSource mobilePhoneAudioSource;
	[SerializeField]
	private Text mobilePhoneText;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Player")
			return;

		mobilePhoneAudioSource.Play();
		mobilePhoneText.enabled = true;
	}
}
