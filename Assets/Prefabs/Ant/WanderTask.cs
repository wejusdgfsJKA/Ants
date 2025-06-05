using EventBTree;
using MBT;
using UnityEngine;

[AddComponentMenu("")]
// Register node in visual editor node finder
[MBTNode(name = "WanderTask")]
public class WanderTask : Leaf
{
    [SerializeField] protected BTAgent agent;
    public override NodeResult Execute()
    {
        if (agent.AgentFree)
        {
            agent.Wander();
        }
        return NodeResult.running;
    }
}
