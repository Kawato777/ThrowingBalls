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
    [SerializeField]
    PersonAgent personAgent;

    public void CreateBalls()
    {
        Vector3 randomPos;
        Quaternion randomRot;
        for (int i = 0;i < fieldManager.maxBallsNum; i++)
        {
            randomPos = new(Random.Range(-fieldManager.playAreaDiameter / 2 + 1, fieldManager.playAreaDiameter / 2 - 1) + fieldManager.transform.position.x,
                            fieldManager.transform.position.y + 0.15f,
                            Random.Range(-fieldManager.playAreaDiameter / 2 + 1, fieldManager.playAreaDiameter / 2 - 1) + fieldManager.transform.position.z);
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

        Transform[] balls = GetComponentsInChildren<Transform>();
        foreach (Transform t in balls)
        {
            if(t != transform)
            {
                Destroy(t.gameObject);
            }
        }
    }

    public void CountHighBall()
    {
        personAgent.CountHighBall();
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
