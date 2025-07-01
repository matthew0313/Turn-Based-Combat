using System.Collections.Generic;
using UnityEngine;

public abstract class PooledPrefab<T> : MonoBehaviour where T : PooledPrefab<T>
{
    List<T> pool;
    bool instantiated = false;
    public bool released { get; private set; } = false;
    protected virtual T Create()
    {
        T tmp = Instantiate(this as T);
        tmp.pool = pool;
        tmp.instantiated = true;
        tmp.OnCreate();
        return tmp;
    }
    protected virtual void OnCreate() { }
    public T Get()
    {
        if (instantiated) return null;

        T tmp;
        if (pool == null) pool = new();
        if (pool.Count == 0) tmp = Create();
        else
        {
            tmp = pool[0];
            pool.RemoveAt(0);
            tmp.released = false;
        }
        tmp.OnGet();
        return tmp;
    }
    public T Get(Vector3 position, Quaternion rotation)
    {
        T tmp = Get();
        if(tmp != null)
        {
            tmp.transform.position = position;
            tmp.transform.rotation = rotation;
        }
        return tmp;
    }
    public void Release()
    {
        if (released) return;

        OnRelease();
        pool.Add(this as T);
        released = true;
    }
    protected virtual void OnGet() => gameObject.SetActive(true);
    protected virtual void OnRelease() => gameObject.SetActive(false);
    private void OnDestroy()
    {
        if (released) pool.Remove(this as T);
    }
}