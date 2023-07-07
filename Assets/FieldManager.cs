using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    private static FieldManager instance;
    public bool isPlaying = false;
    int allBallsCount, goalBallsCount = 0;
    public int throwBallsCount = 100;

    public static FieldManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FieldManager>();
            }
            return instance;
        }
    }

    public void BallCount(bool isGoal) 
    {
        allBallsCount++;
        if (isGoal)
        {
            goalBallsCount++;
        }
        if(allBallsCount == throwBallsCount)
        {
            Debug.Log(goalBallsCount);
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
