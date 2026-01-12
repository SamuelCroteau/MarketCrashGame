using UnityEngine;

public class Object : MonoBehaviour, IBreakable, IStockInfluence
{
    [SerializeField] private Texture normalTexture;
    [SerializeField] private Texture brokenTexture;

    [SerializeField] private StockInteraction stockInteraction = new StockInteraction();

    private AudioPlayerPool audioPool;

    private bool _isBroken = false;
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

    public StockInteraction GetStockInteraction()
    {
        return stockInteraction;
    }
}
