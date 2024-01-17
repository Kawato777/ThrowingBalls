using System.Collections;
using System.Collections.Generic;
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

    public List<PlayerInfo> PlayerInfos;

    SimpleMultiAgentGroup agentGroup;

    private int m_ResetTimer;

    public BallManager2 ballManager;

    public float playAreaDiameter, personTall,maxBallsNum;
    public float gosa = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        agentGroup = new SimpleMultiAgentGroup();
        foreach (PlayerInfo info in PlayerInfos)
        {
            info.StartingPos = info.Agent.personShape.transform.position;
            info.Rb = info.Agent.personShape.GetComponent<Rigidbody>();
            agentGroup.RegisterAgent(info.Agent);
        }
        ResetScene();
    }

    private void FixedUpdate()
    {
        m_ResetTimer++;
        if(m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            agentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }
    }

    public void Goal()
    {
        agentGroup.AddGroupReward(0.05f);
    }

    void ResetScene()
    {
        m_ResetTimer = 0;

        foreach(PlayerInfo info in PlayerInfos)
        {
            info.Agent.personShape.transform.position = info.StartingPos;
            info.Rb.velocity = Vector3.zero;
            info.Rb.angularVelocity = Vector3.zero;
        }

        ballManager.ResetBall();
    }
}
