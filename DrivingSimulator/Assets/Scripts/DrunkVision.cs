/*
 * Script Source: Drunk Man Asset Pack
 * Link: https://assetstore.unity.com/packages/vfx/shaders/fullscreen-camera-effects/drunk-man-47438
 *
 * modified by: Marco Herzog
 * University of Applied Sciences Salzburg
 * Bachelor Thesis 2
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DrunkVision : MonoBehaviour
{
    private List<InputDevice> m_HMDs;
    private Quaternion m_hmdRotation;
    private float m_sleepyEyeTimer = 0f;
    private float m_ghostSeeTimer = 0f;

    private bool m_shaderActive = true;
    //private float m_ghostSeeMagnitude = 0f;

    [Header("Custom Params")] public float m_EyeCloseTime = 3.0f;
    public float m_EyeOpenModifier = 5f;
    public float m_GhostSeeTime = 1.0f;
    public float m_GhostSeeModifier = 0.5f;
    public float m_MaxGhostSeeRadius = 0.005f;

    [Header("Shader Material")]
    public Material m_Mat;

    [Header("RGB Shift")] [Range(0f, 0.05f)]
    public float m_RGBShiftFactor = 0;

    [Range(1f, 16f)] public float m_RGBShiftPower = 3f;
    [Header("Ghost")] [Range(0f, 0.06f)] public float m_GhostSeeRadius = 0.01f;
    [Range(0.01f, 1f)] public float m_GhostSeeMix = 0.5f;
    [Range(0.01f, 0.2f)] public float m_GhostSeeAmplitude = 0.05f;

    [Header("Distortion")] [Range(0.5f, 8f)]
    public float m_Frequency = 1f;

    [Range(0.1f, 4f)] public float m_Period = 1.5f;
    [Range(1f, 16f)] public float m_Amplitude = 1f;

    [Header("Radial Blur")] [Range(0f, 1f)]
    public float m_BlurMin = 0.1f;

    [Range(0f, 1f)] public float m_BlurMax = 0.3f;
    [Range(1f, 6f)] public float m_BlurSpeed = 3f;
    [Header("SleepyEye")] public bool m_SleepyEye = false;
    [Range(0f, 0.9f)] public float m_EyeClose = 0.2f;

    public enum EType
    {
        ET_Rotated = 0,
        ET_Splitted
    };

    public EType m_Type = EType.ET_Rotated;

    // Start is called before the first frame update
    void Start()
    {
        m_HMDs = new List<InputDevice>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_HMDs.Count == 0)
        {
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, m_HMDs);

            //Debug.Log("Count HMD: " + m_HMDs.Count);
        }
        else
        {
            UpdateVisionEffects();
        }


        m_Mat.SetFloat("_RGBShiftFactor", m_RGBShiftFactor);
        m_Mat.SetFloat("_RGBShiftPower", m_RGBShiftPower);
        m_Mat.SetFloat("_GhostSeeRadius", m_GhostSeeRadius);
        m_Mat.SetFloat("_GhostSeeMix", m_GhostSeeMix);
        m_Mat.SetFloat("_GhostSeeAmplitude", m_GhostSeeAmplitude);
        //		float strength = Mathf.Sin (Time.time) * 0.5f + 0.5f + 0.1f;
        m_Mat.SetVector("_Dimensions", new Vector4(0.8f, m_EyeClose, 0f, 0f));
        m_Mat.SetFloat("_Frequency", m_Frequency);
        m_Mat.SetFloat("_Period", m_Period);
        m_Mat.SetFloat("_RandomNumber", 1f);
        m_Mat.SetFloat("_Amplitude", m_Amplitude);
        m_Mat.SetFloat("_BlurMin", m_BlurMin);
        m_Mat.SetFloat("_BlurMax", m_BlurMax);
        m_Mat.SetFloat("_BlurSpeed", m_BlurSpeed);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        RenderTexture rt1 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
        RenderTexture rt2 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);

        Graphics.Blit(src, rt1, m_Mat, 0);
        if (EType.ET_Rotated == m_Type)
        {
            if (m_SleepyEye)
            {
                Graphics.Blit(rt1, rt2, m_Mat, 1);
                Graphics.Blit(rt2, rt1, m_Mat, 4);
                Graphics.Blit(rt1, rt2, m_Mat, 5);
                Graphics.Blit(rt2, dst, m_Mat, 3);
            }
            else
            {
                Graphics.Blit(rt1, rt2, m_Mat, 1);
                Graphics.Blit(rt2, rt1, m_Mat, 4);
                Graphics.Blit(rt1, dst, m_Mat, 5);
            }
        }
        else
        {
            if (m_SleepyEye)
            {
                Graphics.Blit(rt1, rt2, m_Mat, 2);
                Graphics.Blit(rt2, rt1, m_Mat, 4);
                Graphics.Blit(rt1, rt2, m_Mat, 5);
                Graphics.Blit(rt2, dst, m_Mat, 3);
            }
            else
            {
                Graphics.Blit(rt1, rt2, m_Mat, 2);
                Graphics.Blit(rt2, rt1, m_Mat, 4);
                Graphics.Blit(rt1, dst, m_Mat, 5);
            }
        }

        RenderTexture.ReleaseTemporary(rt1);
        RenderTexture.ReleaseTemporary(rt2);
    }


    private void UpdateVisionEffects()
    {
        m_sleepyEyeTimer += Time.deltaTime;
        m_ghostSeeTimer += Time.deltaTime;

        //Vector3 hmdPosition = Vector3.zero;
        Quaternion newHmdRotation = Quaternion.identity;
        m_HMDs[0].TryGetFeatureValue(CommonUsages.centerEyeRotation, out newHmdRotation);

        if (newHmdRotation != Quaternion.identity)
        {
            //Quaternion.x = Yaw
            //Quaternion.y = Pitch

            float newYaw = newHmdRotation.x;
            float newPitch = newHmdRotation.y;

            float offsetYaw = Mathf.Abs(m_hmdRotation.x - newYaw);
            float offsetPitch = Mathf.Abs(m_hmdRotation.y - newPitch);

            m_sleepyEyeTimer -= (offsetPitch + offsetYaw) * m_EyeOpenModifier; //more rotation of the HMD increases the time it takes for the sleepy eye to kick in
            m_sleepyEyeTimer = Mathf.Clamp(m_sleepyEyeTimer, 0f, m_EyeCloseTime); //value should not be < 0 or > than the max time to close the eyes

            m_EyeClose = Mathf.Clamp((m_EyeCloseTime - m_sleepyEyeTimer) / m_EyeCloseTime, 0.0f, 0.9f);

            //m_ghostSeeTimer = m_ghostSeeTimer > m_GhostSeeTime ? m_GhostSeeTime : m_ghostSeeTimer; //value should not exceed max ghost see time

            m_ghostSeeTimer -= (offsetPitch + offsetYaw) * m_GhostSeeModifier; //same as with the sleepy eye, except more HMD rotation intensifies the ghost see effect
            m_ghostSeeTimer = Mathf.Clamp(m_ghostSeeTimer, 0f, m_GhostSeeTime);

            float ghostSeeMagnitude = 1f - Mathf.Clamp(m_ghostSeeTimer / m_GhostSeeTime, 0.0f, 1.0f); //defines the intensity of the ghost see effect
            m_GhostSeeRadius = ghostSeeMagnitude * m_MaxGhostSeeRadius;

            m_hmdRotation = newHmdRotation;
        }
    }
}