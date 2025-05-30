using EventBTree;
using UnityEngine;
public class AntBTree : BTree
{
    public enum AntType
    {
        Worker,
        Soldier
    }
    public readonly struct Strings
    {
        public static readonly string NestLocation = "NestLocation";
        public static readonly string ClosestFood = "ClosestFood";
        public static readonly string FoodStorage = "FoodStorage";
        public static readonly string TakenTag = "Taken";
    }
    [SerializeField] protected float stoppingDistance = 0.1f;
    [SerializeField] protected float meleeRange;
    [SerializeField] protected float sphereCheckRadius;
    [SerializeField] protected LayerMask foodMask;
    [SerializeField] protected Transform carryPoint;
    [field: SerializeField] public AntType Type { get; protected set; }
    protected override void SetupTree()
    {
        root = new Selector("Root");

        //self-defense node has highest prio


        if (Type == AntType.Worker)
        {
            root.AddChild(BuildForageSubtree());
        }
    }
    protected Sequence BuildForageSubtree()
    {
        LeafNode searchNode = new LeafNode("check", () =>
        {
            //wander around
            if (agent.AgentFree)
            {
                agent.Wander();
            }
            //check in a radius around the ant
            Collider[] colliders = new Collider[50];
            int nr = Physics.OverlapSphereNonAlloc(transform.position, sphereCheckRadius,
                colliders, foodMask);
            if (nr > 0)
            {

                Transform closestFood = null;
                float bestDist = 0;
                for (int i = 0; i < nr; i++)
                {
                    if (closestFood == null)
                    {
                        if (colliders[i].gameObject.tag != Strings.TakenTag)
                        {
                            closestFood = colliders[i].transform;
                            bestDist = Vector3.Distance(transform.position, colliders[i].transform.position);
                        }
                    }
                    else
                    {
                        if (colliders[i].gameObject.tag != Strings.TakenTag)
                        {
                            float newDist = Vector3.Distance(transform.position, colliders[i].transform.position);
                            if (newDist < bestDist)
                            {
                                closestFood = colliders[i].transform;
                                bestDist = newDist;
                            }
                        }
                    }
                }

                if (closestFood != null)
                {
                    localMemory.SetData(Strings.ClosestFood, closestFood);
                    closestFood.gameObject.tag = Strings.TakenTag;
                    return NodeState.SUCCESS;
                }
            }
            return NodeState.RUNNING;
        }, () =>
        {
            localMemory.SetData<Transform>(Strings.ClosestFood, null);
            agent.Wander();
        }, () =>
        {
            agent.Stop();
        });

        LeafNode acquireNode = new LeafNode("Acquire", () =>
        {
            Transform closestFood = localMemory.GetData<Transform>(Strings.ClosestFood);
            if (Vector3.Distance(transform.position, closestFood.position) <= meleeRange)
            {
                closestFood.transform.position = carryPoint.position;
                closestFood.parent = carryPoint;
                return NodeState.SUCCESS;
            }
            return NodeState.RUNNING;
        }, () =>
        {
            agent.MoveToPoint(localMemory.GetData<Transform>(Strings.ClosestFood).position);
        });
        acquireNode.AddDecorator(new Decorator("HasFood", () =>
        {
            return localMemory.GetData<Transform>(Strings.ClosestFood) != null;
        })).MonitorValue<Transform>(localMemory, Strings.ClosestFood);

        LeafNode deliverNode = new LeafNode("deliver", () =>
        {
            if (carryPoint.childCount == 0)
            {
                //we've lost the food we were carrying
                return NodeState.FAILURE;
            }
            if (Vector3.Distance(transform.position, localMemory.GetData<Vector3>(Strings.
                FoodStorage)) < stoppingDistance)
            {
                //we have delivered the food to the nest, unload
                Transform food = carryPoint.GetChild(0);
                food.parent = null;
                food.GetComponent<Collider>().enabled = false;
                return NodeState.SUCCESS;
            }
            return NodeState.RUNNING;
        }, () =>
        {
            agent.MoveToPoint(localMemory.GetData<Vector3>(Strings.FoodStorage));
        });

        Sequence forage = new Sequence("Forage");
        forage.AddChild(searchNode);
        forage.AddChild(acquireNode);
        forage.AddChild(deliverNode);
        return forage;
    }
}
