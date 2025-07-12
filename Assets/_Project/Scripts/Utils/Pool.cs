using System.Collections.Generic;
public interface Identifiable<Id>
{
    public Id ID { get; set; }
}
public class Pool<T>
{
    protected Stack<T> stack = new();
    public void Release(T obj)
    {
        stack.Push(obj);
    }
    public T Get()
    {
        if (stack.Count > 0)
        {
            return stack.Pop();
        }
        return default;
    }
}
public class MultiPool<T, Id> where T : Identifiable<Id>
{
    protected Dictionary<Id, Pool<T>> pools = new();
    public void Release(T obj)
    {
        Pool<T> pool;
        if (pools.TryGetValue(obj.ID, out pool))
        {
            pool.Release(obj);
        }
        else
        {
            pool = new();
            pool.Release(obj);
            pools.Add(obj.ID, pool);
        }
    }
    public T Get(Id id)
    {
        Pool<T> pool;
        if (pools.TryGetValue(id, out pool))
        {
            return pool.Get();
        }
        return default;
    }
}