using UnityEngine;

[System.Serializable]
public class StockInteraction
{
    public StockModifier[] aliveModifiers = new StockModifier[0];
    public StockModifier[] deadModifiers = new StockModifier[0];

    public StockModifier[] currentModifiers = new StockModifier[0];
}