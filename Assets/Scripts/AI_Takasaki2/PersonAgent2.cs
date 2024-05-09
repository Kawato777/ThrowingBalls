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
    [SerializeField]
    bool isLightLearning = false;
    [SerializeField]
    BallManager2 ballManager;

    BufferSensorComponent bufferSensor;

    public void TakeBallToPocket(GameObject ball)
    {
        if(ballPocket.Count >= 6)
        {
            return;
        }

        ballPocket.Add(ball);

        AddReward(0.01f);   // �{�[�����Q�b�g������+0.01

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
        if (isRemoved) // �͈͂���o��
        {
            passablePersons.Remove(person);
        }
        else // �͈͂ɓ���
        {
            passablePersons.Add(person);
        }
    }

    public override void Initialize()
    {
        personShape_rb = personShape.GetComponent<Rigidbody>();
        bufferSensor = GetComponent<BufferSensorComponent>();
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
        if (isLightLearning)
        {
            foreach (var item in fieldManager.PersonInfos)
            {
                sensor.AddObservation(item.Agent.personShape.transform.position);
            }
        }

        foreach (var item in ballManager.transform.GetComponentsInChildren<Transform>())
        {
            if(item == ballManager.transform)
            {
                continue;
            }
            float[] ballPos = new float[] {item.position.x,item.position.y,item.position.z};
            bufferSensor.AppendObservation(ballPos);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // �s���]�[��
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

        // ��V�]�[��
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
        // �ʂ�ς�
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
                    Debug.Log("�ʎ��������B");
                    break;
            }
            ball.transform.position = pos;
            ball.transform.lossyScale.Set(10f, 10f, 10f);
        }

        // �ʓ���
        Vector3 velocity = Vector3.zero;
        Vector3 startPoint = headTF.position;
        Vector3 goalPoint = fieldManager.goalTF.position;
        float height = fieldManager.transform.position.y + fieldManager.height;
        startPoint.y += 0.5f;

        float t1 = CalculateTimeFromStartToMaxHeight(startPoint, height);
        float t2 = CalculateTimeFromMaxHeightToEnd(goalPoint, height);

        if (t1 <= 0.0f && t2 <= 0.0f)
        {
            // ���̈ʒu�ɒ��n�����邱�Ƃ͕s�\�̂悤���I
            Debug.LogWarning("!!");
            return;
        }

        float time = t1 + t2;

        float speedVec = ComputeVectorFromTime(goalPoint, time, startPoint);
        float angle = ComputeAngleFromTime(goalPoint, time, startPoint);

        if (speedVec <= 0.0f)
        {
            // ���̈ʒu�ɒ��n�����邱�Ƃ͕s�\�̂悤���I
            Debug.LogWarning("!!");
            return;
        }

        Vector3 vec = ConvertVectorToVector3(speedVec, angle, goalPoint, startPoint);

        foreach (var item in ballPocket)
        {
            Rigidbody rigidbody = item.GetComponent<Rigidbody>();
            Vector3 force = vec * rigidbody.mass * Random.Range(1 - fieldManager.gosa, 1 + fieldManager.gosa);
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
        // �����𕽕����v�Z����Ƌ����ɂȂ��Ă��܂��B
        // ������float�ł͕\���ł��Ȃ��B
        // ���������ꍇ�͂���ȏ�̌v�Z�͑ł��؂낤�B
        if (v0Square <= 0.0f)
        {
            return 0.0f;
        }

        float v0 = Mathf.Sqrt(v0Square);

        return v0;
    }

    private float ComputeAngleFromTime(Vector3 i_targetPosition, float i_time,Vector3 m_shootPoint)
    {
        Vector2 vec = ComputeVectorXYFromTime(i_targetPosition, i_time, m_shootPoint);

        float v_x = vec.x;
        float v_y = vec.y;

        float rad = Mathf.Atan2(v_y, v_x);
        float angle = rad * Mathf.Rad2Deg;

        return angle;
    }

    private Vector2 ComputeVectorXYFromTime(Vector3 i_targetPosition, float i_time,Vector3 m_shootPoint)
    {
        // �u�Ԉړ��͂�����Ɓc�c�B
        if (i_time <= 0.0f)
        {
            return Vector2.zero;
        }


        // xz���ʂ̋������v�Z�B
        Vector2 startPos = new Vector2(m_shootPoint.x, m_shootPoint.z);
        Vector2 targetPos = new Vector2(i_targetPosition.x, i_targetPosition.z);
        float distance = Vector2.Distance(targetPos, startPos);

        float x = distance;
        // �ȁA�Ȃ��d�͂𔽓]���˂΂Ȃ�Ȃ��̂�...
        float g = -Physics.gravity.y;
        float y0 = m_shootPoint.y;
        float y = i_targetPosition.y;
        float t = i_time;

        float v_x = x / t;
        float v_y = (y - y0) / t + (g * t) / 2;

        return new Vector2(v_x, v_y);
    }

    private Vector3 ConvertVectorToVector3(float i_v0, float i_angle, Vector3 i_targetPosition,Vector3 m_shootPoint)
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
