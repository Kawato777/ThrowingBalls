using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalArea2 : MonoBehaviour
{
    [SerializeField]
    FieldManager2 fieldManager;
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("なんか来た");
        if (other.CompareTag("Ball"))
        {
            fieldManager.Goal();
            Destroy(other.gameObject);
        }
    }
}
