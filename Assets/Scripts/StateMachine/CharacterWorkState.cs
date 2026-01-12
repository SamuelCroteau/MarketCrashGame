using UnityEngine;

public class CharacterWorkState : ICharacterState
{
    private readonly Character character;
    private readonly Transform workZone;

    public CharacterWorkState(Character character, Transform workZone)
    {
        this.character = character;
        this.workZone = workZone;
    }

    public void Enter()
    {
        float yPos = character.transform.position.y;
        Vector3 newPosition = new Vector3(workZone.position.x, yPos, workZone.position.z);
        if (!character.IsCloseTo(newPosition))
        {
            character.currentState.Exit();
            character.currentState = new CharacterMoveState(character, newPosition, this);
            character.currentState.Enter();
            return;
        }
        character.PlayIdleAnimation();
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}