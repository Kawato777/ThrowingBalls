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
