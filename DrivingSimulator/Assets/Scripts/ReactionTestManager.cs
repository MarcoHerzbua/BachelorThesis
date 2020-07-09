using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReactionTestManager : MonoBehaviour
{
    public bool m_PlayerIsBraking { get; set; }

    private List<ReactionTest> m_reactionTestSetups;
    private List<ReactionTest> m_activeReactionTests;
    private ReactionTest m_currentTest;
    private StreamWriter m_streamWriter;
    private int m_counterCompletedReactionTests = 0;
    private int m_maxNumberOfActiveTests = 1;

    // Start is called before the first frame update
    void Start()
    {
        m_streamWriter = new StreamWriter(Application.dataPath + "/../Logs/ReactionTimes.csv");

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

                reactionTest.enabled = false;
            }
        }

        m_activeReactionTests = new List<ReactionTest>();
        //choose 3 setups at random which are then active during the test
        for (int i = 0; i < m_maxNumberOfActiveTests; i++)
        {
            var randomTest = m_reactionTestSetups[Random.Range(0, m_reactionTestSetups.Count - 1)];
            m_activeReactionTests.Add(randomTest);
            randomTest.enabled = true;
        }
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
        m_currentTest = obj;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_currentTest && m_currentTest.m_testActive && m_currentTest.m_dummyInLineOfSight)
        {
            if (m_PlayerIsBraking)
            {
                LogTimeOnPlayerBraking();
                m_currentTest.StartDeactivatingTest();
                m_counterCompletedReactionTests++;
                m_currentTest = null;
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
    }

    private void LogTimeOnDummyMoving()
    {
        m_streamWriter.WriteLine((m_counterCompletedReactionTests + 1) + ". Time Dummy Moving:;" + Time.time);
    }

    private void LogTimeOnDummyInLOS()
    {
        m_streamWriter.WriteLine((m_counterCompletedReactionTests + 1) + ". Time Dummy in LOS:;" + Time.time);
    }

    private void LogTimeOnPlayerBraking()
    {
        m_streamWriter.WriteLine((m_counterCompletedReactionTests + 1) + ". Time Player Braking:;" + Time.time);
    }
}
