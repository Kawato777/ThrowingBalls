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
        int i = 0;
        foreach (PlayerInfo info in PlayerInfos)
        {
            Vector3 startPos = transform.position;
            float startAngle = 24 * i * Mathf.Deg2Rad;   //24ìxÅ~iî‘ñ⁄=äpìx
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
            info.Rb.velocity = Vector3.zero;
            info.Rb.angularVelocity = Vector3.zero;
            info.Agent.personShape.transform.position = info.StartingPos;
        }

        ballManager.ResetBall();
    }
}
