using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    private static FieldManager instance;
    public bool isPlaying = false;

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

    public void GetBallsCountNum(int num) 
    {
        StartCoroutine(Banana(num));
    }

    IEnumerator Banana(int num)
    {
        yield return new WaitForSeconds(3f);
        Debug.Log(BallController.Instance.GetGoalBallsCount(num));
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
