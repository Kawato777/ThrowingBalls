using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurroundSensor2 : MonoBehaviour
{
    [SerializeField]
    PersonAgent2 agent;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && other.gameObject != agent.personShape.gameObject)
        {
            agent.SetPassablePerson(other.gameObject.GetComponent<PersonAgent2>(), false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject != agent.personShape.gameObject)
        {
            agent.SetPassablePerson(other.gameObject.GetComponent<PersonAgent2>(), true);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
