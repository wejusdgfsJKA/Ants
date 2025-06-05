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
            //localMemory.SetData<Transform>(Strings.ClosestFood, null);
            root.AddChild(BuildForageSubtree());
        }
    }
    protected Selector BuildForageSubtree()
    {
        LeafNode wanderNode = new LeafNode("check", () =>
        {
            //wander around
            if (agent.AgentFree)
            {
                agent.Wander();
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

        Selector forage = new Selector("Forage");
        Sequence transport = BuildTransportSubtree();
        forage.AddService(new Service("Check for food", () =>
        {
            if (carryPoint.childCount == 0)
            {
                //check in a radius around the ant
                Collider[] colliders = new Collider[50];
                int nr = Physics.OverlapSphereNonAlloc(transform.position, sphereCheckRadius,
                    colliders, foodMask);
                if (nr > 0)
                {
                    bool changed = false;
                    Transform currentFood = localMemory.GetData<Transform>(Strings.ClosestFood);
                    Transform closestFood = currentFood;
                    float bestDist = 0;
                    if (currentFood != null)
                    {
                        bestDist = Vector3.Distance(transform.position, currentFood.position);
                    }
                    for (int i = 0; i < nr; i++)
                    {
                        Debug.DrawLine(transform.position, colliders[i].transform.position, Color.green, 0.5f);
                        if (colliders[i].gameObject.tag != Strings.TakenTag)
                        {
                            if (closestFood == null)
                            {
                                closestFood = colliders[i].transform;
                                bestDist = Vector3.Distance(transform.position, colliders[i].transform.position);
                                changed = true;
                            }
                            else
                            {
                                float newDist = Vector3.Distance(transform.position, colliders[i].transform.position);
                                if (newDist < bestDist)
                                {
                                    closestFood = colliders[i].transform;
                                    bestDist = newDist;
                                    changed = true;
                                }
                            }
                        }
                    }
                    if (changed)
                    {
                        if (currentFood != null)
                        {
                            currentFood.gameObject.tag = "Untagged";
                        }
                        localMemory.SetData(Strings.ClosestFood, closestFood);
                        closestFood.gameObject.tag = Strings.TakenTag;
                        transport.Restart();
                    }
                }
                if (localMemory.GetData<Transform>(Strings.ClosestFood) != null)
                {
                    Debug.DrawLine(transform.position, localMemory.GetData<Transform>(Strings.ClosestFood).position, Color.blue, 0.5f);
                }
            }
        }));
        forage.AddChild(transport);
        forage.AddChild(wanderNode);
        return forage;
    }
    protected Sequence BuildTransportSubtree()
    {
        LeafNode acquireNode = new LeafNode("Acquire", () =>
        {
            Transform closestFood = localMemory.GetData<Transform>(Strings.ClosestFood);
            if (closestFood != null)
            {
                if (Vector3.Distance(transform.position, closestFood.position) <= meleeRange)
                {
                    closestFood.transform.position = carryPoint.position;
                    closestFood.parent = carryPoint;
                    return NodeState.SUCCESS;
                }
                return NodeState.RUNNING;
            }
            return NodeState.FAILURE;
        }, () =>
        {
            Transform food = localMemory.GetData<Transform>(Strings.ClosestFood);
            if (food != null)
            {
                agent.MoveToPoint(food.position);
            }
        });

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
                localMemory.SetData<Transform>(Strings.ClosestFood, null);
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

        Sequence transport = new Sequence("Transport");
        transport.AddDecorator(new Decorator("HasFood", () =>
        {
            return localMemory.GetData<Transform>(Strings.ClosestFood) != null;
        })).MonitorValue<Transform>(localMemory, Strings.ClosestFood);
        transport.AddChild(acquireNode);
        transport.AddChild(deliverNode);
        return transport;
    }
}
