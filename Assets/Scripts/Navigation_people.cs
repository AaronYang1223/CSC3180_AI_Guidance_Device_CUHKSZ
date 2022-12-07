using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class Navigation_people : MonoBehaviour
{
    //private NavMeshAgent nav;
    
    public Transform destination;
    public Transform start;
    //private Animator animator;
    void Start()
    {
        //print("satrt");
        //this.GetComponent<NavMeshAgent>().destination = destination.position;
        //animator = GetComponent<Animator>();
    }

    
    public void OnTriggerEnter(Collider other)
    {
        //print(other.name);
        if (other.name == this.name + "_destination")
        {
            this.GetComponent<NavMeshAgent>().destination = start.position;
            return;
        }

        if (other.name == this.name + "_start")
        {
            this.GetComponent<NavMeshAgent>().destination = destination.position;
            return;
        }
    }
    

}