using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager2 : MonoBehaviour
{
    [SerializeField]
    GameObject ball;
    [SerializeField]
    FieldManager2 fieldManager;

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
        for (int i = 0; i < fieldManager.maxBallsNum; i++)
        {
            randomPos = new(Random.Range(-fieldManager.playAreaDiameter / 2 + 1, fieldManager.playAreaDiameter / 2 - 1) + fieldManager.transform.position.x,
                            fieldManager.transform.position.y + 0.15f,
                            Random.Range(-fieldManager.playAreaDiameter / 2 + 1, fieldManager.playAreaDiameter / 2 - 1) + fieldManager.transform.position.z);
            randomRot = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
            GameObject _ball = Instantiate(ball, randomPos, randomRot, this.transform);
            _ball.AddComponent<Rigidbody>();
            // _ball.AddComponent<Rigidbody>();
        }
    }
}
