using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager3 : MonoBehaviour
{
    [SerializeField]
    GameObject ball;
    [SerializeField]
    FieldAgent fieldAgent;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetBall()
    {
        //子要素がいなければスキップ
        if (transform.childCount != 0)
        {
            Transform[] balls = GetComponentsInChildren<Transform>();
            foreach (Transform t in balls)
            {
                if (t != transform)
                {
                    Destroy(t.gameObject);
                }
            }
        }

        //ボール生成
        Vector3 randomPos;
        Quaternion randomRot;
        for (int i = 0; i < fieldAgent.maxBallsNum; i++)
        {
            randomPos = new(Random.Range(-fieldAgent.playAreaDiameter / 2 + 1, fieldAgent.playAreaDiameter / 2 - 1) + fieldAgent.transform.position.x,
                            fieldAgent.transform.position.y + 0.15f,
                            Random.Range(-fieldAgent.playAreaDiameter / 2 + 1, fieldAgent.playAreaDiameter / 2 - 1) + fieldAgent.transform.position.z);
            randomRot = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
            GameObject _ball = Instantiate(ball, randomPos, randomRot, this.transform);
            _ball.AddComponent<Rigidbody>();
            // _ball.AddComponent<Rigidbody>();
        }
    }
}
