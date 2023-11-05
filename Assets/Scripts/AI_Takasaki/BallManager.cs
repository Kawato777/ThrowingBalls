using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField]
    GameObject ball;
    [SerializeField]
    Transform ground;
    [SerializeField]
    FieldManager fieldManager;

    public void CreateBalls()
    {
        Vector3 randomPos;
        Quaternion randomRot;
        for (int i = 0;i < fieldManager.maxBallsNum; i++)
        {
            randomPos = new(Random.Range(-fieldManager.playAreaDiameter / 2 + 1, fieldManager.playAreaDiameter / 2 - 1), 0.15f, Random.Range(-fieldManager.playAreaDiameter / 2 + 1, fieldManager.playAreaDiameter / 2 - 1));
            randomRot = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
            GameObject _ball = Instantiate(ball, randomPos , randomRot , this.transform);
            _ball.AddComponent<Rigidbody>();
            // _ball.AddComponent<Rigidbody>();
        }
    }

    public void DestroyBalls()
    {
        //éqóvëfÇ™Ç¢Ç»ÇØÇÍÇŒèIóπ
        if (transform.childCount == 0)
        {
            return;
        }

        GameObject[] balls = GetComponentsInChildren<GameObject>();
        foreach (GameObject t in balls)
        {
            Destroy(t);
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
