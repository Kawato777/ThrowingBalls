using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AshimotoSensor3 : MonoBehaviour
{
    [SerializeField]
    Person3 person;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            person.TakeBallToPocket(other.gameObject);
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
