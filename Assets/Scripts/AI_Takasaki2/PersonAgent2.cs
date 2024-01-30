using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.VisualScripting;

public class PersonAgent2 : Agent
{
    public GameObject personShape;
    List<GameObject> ballPocket = new List<GameObject>();
    List<PersonAgent2> passablePersons = new List<PersonAgent2>();
    bool throwable = false;
    Rigidbody personShape_rb;
    [SerializeField]
    FieldManager2 fieldManager;
    [SerializeField]
    bool isHeuristic = false;
    [SerializeField]
    Transform headTF;

    public void TakeBallToPocket(GameObject ball)
    {
        if(ballPocket.Count >= 6)
        {
            return;
        }

        ballPocket.Add(ball);

        AddReward(0.01f);   // ボールをゲットしたら+0.01

        if(ballPocket.Count == 6)
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

    public void SetPassablePerson(PersonAgent2 person,bool isRemoved)
    {
        if (isRemoved) // 範囲から出る
        {
            passablePersons.Remove(person);
        }
        else // 範囲に入る
        {
            passablePersons.Add(person);
        }
    }

    public override void Initialize()
    {
        personShape_rb = personShape.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        throwable = false;
        if(ballPocket.Count > 0)
        {
            foreach (GameObject ball in ballPocket)
            {
                Destroy(ball);
            }
            ballPocket = new List<GameObject>();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(ballPocket.Count);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 行動ゾーン
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        personShape_rb.AddForce(controlSignal * 10);

        if (actions.DiscreteActions[0] == 1)
        {
            Throw();
        }
        else if (actions.DiscreteActions[1] >= 1 && ballPocket.Count >= 1)
        {
            Pass(actions.DiscreteActions[1]);
        }

        // 報酬ゾーン
        if(personShape.transform.position.y < fieldManager.transform.position.y - 5)
        {
            AddReward(-1.0f);
            fieldManager.EndEpisode();
        }
    }

    void Pass(int personNum)
    {
        PersonAgent2 partnerAgent = fieldManager.PersonInfos[personNum].Agent;
        if(partnerAgent.ballPocket.Count < 6)
        {
            foreach(var obj in ballPocket)
            {
                partnerAgent.ballPocket.Add(obj);
                ballPocket.Remove(obj);
                if(partnerAgent.ballPocket.Count == 6)
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
            Vector3 target = fieldManager.goalTF.transform.position;
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
        startPoint.y += 0.5f;
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

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        actionMask.SetActionEnabled(0, 1, throwable);
        for(int i = 1;i <= 15; i++)
        {
            actionMask.SetActionEnabled(1, i, false);
        }
        foreach(var person in passablePersons)
        {
            actionMask.SetActionEnabled(1,fieldManager.GetPersonNum(person) + 1, true);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        if (isHeuristic)
        {
            var banana0 = actionsOut.ContinuousActions;
            banana0[0] = Input.GetAxis("Horizontal");
            banana0[1] = Input.GetAxis("Vertical");
            var banana1 = actionsOut.DiscreteActions;
            if (Input.GetKeyDown(KeyCode.Space) && throwable){
                banana1[0] = 1;
            }
            else
            {
                banana1[0] = 0;
            }
        }
    }
}
