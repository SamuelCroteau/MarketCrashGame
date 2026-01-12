using JetBrains.Annotations;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Initial setup")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialCount = 10;
    
    [Header("Runtime behaviour")]
    [SerializeField] private bool allowInstanciation = true;

    private int count;
    private int activeCount;

    public int Count => count;
    public int ActiveCount => activeCount;

    private void Awake()
    {
        // Create initial instances.
        for (var i = 0; i < initialCount; i++)
        {
            Create(isActive: false);
        }
    }

    [CanBeNull]
    public GameObject Get()
    {
        // Try to find inactive instance.
        for (var i = 0; i < transform.childCount; i++)
        {
            var obj = transform.GetChild(i).gameObject;
            if (obj.activeSelf) continue;
            
            obj.SetActive(true);
            activeCount += 1;
            return obj;
        }

        // Otherwise, create if allowed (or return null).
        return allowInstanciation ? Create(isActive: true) : null;
    }

    [CanBeNull]
    public T Get<T>() where T : Component
    {
        // Try to get an instance.
        var obj = Get();
        if (obj == null) return null;
        
        // Try to get the specified component on the instance.
        var component = obj.GetComponent<T>();
        Debug.Assert(component != null, $"Object from pool {name} doesn't have a component of type {typeof(T)}.");
        
        // Return component.
        return component;
    }

    [CanBeNull]
    public GameObject Place(Vector3 position)
    {
        return Place(position, Quaternion.identity);
    }

    [CanBeNull]
    public GameObject Place(Vector3 position, Quaternion rotation)
    {
        // Try to get an instance.
        var obj = Get();
        if (obj == null) return null;
        
        // Place the instance in the world.
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        
        // Return instance.
        return obj;
    }

    [CanBeNull]
    public T Place<T>(Vector3 position) where T : Component
    {
        return Place<T>(position, Quaternion.identity);
    }

    [CanBeNull]
    public T Place<T>(Vector3 position, Quaternion rotation) where T : Component
    {
        // Try to get (and position) an instance.
        var obj = Place(position, rotation);
        if (obj == null) return null;
        
        // Try to get the specified component on the instance.
        var component = obj.GetComponent<T>();
        Debug.Assert(component != null, $"Object from pool {name} doesn't have a component of type {typeof(T)}.");
        
        // Return component.
        return component;
    }

    public void Release(GameObject instance)
    {
        // Disable the instance.
        instance.SetActive(false);
        
        // Decrease active instance count.
        activeCount -= 1;
        
        // Put object in pool, in case it was moved.
        instance.transform.parent = transform;
    }

    public void Release(Component instance)
    {
        Release(instance.gameObject);
    }

    private GameObject Create(bool isActive)
    {
        // Create new instance.
        var instance = Instantiate(prefab, transform);
        
        // Set active if needed (and increase active instance count).
        instance.SetActive(isActive);
        if (isActive) activeCount += 1;
        
        // Increate total instance count.
        count += 1;
        
        // Return instance.
        return instance;
    }
}