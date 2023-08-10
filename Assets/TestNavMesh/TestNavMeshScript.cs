using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestNavMeshScript : MonoBehaviour
{
    public List<GameObject> players;
    public GameObject target;
    // Update is called once per frame
    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.G))
        {
            foreach(var player in players)
            {
                NavMeshAgent agent = player.GetComponent<NavMeshAgent>();
                agent.SetDestination(target.transform.position);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach(var player in players)
            {
                NavMeshAgent agent = player.GetComponent<NavMeshAgent>();
                agent.isStopped = !agent.isStopped;
            }
        }
    }
}
