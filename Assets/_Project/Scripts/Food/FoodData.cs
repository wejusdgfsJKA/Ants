using UnityEngine;

[CreateAssetMenu(fileName = "Food", menuName = "Ants/Food")]
public class FoodData : EntityData<FoodType>
{
    [field: SerializeField] public float Value { get; protected set; }
}
