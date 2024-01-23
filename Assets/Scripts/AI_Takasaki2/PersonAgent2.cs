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
    List<GameObject> passablePersons = new List<GameObject>();
    bool throwable = false;
    bool passable = false;

    public void TakeBallToPocket(GameObject ball)
    {
        if(ballPocket.Count >= 6)
        {
            return;
        }

        passable = true;
        ballPocket.Add(ball);
        
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

    public void SetPassablePerson(GameObject person,bool isRemoved)
    {
        if (isRemoved) // îÕàÕÇ©ÇÁèoÇÈ
        {
            passablePersons.Remove(person);
        }
        else // îÕàÕÇ…ì¸ÇÈ
        {
            passablePersons.Add(person);
        }
    }

    public override void Initialize()
    {
    }

    public override void OnEpisodeBegin()
    {
    }

    public override void CollectObservations(VectorSensor sensor)
    {
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
    }

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
    }

    
}
