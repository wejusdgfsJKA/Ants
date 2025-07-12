public enum FoodType
{
    Seed,
    BigSeed
}
public class FoodManager : EntityManager<FoodType>
{
    public static FoodManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
