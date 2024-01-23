using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

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
        }
    }
}
