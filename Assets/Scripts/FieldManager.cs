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
    [SerializeField, Range(65, 89)]
    int shootAngle = 65;
    [SerializeField]
    float distance = 1.0f;
    [SerializeField]
    float personTall = 1.60f;
    int nowThrowTimes = 0;
    List<int> goalBallsCounts = new List<int>();
    [SerializeField]
    Person person;
    [SerializeField, Range(0.0f, 1.0f)]
    private float GosaNum = 0.1f;
    [SerializeField]
    bool isAirResistant = false;
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
            //Debug.Log(goalBallsCount);
            nowThrowTimes++;
            goalBallsCounts.Add(goalBallsCount);
            goalBallsCount = 0;
            allBallsCount = 0;
            if(nowThrowTimes == 3 && goalBallsCounts.Average() <= 1)
            {
                Debug.Log($"î≠éÀäpìx : {shootAngle}Åã,ïΩãœíl : {goalBallsCounts.Average()}");
                shootAngle++;
                goalBallsCounts = new List<int>();
                nowThrowTimes = 0;
            }
            if(nowThrowTimes == throwTimes)
            {
                Debug.Log($"î≠éÀäpìx : {shootAngle}Åã,ïΩãœíl : {goalBallsCounts.Average()}");
                shootAngle++;
                goalBallsCounts = new List<int>();
                nowThrowTimes = 0;
            }
            if(shootAngle == 90)
            {
                return;
            }
            BallController.Instance.DestroyBalls();
            StartCoroutine(person.ThrowBalls(throwBallsCount,shootAngle, isAirResistant));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        person.SetLocating(distance, personTall);
        BallController.Instance.SetGosaNum(GosaNum);
        StartCoroutine(person.ThrowBalls(throwBallsCount,shootAngle, isAirResistant));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
