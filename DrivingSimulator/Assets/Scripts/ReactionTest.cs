using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class ReactionTest : MonoBehaviour
{
    public Transform m_LOSTarget;
    public GameObject m_DummyPrefab;
    public bool m_dummyInLineOfSight { get; private set; }
    public bool m_dummyIsMoving { get; private set; }
    public bool m_testActive { get; private set; }

    private ReactionTestTrigger m_reactionTestTrigger;
    private ChrashDummy m_chrashDummy;
    private Transform m_playerTransform;
    private Camera m_activeCamera;

    public Action<ReactionTest> OnTestActivated;
    public Action<ReactionTest> OnTestDeactivated;
    public Action OnDummyMoving;
    public Action OnDummyInLineOfSight;
    // Start is called before the first frame update
    void Start()
    {
        m_reactionTestTrigger = GetComponentInChildren<ReactionTestTrigger>();
        if (!m_reactionTestTrigger)
        {
            Debug.LogError("ReactionTest: No trigger set as child");
            return;
        }

        m_reactionTestTrigger.OnPlayerEntersTrigger += ActivateReactionTest;

        m_chrashDummy = GetComponentInChildren<ChrashDummy>();
        if (!m_chrashDummy)
        {
            Debug.LogError("ReactionTest: No CrashDummy set as child");
            return;
        }

        m_chrashDummy.OnDummyHit += StartDeactivatingTest;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_testActive)
        {
            if (!m_dummyIsMoving)
            {
                m_dummyIsMoving = CheckTimeToDestination();
            }
            else if (m_dummyIsMoving && !m_dummyInLineOfSight)
            {
                m_dummyInLineOfSight = CheckLineOfSight();
            }

        }
    }

    private bool CheckTimeToDestination()
    {
        Rigidbody playerRB = m_playerTransform.GetComponent<Rigidbody>();
        float distanceToDummyDestination =
            Mathf.Abs(Vector3.Distance(m_playerTransform.position, m_chrashDummy.m_Destination.position));
        float playerSpeed = playerRB.velocity.magnitude;
        float playerTimeToDestination = distanceToDummyDestination / playerSpeed;
        //Debug.Log("Player" + playerTimeToDestination);

        var dummyNavAgent = m_chrashDummy.GetComponent<NavMeshAgent>();
        float dummyTimeToDestination = m_chrashDummy.m_InitialDistanceToDestination / dummyNavAgent.speed;
        //Debug.Log("Dummy: " + dummyTimeToDestination);

        if (playerTimeToDestination <= dummyTimeToDestination * 2)
        {
            MoveDummy();
            return true;
        }

        return false;
    }
    private void MoveDummy()
    {
        m_chrashDummy.MoveToDestination();
        OnDummyMoving.Invoke();
    }

    private void ActivateReactionTest(Transform player)
    {
        m_playerTransform = player;
        m_activeCamera = Camera.main;
        m_testActive = true;
        OnTestActivated.Invoke(this);
        CheckTimeToDestination();
    }

    public void StartDeactivatingTest()
    {
        StartCoroutine("DeactivateTest");
    }

    private IEnumerator DeactivateTest()
    {
        yield return new WaitForSeconds(10);
        //m_chrashDummy.CustomReset();
        m_testActive = m_dummyInLineOfSight = m_dummyIsMoving = false;
        m_reactionTestTrigger.m_isTriggered = false;

        //Vector3 spawnPos = m_chrashDummy.m_initialLocation;
        //Destroy(m_chrashDummy);
        //m_chrashDummy = Instantiate(m_DummyPrefab, spawnPos, Quaternion.identity, this.transform).GetComponent<ChrashDummy>();
        OnTestDeactivated.Invoke(this);
        gameObject.SetActive(false);
    }

    private bool CheckLineOfSight()
    {
        Ray ray = new Ray(m_activeCamera.transform.position,  (m_LOSTarget.position  - m_activeCamera.transform.position ).normalized);
        RaycastHit hit;
        //Debug.DrawLine(m_activeCamera.transform.position, m_activeCamera.transform.position + (m_LOSTarget.transform.position - m_activeCamera.transform.position).normalized * 1000, Color.blue);
        //use mask "Default" so it ignores colliders like triggers and the player
        if (Physics.Raycast(ray, out hit, 10000f, LayerMask.GetMask("Default")))
        {
            //Debug.Log(hit.transform.gameObject);
            if (hit.transform.gameObject == m_chrashDummy.gameObject)
            {
                OnDummyInLineOfSight.Invoke();
                return true;
            }
        }

        return false;
    }

    private void OnDestroy()
    {
        m_chrashDummy.OnDummyHit -= StartDeactivatingTest;
    }
}
