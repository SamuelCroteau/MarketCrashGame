using UnityEngine;

public class StocksManager : MonoBehaviour
{
    public StockData[] playableStocks;
    public StockData[] unplayableStocks;

    [HideInInspector]
    public StockData[] allStocks;

    public StockAsset[] playerStockAssets;
    
    public float playerMoney = 200f;

    private bool firstHourPassed = false;

    private StockInteraction[] stockInteractions;

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("StockInteraction");
        stockInteractions = new StockInteraction[objs.Length];

        for (int i = 0; i < objs.Length; i++)
            stockInteractions[i] = objs[i].GetComponent<IStockInfluence>().GetStockInteraction();

        allStocks = new StockData[playableStocks.Length + unplayableStocks.Length];
        playableStocks.CopyTo(allStocks, 0);
        unplayableStocks.CopyTo(allStocks, playableStocks.Length);

        // Count investable stocks
        int investableCount = 0;
        foreach (StockData stock in allStocks)
        {
            if (stock.canBeInvestedIn)
                investableCount++;
        }

        // Only create assets for investable stocks
        playerStockAssets = new StockAsset[investableCount];
        int assetIndex = 0;
        for (int i = 0; i < allStocks.Length; i++)
        {
            if (allStocks[i].canBeInvestedIn)
            {
                playerStockAssets[assetIndex] = new StockAsset();
                playerStockAssets[assetIndex].stock = allStocks[i];
                playerStockAssets[assetIndex].quantityOwned = 0;
                assetIndex++;
            }
        }
        
        SetupStocks();
    }

    private void SetupStocks()
    {
        for (int i = 0; i < allStocks.Length; i++)
        {
            allStocks[i].currentValue = allStocks[i].startValue;
            allStocks[i].lastPrice = allStocks[i].startValue;
        }
    }

    public void TryBuy(string tick)
    {
        foreach (StockAsset stockAsset in playerStockAssets)
        {
            if (stockAsset.stock.tick == tick)
            {
                if (playerMoney >= stockAsset.stock.currentValue && stockAsset.stock.currentValue > 0)
                {
                    playerMoney -= stockAsset.stock.currentValue;
                    stockAsset.quantityOwned += 1;
                }
            }
        }
    }

    public void TrySell(string tick)
    {
        foreach (StockAsset stockAsset in playerStockAssets)
        {
            if (stockAsset.stock.tick == tick)
            {
                if (stockAsset.quantityOwned > 0)
                {
                    if (stockAsset.stock.currentValue > 0)
                        playerMoney += stockAsset.stock.currentValue;
                    stockAsset.quantityOwned -= 1;
                }
            }
        }
    }

    public void UpdateStocks()
    {
        if (firstHourPassed)
        {
            foreach (StockData stock in allStocks)
            {
                stock.lastPrice = stock.currentValue;
            }
            firstHourPassed = true;
        }
        foreach (StockInteraction interaction in stockInteractions)
        {
            foreach (StockModifier modifier in interaction.currentModifiers)
            {
                ApplyModifier(modifier);
            }
        }
    }

    private void ApplyModifier(StockModifier modifier)
    {
        foreach (StockData stock in allStocks)
        {
            if (stock.tick == modifier.tick)
            {
                stock.currentValue = GetNewValue(stock, modifier.value);
            }
        }
    }

    private float GetNewValue(StockData stock, ModValue modValue)
    {
        float value = stock.currentValue;

        switch (modValue)
        {
            case ModValue.EXPONENTIALLY_RISE:
                value += 30f;
                break;

            case ModValue.STRONGLY_RISE:
                value += 10f;
                break;

            case ModValue.LIGHTLY_RISE:
                value += 2f;
                break;

            case ModValue.LIGHTLY_LOWER:
                value -= 2f;
                break;

            case ModValue.STRONGLY_LOWER:
                value -= 10f;
                break;

            case ModValue.EXPONENTIALLY_LOWER:
                value -= 30f;
                break;
        }
        return value;
    }

    public float EvaluatePortfolio()
    {
        float totalValue = 0f;
        foreach (StockAsset asset in playerStockAssets)
            totalValue += asset.quantityOwned * asset.stock.currentValue;
        return totalValue;
    }

    public void PrintStocks()
    {
        Debug.Log("===============STOCKS===============");
        foreach (StockData stock in allStocks)
            Debug.Log(stock.stockName + "(" + stock.tick + ") : " + stock.currentValue + "$");
        Debug.Log("====================================");
    }
}
