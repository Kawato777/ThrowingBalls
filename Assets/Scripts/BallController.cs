using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private static BallController instance;

    public static BallController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<BallController>();
            }
            return instance;
        }
    }
    // 射出角度
    public float ThrowingAngle = 60;

    [SerializeField]
    private Transform m_shootPoint = null;
    [SerializeField]
    private Transform m_target = null;
    [SerializeField]
    private GameObject m_shootObject = null;
    [SerializeField,Range(0.0f,1.0f)]
    private float GosaNum = 0.1f;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            _Throw();
        }
    }

    void _Throw()
    {
        GameObject ball = Instantiate(m_shootObject, m_shootPoint.position, Quaternion.identity);
        ball.transform.parent = this.transform;

        // 射出速度を算出
        Vector3 velocity = CalculateVelocity(ball.transform.position, m_target.transform.position, ThrowingAngle);

        // 誤差を付与
        velocity.x = Random.Range(velocity.x * (1 - GosaNum), velocity.x * (1 + GosaNum));
        velocity.y = Random.Range(velocity.y * (1 - GosaNum), velocity.y * (1 + GosaNum));
        velocity.z = Random.Range(velocity.z * (1 - GosaNum), velocity.z * (1 + GosaNum));

        // 射出
        Rigidbody rid = ball.GetComponent<Rigidbody>();
        rid.AddForce(velocity * rid.mass, ForceMode.Impulse);
    }

    private Vector3 CalculateVelocity(Vector3 pointA, Vector3 pointB, float angle)
    {
        // 射出角をラジアンに変換
        float rad = angle * Mathf.PI / 180;

        // 水平方向の距離x
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));

        // 垂直方向の距離y
        float y = pointA.y - pointB.y;

        // 斜方投射の公式を初速度について解く
        float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));

        if (float.IsNaN(speed))
        {
            // 条件を満たす初速を算出できなければVector3.zeroを返す
            return Vector3.zero;
        }
        else
        {
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
        }
    }

    public Transform GetBallOfTheNearest(Transform target)
    {
        Vector2 targetXZ = new Vector2(target.position.x, target.position.z);
        Transform theNearestTF = null;
        float theNearestDistance = 0f;
        int countNum = 1;
        foreach (var item in transform.GetComponentsInChildren<Transform>())
        {
            if(item.transform == this.transform)
            {
                continue;
            }
            float distance = Vector2.Distance(targetXZ, new Vector2(item.position.x, item.position.z));
            if (countNum == 1 || distance < theNearestDistance)
            {
                theNearestTF = item;
                theNearestDistance = distance;
            }
            countNum++;
        }
        return theNearestTF;
    }
}
