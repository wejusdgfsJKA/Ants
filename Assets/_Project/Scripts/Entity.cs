using UnityEngine;

public abstract class Entity<Id> : MonoBehaviour, IDataRecipient<Id>
{
    public virtual EntityData<Id> Data
    {
        set
        {
            ID = value.ID;
        }
    }
    public Id ID { get; set; }

}
