using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReactionTestManager : MonoBehaviour
{
    public bool m_PlayerIsBraking { get; set; }
    public int m_MaxNumberOfActiveTests = 3;
    public NewLapTrigger m_NewLapTrigger;

    private List<ReactionTest> m_reactionTestSetups;
    private List<ReactionTest> m_activeReactionTests;
    private ReactionTest m_currentTest;
    private StreamWriter m_streamWriter;
    private int m_counterCompletedReactionTests = 0;

    // Start is called before the first frame update
    void Start()
    {
        string filename = "/../Logs/ReactionTimes_" + DateTime.Now.ToString("ddMM_HHmmss") + ".csv";
        m_streamWriter = new StreamWriter(Application.dataPath + filename);

        m_reactionTestSetups = new List<ReactionTest>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            var reactionTest = child.GetComponent<ReactionTest>();
            if (reactionTest && !m_reactionTestSetups.Contains(reactionTest))
            {
                m_reactionTestSetups.Add(reactionTest);

                reactionTest.OnDummyMoving += LogTimeOnDummyMoving;
                reactionTest.OnDummyInLineOfSight += LogTimeOnDummyInLOS;
                reactionTest.OnTestActivated += OnTestActivated;
                reactionTest.OnTestDeactivated += OnTestDeactivated;

                reactionTest.gameObject.SetActive(false);
            }
        }

        ActivateNewSetOfTests();

        m_NewLapTrigger.OnNewLap += ActivateNewSetOfTests;
    }

    private void OnTestDeactivated(ReactionTest obj)
    {
        //if (m_currentTest == obj)
        //{
        //    m_currentTest = null;
        //}
    }

    private void OnTestActivated(ReactionTest obj)
    {
        if (m_currentTest != null)
        {
            CompleteCurrentTest();
        }

        m_currentTest = obj;
    }

    private void CompleteCurrentTest()
    {
        m_currentTest.StartDeactivatingTest();
        m_counterCompletedReactionTests++;
        m_currentTest = null;
    }

    private void ActivateNewSetOfTests()
    {
        if (m_activeReactionTests == null)
        {
            m_activeReactionTests = new List<ReactionTest>();
        }

        if (m_activeReactionTests.Count >= m_reactionTestSetups.Count)
        {
            return;
        }
        for (int i = 0; i < m_MaxNumberOfActiveTests; i++)
        {
            var randomTest = m_reactionTestSetups[UnityEngine.Random.Range(0, m_reactionTestSetups.Count)];
            while (m_activeReactionTests.Contains(randomTest))
            {
                randomTest = m_reactionTestSetups[UnityEngine.Random.Range(0, m_reactionTestSetups.Count)];
            }
            m_activeReactionTests.Add(randomTest);
            randomTest.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_currentTest && m_currentTest.m_testActive && m_currentTest.m_dummyInLineOfSight)
        {
            if (m_PlayerIsBraking)
            {
                LogTimeOnPlayerBraking();
                CompleteCurrentTest();
            }
        }
    }

    void OnDestroy()
    {
        m_streamWriter.Close();

        foreach (var test in m_reactionTestSetups)
        {
            test.OnDummyMoving -= LogTimeOnDummyMoving;
            test.OnDummyInLineOfSight -= LogTimeOnDummyInLOS;
        }

        m_NewLapTrigger.OnNewLap -= ActivateNewSetOfTests;
    }

    private void LogTimeOnDummyMoving()
    {
        m_streamWriter.WriteLine((m_counterCompletedReactionTests + 1) + ". Time Dummy Moving:;" + Time.time);
    }

    private void LogTimeOnDummyInLOS()
    {
        //Debug.Log("Dummy In LOS");
        m_streamWriter.WriteLine((m_counterCompletedReactionTests + 1) + ". Time Dummy in LOS:;" + Time.time);
    }

    private void LogTimeOnPlayerBraking()
    {
        m_streamWriter.WriteLine((m_counterCompletedReactionTests + 1) + ". Time Player Braking:;" + Time.time);
    }
}
