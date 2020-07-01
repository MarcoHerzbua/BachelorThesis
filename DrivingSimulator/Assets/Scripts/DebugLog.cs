using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DebugLog : MonoBehaviour
{
    private List<InputDevice> inputDevices;
    // Start is called before the first frame update
    void Start()
    {
        inputDevices = new List<InputDevice>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (inputDevices.Count == 0)
        //{
        //    InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, inputDevices);
        //    //InputDevices.GetDevices(inputDevices);
        //    Debug.Log("Count HMD: " + inputDevices.Count);
        //}

        //if (inputDevices.Count > 0)
        //{
        //    Vector3 hmdPosition = Vector3.zero;
        //    Quaternion hmdRotation = Quaternion.identity;
        //    inputDevices[0].TryGetFeatureValue(CommonUsages.centerEyeRotation, out hmdRotation);

        //    Debug.Log("HMD Position: " + hmdRotation);
        //}
    }
}
