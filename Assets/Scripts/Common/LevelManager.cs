using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    [SerializeField] public InGameUI inGameUI;
    [SerializeField] private int gameTimeInCycle = 3;

    private bool isCallingHour = false;

    [HideInInspector] public StocksManager stocksManager;
    private PlayerCharacter player;

    private Character[] characters;

    private void Awake()
    {
        stocksManager = GetComponent<StocksManager>();
        characters = FindObjectsByType<Character>(FindObjectsSortMode.None);
        player = Finder.Player;
    }

    private void Start()
    {
        player.stock = stocksManager.playableStocks[Random.Range(0, stocksManager.playableStocks.Length)].tick;

        InitializeCharactersStockPreferences();
        Finder.Clock.UpdateHours();
    }

    public void ComputeHourlyChanges()
    {
        stocksManager.UpdateStocks();

        foreach (var character in characters)
            character.AdvanceHour();

        if (Finder.Clock.currentCycle >= gameTimeInCycle || isCallingHour)
            EndGame();
    }

    private void ForceAdvanceOneHour()
    {
        stocksManager.UpdateStocks();
    }

    public void EndGame()
    {
        var clock = Finder.Clock;
        int remainingHours = (gameTimeInCycle * clock.maxHoursInCycle)
            - ((clock.currentCycle * clock.maxHoursInCycle) + clock.currentHour);
        Debug.Log($"Advancing {remainingHours} remaining hours...");

        for (int i = 0; i < remainingHours; i++)
            ForceAdvanceOneHour();

        stocksManager.PrintStocks();

        if (isCallingHour)
            Debug.Log("COPS ARE HERE!!");

        clock.shouldAdvanceTime = false;
        player.controller.enabled = false;
        Cursor.lockState = CursorLockMode.None;

        float finalScore = GetFinalScore();
        string grade = GetGrade(finalScore);

        var rankedStocks = stocksManager.allStocks
            .OrderByDescending(s => s.currentValue)
            .Select(s => new StockResult(s.stockName, s.tick, s.currentValue))
            .ToArray();

        var gameResults = FindFirstObjectByType<GameResults>();
        if (gameResults == null)
        {
            var go = new GameObject("GameResults");
            gameResults = go.AddComponent<GameResults>();
        }
        gameResults.SetResults(finalScore, grade, stocksManager.playerMoney, rankedStocks);

        var appController = Finder.AppController;
        if (appController != null)
        {
            appController.LoadCredits();
        }
        else
        {
            Debug.LogError("AppController not found! Make sure it exists in MainMenu scene with the 'AppController' tag.");
        }
    }

    private void InitializeCharactersStockPreferences()
    {
        foreach (var character in characters)
        {
            character.InitializeStockPreference(stocksManager.allStocks);
            character.GetStockInteraction().currentModifiers = character.GetStockInteraction().aliveModifiers;
        }
    }

    public void ReportCrime()
    {
        if (!isCallingHour)
        {
            isCallingHour = true;
            inGameUI.SetAlertVisuals();
        }
    }

    private string GetGrade(float score)
    {
        if (score < 200) return "F";
        if (score < 300) return "D";
        if (score < 400) return "C-";
        if (score < 500) return "C";
        if (score < 600) return "C+";

        if (score < 700) return "B-";
        if (score < 800) return "B";
        if (score < 900) return "B+";

        if (score < 1000) return "A-";
        if (score < 1100) return "A";
        if (score < 1200) return "A+";

        return "S";
    }

    private float GetFinalScore()
    {
        var rankedStocks = stocksManager.allStocks
            .OrderByDescending(s => s.currentValue)
            .ToArray();

        int playerStockRank = 1;
        for (int i = 0; i < rankedStocks.Length; i++)
        {
            if (rankedStocks[i].tick == player.stock)
            {
                playerStockRank = i + 1;
                break;
            }
        }

        float maxRankScore = 600f;
        float minRankScore = 50f;
        float rankScore = Mathf.Lerp(maxRankScore, minRankScore, 
            (playerStockRank - 1f) / (rankedStocks.Length - 1f));

        float portfolioValue = stocksManager.EvaluatePortfolio();
        stocksManager.playerMoney += portfolioValue;
        
        float netProfit = Mathf.Max(0, stocksManager.playerMoney - 200f);
        float wealthScore = netProfit * 0.5f;

        int totalHours = Finder.Clock.currentCycle * Finder.Clock.maxHoursInCycle
                        + Finder.Clock.currentHour;
        float timePenalty = totalHours * 3f;

        float caughtPenalty = isCallingHour ? 200f : 0f;

        float finalScore = rankScore + wealthScore - timePenalty - caughtPenalty;
        
        return finalScore;
    }

    public void StartDialogue(string characterName, string dialogueText)
    {
        inGameUI.StartDialogue(characterName, dialogueText);
    }

}