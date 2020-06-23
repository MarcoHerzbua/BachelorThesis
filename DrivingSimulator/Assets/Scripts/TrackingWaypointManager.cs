using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingWaypointManager : MonoBehaviour
{
    [SerializeField] private GameObject playerCar;

    private GameObject[] waypointArr;
    private int currentWaypointIdx;

    // Start is called before the first frame update
    void Start()
    {
        if (playerCar == null)
        {
            Debug.LogError("TrackingWaypoints: Playercar not set as parameter");
            return;
        }

        waypointArr = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            waypointArr[i] = transform.GetChild(i).gameObject;
            waypointArr[i].SetActive(false);
            waypointArr[i].GetComponent<TrackingWaypoint>().OnWaypointEnter += ActivateNextWaypoint;
        }

        currentWaypointIdx = FindClosestWaypointIdx();
        waypointArr[currentWaypointIdx].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        foreach (var waypoint in waypointArr)
        {
            waypoint.GetComponent<TrackingWaypoint>().OnWaypointEnter -= ActivateNextWaypoint;
        }
    }

    private GameObject FindClosestWaypoint()
    {
        return waypointArr[FindClosestWaypointIdx()];
    }

    private int FindClosestWaypointIdx()
    {
        Vector3 closestDistanceVec = Vector3.positiveInfinity;
        int closestWaypointIdx = int.MaxValue;

        for (int i = 0; i < waypointArr.Length; i++)
        {
            Vector3 distanceVec = waypointArr[i].transform.position - playerCar.transform.position;

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
        waypointArr[currentWaypointIdx].SetActive(false);

        currentWaypointIdx++;
        if (currentWaypointIdx > waypointArr.Length - 1)
        {
            currentWaypointIdx = 0;
        }

        waypointArr[currentWaypointIdx].SetActive(true);
    }
}
