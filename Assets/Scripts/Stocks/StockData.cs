using UnityEngine;

[CreateAssetMenu(menuName = "Stock/StockData")]
public class StockData : ScriptableObject
{
    public string stockName;
    public string tick;
    public float startValue;
    public bool canBePreferredOrDisliked = true;
    public bool canBeInvestedIn = true;
   
    [SerializeField] public DialoguePools LoveDialoguePools;
    [SerializeField] public DialoguePools HateDialoguePools;

    [HideInInspector] public float currentValue = 0.0f;
    [HideInInspector] public float lastPrice = 0.0f;
}