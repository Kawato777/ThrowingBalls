using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurroundSensor3 : MonoBehaviour
{
    [SerializeField]
    Person3 person;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Body") && other.gameObject != person.personShape.gameObject)
        {
            person.SetPassablePerson(other.gameObject.GetComponent<PersonBody3>().person, false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Body") && other.gameObject != person.personShape.gameObject)
        {
            person.SetPassablePerson(other.gameObject.GetComponent<PersonBody3>().person, true);
        }
    }
}
