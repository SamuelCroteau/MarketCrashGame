using UnityEngine;

public class CharacterSocialState : ICharacterState
{
    private readonly Character character;
    private readonly Transform socialZone;

    public CharacterSocialState(Character character, Transform socialZone)
    {
        this.character = character;
        this.socialZone = socialZone;
    }

    public void Enter()
    {
        float yPos = character.transform.position.y;
        Vector3 newPosition = new Vector3(socialZone.position.x, yPos, socialZone.position.z);
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