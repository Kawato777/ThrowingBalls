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
    Transform pocket, nose;
    Rigidbody rBody;
    bool isThrowable;
    List<GameObject> havingBalls = new List<GameObject>();
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
        // Rigidbodyë„ì¸
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
            if(havingBalls.Count == 6)
            {
                isThrowable = true;
            }
            ball.transform.parent = pocket;
            ball.GetComponent<Collider>().enabled = false;
            ball.GetComponent<Rigidbody>().isKinematic = true;
            ball.SetActive(false);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // çsìÆÉ]Å[Éì
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

        // ïÒèVälìæÉ]Å[Éì
    }

    void Throw(Vector3 firstVelocity)
    {
        foreach(var ball in havingBalls)
        {
            ball.SetActive(true);
            Quaternion quaternion = Quaternion.identity;
            ball.transform.localRotation = quaternion;
            ball.transform.position = new(nose.position.x, nose.position.y + 0.2f, nose.position.z);
            
            // ballManager.SetBallParent(ball);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var banana = actionsOut.ContinuousActions;
        var banana2 = actionsOut.DiscreteActions;
        banana[0] = Input.GetAxis("Horizontal");
        banana[1] = Input.GetAxis("Vertical");
        banana[2] = Input.GetAxis("Mouse X");
        if (Input.GetKeyDown(KeyCode.Space))
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
