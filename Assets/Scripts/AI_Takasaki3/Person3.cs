using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class Person3 : MonoBehaviour
{
    public GameObject personShape;
    [HideInInspector]
    public List<GameObject> ballPocket = new List<GameObject>();
    [HideInInspector]
    public List<Person3> passablePersons = new List<Person3>();
    [HideInInspector]
    public bool throwable = false;
    public Rigidbody personShape_rb;
    [SerializeField]
    FieldAgent fieldAgent;
    [SerializeField]
    Transform headTF;
    [SerializeField]
    BallManager3 ballManager;

    private void Start()
    {
        personShape_rb = personShape.GetComponent<Rigidbody>();
    }

    public void TakeBallToPocket(GameObject ball)
    {
        if (ballPocket.Count >= 6)
        {
            return;
        }

        ballPocket.Add(ball);

        fieldAgent.AddReward(0.01f);   // ボールをゲットしたら+0.01

        if (ballPocket.Count == 6)
        {
            throwable = true;
        }

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        ball.GetComponent<Collider>().enabled = false;
        ball.SetActive(false);
    }

    public void SetPassablePerson(Person3 person, bool isRemoved)
    {
        if(person == this)
        {
            return;
        }

        if (isRemoved) // 範囲から出る
        {
            passablePersons.Remove(person);
        }
        else // 範囲に入る
        {
            passablePersons.Add(person);
        }
    }

    public void ResetPerson()
    {
        throwable = false;
        if (ballPocket.Count > 0)
        {
            foreach (GameObject ball in ballPocket)
            {
                Destroy(ball);
            }
            ballPocket = new List<GameObject>();
        }
    }

    public void Action(Vector2 ContinuousActions, int throwState,int passState)
    {
        // 行動ゾーン
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = ContinuousActions.x;
        controlSignal.z = ContinuousActions.y;
        personShape_rb.AddForce(controlSignal * 10);

        if (throwState == 1)
        {
            Throw();
        }
        else if (passState >= 1 && ballPocket.Count >= 1)
        {
            Pass(passState - 1);
        }

        // 報酬ゾーン
        if (personShape.transform.position.y < fieldAgent.transform.position.y - 5)
        {
            fieldAgent.AddReward(-1.0f);
            fieldAgent.EndEpisodeFromOthers();
        }
    }

    void Pass(int personNum)
    {
        Person3 person = fieldAgent.PersonInfos[personNum].person;
        if (person.ballPocket.Count < 6)
        {
            foreach (var obj in ballPocket)
            {
                person.ballPocket.Add(obj);
                ballPocket.Remove(obj);
                if (person.ballPocket.Count == 6)
                {
                    return;
                }
            }
        }
    }

    void Throw()
    {
        throwable = false;
        Debug.Log("Throw");
        // 玉を積む
        for (int i = 0; i < ballPocket.Count; i++)
        {
            GameObject ball = ballPocket[i];
            ball.SetActive(true);
            Vector3 headPos = headTF.position;
            headPos.y += 0.5f;
            ball.transform.position = headPos;
            Vector3 target = fieldAgent.goalTF.transform.position;
            target.y = this.transform.position.y;
            ball.transform.LookAt(target);

            Vector3 pos = ball.transform.position;
            switch (i)
            {
                case 0:
                    pos += -ball.transform.right.normalized * 0.025f;
                    break;
                case 1:
                    pos += ball.transform.right.normalized * 0.025f;
                    break;
                case 2:
                    ball.transform.Rotate(new(0f, 0f, 90f));
                    pos.y += 0.075f;
                    pos += -ball.transform.right.normalized * 0.025f;
                    break;
                case 3:
                    ball.transform.Rotate(new(0f, 0f, 90f));
                    pos.y += 0.075f;
                    pos += ball.transform.right.normalized * 0.025f;
                    break;
                case 4:
                    pos.y += 0.15f;
                    pos += -ball.transform.right.normalized * 0.025f;
                    break;
                case 5:
                    pos.y += 0.15f;
                    pos += ball.transform.right.normalized * 0.025f;
                    break;
                default:
                    Debug.Log("玉持ちすぎ。");
                    break;
            }
            ball.transform.position = pos;
            ball.transform.lossyScale.Set(10f, 10f, 10f);
        }

        // 玉投げ
        Vector3 velocity = Vector3.zero;
        Vector3 startPoint = headTF.position;
        Vector3 goalPoint = fieldAgent.goalTF.position;
        float height = fieldAgent.transform.position.y + fieldAgent.height;
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

        foreach (var item in ballPocket)
        {
            Rigidbody rigidbody = item.GetComponent<Rigidbody>();
            Vector3 force = vec * rigidbody.mass * Random.Range(1 - fieldAgent.gosa, 1 + fieldAgent.gosa);
            rigidbody.isKinematic = false;
            item.GetComponent<Collider>().enabled = true;
            rigidbody.AddForce(force, ForceMode.Impulse);
        }
        ballPocket = new List<GameObject>();
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
}
