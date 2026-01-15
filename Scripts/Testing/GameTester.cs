using System.Collections.Generic;
using UnityEngine;
using System.Collections;


public class GameTester : MonoBehaviour
{

    GameBoardController gbc;
    GameBoardView v;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gbc = GetComponent<GameBoardController>();
        v = GetComponent<GameBoardView>();

        //TestEnterChip();
        //TestShowPlaceChipOnStack();
        //TestShowPlaceStack();
        StartCoroutine(RunTests());
        //TestResetStack();

        //ShowPlaceSourceChips();

        // v.WaitForAllChipMoves(() => {
        //     ShowPlaceBankChips();
        // });
    }

    private IEnumerator RunTests()
    {
        List<ChipData>[] chipSources = gbc.GetChipSources();
        int playerIndex = 0;
        List<ChipData>[,] board = gbc.GetGameBoard();
        Vector2Int pos1 = new Vector2Int(1,3);
        Vector2Int pos2 = new Vector2Int(1,4);

        playerIndex = 0;
        // Enter chip for player 0
        ChipData cd = gbc.PopSourceChip(playerIndex);
        board[pos1.x,pos1.y].Add(cd);
        v.ShowPlaceChip(cd, pos1);

        ChipData cd1 = gbc.PopSourceChip(playerIndex);
        board[pos1.x,pos1.y].Add(cd1);
        v.ShowPlaceChip(cd1, pos1);

        playerIndex = 1;
        ChipData cd2 = gbc.PopSourceChip(playerIndex);
        board[pos1.x,pos1.y].Add(cd2);
        v.ShowPlaceChip(cd2, pos1);

        playerIndex = 0;
        ChipData cd3 = gbc.PopSourceChip(playerIndex);
        board[pos1.x,pos1.y].Add(cd3);
        v.ShowPlaceChip(cd3, pos1);

        yield return new WaitForSeconds(2);

        v.StartHoverStackShell(pos1);

        yield return new WaitForSeconds(6);

        v.StopHover();
        v.ShowResetStack(pos1);

        yield return new WaitForSeconds(2);

        v.StartHoverStackShell(pos1);

        yield return new WaitForSeconds(5);

        v.StopHover();
        v.ShowPlaceStack(pos1, new Vector2Int(1,0));

        //  for (int i = 0; i < 4; i ++)
        //  {
            Vector2Int posNew = new Vector2Int(2,0);
            playerIndex = 1;
            ChipData cd4 = gbc.PopSourceChip(playerIndex);
            board[posNew.x,posNew.y].Add(cd4);

            yield return new WaitForSeconds(3);

            v.StartHoverChip(cd4);

            yield return new WaitForSeconds(4);

            v.StopHover();
            v.ShowResetChipToSource(cd4);

            yield return new WaitForSeconds(3);

            v.StartHoverChip(cd4);

            yield return new WaitForSeconds(4);

            v.StopHover();
            v.ShowPlaceChip(cd4, posNew);

            yield return new WaitForSeconds(4);

            v.StartHoverChip(cd4);
            
            yield return new WaitForSeconds(3);

            v.StopHover();
            v.ShowPlaceBankChip(cd4);
        //  }
        // Vector2Int posNew = new Vector2Int(2,0);
        // playerIndex = 1;
        // ChipData cd4 = gbc.PopSourceChip(playerIndex);
        // board[posNew.x,posNew.y].Add(cd4);

        // yield return new WaitForSeconds(3);

        // v.StartHoverChip(cd4);

        // yield return new WaitForSeconds(4);

        // v.StopHover();
        // v.ShowPlaceSourceChip(cd4);

        // yield return new WaitForSeconds(3);

        // v.StartHoverChip(cd4);

        // yield return new WaitForSeconds(4);

        // v.StopHover();
        // v.ShowPlaceChip(cd4, posNew);

        // yield return new WaitForSeconds(4);

        // v.StartHoverChip(cd4);
        
        // yield return new WaitForSeconds(3);

        // v.StopHover();
        // v.ShowPlaceBankChip(cd4);

        // ==================

        // TestEnterChip();
        // yield return new WaitForSeconds(1);

        // List<ChipData>[] chipSources = gbc.GetChipSources();
        // int playerIndex = 0;
        // List<ChipData>[,] board = gbc.GetGameBoard();
        // Vector2Int pos1 = new Vector2Int(1,3);
        // Vector2Int pos2 = new Vector2Int(1,4);

        // playerIndex = 0;
        // // Enter chip for player 0
        // ChipData cd = gbc.PopSourceChip(playerIndex);
        // board[pos1.x,pos1.y].Add(cd);
        // v.ShowPlaceChip(cd, pos1);

        // ChipData cd1 = gbc.PopSourceChip(playerIndex);
        // board[pos1.x,pos1.y].Add(cd1);
        // v.ShowPlaceChip(cd1, pos1);

        // yield return new WaitForSeconds(1);

        // v.StartHoverChip(cd1);
        // yield return new WaitForSeconds(2);

        // v.StopHover();
        // v.ShowPlaceChip(cd1, pos2);

        // yield return new WaitForSeconds(2);

        // board[pos2.x,pos2.y].Add(cd1);
        // board[pos1.x,pos1.y].Remove(cd1);
        // v.StartHoverStackShell(pos2);

        // yield return new WaitForSeconds(10);

        // v.StopHover();
        // v.ShowPlaceStack(pos2, pos1);
    }

    void Update()
    {
        
    }

    public void TestEnterChip()
    {
        int sourceStackLength = 0;
        List<ChipData>[] chipSources = gbc.GetChipSources();
        int playerIndex = 0;

        playerIndex = 0;
        Vector2Int moveSet = new Vector2Int(2, 1);
        Vector2Int pos1 = new Vector2Int(0,4);
        Vector2Int pos2 = new Vector2Int(1,4);
        
        // Enter chip for player 0
        sourceStackLength = chipSources[playerIndex].Count;
        ChipData cd1 = chipSources[playerIndex][sourceStackLength - 1];
        (bool succes, Vector2Int newMoves) = gbc.EnterChip(pos1, moveSet, playerIndex);
        if (succes) v.ShowPlaceChip(cd1, pos1);
        else Debug.Log("FAILURE");

        // Enter 2nd chip for player 0
        sourceStackLength = chipSources[playerIndex].Count;
        ChipData cd2 = chipSources[playerIndex][sourceStackLength - 1];
        (bool succes2, Vector2Int newMoves2) = gbc.EnterChip(pos2, moveSet, playerIndex);
        if (succes2) v.ShowPlaceChip(cd2, pos2);
        else Debug.Log("FAILURE");

        Vector2Int pos3 = new Vector2Int(2,4);

        // Attempt to enter chip without proper moves for player 0
        sourceStackLength = chipSources[playerIndex].Count;
        ChipData cd3 = chipSources[playerIndex][sourceStackLength - 1];
        (bool succes3, Vector2Int newMoves3) = gbc.EnterChip(pos3, moveSet, playerIndex);
        if (succes3) v.ShowPlaceChip(cd3, pos3);
        else Debug.Log("FAILURE");

        Vector2Int pos4 = new Vector2Int(0,0);

        // Attempt to enter chip in an invalid spot for player 0
        sourceStackLength = chipSources[playerIndex].Count;
        ChipData cd4 = chipSources[playerIndex][sourceStackLength - 1];
        (bool succes4, Vector2Int newMoves4) = gbc.EnterChip(pos4, moveSet, playerIndex);
        if (succes3) v.ShowPlaceChip(cd4, pos4);
        else Debug.Log("FAILURE");

        playerIndex = 1;
        moveSet = new Vector2Int(5, 6);
        Vector2Int pos5 = new Vector2Int(1,0);
        Vector2Int pos6 = new Vector2Int(2,0);

        // Enter 2 chips for player 1
        sourceStackLength = chipSources[playerIndex].Count;
        ChipData cd5 = chipSources[playerIndex][sourceStackLength - 1];
        (bool succes5, Vector2Int newMoves5) = gbc.EnterChip(pos5, moveSet, playerIndex);
        if (succes5) v.ShowPlaceChip(cd5, pos5);
        else Debug.Log("FAILURE");

        sourceStackLength = chipSources[playerIndex].Count;
        ChipData cd6 = chipSources[playerIndex][sourceStackLength - 1];
        (bool success6, Vector2Int newMoves6) = gbc.EnterChip(pos6, moveSet, playerIndex);
        if (success6) v.ShowPlaceChip(cd6, pos6);
        else Debug.Log("FAILURE");
    }

    public void TestShowPlaceChipOnStack()
    {
        List<ChipData>[] chipSources = gbc.GetChipSources();
        int playerIndex = 0;

        playerIndex = 0;
        Vector2Int pos1 = new Vector2Int(1,3);
        Vector2Int pos2 = new Vector2Int(1,4);
        
        // Enter chip for player 0
        ChipData cd = chipSources[playerIndex][11];
        v.ShowPlaceChip(cd, pos1);
        ChipData cd1 = chipSources[playerIndex][10];
        v.ShowPlaceChip(cd1, pos1);

        playerIndex = 1;
        ChipData cd2 = chipSources[playerIndex][11];
        v.ShowPlaceChip(cd2, pos1);

        playerIndex = 0;
        ChipData cd3 = chipSources[playerIndex][9];
        v.ShowPlaceChip(cd3, pos1);
    }

    public void TestShowPlaceStack()
    {

        List<ChipData>[] chipSources = gbc.GetChipSources();
        int playerIndex = 0;
        List<ChipData>[,] board = gbc.GetGameBoard();
        Vector2Int pos1 = new Vector2Int(1,3);
        Vector2Int pos2 = new Vector2Int(1,4);

            playerIndex = 0;
            // Enter chip for player 0
            ChipData cd = gbc.PopSourceChip(playerIndex);
            board[pos1.x,pos1.y].Add(cd);
            v.ShowPlaceChip(cd, pos1);

            ChipData cd1 = gbc.PopSourceChip(playerIndex);
            board[pos1.x,pos1.y].Add(cd1);
            v.ShowPlaceChip(cd1, pos1);

            playerIndex = 1;
            ChipData cd2 = gbc.PopSourceChip(playerIndex);
            board[pos1.x,pos1.y].Add(cd2);
            v.ShowPlaceChip(cd2, pos1);

            playerIndex = 0;
            ChipData cd3 = gbc.PopSourceChip(playerIndex);
            board[pos1.x,pos1.y].Add(cd3);
            v.ShowPlaceChip(cd3, pos1);



            // end-turn logic here
            playerIndex = 0;
            ChipData cd4 = gbc.PopSourceChip(playerIndex);
            board[pos2.x,pos2.y].Add(cd4);
            v.ShowPlaceChip(cd4, pos2);

            ChipData cd5 = gbc.PopSourceChip(playerIndex);
            board[pos2.x,pos2.y].Add(cd5);
            v.ShowPlaceChip(cd5, pos2);

            playerIndex = 1;
            ChipData cd6 = gbc.PopSourceChip(playerIndex);
            board[pos2.x,pos2.y].Add(cd6);
            v.ShowPlaceChip(cd6, pos2);
            ChipData cd7 = gbc.PopSourceChip(playerIndex);
            board[pos2.x,pos2.y].Add(cd7);
            v.ShowPlaceChip(cd7, pos2);


        v.WaitForAllChipMoves(() =>
        {
            v.ShowPlaceStack(pos2, pos1);  
        });

        Debug.Log("NOW");
    }

    public void TestResetStack()
    {
        List<ChipData>[] chipSources = gbc.GetChipSources();
        int playerIndex = 0;
        List<ChipData>[,] board = gbc.GetGameBoard();
        Vector2Int pos1 = new Vector2Int(1,3);
        Vector2Int pos2 = new Vector2Int(1,4);

        playerIndex = 0;
        // Enter chip for player 0
        ChipData cd = gbc.PopSourceChip(playerIndex);
        board[pos1.x,pos1.y].Add(cd);
        v.ShowPlaceChip(cd, pos1);

        ChipData cd1 = gbc.PopSourceChip(playerIndex);
        board[pos1.x,pos1.y].Add(cd1);
        v.ShowPlaceChip(cd1, pos1);

        playerIndex = 1;
        ChipData cd2 = gbc.PopSourceChip(playerIndex);
        board[pos1.x,pos1.y].Add(cd2);
        v.ShowPlaceChip(cd2, pos1);

        playerIndex = 0;
        ChipData cd3 = gbc.PopSourceChip(playerIndex);
        board[pos1.x,pos1.y].Add(cd3);
        v.ShowPlaceChip(cd3, pos1);

        GameObject[,] shells = v.GetShells();

        GameObject stackShell = shells[pos1.x, pos1.y];
        Vector3 curPos = stackShell.transform.localPosition;
        curPos.y += 2f;
        stackShell.transform.localPosition = curPos;

        v.WaitForAllChipMoves(() =>
        {
            v.ShowResetStack(pos1);
        });
    }

    public void TestHoverChipThenPlace()
    {
        List<ChipData>[] chipSources = gbc.GetChipSources();
        int playerIndex = 0;
        List<ChipData>[,] board = gbc.GetGameBoard();
        Vector2Int pos1 = new Vector2Int(1,3);
        Vector2Int pos2 = new Vector2Int(1,4);

        playerIndex = 0;
        // Enter chip for player 0
        ChipData cd = gbc.PopSourceChip(playerIndex);
        board[pos1.x,pos1.y].Add(cd);
        v.ShowPlaceChip(cd, pos1);

        ChipData cd1 = gbc.PopSourceChip(playerIndex);
        board[pos1.x,pos1.y].Add(cd1);
        v.ShowPlaceChip(cd1, pos1);

        v.StartHoverChip(cd1);
        v.StopHover();
        v.StartHoverChip(cd1);
    }

    public void ShowPlaceSourceChips()
    {
        List<ChipData>[] chipSources = gbc.GetChipSources();
        int playerIndex = 0;
        List<ChipData>[,] board = gbc.GetGameBoard();
        Vector2Int pos1 = new Vector2Int(1,3);
        Vector2Int pos2 = new Vector2Int(1,4);

        playerIndex = 0;

        ChipData cd = gbc.PopSourceChip(playerIndex);
        board[pos1.x,pos1.y].Add(cd);
        v.ShowPlaceChip(cd, pos1);

        ChipData cd1 = gbc.PopSourceChip(playerIndex);
        board[pos1.x,pos1.y].Add(cd1);
        v.ShowPlaceChip(cd1, pos1);

        v.WaitForAllChipMoves(() =>
        {
            v.ShowPlaceSourceChip(cd);
            v.ShowPlaceSourceChip(cd1);
        });
    }

    public void ShowPlaceBankChips()
    {
        List<ChipData>[] chipSources = gbc.GetChipSources();
        int playerIndex = 0;
        List<ChipData>[,] board = gbc.GetGameBoard();
        Vector2Int pos1 = new Vector2Int(1,3);
        Vector2Int pos2 = new Vector2Int(1,4);

        playerIndex = 0;

        ChipData cd = gbc.PopSourceChip(playerIndex);
        board[pos1.x,pos1.y].Add(cd);
        v.ShowPlaceChip(cd, pos1);

        ChipData cd1 = gbc.PopSourceChip(playerIndex);
        board[pos1.x,pos1.y].Add(cd1);
        v.ShowPlaceChip(cd1, pos1);

        v.WaitForAllChipMoves(() =>
        {
            v.ShowPlaceBankChip(cd);
            v.ShowPlaceBankChip(cd1);
        });
    }
}
