using DefaultNamespace;
using UnityEngine;
using UnityEngine.Pool;

public abstract class WeaponRange : WeaponBase
{
    [SerializeField] private Tracer tracerPrefab;
    private ObjectPool<Tracer> tracerPool;

    protected override void Awake()
    {
        base.Awake();
        tracerPool = new ObjectPool<Tracer>(() => Instantiate(tracerPrefab), t => t.gameObject.SetActive(true),
            t => t.gameObject.SetActive(false));
    }

    protected void SpawnTracer(Vector3 start, Vector3 end, float speed)
    {
        Tracer tracer = tracerPool.Get();
        tracer.Initialize(start, end, speed, () => tracerPool.Release(tracer));
    }
}
