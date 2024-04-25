using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.MLAgents;
using UnityEngine;

public class FieldManager2 : MonoBehaviour
{
    [System.Serializable]
    public class PlayerInfo
    {
        public PersonAgent2 Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Rigidbody Rb;
    }

    [Tooltip("Max Environment Steps")] public int MaxEnvironmentSteps = 30000;

    public List<PlayerInfo> PersonInfos;
    List<PersonAgent2> personAgents = new List<PersonAgent2>();

    SimpleMultiAgentGroup agentGroup;

    private int m_ResetTimer;

    public BallManager2 ballManager;

    public float playAreaDiameter, personTall,maxBallsNum;
    public float gosa = 0.5f;
    public Transform goalTF;
    public float height;

    // Start is called before the first frame update
    void Start()
    {
        agentGroup = new SimpleMultiAgentGroup();
        int i = 0;
        foreach (PlayerInfo info in PersonInfos)
        {
            Debug.Log(PersonInfos.Count);
            personAgents.Add(info.Agent);
            Vector3 startPos = transform.position;
            float startAngle = 360/PersonInfos.Count * i * Mathf.Deg2Rad;   //24�x�~i�Ԗ�=�p�x
            startPos.x += playAreaDiameter / 2 * Mathf.Cos(startAngle);
            startPos.z += playAreaDiameter / 2 * Mathf.Sin(startAngle);
            startPos.y += personTall / 2;
            info.StartingPos = startPos;
            Vector3 scale = info.Agent.personShape.transform.localScale;
            scale.y = personTall;
            info.Agent.personShape.transform.localScale = scale;
            info.Rb = info.Agent.personShape.GetComponent<Rigidbody>();
            agentGroup.RegisterAgent(info.Agent);
            i++;
        }
        ResetScene();
    }

    private void FixedUpdate()
    {
        m_ResetTimer++;
        if(m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            EndEpisode();
        }
        agentGroup.AddGroupReward(-1 / MaxEnvironmentSteps);
    }

    public void Goal()
    {
        agentGroup.AddGroupReward(0.05f);
    }

    public void EndEpisode()
    {
        agentGroup.GroupEpisodeInterrupted();
        ResetScene();
    }

    void ResetScene()
    {
        m_ResetTimer = 0;

        foreach(PlayerInfo info in PersonInfos)
        {
            info.Rb.velocity = Vector3.zero;
            info.Rb.angularVelocity = Vector3.zero;
            info.Agent.personShape.transform.position = info.StartingPos;
        }

        ballManager.ResetBall();
    }

    public int GetPersonNum(PersonAgent2 person)
    {
        return personAgents.IndexOf(person);
    }
}
