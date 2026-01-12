using UnityEngine;

public class CharacterBlackboard : MonoBehaviour
{
    [Header("Knowledge")]
    public string characterName = "Employee";
    [SerializeField] public Transform[] WorkZones = new Transform[0];
    [SerializeField] public Transform[] EatZones = new Transform[0];
    [SerializeField] public Transform[] ToiletZones = new Transform[0];
    [SerializeField] public Transform[] SocialZones = new Transform[0];
    [Header("Routine")]
    public Routine routine;

    [HideInInspector]
    private Character character;

    [Header("Dialogue")]
    [HideInInspector] public string currentDialogue;
    [SerializeField] public DialoguePools dialoguePools;

    [Header("Preferences")]
    public StockData[] preferredStocks = new StockData[0];
    public StockData[] dislikedStocks = new StockData[0];
    
    // Initialization.
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void SetNewDialogue()
    {
        currentDialogue = GetRandomDialogueFromState(character.currentState);
    }

    private string GetRandomDialogueFromState(ICharacterState state)
    {
        if (state is CharacterWorkState)
            return dialoguePools.workDialogues.dialogues[Random.Range(0, dialoguePools.workDialogues.dialogues.Length)];
        else if (state is CharacterEatState)
            return dialoguePools.eatDialogues.dialogues[Random.Range(0, dialoguePools.eatDialogues.dialogues.Length)];
        else if (state is CharacterToiletState)
            return dialoguePools.toiletDialogues.dialogues[Random.Range(0, dialoguePools.toiletDialogues.dialogues.Length)];
        else if (state is CharacterSocialState)
            return dialoguePools.socialDialogues.dialogues[Random.Range(0, dialoguePools.socialDialogues.dialogues.Length)];
        else if (state is CharacterCallState)
            return dialoguePools.callDialogues.dialogues[Random.Range(0, dialoguePools.callDialogues.dialogues.Length)];
        else
            return "...";
    }
}