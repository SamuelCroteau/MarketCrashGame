using UnityEngine;

public class CharacterToiletState : ICharacterState
{
    private readonly Character character;
    private readonly Transform toiletZone;

    public CharacterToiletState(Character character, Transform toiletZone)
    {
        this.character = character;
        this.toiletZone = toiletZone;
    }

    public void Enter()
    {
        float yPos = character.transform.position.y;
        Vector3 newPosition = new Vector3(toiletZone.position.x, yPos, toiletZone.position.z);
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