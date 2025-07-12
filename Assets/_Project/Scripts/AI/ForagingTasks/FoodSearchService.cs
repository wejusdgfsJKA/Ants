using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("FoodSearchService")]
public class FoodSearchService : Service
{
    [SerializeField] protected TransformReference closestFood;
    [SerializeField] protected TransformReference currentLoad;
    [SerializeField] protected LayerMask foodMask = 1 << 6;
    [SerializeField] protected float sphereCheckRadius = 25;
    public override void Task()
    {
        if (currentLoad.Value == null)
        {
            //check in a radius around the ant
            Collider[] colliders = new Collider[50];
            int nr = Physics.OverlapSphereNonAlloc(transform.position, sphereCheckRadius,
                colliders, foodMask);
            if (nr > 0)
            {
                bool changed = false;
                Transform currentFood = closestFood.Value;
                Transform closest = currentFood;
                float bestDist = 0;
                if (currentFood != null)
                {
                    bestDist = Vector3.Distance(transform.position, currentFood.position);
                }
                for (int i = 0; i < nr; i++)
                {
                    if (colliders[i].gameObject.tag != ForageStrings.TakenTag)
                    {
                        if (closestFood.Value == null)
                        {
                            closest = colliders[i].transform;
                            bestDist = Vector3.Distance(transform.position, colliders[i].transform.position);
                            changed = true;
                        }
                        else
                        {
                            float newDist = Vector3.Distance(transform.position, colliders[i].transform.position);
                            if (newDist < bestDist)
                            {
                                closest = colliders[i].transform;
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
                    closestFood.Value = closest;
                    closest.gameObject.tag = ForageStrings.TakenTag;
                }
            }
        }
    }
}