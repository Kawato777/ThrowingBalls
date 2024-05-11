using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalArea3 : MonoBehaviour
{
    [SerializeField]
    FieldAgent fieldAgent;
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("‚È‚ñ‚©—ˆ‚½");
        if (other.CompareTag("Ball"))
        {
            Debug.Log("Goal");
            fieldAgent.Goal();
            Destroy(other.gameObject);
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
