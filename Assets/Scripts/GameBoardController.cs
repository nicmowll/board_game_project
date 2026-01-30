using System;
using System.Collections.Generic;
using Mono.Cecil;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Scripting;
using UnityEngine.Events;

public class GameBoardController : MonoBehaviour
{

    [SerializeField]
    private int boardWidth = 3;
    [SerializeField]
    private int boardHeight = 5;
    public int numPlayerStartingChips = 12;
    public int maxChipStack = 6;

    // This is to be indexed left to right, then top to bottom
    private List<ChipData>[,] board;

    private List<ChipData>[] chipSources;
    private List<ChipData>[] chipBanks;
    private UnityEvent<int> OnPlayerBankComplete = new UnityEvent<int>();

    void Awake()
    {
        board = new List<ChipData>[boardWidth, boardHeight];
        for (int x = 0; x < boardWidth; x++)
            for (int y = 0; y < boardHeight; y++)
                board[x, y] = new List<ChipData>();

        chipSources = new List<ChipData>[2];
        for (int i = 0; i < 2; i++)
        {
            chipSources[i] = new List<ChipData>();
        }

        for (int p = 0; p <= 1; p++)
        {
            for (int i = 0; i < numPlayerStartingChips; i++)
            {
                ChipData chipData = new ChipData();
                chipData.Initialize(p, "Default");
                chipSources[p].Add(chipData);
            }
        }

        chipBanks = new List<ChipData>[2];
        for (int i = 0; i < 2; i++)
        {
            chipBanks[i] = new List<ChipData>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public int GetBoardWidth()
    {
        return boardWidth;
    }

    public int GetBoardHeight()
    {
        return boardHeight;
    }

    public List<ChipData> GetBoardChipStack(Vector2Int pos)
    {
        return board[pos.x,pos.y];
    }

    public List<ChipData>[,] GetGameBoard()
    {
        return board;
    }

    public List<ChipData>[] GetChipSources()
    {
        return chipSources;
    }

    public List<ChipData>[] GetChipBanks()
    {
        return chipBanks;
    }

    public void PrintBoard()
    {
        string boardString = "";
        for (int y = 0; y < boardHeight; y++) {
            boardString += "\n";
            for (int x = 0; x < boardWidth; x++)
                boardString += board[x, y].Count + " ";
        }
        print(boardString);
    }
    public void PrintSourceChips()
    {
        print("Player 0 Chips: " + chipSources[0].Count + ". Player 1 Chips: " + chipSources[1].Count);
    }

    public void ClearGameBoard()
    {
        for (int x = 0; x < boardWidth; x++)
            for (int y = 0; y < boardHeight; y++)
                board[x, y].Clear();
    }

    public ChipData GetTopChip(Vector2Int pos)
    {
        var stack = board[pos.x, pos.y];
        if (stack.Count == 0) {return null;}
        var top = stack[stack.Count - 1];
        return top;
    }

    public int GetStackCount(Vector2Int pos)
    {
        var stack = board[pos.x, pos.y];
        return stack.Count;
    }

    public List<ChipData> PopChipStack(Vector2Int pos)
    {
        if (board[pos.x,pos.y].Count == 0) {return null;}
        List<ChipData> stack = new List<ChipData>();
        for (int i = 0; i < board[pos.x,pos.y].Count; i++)
        {
            var chip = board[pos.x,pos.y][i];
            stack.Add(chip);
        }
        board[pos.x,pos.y].Clear();
        return stack;
    }

    public void AddChipStack(Vector2Int pos, List<ChipData> stack)
    {
        for (int c = 0; c < stack.Count; c++)
        {
            var chip = stack[c];
            board[pos.x,pos.y].Add(chip);
        }
    }

    public ChipData PopSourceChip(int playerIndex)
    {
        if (chipSources[playerIndex].Count == 0) {return null;}
        var top = chipSources[playerIndex][chipSources[playerIndex].Count - 1];
        chipSources[playerIndex].RemoveAt(chipSources[playerIndex].Count - 1);
        return top;
    }

    public void AddSourceChip(int playerIndex, ChipData chip)
    {
        chipSources[playerIndex].Add(chip);
    }

    public Vector2Int GetNewMoveSet(Vector2Int moveSet, bool matchesFirst, bool matchesSecond)
    {
        Vector2Int newMoveSet = moveSet;

        if (moveSet.x == moveSet.y) {
            newMoveSet.x = 0;
            return newMoveSet;
        }

        if (matchesFirst && matchesSecond)
        {
            if (moveSet.x < moveSet.y) {newMoveSet.x = 0;}
            else {newMoveSet.y = 0;}
        }
        else if (matchesFirst) {newMoveSet.x = 0;}
        else {newMoveSet.y = 0;}

        return newMoveSet;
    }

    public int GetPlayerOrientation(int playerIndex)
    {
        if (playerIndex == 0) return -1;
        else return 1;
    }

    // Inputs:
    //  pos : requested grid square to place new chip
    //  moveSet : the two moves the player has for their turn
    //  chip : object reference to the chip being entered
    //
    // Returns:
    //  on success -> returns same moveSet but with the used die value set to 0
    //  on failure -> returns Vector2Int.zero
    public (bool success, Vector2Int newMoveSet) EnterChip(Vector2Int pos, Vector2Int moveSet, int playerIndex)
    {
        // Validation Checks
        if (chipSources[playerIndex].Count == 0) return (false, Vector2Int.zero);

        if (board[pos.x,pos.y].Count > 0) return (false, Vector2Int.zero);

        if (playerIndex == 0 && !((pos.x == 0 && (pos.y == boardHeight - 1)) || (pos.x == 1 && (pos.y == boardHeight - 1)) || (pos.x == 2 && (pos.y == boardHeight - 1)))) return (false, Vector2Int.zero);
        if (playerIndex == 1 && !((pos.x == 0 && pos.y == 0) || (pos.x == 1 && pos.y == 0) || (pos.x == 2 && pos.y == 0))) return (false, Vector2Int.zero);

        Vector2Int requiredMoves = GetRequiredEntryValues(pos);
        bool matchesFirst = moveSet.x == requiredMoves.x || moveSet.x == requiredMoves.y;
        bool matchesSecond = moveSet.y == requiredMoves.x || moveSet.y == requiredMoves.y;
        if (!matchesFirst && !matchesSecond) return (false, Vector2Int.zero);

        ChipData chip = PopSourceChip(playerIndex);
        board[pos.x,pos.y].Add(chip);
        
        Vector2Int newMoveSet = GetNewMoveSet(moveSet, matchesFirst, matchesSecond);

        return (true, newMoveSet);
    }

    private Vector2Int GetRequiredEntryValues(Vector2Int pos)
    {
        if ((pos.x == 0 && pos.y == 0) || (pos.x == 2 && (pos.y == boardHeight - 1))) {return new Vector2Int(3,4);}
        else if ((pos.x == 1 && pos.y == 0) || (pos.x == 1 && (pos.y == boardHeight - 1))) {return new Vector2Int(2,5);}
        else if ((pos.x == 2 && pos.y == 0) || (pos.x == 0 && (pos.y == boardHeight - 1))) {return new Vector2Int(1,6);}
        else return Vector2Int.zero;
    }

    public (bool success, Vector2Int newMoveSet) MoveChipStack(Vector2Int srcPos, Vector2Int targPos, Vector2Int moveSet, int playerIndex)
    {
        //make sure the targ pos is valid option in context for src pos and player index (player is moving forward 1 tile and straight or diagonally one tile)
        if (srcPos.y + GetPlayerOrientation(playerIndex) != targPos.y) return (false, Vector2Int.zero);
        if ( !(srcPos.x == targPos.x || srcPos.x == (targPos.x + 1) || srcPos.x == (targPos.x - 1)) ) return (false, Vector2Int.zero);
        
        //make sure the source stack count is more than 0
        if (board[srcPos.x,srcPos.y].Count == 0) return (false, Vector2Int.zero);

        //make sure the top chip of the src stack belongs to the current player
        if (GetTopChip(srcPos).playerIndex != playerIndex) return (false, Vector2Int.zero);

        //make sure that the sum of the counts of the two stacks is <= 6
        int minMove = board[srcPos.x,srcPos.y].Count + board[targPos.x,targPos.y].Count;
        if (minMove > 6) return (false, Vector2Int.zero);
        
        //make sure that the sum of the counts of the two stacks is a possible move for the player
        bool matchesFirst = moveSet.x >= minMove;
        bool matchesSecond = moveSet.y >= minMove;
        if (!matchesFirst && !matchesSecond) return (false, Vector2Int.zero);

        //make the move
        List<ChipData> stack = PopChipStack(srcPos);
        AddChipStack(targPos, stack);

        //remove the lowest possible move value from the players moveSet and return the new moveSet
        Vector2Int newMoveSet = GetNewMoveSet(moveSet, matchesFirst, matchesSecond);

        return (true, newMoveSet);
    }

    public (bool success, Vector2Int newMoveSet) BankChipStack(Vector2Int pos, Vector2Int moveSet, int playerIndex)
    {
        //make sure pos is a valid option in context for board and player index
        if (!(playerIndex == 0 && pos.y == 0)) return (false, Vector2Int.zero);
        if (!(playerIndex == 1 && pos.y == boardHeight -1)) return (false, Vector2Int.zero);

        //make sure stack count is more than 0
        if (board[pos.x,pos.y].Count == 0) return (false, Vector2Int.zero);

        //make sure the top chip of the stack belongs to the current player
        if (GetTopChip(pos).playerIndex != playerIndex) return (false, Vector2Int.zero);

        //make sure the stack count is a possible move for the player
        int minMove = board[pos.x,pos.y].Count;
        
        bool matchesFirst = moveSet.x >= minMove;
        bool matchesSecond = moveSet.y >= minMove;
        if (!matchesFirst && !matchesSecond) return (false, Vector2Int.zero);

        //bank the chips - add each of them to their player's banks and then remove them from the board.
        List<ChipData> stack = PopChipStack(pos);
        for (int c = 0; c < stack.Count; c++)
        {
            var chip = stack[c];
            chipBanks[chip.playerIndex].Add(chip);
        }

        //remove the lowest possible move value from the players moveSet and return the new moveset
        Vector2Int newMoveSet = GetNewMoveSet(moveSet, matchesFirst, matchesSecond);

        // Since we are banking chips we want to check if the player wins, so if true we invoke the event saying the player has banked all chips
        if(chipBanks[playerIndex].Count == numPlayerStartingChips) OnPlayerBankComplete?.Invoke(playerIndex);

        return (true, newMoveSet);
    }


}
