using UnityEngine;

public class CharacterDeadState : ICharacterState
{
    private readonly Character character;

    public CharacterDeadState(Character character)
    {
        this.character = character;
    }

    public void Enter()
    {
        character.PlayDeadAnimation();
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}