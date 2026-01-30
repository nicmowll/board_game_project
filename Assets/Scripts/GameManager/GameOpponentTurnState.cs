using UnityEngine;

public class GameOpponentTurnState : GameBaseState
{
    public override void EnterState(GameStateManager sm)
    {
        Debug.Log("Opponent Turn State - opponent is making decisions...");
    }

    public override void UpdateState(GameStateManager sm)
    {
        sm.SwitchState(sm.BeginTurn);
    }

    public override void ExitState(GameStateManager sm)
    {
    }
}
