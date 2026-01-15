using Mono.Cecil.Cil;
using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Interactions;

public class GameMoveState : GameBaseState
{
    Vector2Int pos;
    Vector2Int targPos;

    public override void EnterState(GameStateManager sm)
    {
        Debug.Log("PLAYER MOVE STATE ENTERED, awaiting user input");
        pos = Vector2Int.zero;
        sm.gameBoard.PrintBoard();

    }

    public override void UpdateState(GameStateManager sm)
    {
        if (sm.playerMoves.x == 0 && sm.playerMoves.y == 0) {sm.SwitchState(sm.EndTurn);}

        if (Input.GetKeyDown(KeyCode.Z))
        {
            pos.x = 0;
            pos.y = 4;

            //var (success, remainingMoveSet) = sm.gameBoard.EnterChip(pos,sm.playerMoves,0);

            // if (success) {sm.playerMoves = remainingMoveSet;}
            // else {Debug.Log("Move not allowed!");}

            // sm.gameBoard.PrintBoard();
            // sm.gameBoard.PrintSourceChips();
            // Debug.Log(sm.playerMoves);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            pos.x = 1;
            pos.y = 4;

            //var (success, remainingMoveSet) = sm.gameBoard.EnterChip(pos,sm.playerMoves,0);

            // if (success) {sm.playerMoves = remainingMoveSet;}
            // else {Debug.Log("Move not allowed!");}

            // sm.gameBoard.PrintBoard();
            // sm.gameBoard.PrintSourceChips();
            // Debug.Log(sm.playerMoves);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            pos.x = 2;
            pos.y = 4;

            //var (success, remainingMoveSet) = sm.gameBoard.EnterChip(pos,sm.playerMoves,0);

            // if (success) {sm.playerMoves = remainingMoveSet;}
            // else {Debug.Log("Move not allowed!");}

            // sm.gameBoard.PrintBoard();
            // sm.gameBoard.PrintSourceChips();
            // Debug.Log(sm.playerMoves);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            pos.x = 0;
            pos.y = 4;

            targPos.x = 0;
            targPos.y = 3;

            var (success, remainingMoveSet) = sm.gameBoard.MoveChipStack(pos, targPos, sm.playerMoves, 0);

            if (success) {sm.playerMoves = remainingMoveSet;}
            else {Debug.Log("Move not allowed!");}

            sm.gameBoard.PrintBoard();
            sm.gameBoard.PrintSourceChips();
            Debug.Log(sm.playerMoves);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            pos.x = 1;
            pos.y = 4;

            targPos.x = 1;
            targPos.y = 3;

            var (success, remainingMoveSet) = sm.gameBoard.MoveChipStack(pos, targPos, sm.playerMoves, 0);

            if (success) {sm.playerMoves = remainingMoveSet;}
            else {Debug.Log("Move not allowed!");}

            sm.gameBoard.PrintBoard();
            sm.gameBoard.PrintSourceChips();
            Debug.Log(sm.playerMoves);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            pos.x = 2;
            pos.y = 4;

            targPos.x = 2;
            targPos.y = 3;

            var (success, remainingMoveSet) = sm.gameBoard.MoveChipStack(pos, targPos, sm.playerMoves, 0);

            if (success) {sm.playerMoves = remainingMoveSet;}
            else {Debug.Log("Move not allowed!");}

            sm.gameBoard.PrintBoard();
            sm.gameBoard.PrintSourceChips();
            Debug.Log(sm.playerMoves);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            pos.x = 1;
            pos.y = 4;

            targPos.x = 2;
            targPos.y = 3;

            var (success, remainingMoveSet) = sm.gameBoard.MoveChipStack(pos, targPos, sm.playerMoves, 0);

            if (success) {sm.playerMoves = remainingMoveSet;}
            else {Debug.Log("Move not allowed!");}

            sm.gameBoard.PrintBoard();
            sm.gameBoard.PrintSourceChips();
            Debug.Log(sm.playerMoves);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            sm.SwitchState(sm.EndTurn);
        }
    }

    public override void ExitState(GameStateManager sm)
    {
        sm.playerMoves.x = 0;
        sm.playerMoves.y = 0;
    }
}
