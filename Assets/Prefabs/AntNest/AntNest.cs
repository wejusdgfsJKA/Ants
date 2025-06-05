using MBT;
using UnityEngine;

public class AntNest : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] int nrOfAnts;
    private void OnEnable()
    {
        for (int i = 0; i < nrOfAnts; i++)
        {
            var ant = Instantiate(prefab, transform.position, Quaternion.identity);
            ant.transform.GetChild(1).GetComponent<Blackboard>().
                GetVariable<TransformVariable>("FoodStorage").Value = transform;
        }
    }
}
