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

    [SerializeField]
    private Transform m_target = null;
    [SerializeField]
    private GameObject m_shootObject = null;
    private float GosaNum = 0.1f;
    [SerializeField]
    float limidSpeed = 6.84f;
    [SerializeField]
    float initialSpeed = 6.84f;

    public void SetGosaNum(float gosaNum)
    {
        GosaNum = gosaNum;
    }

    public void _Throw(float throwingAngle,Vector3 shootPointPos,bool isAirResistant)
    {
        GameObject ball = Instantiate(m_shootObject, shootPointPos, Quaternion.identity);
        ball.transform.parent = this.transform;

        // 射出速度を算出
        Vector3 velocity = Vector3.zero;
        if (isAirResistant)
        {
            // 初速度を求めるけど、2次元空間で考えているから、3次元に変えてあげる。https://qiita.com/kamasu/items/0874022be9a327446665
            // velocity = GetInitialVelocity();
        }
        else
        {
            velocity = CalculateVelocity(ball.transform.position,m_target.transform.position, throwingAngle);
            if(velocity == Vector3.zero)
            {
                FieldManager.Instance.BallCount(false); 
                Destroy(ball);
                return;
            }
        }

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

        // float speed = initialSpeed;

        if (float.IsNaN(speed) || speed >= 10.0f)
        {
            // 条件を満たす初速を算出できなければVector3.zeroを返す
            Debug.LogError("初速計算不能");
            return Vector3.zero;
        }
        else
        {
            return new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed;
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

    public void DestroyBalls()
    {
        foreach(Transform item in transform)
        {
            Destroy(item.gameObject);
        }
    }


    public static float GetInitialVelocity(float gravity, float resistance, float mass, float distance, float theta, float offsetY)
    {
        var k = resistance;
        var g = gravity;
        var m = mass;
        var y0 = offsetY;
        var r = theta / 180.0f * Mathf.PI;
        var L = distance;

        var A = -m / k * g;
        var B = -k / m;
        var C = -g;
        var D = k / m * y0 + Mathf.Tan(r) * k / m * L + m / k * g;

        var T = -D / C - 1.0f / B * GetLambertW(A * B / C * Mathf.Exp(-B * D / C));
        return k * L / m / (1.0f - Mathf.Exp(-k / m * T)) / Mathf.Cos(r);
    }

    public static float GetLambertW(float x)
    {
        var res = _Prec_LambertW(x, _Desy_LambertW(x));
        return res;
    }

    static float _Prec_LambertW(float x, float initial = 0, float prec = 0.00000001f, int iteration = 100)
    {
        var w = initial;
        var i = 0;
        for (i = 0; i < iteration; i++)
        {
            var wTimesExpW = w * Mathf.Exp(w);
            var wPlusOneTimesExpW = (w + 1) * Mathf.Exp(w);
            if (prec > Mathf.Abs((x - wTimesExpW) / wPlusOneTimesExpW))
            {
                break;
            }
            w = w - (wTimesExpW - x) / (
                wPlusOneTimesExpW - (w + 2) * (wTimesExpW - x) / (2 * w + 2));
        }
        return w;
    }
    static float _Desy_LambertW(float x)
    {
        float lx1;
        if (x <= 500.0)
        {
            lx1 = Mathf.Log(x + 1.0f);
            return 0.665f * (1.0f + 0.0195f * lx1) * lx1 + 0.04f;
        }
        var res = Mathf.Log(x - 4.0f) - (1.0f - 1.0f / Mathf.Log(x)) * Mathf.Log(Mathf.Log(x));
        return res;
    }

}
