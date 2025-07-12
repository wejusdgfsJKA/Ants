using MBT;
using UnityEngine;

[AddComponentMenu("")]
// Register node in visual editor node finder
[MBTNode(name = "DropFoodTask")]
public class DropFoodTask : Leaf
{
    [SerializeField] protected float dropRange = 2;
    [SerializeField] protected TransformReference foodStorage;
    [SerializeField] protected TransformReference self;
    [SerializeField] protected TransformReference carryPoint;
    [SerializeField] protected TransformReference currentLoad;

    // This is called every tick as long as node is executed
    public override NodeResult Execute()
    {
        try
        {
            if (Vector3.Distance(self.Value.position, foodStorage.Value.position) <= dropRange)
            {
                //drop the current food
                currentLoad.Value.SetParent(foodStorage.Value);
                currentLoad.Value = null;
                return NodeResult.success;
            }
            return NodeResult.failure;
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            return NodeResult.failure;
        }
    }
}