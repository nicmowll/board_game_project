using System;
using UnityEngine;

public class GameRollDiceState : GameBaseState
{
    private int diceValue1;
    private int diceValue2;

    public override void EnterState(GameStateManager sm)
    {
        Debug.Log("ENTERING RollDice State");
        diceValue1 = 0;
        diceValue2 = 0;
        Dice.OnDiceResult += SetDiceValues; 
        sm.TriggerRollDice();
    }

    public override void UpdateState(GameStateManager sm)
    {
        if (diceValue1 != 0 && diceValue2 != 0)
        {
            sm.playerMoves.x = diceValue1;
            sm.playerMoves.y = diceValue2;

            Debug.Log("Dice Roll Completed with values: " + diceValue1 + ", " + diceValue2);
            sm.SwitchState(sm.Move);

        }
    }

    public override void ExitState(GameStateManager sm)
    {
        Dice.OnDiceResult -= SetDiceValues;
    }

    private void SetDiceValues(int _diceIndex, int _diceResult)
    {
        if (_diceIndex == 0)
        {
            diceValue1 = _diceResult;
        }
        else if (_diceIndex == 1)
        {
            diceValue2 = _diceResult;
        }
    }
}
