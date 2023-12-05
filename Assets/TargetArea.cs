using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArea : MonoBehaviour
{
    [SerializeField]
    PersonAgent agent;

    List<GameObject> inBalls = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            bool isInList = false;
            foreach (GameObject obj in inBalls)
            {
                if(obj == other)
                {
                    isInList = true;
                }
            }

            if (isInList)
            {
                inBalls.Add(other.gameObject);
                agent.CountInBall();
            }
        } 
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            foreach (GameObject obj in inBalls)
            {
                if(obj == other.gameObject)
                {
                    inBalls.Remove(obj);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
