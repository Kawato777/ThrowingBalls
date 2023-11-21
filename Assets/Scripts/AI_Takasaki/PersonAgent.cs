using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class PersonAgent : Agent
{
    [SerializeField]
    BallManager ballManager;
    [SerializeField]
    FieldManager fieldManager;
    [SerializeField]
    Transform nose, target;
    Rigidbody rBody;
    bool isThrowable;
    List<GameObject> havingBalls = new();
    int getBallsNum, goalBallsNum, highBallsNum;

    // 環境パラメータ
    float getBallReward; // 玉を得たときの報酬
    float goalReward; // 玉がかごに入った時の報酬
    float ballAngleReward; // 玉を投げる角度の報酬（高さは別で）
    float ballHighReward; // 玉の高さの報酬（一定の高さを超えたら）
    float stepReward; // ステップ毎のペナルティ

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Initialize()
    {
        // Rigidbody代入
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        ballManager.DestroyBalls();
        ballManager.CreateBalls();
        SetPersonPos();
        isThrowable = false;
        if(havingBalls.Count > 0 )
        {
            foreach (var item in havingBalls)
            {
                Destroy(item);
            }
        }
        havingBalls = new List<GameObject>();
        getBallsNum = 0;
        goalBallsNum = 0;

        // 環境パラメータの設定
        EnvironmentParameters envParams = Academy.Instance.EnvironmentParameters;
        getBallReward = envParams.GetWithDefault("getball_reward", 0.01f);
        goalReward = envParams.GetWithDefault("goal_reward", 0.0f);
        ballAngleReward = envParams.GetWithDefault("ballangle_reward",0.0f);
        ballHighReward = envParams.GetWithDefault("ballhigh_reward", 0.0f);
        stepReward = envParams.GetWithDefault("step_reward", 0.0f);
    }

    void SetPersonPos()
    {
        float playerTall = Random.Range(1.6f, 1.9f);
        fieldManager.personTall = playerTall;
        float radius = fieldManager.playAreaDiameter / 2;
        Vector3 center = fieldManager.transform.position;
        float angle = Random.Range(0, 360);
        float rad = angle * Mathf.Deg2Rad;
        float px = Mathf.Cos(rad) * radius + center.x;
        float pz = Mathf.Sin(rad) * radius + center.z;
        transform.localScale = new(1f, playerTall, 1f);
        transform.position = new(px, playerTall / 2 + center.y, pz);
        transform.LookAt(target);
        Quaternion banana2 = transform.rotation;
        banana2.x = 0f;
        banana2.z = 0f;
        transform.localRotation = banana2;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(havingBalls.Count);
    }

    public void SendBallToPocket(GameObject ball)
    {
        if(havingBalls.Count >= 6)
        {
            return;
        }
        else
        {
            havingBalls.Add(ball);
            getBallsNum++;
            if(havingBalls.Count == 6)
            {
                isThrowable = true;
            }
            ball.GetComponent<Collider>().enabled = false;
            ball.GetComponent<Rigidbody>().isKinematic = true;
            ball.SetActive(false);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 行動ゾーン
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        rBody.AddForce(controlSignal * 10);
        controlSignal = Vector3.zero;
        controlSignal.y = actions.ContinuousActions[2];
        rBody.AddTorque(controlSignal * 10);
        
        if (actions.DiscreteActions[0] == 1)
        {
            isThrowable = false;
            Vector3 firstVelocity = new(actions.ContinuousActions[3], actions.ContinuousActions[4], actions.ContinuousActions[5]);
            Throw(firstVelocity);
        }

        // 報酬獲得ゾーン
        // ボールを拾ったら+0.01
        for (int i = 0; i < getBallsNum; i++)
        {
            AddReward(getBallReward);
            getBallsNum--;
        }

        // ゴールしたら+1.0
        for (int i = 0; i < goalBallsNum; i++)
        {
            AddReward(goalReward);
            goalBallsNum--;
        }

        // 4.2m超えた球の数+0.05
        for (int i = 0; i < highBallsNum; i++)
        {
            AddReward(ballHighReward);
            highBallsNum--;
        }

        //　時間がたつごとに-0.0005
        AddReward(stepReward);

        // 落下していたら終了
        if(transform.position.y < fieldManager.transform.position.y - 3)
        {
            EndEpisode();
        }
    }

    public void CountHighBall()
    {
        highBallsNum++;
    }

    void Throw(Vector3 firstVelocity)
    {
        // 玉を積む
        for (int i = 0; i < havingBalls.Count; i++) 
        {
            GameObject ball = havingBalls[i];
            ball.SetActive(true);
            ball.transform.position = new(nose.position.x, nose.position.y + 0.3f, nose.position.z);
            Quaternion quaternion = Quaternion.identity;
            Vector3 planeNormal = Vector3.up;
            var signedAngle = Vector3.SignedAngle(Vector3.ProjectOnPlane(ball.transform.forward.normalized, planeNormal), Vector3.ProjectOnPlane(nose.forward.normalized, planeNormal), planeNormal);
            quaternion.eulerAngles = new(90f, signedAngle, 0f);
            ball.transform.rotation = quaternion;
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
                    pos.y += 0.05f;
                    pos += -ball.transform.right.normalized * 0.025f;
                    break;
                case 3:
                    ball.transform.Rotate(new(0f, 0f, 90f));
                    pos.y += 0.05f;
                    pos += ball.transform.right.normalized * 0.025f;
                    break;
                case 4:
                    pos.y += 0.1f;
                    pos += -ball.transform.right.normalized * 0.025f;
                    break;
                case 5:
                    pos.y += 0.1f;
                    pos += ball.transform.right.normalized * 0.025f;
                    break;
                default:
                    Debug.Log("玉持ちすぎ。");
                    break;
            }
            ball.transform.position = pos;
            ball.transform.lossyScale.Set(10f, 10f, 10f); 
        }

        // 玉を投げる
        firstVelocity = new(firstVelocity.x * Random.Range(1 - fieldManager.gosa, 1 + fieldManager.gosa), 
                            firstVelocity.y * Random.Range(1 - fieldManager.gosa, 1 + fieldManager.gosa), 
                            firstVelocity.z * Random.Range(1 - fieldManager.gosa, 1 + fieldManager.gosa));
        foreach (var ball in havingBalls)
        {
            ball.GetComponent<Collider>().enabled = true;
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce(firstVelocity * fieldManager.multipulV0SpeedNum, ForceMode.Impulse);
        }
        isThrowable = false;
        havingBalls = new List<GameObject>();
    }

    public void GoalBall(GameObject ball)
    {
        goalBallsNum++;
        Destroy(ball);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var banana = actionsOut.ContinuousActions;
        var banana2 = actionsOut.DiscreteActions;
        banana[0] = Input.GetAxis("Horizontal");
        banana[1] = Input.GetAxis("Vertical");
        // banana[2] = Input.GetAxis("Mouse X");
        banana[3] = 0.1f;
        banana[4] = 0.5f;
        banana[5] = 0.1f;
        if (Input.GetKeyDown(KeyCode.Space) && isThrowable)
        {
              banana2[0] = 1;
        }
        else
        {
            banana2[0] = 0;
        }
    }

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        if (isThrowable)
        {
            actionMask.SetActionEnabled(0, 1, true);
        }
        else
        {
            actionMask.SetActionEnabled(0, 1, false);
        }
    }

}
