using UnityEngine;

public class CharacterEatState : ICharacterState
{
    private readonly Character character;
    private readonly Transform eatZone;
    
    public CharacterEatState(Character character, Transform eatZone)
    {
        this.character = character;
        this.eatZone = eatZone;
    }

    public void Enter()
    {
        float yPos = character.transform.position.y;
        Vector3 newPosition = new Vector3(eatZone.position.x, yPos, eatZone.position.z);
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