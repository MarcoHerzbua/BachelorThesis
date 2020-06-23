/*
 * Author: Christoph Birgmann
 * University of Applied Sciences Salzburg
 * MMP3
 */

using UnityEngine;
using Valve.VR;
using UnityEngine.UI;

public class MobilePhoneScroll : MonoBehaviour
{
	[SerializeField]
	private SteamVR_TrackedObject trackedObject;
	private SteamVR_Controller.Device inputDevice;

	[SerializeField]
	private Text scrollText;
	[SerializeField]
	private ScrollRect scrollRect;

	private void Update()
	{
		inputDevice = SteamVR_Controller.Input((int)trackedObject.index);

		if (inputDevice.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
		{
			Vector2 touchpad = (inputDevice.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));

			if (touchpad.y > 0.7f && ((RectTransform)scrollText.transform).localPosition.y > -200.0f)
			{
				((RectTransform)scrollText.transform).localPosition = new Vector3(((RectTransform)scrollText.transform).localPosition.x, ((RectTransform)scrollText.transform).localPosition.y - 10.0f, ((RectTransform)scrollText.transform).localPosition.z);
			}

			else if (touchpad.y < -0.7f && ((RectTransform)scrollText.transform).localPosition.y < 180.0f)
			{
				((RectTransform)scrollText.transform).localPosition = new Vector3(((RectTransform)scrollText.transform).localPosition.x, ((RectTransform)scrollText.transform).localPosition.y + 10.0f, ((RectTransform)scrollText.transform).localPosition.z);

			}

		}
	}
}
