using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Character : MonoBehaviour, IBreakable, IStockInfluence, IWitness, IInteractible
{
    const int NUMBER_OF_CRIMES_TO_WITNESS_BEFORE_CALL = 3;
    const int MAX_NUMBER_OF_DISLIKED_STOCKS = 2;
    const int MAX_NUMBER_OF_PREFERRED_STOCKS = 2;
    const int MAX_DIALOGUES_PER_STOCK = 2;

    [Header("Stock Interaction")]
    [SerializeField] private StockInteraction stockInteraction = new StockInteraction();
    private CharacterBlackboard blackboard;

    [Header("Visuals")]
    [SerializeField] private Texture normalTexture;
    [SerializeField] private Texture brokenTexture;

    [Header("Animations")]
    [SerializeField] private Animator animator;

    [HideInInspector] public ICharacterState currentState;

    [Header("Stats")]
    [SerializeField] private float speed = 1.0f;

    private Renderer _renderer;
    private NavMeshAgent navMeshAgent;

    private Crime[] witnessedCrimes = new Crime[NUMBER_OF_CRIMES_TO_WITNESS_BEFORE_CALL];

    private bool _isBroken = false;
    public bool IsBroken
    { 
        get { return _isBroken; } 
        set { _isBroken = value; }
    }

    private bool isCalling = false;

    private void Awake()
    {
        blackboard = GetComponent<CharacterBlackboard>();
        _renderer = GetComponentInChildren<Renderer>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        navMeshAgent.enabled = true;

        if (_renderer != null && normalTexture != null)
            _renderer.material.SetTexture("_BaseMap", normalTexture);
        stockInteraction.currentModifiers = stockInteraction.aliveModifiers;

        blackboard.routine = GetRandomRoutine();
    }

    private void Update()
    {
        if (currentState != null)
            currentState.Update();
    }

    public void Break()
    {
        if (!IsBroken)
        {
            IsBroken = true;

            if (currentState != null)
                currentState.Exit();
            currentState = new CharacterDeadState(this);
            blackboard.SetNewDialogue();
            currentState.Enter();

            stockInteraction.currentModifiers = stockInteraction.deadModifiers;

            _renderer.material.SetTexture("_BaseMap", brokenTexture);

            GetComponent<Crime>().triggerCrime();
            navMeshAgent.enabled = false;

            Finder.LevelManager.inGameUI.ShowAlert(GetCharacterBreakMessage());
        }
    }

    private string GetCharacterBreakMessage()
    {
        string message = "You killed someone! that's a crime! ";
        if (blackboard.preferredStocks.Length > 0)
        {
            message += "they liked";
            for (int i = 0; i < blackboard.preferredStocks.Length; i++)
            {
                message += $" {blackboard.preferredStocks[i].tick}";
            }
            if (blackboard.dislikedStocks.Length > 0)
            {
                message += ", but hated";
                for (int i = 0; i < blackboard.dislikedStocks.Length; i++)
                {
                    message += $" {blackboard.dislikedStocks[i].tick}";
                }
            }
        }
        else if (blackboard.dislikedStocks.Length > 0)
        {
            message += "they hated ";
            for (int i = 0; i < blackboard.dislikedStocks.Length; i++)
            {
                message += $" {blackboard.dislikedStocks[i].tick}";
            }
        }
        return message;
    }

    public void Interact(PlayerCharacter player)
    {
        Finder.LevelManager.StartDialogue(blackboard.characterName, blackboard.currentDialogue);
    }

    public void AdvanceHour()
    {
        if (currentState != null)
            currentState.Exit();
        if (IsBroken || isCalling)
            return;
        currentState = blackboard.routine.states[Finder.Clock.currentHour-1];
        blackboard.SetNewDialogue();
        currentState.Enter();
    }

    public StockInteraction GetStockInteraction()
    {
        return stockInteraction;
    }

    public void PlayIdleAnimation()
    {
        animator.Play("actor1_idle");
    }

    public void PlayWalkAnimation()
    {
        animator.Play("actor1_walk");
    }

    public void PlayDeadAnimation()
    {
        animator.Play("actor1_dead");
    }

    public bool IsCloseTo(Vector3 position, float threshold = 1.0f)
    {
        float distance = Vector3.Distance(this.transform.position, position);
        return distance <= threshold;
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
        Debug.Log($"{this.name} is calling the authorities!");
        isCalling = true;
        if (currentState != null)
            currentState.Exit();
        currentState = new CharacterCallState(this);
        blackboard.SetNewDialogue();
        Debug.Log(blackboard.currentDialogue);
        currentState.Enter();
        Finder.LevelManager.ReportCrime();
    }

    public Routine GetRandomRoutine()
    {
        int hoursInCycle = Finder.Clock.maxHoursInCycle;
        Routine routine = new Routine();
        routine.states = new ICharacterState[hoursInCycle];
        for (int i = 0; i < hoursInCycle; i++)
            routine.states[i] = GetRandomState();
        return routine;
    }
    
    private ICharacterState GetRandomState()
    {
        int[] weightedChoices = { 0, 1, 2, 3, 3 };

        int randomIndex = Random.Range(0, weightedChoices.Length);
        int random = weightedChoices[randomIndex];

        int[] fallbackChoices = { 0, 1, 2, 3 };
        for (int i = fallbackChoices.Length - 1; i > 0; i--)
        {
            int swap = Random.Range(0, i + 1);
            int temp = fallbackChoices[i];
            fallbackChoices[i] = fallbackChoices[swap];
            fallbackChoices[swap] = temp;
        }

        switch (random)
        {
            case 0:
                if (blackboard.EatZones.Length > 0)
                    return new CharacterEatState(this, blackboard.EatZones[Random.Range(0, blackboard.EatZones.Length)]);
                break;
            case 1:
                if (blackboard.SocialZones.Length > 0)
                    return new CharacterSocialState(this, blackboard.SocialZones[Random.Range(0, blackboard.SocialZones.Length)]);
                break;
            case 2:
                if (blackboard.ToiletZones.Length > 0)
                    return new CharacterToiletState(this, blackboard.ToiletZones[Random.Range(0, blackboard.ToiletZones.Length)]);
                break;
            case 3:
                if (blackboard.WorkZones.Length > 0)
                    return new CharacterWorkState(this, blackboard.WorkZones[Random.Range(0, blackboard.WorkZones.Length)]);
                break;
        }

        foreach (int choice in fallbackChoices)
        {
            switch (choice)
            {
                case 0:
                    if (blackboard.EatZones.Length > 0)
                        return new CharacterEatState(this, blackboard.EatZones[Random.Range(0, blackboard.EatZones.Length)]);
                    break;
                case 1:
                    if (blackboard.SocialZones.Length > 0)
                        return new CharacterSocialState(this, blackboard.SocialZones[Random.Range(0, blackboard.SocialZones.Length)]);
                    break;
                case 2:
                    if (blackboard.ToiletZones.Length > 0)
                        return new CharacterToiletState(this, blackboard.ToiletZones[Random.Range(0, blackboard.ToiletZones.Length)]);
                    break;
                case 3:
                    if (blackboard.WorkZones.Length > 0)
                        return new CharacterWorkState(this, blackboard.WorkZones[Random.Range(0, blackboard.WorkZones.Length)]);
                    break;
            }
        }
        
        if (blackboard.WorkZones.Length > 0)
            return new CharacterWorkState(this, blackboard.WorkZones[0]);
        return new CharacterWorkState(this, this.transform);
    }

    public void SetDestination(Vector3 destination)
    {
        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(destination);
        }
    }

    // J'ai utilisé un peu l'IA pour rendre cet algorithme plus efficace et éviter les doublons.
    public void InitializeStockPreference(StockData[] stocks)
    {
        StockData[] availableStocks = stocks
            .Where(s => s.canBePreferredOrDisliked
                    && !(blackboard.preferredStocks?.Any(ps => ps.tick == s.tick) ?? false)
                    && !(blackboard.dislikedStocks?.Any(ds => ds.tick == s.tick) ?? false))
            .ToArray();

        int numberOfPreferences = Random.Range(1, MAX_NUMBER_OF_PREFERRED_STOCKS + 1);
        if (numberOfPreferences > availableStocks.Length)
            numberOfPreferences = availableStocks.Length;
        for (int i = 0; i < numberOfPreferences; i++)
        {
            StockData stock = availableStocks[Random.Range(0, availableStocks.Length)];
            if (System.Array.Exists(blackboard.preferredStocks, s => s.tick == stock.tick))
                continue;
            System.Array.Resize(ref blackboard.preferredStocks, blackboard.preferredStocks.Length + 1);
            blackboard.preferredStocks[blackboard.preferredStocks.Length - 1] = stock;
            availableStocks = System.Array.FindAll(availableStocks, s => s.tick != stock.tick);
        }

        if (availableStocks.Length != 0)
        {
            int numberOfDislikes = Random.Range(0, MAX_NUMBER_OF_DISLIKED_STOCKS + 1);
            if (numberOfDislikes > availableStocks.Length)
                numberOfDislikes = availableStocks.Length;
            for (int i = 0; i < numberOfDislikes; i++)
            {
                StockData stock = availableStocks[Random.Range(0, availableStocks.Length)];
                if (System.Array.Exists(blackboard.dislikedStocks, s => s.tick == stock.tick))
                    continue;
                System.Array.Resize(ref blackboard.dislikedStocks, blackboard.dislikedStocks.Length + 1);
                blackboard.dislikedStocks[blackboard.dislikedStocks.Length - 1] = stock;
                availableStocks = System.Array.FindAll(availableStocks, s => s.tick != stock.tick);
            }
        }

        // Add 1 to 2 dialogues for each preferred stock, only for states present in routine
        foreach (var stock in blackboard.preferredStocks)
        {
            var possibleTypes = new[]
            {
                new { Type = typeof(CharacterWorkState), Pool = stock.LoveDialoguePools.workDialogues, Name = "work" },
                new { Type = typeof(CharacterEatState), Pool = stock.LoveDialoguePools.eatDialogues, Name = "eat" },
                new { Type = typeof(CharacterToiletState), Pool = stock.LoveDialoguePools.toiletDialogues, Name = "toilet" },
                new { Type = typeof(CharacterSocialState), Pool = stock.LoveDialoguePools.socialDialogues, Name = "social" }
            };

            int dialoguesToAdd = Random.Range(1, MAX_DIALOGUES_PER_STOCK + 1);
            var validTypes = possibleTypes.Where(t =>
                blackboard.routine.states.Any(s => s.GetType() == t.Type)).ToList();

            for (int i = 0; i < dialoguesToAdd && validTypes.Count > 0; i++)
            {
                var chosenType = validTypes[Random.Range(0, validTypes.Count)];
                var pool = chosenType.Pool.dialogues;
                if (pool.Length > 0)
                    AddDialogue(pool[Random.Range(0, pool.Length)], chosenType.Name);
            }
        }

        // Add 1 to 2 dialogues for each disliked stock, only for states present in routine
        foreach (var stock in blackboard.dislikedStocks)
        {
            var possibleTypes = new[]
            {
                new { Type = typeof(CharacterWorkState), Pool = stock.HateDialoguePools.workDialogues, Name = "work" },
                new { Type = typeof(CharacterEatState), Pool = stock.HateDialoguePools.eatDialogues, Name = "eat" },
                new { Type = typeof(CharacterToiletState), Pool = stock.HateDialoguePools.toiletDialogues, Name = "toilet" },
                new { Type = typeof(CharacterSocialState), Pool = stock.HateDialoguePools.socialDialogues, Name = "social" }
            };

            int dialoguesToAdd = Random.Range(1, MAX_DIALOGUES_PER_STOCK + 1);
            var validTypes = possibleTypes.Where(t =>
                blackboard.routine.states.Any(s => s.GetType() == t.Type)).ToList();

            for (int i = 0; i < dialoguesToAdd && validTypes.Count > 0; i++)
            {
                var chosenType = validTypes[Random.Range(0, validTypes.Count)];
                var pool = chosenType.Pool.dialogues;
                if (pool.Length > 0)
                    AddDialogue(pool[Random.Range(0, pool.Length)], chosenType.Name);
            }
        }

        foreach (var stock in blackboard.preferredStocks)
        {
            bool hasModifier = false;
            foreach (StockModifier modifier in stockInteraction.aliveModifiers)
                if (modifier.tick == stock.tick)
                    hasModifier = true;
            if (!hasModifier)
                AddModifier(stock);
        }
        foreach (var stock in blackboard.dislikedStocks)
        {
            bool hasModifier = false;
            foreach (StockModifier modifier in stockInteraction.deadModifiers)
                if (modifier.tick == stock.tick)
                    hasModifier = true;
            if (!hasModifier)
                AddModifier(stock, false);
        }
    }

    private void AddDialogue(string dialogue, string dialogueType = "")
    {
        switch (dialogueType)
        {
            case "work":
                int length = blackboard.dialoguePools.workDialogues.dialogues.Length;
                System.Array.Resize(ref blackboard.dialoguePools.workDialogues.dialogues, length + 1);
                blackboard.dialoguePools.workDialogues.dialogues[length] = dialogue;
                break;
            case "eat":
                length = blackboard.dialoguePools.eatDialogues.dialogues.Length;
                System.Array.Resize(ref blackboard.dialoguePools.eatDialogues.dialogues, length + 1);
                blackboard.dialoguePools.eatDialogues.dialogues[length] = dialogue;
                break;
            case "toilet":
                length = blackboard.dialoguePools.toiletDialogues.dialogues.Length;
                System.Array.Resize(ref blackboard.dialoguePools.toiletDialogues.dialogues, length + 1);
                blackboard.dialoguePools.toiletDialogues.dialogues[length] = dialogue;
                break;
            case "social":
                length = blackboard.dialoguePools.socialDialogues.dialogues.Length;
                System.Array.Resize(ref blackboard.dialoguePools.socialDialogues.dialogues, length + 1);
                blackboard.dialoguePools.socialDialogues.dialogues[length] = dialogue;
                break;
        }
        
    }

    private void AddModifier(StockData stock, bool isPreferred = true)
    {
        int length = stockInteraction.aliveModifiers.Length;
        System.Array.Resize(ref stockInteraction.aliveModifiers, length + 1);
        stockInteraction.aliveModifiers[length] = new StockModifier()
        {
            tick = stock.tick,
            value = isPreferred ? ModValue.STRONGLY_RISE : ModValue.STRONGLY_LOWER
        };

        length = stockInteraction.deadModifiers.Length;
        System.Array.Resize(ref stockInteraction.deadModifiers, length + 1);
        stockInteraction.deadModifiers[length] = new StockModifier()
        {
            tick = stock.tick,
            value = isPreferred ? ModValue.STRONGLY_LOWER : ModValue.STRONGLY_RISE
        };
    }
}