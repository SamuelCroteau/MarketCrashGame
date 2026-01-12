using UnityEngine;

public class Crime : MonoBehaviour
{
    [SerializeField] float range = 1.0f;
    [SerializeField] bool crimeWhileBroken = false;
    [SerializeField] float crimeCheckInterval = 1.0f;

    private IBreakable breakable;
    private float timeSinceLastCheck = 0f;
    
    private Collider[] hitBuffer = new Collider[20];

    private void Awake()
    {
        breakable = GetComponent<IBreakable>();
    }

    public void triggerCrime()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, range, hitBuffer);
        
        for (int i = 0; i < hitCount; i++)
        {
            var witness = hitBuffer[i].GetComponent<IWitness>();
            if (witness == null)
                continue;
                
            Vector3 direction = (hitBuffer[i].transform.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, direction, range))
                witness.WitnessCrime(this);
        }
    }

    private void Update()
    {
        if (crimeWhileBroken && breakable != null && breakable.IsBroken)
        {
            timeSinceLastCheck += Time.deltaTime;
            
            if (timeSinceLastCheck >= crimeCheckInterval)
            {
                timeSinceLastCheck = 0f;
                triggerCrime();
            }
        }
    }
}