using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

public class TrackingWaypointManager : MonoBehaviour
{
    public TrackingWaypoint m_WaypointPrefab;
    public GameObject m_PlayerCar;
    //TrackingCurves needs to be split up, since to many points in a curve is a performance nightmare when editing the curve
    public List<BGCurve> m_TrackingCurves;

    private GameObject[] m_waypointArr;
    private int m_currentWaypointIdx;
    //Key: math Component - Value: Waypoint Idx on when the next math component should be used for calculation
    private Dictionary<BGCcMath, int> m_curveMath;
    private StreamWriter m_streamWriter;


    // Start is called before the first frame update
    void Start()
    {
        string filename = "/../Logs/Tracking_" + DateTime.Now.ToString("ddMM_HHmmss") + ".csv";
        m_streamWriter = new StreamWriter(Application.dataPath + filename);

        if (m_WaypointPrefab == null)
        {
            Debug.LogError("TrackingWaypointManager: WaypointPrefab not set as parameter");
            return;
        }

        if (m_PlayerCar == null)
        {
            Debug.LogError("TrackingWaypointManager: Playercar not set as parameter");
            return;
        }

        if (m_TrackingCurves == null)
        {
            Debug.LogError("TrackingWaypointManager: TrackingCurve not set as parameter");
            return;
        }

        int numPoints = 0;
        m_curveMath = new Dictionary<BGCcMath, int>(m_TrackingCurves.Count);
        foreach (var curve in m_TrackingCurves)
        {
            m_curveMath.Add(curve.GetComponent<BGCcMath>(), numPoints);

            numPoints += curve.Points.Length;
        }

        //Spawn waypoints on the curve points
        int pointCounter = 0;
        m_waypointArr = new GameObject[numPoints];
        foreach (var curve in m_TrackingCurves)
        {
            var curvePointsArr = curve.Points;
            for (int i = 0; i < curve.Points.Length; i++)
            {
                var waypoint = Instantiate(m_WaypointPrefab, curvePointsArr[i].PositionWorld, Quaternion.identity, this.transform);
                waypoint.OnWaypointEnter += ActivateNextWaypoint;
                m_waypointArr[pointCounter] = waypoint.gameObject;
                m_waypointArr[pointCounter].SetActive(false);
                pointCounter++;
            }
        }

        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    m_waypointArr[i] = transform.GetChild(i).gameObject;
        //    m_waypointArr[i].SetActive(false);
        //    m_waypointArr[i].GetComponent<TrackingWaypoint>().OnWaypointEnter += ActivateNextWaypoint;
        //}

        m_currentWaypointIdx = FindClosestWaypointIdx();
        m_waypointArr[m_currentWaypointIdx].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDestroy()
    {
        m_streamWriter.Close();
        
        foreach (var waypoint in m_waypointArr)
        {
            waypoint.GetComponent<TrackingWaypoint>().OnWaypointEnter -= ActivateNextWaypoint;
        }
    }

    public GameObject GetLastTriggeredWaypoint()
    {
        return m_currentWaypointIdx != 0 ? m_waypointArr[m_currentWaypointIdx - 1] : m_waypointArr.Last();
    }

    public GameObject GetCurrentWaypoint()
    {
        return m_waypointArr[m_currentWaypointIdx];
    }
    private GameObject FindClosestWaypoint()
    {
        return m_waypointArr[FindClosestWaypointIdx()];
    }

    private int FindClosestWaypointIdx()
    {
        Vector3 closestDistanceVec = Vector3.positiveInfinity;
        int closestWaypointIdx = int.MaxValue;

        for (int i = 0; i < m_waypointArr.Length; i++)
        {
            Vector3 distanceVec = m_waypointArr[i].transform.position - m_PlayerCar.transform.position;

            if (distanceVec.magnitude < closestDistanceVec.magnitude)
            {
                closestDistanceVec = distanceVec;
                closestWaypointIdx = i;
            }
        }

        return closestWaypointIdx;
    }

    private void ActivateNextWaypoint()
    {
        LogTrackingOffset();

        m_waypointArr[m_currentWaypointIdx].SetActive(false);

        m_currentWaypointIdx++;
        if (m_currentWaypointIdx > m_waypointArr.Length - 1)
        {
            m_currentWaypointIdx = 0;
        }

        m_waypointArr[m_currentWaypointIdx].SetActive(true);
    }

    private void LogTrackingOffset()
    {
        Vector3 playerPosition = m_PlayerCar.transform.position;

        BGCcMath mathCmpToUse = null;
        foreach (var math in m_curveMath)
        {
            if (m_currentWaypointIdx >= math.Value)
            {
                mathCmpToUse = math.Key;
            }
        }

        float trackingOffset = Vector3.Distance(mathCmpToUse.CalcPositionByClosestPoint(playerPosition), playerPosition);

        //Debug.Log("trackingOffset" + trackingOffset);

        m_streamWriter.WriteLine((m_currentWaypointIdx+ 1) + ";" + trackingOffset);
    }
}
