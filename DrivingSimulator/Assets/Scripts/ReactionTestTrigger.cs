using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ReactionTestTrigger : MonoBehaviour
{
    public bool m_isTriggered { get; set; }

    public Action<Transform> OnPlayerEntersTrigger;

    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player" || m_isTriggered)
        {
            return;
        }

        m_isTriggered = true;

        OnPlayerEntersTrigger.Invoke(other.transform);
    }
}
