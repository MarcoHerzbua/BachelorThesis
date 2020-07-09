using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public ReactionTestManager m_ReactionTestManager;
    public TrackingWaypointManager m_TrackingTestManager;

    private DrunkVision m_drunkVision;
    // Start is called before the first frame update
    void Start()
    {
        if (!m_ReactionTestManager || !m_TrackingTestManager)
        {
            Debug.LogError("InputManager: no test managers set");
            return;
        }

        m_drunkVision = GetComponentInChildren<DrunkVision>();
        if (!m_drunkVision)
        {
            Debug.LogError("InputManager: no drunk vision script found");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            m_drunkVision.enabled = !m_drunkVision.enabled;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Transform spawnPoint = m_TrackingTestManager.GetLastTriggeredWaypoint().transform;
            Transform nextWaypoint = m_TrackingTestManager.GetCurrentWaypoint().transform;

            Quaternion spawnRotation = Quaternion.LookRotation((nextWaypoint.position - spawnPoint.position).normalized);

            GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.transform.SetPositionAndRotation(spawnPoint.position, spawnRotation);
        }

        if (Input.GetAxis("Vertical") < 0)
        {
            m_ReactionTestManager.m_PlayerIsBraking = true;
        }
        else
        {
            m_ReactionTestManager.m_PlayerIsBraking = false;
        }
   
    }
}
