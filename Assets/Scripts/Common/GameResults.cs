using UnityEngine;
using System;

[Serializable]
public class StockResult
{
    public string stockName;
    public string tick;
    public float currentValue;

    public StockResult(string stockName, string tick, float currentValue)
    {
        this.stockName = stockName;
        this.tick = tick;
        this.currentValue = currentValue;
    }
}

public class GameResults : MonoBehaviour
{
    public static GameResults Instance { get; private set; }

    public float playerScore;
    public string scoreGrade;
    public float playerMoney;
    public StockResult[] rankedStocks;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetResults(float score, string grade, float money, StockResult[] stocks)
    {
        playerScore = score;
        scoreGrade = grade;
        playerMoney = money;
        
        if (stocks != null && stocks.Length > 0)
        {
            rankedStocks = new StockResult[stocks.Length];
            Array.Copy(stocks, rankedStocks, stocks.Length);
            Array.Sort(rankedStocks, (a, b) => b.currentValue.CompareTo(a.currentValue));
        }
        else
        {
            rankedStocks = stocks;
        }
    }

    public void ClearResults()
    {
        playerScore = 0;
        scoreGrade = "";
        playerMoney = 0;
        rankedStocks = null;
    }
}