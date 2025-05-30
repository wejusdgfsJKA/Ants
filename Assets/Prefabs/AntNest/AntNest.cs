using UnityEngine;

public class AntNest : MonoBehaviour
{
    [SerializeField] AntBTree prefab;
    [SerializeField] int nrOfAnts;
    private void OnEnable()
    {
        for (int i = 0; i < nrOfAnts; i++)
        {
            AntBTree ant = Instantiate(prefab, transform.position, Quaternion.identity);
            ant.SetData(AntBTree.Strings.NestLocation, transform.position);
            ant.SetData(AntBTree.Strings.FoodStorage, transform.position);
        }
    }
}
