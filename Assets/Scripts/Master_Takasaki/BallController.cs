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

    [SerializeField]
    Transform goalTransform;
    [SerializeField]
    Person person;

    public void SetGosaNum(float gosaNum)
    {
        GosaNum = gosaNum;
    }

    public void Throw(Vector3 shootPointPos)
    {
        // 玉投げできるか確認
        Vector3 velocity = Vector3.zero;
        Vector3 startPoint = shootPointPos;
        Vector3 goalPoint = goalTransform.position;
        float height = goalTransform.position.y + 1.0f;
        startPoint.y += 0.5f;

        float t1 = CalculateTimeFromStartToMaxHeight(startPoint, height);
        float t2 = CalculateTimeFromMaxHeightToEnd(goalPoint, height);

        if (t1 <= 0.0f && t2 <= 0.0f)
        {
            // その位置に着地させることは不可能のようだ！
            Debug.LogWarning("!!");
            return;
        }

        float time = t1 + t2;

        float speedVec = ComputeVectorFromTime(goalPoint, time, startPoint);
        float angle = ComputeAngleFromTime(goalPoint, time, startPoint);

        if (speedVec <= 0.0f)
        {
            // その位置に着地させることは不可能のようだ！
            Debug.LogWarning("!!");
            return;
        }

        Vector3 vec = ConvertVectorToVector3(speedVec, angle, goalPoint, startPoint);

        GameObject ball = Instantiate(m_shootObject, shootPointPos, Quaternion.identity);
        ball.transform.parent = this.transform;

        Rigidbody rigidbody = ball.GetComponent<Rigidbody>();
        Vector3 force = vec * rigidbody.mass * Random.Range(1 - GosaNum, 1 + GosaNum);
        // Vector3 force = vec * rigidbody.mass;
        rigidbody.isKinematic = false;
        ball.GetComponent<Collider>().enabled = true;
        rigidbody.AddForce(force, ForceMode.Impulse);
    }

    private float CalculateTimeFromStartToMaxHeight(Vector3 m_shootPoint, float i_height)
    {
        float g = Physics.gravity.y;
        float y0 = m_shootPoint.y;

        float timeSquare = 2 * (y0 - i_height) / g;
        if (timeSquare <= 0.0f)
        {
            return 0.0f;
        }

        float time = Mathf.Sqrt(timeSquare);
        return time;
    }

    private float CalculateTimeFromMaxHeightToEnd(Vector3 i_targetPosition, float i_height)
    {
        float g = Physics.gravity.y;
        float y = i_targetPosition.y;

        float timeSquare = 2 * (y - i_height) / g;
        if (timeSquare <= 0.0f)
        {
            return 0.0f;
        }

        float time = Mathf.Sqrt(timeSquare);
        return time;
    }

    private float ComputeVectorFromTime(Vector3 i_targetPosition, float i_time, Vector3 m_shootPoint)
    {
        Vector2 vec = ComputeVectorXYFromTime(i_targetPosition, i_time, m_shootPoint);

        float v_x = vec.x;
        float v_y = vec.y;

        float v0Square = v_x * v_x + v_y * v_y;
        // 負数を平方根計算すると虚数になってしまう。
        // 虚数はfloatでは表現できない。
        // こういう場合はこれ以上の計算は打ち切ろう。
        if (v0Square <= 0.0f)
        {
            return 0.0f;
        }

        float v0 = Mathf.Sqrt(v0Square);

        return v0;
    }

    private float ComputeAngleFromTime(Vector3 i_targetPosition, float i_time, Vector3 m_shootPoint)
    {
        Vector2 vec = ComputeVectorXYFromTime(i_targetPosition, i_time, m_shootPoint);

        float v_x = vec.x;
        float v_y = vec.y;

        float rad = Mathf.Atan2(v_y, v_x);
        float angle = rad * Mathf.Rad2Deg;

        return angle;
    }

    private Vector2 ComputeVectorXYFromTime(Vector3 i_targetPosition, float i_time, Vector3 m_shootPoint)
    {
        // 瞬間移動はちょっと……。
        if (i_time <= 0.0f)
        {
            return Vector2.zero;
        }


        // xz平面の距離を計算。
        Vector2 startPos = new Vector2(m_shootPoint.x, m_shootPoint.z);
        Vector2 targetPos = new Vector2(i_targetPosition.x, i_targetPosition.z);
        float distance = Vector2.Distance(targetPos, startPos);

        float x = distance;
        // な、なぜ重力を反転せねばならないのだ...
        float g = -Physics.gravity.y;
        float y0 = m_shootPoint.y;
        float y = i_targetPosition.y;
        float t = i_time;

        float v_x = x / t;
        float v_y = (y - y0) / t + (g * t) / 2;

        return new Vector2(v_x, v_y);
    }

    private Vector3 ConvertVectorToVector3(float i_v0, float i_angle, Vector3 i_targetPosition, Vector3 m_shootPoint)
    {
        Vector3 startPos = m_shootPoint;
        Vector3 targetPos = i_targetPosition;
        startPos.y = 0.0f;
        targetPos.y = 0.0f;

        Vector3 dir = (targetPos - startPos).normalized;
        Quaternion yawRot = Quaternion.FromToRotation(Vector3.right, dir);
        Vector3 vec = i_v0 * Vector3.right;

        vec = yawRot * Quaternion.AngleAxis(i_angle, Vector3.forward) * vec;

        return vec;
    }

    /* ..0...................  */

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
                FieldManager_NoAI.Instance.BallCount(false); 
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
