using MBT;
using UnityEngine;

[AddComponentMenu("")]
// Register node in visual editor node finder
[MBTNode(name = "GrabFoodTask")]
public class GrabFoodTask : Leaf
{
    [SerializeField] protected float grabRange = 2;
    [SerializeField] protected TransformReference closestFood;
    [SerializeField] protected TransformReference self;
    [SerializeField] protected TransformReference carryPoint;
    [SerializeField] protected TransformReference currentLoad;

    // This is called every tick as long as node is executed
    public override void OnExit()
    {
        base.OnExit();
        closestFood.Value = null;
    }
    public override NodeResult Execute()
    {
        if (Vector3.Distance(self.Value.position, closestFood.Value.position) <= grabRange)
        {
            //grab range
            closestFood.Value.SetParent(carryPoint.Value);
            closestFood.Value.GetComponent<Collider>().enabled = false;
            closestFood.Value.position = carryPoint.Value.position;
            currentLoad.Value = closestFood.Value;
            return NodeResult.success;
        }
        return NodeResult.failure;
    }
}