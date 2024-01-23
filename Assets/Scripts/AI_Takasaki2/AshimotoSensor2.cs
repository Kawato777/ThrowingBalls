using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AshimotoSensor2 : MonoBehaviour
{
    [SerializeField]
    PersonAgent2 agent;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            agent.TakeBallToPocket(other.gameObject);
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
