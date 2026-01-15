using UnityEngine;

public class GameEndTurnState : GameBaseState
{
    public override void EnterState(GameStateManager sm)
    {
        Debug.Log("Ending Turn State");
    }

    public override void UpdateState(GameStateManager sm)
    {
        sm.SwitchState(sm.OppTurn);
    }

    public override void ExitState(GameStateManager sm)
    {
    }
}
