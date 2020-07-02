using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

public class TrackingWaypointManager : MonoBehaviour
{
    public TrackingWaypoint m_WaypointPrefab;
    public GameObject m_PlayerCar;
    public BGCurve m_TrackingCurve;

    private GameObject[] m_waypointArr;
    private int m_currentWaypointIdx;
    private BGCcMath m_curveMath;
    private StreamWriter m_streamWriter;


    // Start is called before the first frame update
    void Start()
    {
        m_streamWriter = new StreamWriter(Application.dataPath + "/../Logs/TestFile.csv");

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

        if (m_TrackingCurve == null)
        {
            Debug.LogError("TrackingWaypointManager: TrackingCurve not set as parameter");
            return;
        }

        m_curveMath = m_TrackingCurve.GetComponent<BGCcMath>();
        if (m_curveMath == null)
        {
            Debug.LogError("TrackingWaypointManager: No Math component in the BGCurve Object");
            return;
        }

        //Spawn waypoints on the curve points
        var curvePointsArr = m_TrackingCurve.Points;
        m_waypointArr = new GameObject[curvePointsArr.Length];
        for (int i = 0; i < m_waypointArr.Length; i++)
        {
            var waypoint = Instantiate(m_WaypointPrefab, curvePointsArr[i].PositionWorld, Quaternion.identity, this.transform);
            waypoint.OnWaypointEnter += ActivateNextWaypoint;
            m_waypointArr[i] = waypoint.gameObject;
            m_waypointArr[i].SetActive(false);
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
        MeasureTracking();

        m_waypointArr[m_currentWaypointIdx].SetActive(false);

        m_currentWaypointIdx++;
        if (m_currentWaypointIdx > m_waypointArr.Length - 1)
        {
            m_currentWaypointIdx = 0;
        }

        m_waypointArr[m_currentWaypointIdx].SetActive(true);
    }

    private void MeasureTracking()
    {
        Vector3 playerPosition = m_PlayerCar.transform.position;
        float trackingOffset = Vector3.Distance(m_curveMath.CalcPositionByClosestPoint(playerPosition), playerPosition);

        //Debug.Log("trackingOffset" + trackingOffset);

        m_streamWriter.WriteLine((m_currentWaypointIdx+ 1) + ";" + trackingOffset);
    }
}
