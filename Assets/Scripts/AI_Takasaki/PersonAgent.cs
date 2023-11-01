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
    Rigidbody rBody;
    bool isThrowable;
    int havingBallsNum;
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
        // Rigidbody���
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        ballManager.DestroyBalls();
        ballManager.CreateBalls();
        SetPersonPos();
    }

    void SetPersonPos()
    {
        float playerTall = Random.Range(1.6f, 1.9f);
        float radius = fieldManager.playAreaDiameter / 2;
        Vector3 center = fieldManager.transform.position;
        float angle = Random.Range(0, 360);
        float rad = angle * Mathf.Deg2Rad;
        float px = Mathf.Cos(rad) * radius + center.x;
        float pz = Mathf.Sin(rad) * radius + center.z;
        transform.localScale = new(1f, playerTall, 1f);
        transform.localPosition = new(px, playerTall / 2, pz);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(havingBallsNum);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // �s���]�[��
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        rBody.AddForce(controlSignal * 10);
        if (actions.DiscreteActions[0] == 1)
        {
            Vector3 firstVelocity = new(actions.ContinuousActions[2], actions.ContinuousActions[3], actions.ContinuousActions[4]);
            Throw(firstVelocity);
        }

        // ��V�l���]�[��
    }

    void Throw(Vector3 firstVelocity)
    {

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var banana = actionsOut.ContinuousActions;
        banana[0] = Input.GetAxis("Horizontal");
        banana[1] = Input.GetAxis("Vertical");
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
