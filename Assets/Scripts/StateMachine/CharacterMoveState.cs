using UnityEngine;

public class CharacterMoveState : ICharacterState
{
    private readonly Character character;
    private readonly ICharacterState nextState;
    private readonly Vector3 destination;

    public CharacterMoveState(Character character, Vector3 destination, ICharacterState nextState)
    {
        this.character = character;
        this.destination = destination;
        this.nextState = nextState;
    }

    public void Enter()
    {
        character.currentState = this;
        character.PlayWalkAnimation();
        character.SetDestination(destination);
    }

    public void Update()
    {
        if (character.IsCloseTo(destination, 0.5f))
        {
            Exit();
            character.currentState = nextState;
            character.currentState.Enter();
        }
    }

    public void Exit()
    {
        character.SetDestination(character.transform.position);
        character.PlayIdleAnimation();
    }
}