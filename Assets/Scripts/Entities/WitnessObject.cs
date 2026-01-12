using UnityEngine;
using System.Linq;

public class WitnessObject : MonoBehaviour, IBreakable, IStockInfluence, IWitness
{
    const int NUMBER_OF_CRIMES_TO_WITNESS_BEFORE_CALL = 1;

    [SerializeField] private Texture normalTexture;
    [SerializeField] private Texture brokenTexture;

    [SerializeField] private StockInteraction stockInteraction = new StockInteraction();

    private AudioPlayerPool audioPool;

    private Crime[] witnessedCrimes = new Crime[NUMBER_OF_CRIMES_TO_WITNESS_BEFORE_CALL];
    private bool _isBroken = false;
    private bool isCalling = false;
    private Renderer _renderer;

    public bool IsBroken
    { 
        get { return _isBroken; } 
        set { _isBroken = value; } 
    }

    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();

        if (_renderer != null && normalTexture != null)
            _renderer.material.SetTexture("_BaseMap", normalTexture);
        stockInteraction.currentModifiers = stockInteraction.aliveModifiers;

        audioPool = GetComponent<AudioPlayerPool>();
    }

    public void Break()
    {
        if (!IsBroken)
        {
            GetComponent<SphereCollider>().enabled = false;
            IsBroken = true;
            stockInteraction.currentModifiers = stockInteraction.deadModifiers;

            audioPool.PlaySound();
            _renderer.material.SetTexture("_BaseMap", brokenTexture);
            Finder.LevelManager.inGameUI.ShowAlert(GetObjectBreakMessage());
        }
    }

    private string GetObjectBreakMessage()
    {
        string message = "You broke an object... ";
        if (stockInteraction.currentModifiers.Length > 0)
        {
            message += "it influenced ";
            for (int i = 0; i < stockInteraction.currentModifiers.Length; i++)
            {
                message += $" {stockInteraction.currentModifiers[i].tick}";
            }
        }
        else
        {
            message += "it did nothing lol";
        }
        if (GetComponent<Crime>() != null)
        {
            message += "... but it was a crime!";
        }
        return message;
    }

    public void WitnessCrime(Crime crime)
    {
        if (IsBroken || isCalling)
            return;
        if (witnessedCrimes.Contains(crime))
            return;
        for (int i = 0; i < witnessedCrimes.Length; i++)
        {
            if (witnessedCrimes[i] == null)
            {
                witnessedCrimes[i] = crime;
                break;
            }
        }
        if (witnessedCrimes[NUMBER_OF_CRIMES_TO_WITNESS_BEFORE_CALL - 1] != null)
            CallAuthorities();
    }

    private void CallAuthorities()
    {
        isCalling = true;
        Finder.LevelManager.ReportCrime();
    }

    public StockInteraction GetStockInteraction()
    {
        return stockInteraction;
    }
}
