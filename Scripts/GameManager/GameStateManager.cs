using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    GameBaseState currentState;
    public DiceRoller diceRoller;
    public GameBoardController gameBoard;
    public GameBoardView gameView;

    public GameBeginTurnState BeginTurn = new GameBeginTurnState();
    public GameRollDiceState RollDice = new GameRollDiceState();
    public GameMoveState Move = new GameMoveState();
    public GameEndTurnState EndTurn = new GameEndTurnState();
    public GameOpponentTurnState OppTurn = new GameOpponentTurnState();
    public GameStartGameState StartGame = new GameStartGameState();
    public GameEndGameState EndGame = new GameEndGameState();

    // the move values from rolling the dice for the player
    public Vector2Int playerMoves = Vector2Int.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initial State
        currentState = StartGame;
        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(GameBaseState _state)
    {
        if (currentState != null)
        {
            currentState.ExitState(this);
        }

        currentState = _state;
        currentState.EnterState(this);
    }

    public void TriggerRollDice()
    {
        diceRoller.TriggerRollDice();
    }
}
