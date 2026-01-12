using UnityEngine;

public class ObjectPools : MonoBehaviour
{
    [field: Header("Collectibles")]
    [field: SerializeField] public ObjectPool Coins { get; private set; }

    [field: Header("Fx")]
    [field: SerializeField] public ObjectPool StarFx { get; private set; }
}