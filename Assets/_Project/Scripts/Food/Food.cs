using UnityEngine;

public class Food : Entity<FoodType>
{
    [field: SerializeField] public int Value { get; set; }
}
