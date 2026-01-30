using UnityEngine;

public class GameBeginTurnState : GameBaseState
{
    public override void EnterState(GameStateManager sm)
    {
        Debug.Log("ENTERING Begin Turn State");
        Debug.Log("Awaiting user input to roll dice : SPACE");
    }

    public override void UpdateState(GameStateManager sm)
    {   
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Input Recieved!");

            sm.SwitchState(sm.RollDice);
        }
    }

    public override void ExitState(GameStateManager sm)
    {
    }
}
