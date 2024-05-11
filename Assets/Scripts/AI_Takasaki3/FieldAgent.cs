using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class FieldAgent : Agent
{
    [System.Serializable]
    public class PlayerInfo
    {
        public Person3 person;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Rigidbody Rb;
    }

    public List<PlayerInfo> PersonInfos;
    List<Person3> persons = new List<Person3>();

    private int m_ResetTimer;

    public BallManager3 ballManager;

    public float playAreaDiameter, personTall, maxBallsNum;
    public float gosa = 0.5f;
    public Transform goalTF;
    public float height;

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach (PlayerInfo info in PersonInfos)
        {
            // Debug.Log(PersonInfos.Count);
            persons.Add(info.person);
            Vector3 startPos = transform.position;
            float startAngle = 360 / PersonInfos.Count * i * Mathf.Deg2Rad;   //24ìxÅ~iî‘ñ⁄=äpìx
            startPos.x += playAreaDiameter / 2 * Mathf.Cos(startAngle);
            startPos.z += playAreaDiameter / 2 * Mathf.Sin(startAngle);
            startPos.y += personTall / 2;
            info.StartingPos = startPos;
            Vector3 scale = info.person.personShape.transform.localScale;
            scale.y = personTall;
            info.person.personShape.transform.localScale = scale;
            info.Rb = info.person.personShape.GetComponent<Rigidbody>();
            i++;
        }
        ResetScene();
    }

    private void FixedUpdate()
    {
        m_ResetTimer++;
        if (m_ResetTimer >= MaxStep && MaxStep > 0)
        {
            EndEpisodeFromOthers();
        }
        AddReward(-1 / MaxStep);
    }

    public void EndEpisodeFromOthers()
    {
        ResetScene();
        EndEpisode();
    }

    void ResetScene()
    {
        m_ResetTimer = 0;

        foreach (PlayerInfo info in PersonInfos)
        {
            info.Rb.velocity = Vector3.zero;
            info.Rb.angularVelocity = Vector3.zero;
            info.person.personShape.transform.position = info.StartingPos;
            info.person.ResetPerson();
        }

        ballManager.ResetBall();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        foreach(var info in PersonInfos)
        {
            sensor.AddObservation(info.person.ballPocket.Count);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int n = 0;
        foreach(var info in PersonInfos)
        {
            info.person.Action(new Vector2(actions.ContinuousActions[2 * n],
                                actions.ContinuousActions[2 * n + 1]),
                                actions.DiscreteActions[n],
                                actions.DiscreteActions[n + 15]);
            n++;
        }
    }

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        int n = 0;
        foreach (var info in PersonInfos)
        {
            actionMask.SetActionEnabled(n, 1, info.person.throwable);
            for (int i = 1; i <= 15; i++)
            {
                actionMask.SetActionEnabled(n + 15, i, false);
            }
            foreach (var passablePerson in info.person.passablePersons)
            {
                actionMask.SetActionEnabled(n + 15, persons.IndexOf(passablePerson) + 1, true);
            }
            n++;
        }
        
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var banana0 = actionsOut.ContinuousActions;
        var banana1 = actionsOut.DiscreteActions;
        //int n = 0;
        //foreach(var info in PersonInfos)
        //{
          //  banana0[2 * n] = Input.GetAxis("Horizontal");
            //banana0[2 * n + 1] = Input.GetAxis("Vertical");
            //if (Input.GetKeyDown(KeyCode.Space) && info.person.throwable)
            //{
              //  banana1[0] = 1;
            //}
            //else
            //{
             //   banana1[0] = 0;
            //}
            //n++;
        //}
        banana0[0] = Input.GetAxis("Horizontal");
        banana0[1] = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.Space) && persons[0].throwable)
        {
            banana1[0] = 1;
        }
        else
        {
            banana1[0] = 0;
        }
    }

    public void Goal()
    {
        AddReward(0.05f);
    }
}
