using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewLapTrigger : MonoBehaviour
{
    public Action OnNewLap;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        OnNewLap.Invoke();
    }
}
