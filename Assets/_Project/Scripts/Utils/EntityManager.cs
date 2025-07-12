using System.Collections.Generic;
using UnityEngine;
public abstract class EntityManager<Id> : MonoBehaviour
{
    protected MultiPool<Entity<Id>, Id> multiPool = new();
    [SerializeField] protected List<EntityData<Id>> entries = new();
    protected Dictionary<Id, EntityData<Id>> roster = new();
    protected void OnEnable()
    {
        for (int i = 0; i < entries.Count; i++)
        {
            roster.Add(entries[i].ID, entries[i]);
        }
    }
    public Entity<Id> Create(Id id, Vector3 position)
    {
        return Create(id, position, Quaternion.identity);
    }
    public Entity<Id> Create(Id id, Vector3 position, Quaternion rotation)
    {
        Entity<Id> e = multiPool.Get(id);
        if (e == null)
        {
            EntityData<Id> data;
            if (roster.TryGetValue(id, out data))
            {
                e = Instantiate(data.Prefab, position, rotation);
                e.Data = data;
            }
            else
            {
                Debug.LogError($"No roster entry found for id {id}");
            }
        }
        return e;
    }
    public void ReturnToPool(Entity<Id> entity)
    {
        entity.gameObject.SetActive(false);
        multiPool.Release(entity);
    }
}