using TMPro;
using UnityEngine;
using System.Text;

public class CreditsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stockLeaderboardText;
    [SerializeField] private TextMeshProUGUI playerMoneyText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gradeText;

    private void Start()
    {
        var gameResults = GameResults.Instance;
        if (gameResults == null) return;

        scoreText.text = $"{gameResults.playerScore:F0}";
        
        gradeText.text = $"{gameResults.scoreGrade}";
        
        playerMoneyText.text = $"{gameResults.playerMoney:F0}$!?";

        if (stockLeaderboardText != null && gameResults.rankedStocks != null)
        {
            StringBuilder sb = new StringBuilder();
            
            for (int i = 0; i < gameResults.rankedStocks.Length; i++)
            {
                var stock = gameResults.rankedStocks[i];
                sb.AppendLine($"#{i + 1} {stock.stockName} ({stock.tick}): ${stock.currentValue:F0}");
            }
            
            stockLeaderboardText.text = sb.ToString();
        }
    }
}