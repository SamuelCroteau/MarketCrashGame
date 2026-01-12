using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

// Très Bancale
public class InGameUI : MonoBehaviour
{
    [SerializeField] private Animator cursorAnimator;
    [SerializeField] private Animator fistAnimator;
    [SerializeField] private Animator alertAnimator;

    [SerializeField] private TextMeshProUGUI clockText;

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI alertText;
    [SerializeField] private Image fadeImage;

    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI stockText;

    [SerializeField] private GameObject investmentPanel;

    [SerializeField] private TextMeshProUGUI givenStockText;
    private bool isInvestmentPanelOpen = false;

    [Header("Inputs")]
    [SerializeField] private InputActionReference UIMoveActionRef;
    [SerializeField] private InputActionReference investmentPanelActionRef;
    private InputAction investmentAction;
    private InputAction UIMoveAction;

    private int selectedStockIndex = 0;
    private float inputCooldown = 0f;
    private const float INPUT_COOLDOWN_TIME = 0.2f;

    private Coroutine activeDialogue;

    private void Awake()
    {
        investmentAction = investmentPanelActionRef.action;
        UIMoveAction = UIMoveActionRef.action;
    }

    private void Start()
    {
        SwitchToNormalCursor();
        CloseInvestmentPanel();

        characterNameText.text = "";
        this.dialogueText.text = "";
        fadeImage.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        investmentAction.Enable();
        UIMoveAction.Enable();
        investmentAction.performed += ctx => PressInvestmentAction();
    }

    private void OnDisable()
    {
        investmentAction.Disable();
        UIMoveAction.Disable();
        investmentAction.performed -= ctx => PressInvestmentAction();
    }

    private void Update()
    {
        if (isInvestmentPanelOpen)
        {
            HandleInvestmentPanelInput();
            UpdateInvestmentPanel();
        }

        if (inputCooldown > 0f)
        {
            inputCooldown -= Time.deltaTime;
        }

        givenStockText.text = $"YOUR STOCK IS : {Finder.Player.stock}";
    }

    private void HandleInvestmentPanelInput()
    {
        if (inputCooldown > 0f) return;

        Vector2 moveInput = UIMoveAction.ReadValue<Vector2>();
        
        var stockAssets = Finder.LevelManager.stocksManager.playerStockAssets;

        if (moveInput.y > 0.5f)
        {
            selectedStockIndex--;
            if (selectedStockIndex < 0)
                selectedStockIndex = stockAssets.Length - 1;
            inputCooldown = INPUT_COOLDOWN_TIME;
        }
        else if (moveInput.y < -0.5f)
        {
            selectedStockIndex++;
            if (selectedStockIndex >= stockAssets.Length)
                selectedStockIndex = 0;
            inputCooldown = INPUT_COOLDOWN_TIME;
        }
        
        else if (moveInput.x < -0.5f)
        {
            string selectedTick = stockAssets[selectedStockIndex].stock.tick;
            Finder.LevelManager.stocksManager.TrySell(selectedTick);
            inputCooldown = INPUT_COOLDOWN_TIME;
        }
        else if (moveInput.x > 0.5f)
        {
            string selectedTick = stockAssets[selectedStockIndex].stock.tick;
            Finder.LevelManager.stocksManager.TryBuy(selectedTick);
            inputCooldown = INPUT_COOLDOWN_TIME;
        }
    }

    private void OpenInvestmentPanel()
    {
        Cursor.lockState = CursorLockMode.None;
        selectedStockIndex = 0;
        UpdateInvestmentPanel();
        isInvestmentPanelOpen = true;
        investmentPanel.SetActive(true);
    }

    private void CloseInvestmentPanel()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isInvestmentPanelOpen = false;
        investmentPanel.SetActive(false);
    }

    private void UpdateInvestmentPanel()
    {
        moneyText.text = $"YOU HAVE: ${Finder.LevelManager.stocksManager.playerMoney:F0}$!!!";
        UpdateInvestmentTexts();
    }

    private void PressInvestmentAction()
    {
        if (isInvestmentPanelOpen)
        {
            CloseInvestmentPanel();
        }
        else
        {
            OpenInvestmentPanel();
        }
    }

    private void UpdateInvestmentTexts()
    {
        stockText.text = "";
        var stockAssets = Finder.LevelManager.stocksManager.playerStockAssets;
        
        for (int i = 0; i < stockAssets.Length; i++)
        {
            var stockAsset = stockAssets[i];
            string line = $"{stockAsset.stock.currentValue:F0}$ (diff:{stockAsset.stock.currentValue - stockAsset.stock.lastPrice:F0}$) : ";
            line += $"{stockAsset.stock.tick} ({stockAsset.stock.stockName}) : {stockAsset.quantityOwned} owned";
            
            if (i == selectedStockIndex)
            {
                line = $"{line} <- (left to sell, right to buy)";
            }
            
            stockText.text += (i > 0 ? "\n" : "") + line;
        }
    }
    
    public void SwitchToNormalCursor()
    {
        cursorAnimator.Play("cursor");
    }

    public void SwitchToFistCursor()
    {
        cursorAnimator.Play("cursor_fist");
    }

    public void SwitchToFistTalkCursor()
    {
        cursorAnimator.Play("cursor_fist_talk");
    }

    public void SwitchToSmileCursor()
    {
        cursorAnimator.Play("cursor_smile");
    }

    public void PlayPunchAnimation()
    {
        fistAnimator.Play("ThrowPunch");
    }

    public void ShowAlert(string alertText)
    {
        this.alertText.text = alertText;
        alertAnimator.gameObject.SetActive(true);
        alertAnimator.Play("Alert", 0, 0f);
    }

    public void UpdateClock(int hours, int minutes)
    {
        clockText.text = string.Format("{0:00}:{1:00}", hours, minutes);
    }

    public void SetAlertVisuals()
    {
        clockText.color = Color.red;
        ShowAlert("A CRIME WAS REPORTED!!! Authorities will be here at the end of the hour!");
    }

    public void StartDialogue(string characterName, string text)
    {
        if (activeDialogue != null)
            StopCoroutine(activeDialogue);

        activeDialogue = StartCoroutine(StartDialogueRoutine(characterName, text));
    }

    private IEnumerator StartDialogueRoutine(string characterName, string dialogueText)
    {
        fadeImage.gameObject.SetActive(true);
        characterNameText.text = characterName;
        this.dialogueText.text = "";

        for (int i = 0; i <= dialogueText.Length; i++)
        {
            this.dialogueText.text = dialogueText[..i];
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(dialogueText.Length * 0.03f + 1.0f);

        characterNameText.text = "";
        this.dialogueText.text = "";
        fadeImage.gameObject.SetActive(false);

        activeDialogue = null;
    }
}