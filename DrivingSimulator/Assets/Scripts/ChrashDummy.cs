using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ChrashDummy : MonoBehaviour
{
    public Transform m_Destination;
    public float m_WaitTimeOnDestination = 3f;
    public float m_InitialDistanceToDestination { get; private set; }
    public bool m_IsOnDestination { get; private set; }

    private List<Rigidbody> m_rigidBodies;
    private Animator m_animator;
    private NavMeshAgent m_navAgent;
    private Vector3 m_initialLocation;

    private bool m_isRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBodies = GetComponentsInChildren<Rigidbody>().ToList();

        foreach (var rb in m_rigidBodies)
        {
            rb.isKinematic = true;
        }

        m_navAgent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();

        if (!m_navAgent || !m_animator || !m_Destination)
        {
            Debug.LogError("ChrashDummy: no NavMeshAgent, Animator, or Destination set");
        }

        m_InitialDistanceToDestination = Mathf.Abs(Vector3.Distance(m_Destination.position, m_navAgent.transform.position));
        m_initialLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsOnDestination)
        {
            StartCoroutine("WaitOnDestination");
        }
        //Switch animation when near destination
        if (m_isRunning && Mathf.Abs(Vector3.Distance(m_Destination.position, m_navAgent.transform.position)) <= 0.5f)
        {
            m_animator.SetBool("isRunning", m_isRunning = false);
            m_IsOnDestination = true;
        }

    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        foreach (var rb in m_rigidBodies)
        {
            rb.isKinematic = false;
        }
        GetComponent<Animator>().enabled = false;
        //Destroy(gameObject, 3);
    }

    public void MoveToDestination()
    {
        m_navAgent.destination = m_Destination.position;
        m_animator.SetBool("isRunning", m_isRunning = true);
    }

    public void CustomReset()
    {
        transform.SetPositionAndRotation(m_initialLocation, Quaternion.identity);
        m_navAgent.destination = m_initialLocation;
        GetComponent<Animator>().enabled = true;
        m_animator.SetBool("isRunning", m_isRunning = false);

        foreach (var rb in m_rigidBodies)
        {
            rb.isKinematic = true;
        }

    }

    private IEnumerator WaitOnDestination()
    {
        yield return new WaitForSeconds(m_WaitTimeOnDestination);
        m_navAgent.destination = m_initialLocation;
        m_animator.SetBool("isRunning", m_isRunning = true);
    }
}
