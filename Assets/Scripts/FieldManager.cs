using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    private static FieldManager instance;
    public bool isPlaying = false;
    int allBallsCount, goalBallsCount = 0;
    public int throwBallsCount = 100;
    [SerializeField]
    int throwTimes = 10;
    int nowThrowTimes = 0;
    List<int> goalBallsCounts = new List<int>();
    [SerializeField]
    Person person;

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
            BallController.Instance.DestroyBalls();
            nowThrowTimes++;
            goalBallsCounts.Add(goalBallsCount);
            goalBallsCount = 0;
            allBallsCount = 0;
            if(nowThrowTimes == throwTimes)
            {
                Debug.Log($"•½‹Ï’l : {goalBallsCounts.Average()}");
            }
            else
            {
                StartCoroutine(person.ThrowBalls(throwBallsCount));
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(person.ThrowBalls(throwBallsCount));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
