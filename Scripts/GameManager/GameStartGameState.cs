using System.Threading;
using UnityEngine;

public class GameStartGameState : GameBaseState
{
    public override void EnterState(GameStateManager sm)
    {
        Debug.Log("ENTERING Start Game State");

        // for (int player = 0; player <= 1; player++)
        // {
        //     int chipCount = sm.gameBoard.numPlayerStartingChips;
        //     for (int i = 0; i < chipCount; i++)
        //     {
        //         ChipData data = new ChipData();
        //         data.Initialize(player, "Default");

        //         sm.gameView.SpawnSourceChip(data);
        //         sm.gameBoard.AddSourceChip(player, data);
        //     }
        // }

        sm.SwitchState(sm.BeginTurn);
    }

    public override void UpdateState(GameStateManager sm)
    {
    }

    public override void ExitState(GameStateManager sm)
    {
    }
}
