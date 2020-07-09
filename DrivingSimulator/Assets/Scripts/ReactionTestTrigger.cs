using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ReactionTestTrigger : MonoBehaviour
{
    public Action<Transform> OnPlayerEntersTrigger;

    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        OnPlayerEntersTrigger.Invoke(other.transform);
    }
}
