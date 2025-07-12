using UnityEngine;

public interface IDataRecipient<Id> : Identifiable<Id>
{
    public EntityData<Id> Data { set; }
}
public class EntityData<Id> : ScriptableObject
{
    [field: SerializeField] public Id ID { get; protected set; }
    [field: SerializeField] public Entity<Id> Prefab { get; protected set; }
}
