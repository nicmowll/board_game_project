using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using System;
using UnityEditor.Animations;
using System.Collections;
using UnityEngine.SocialPlatforms;
using System.ComponentModel;


public class GameBoardView : MonoBehaviour
{
    Dictionary<ChipData, Chip> viewMap = new Dictionary<ChipData, Chip>();
    private Dictionary<Chip, Coroutine> chipMoveRoutines = new Dictionary<Chip, Coroutine>();
    private Dictionary<GameObject, Coroutine> stackShellMoveRoutines = new Dictionary<GameObject, Coroutine>();
    private List<GameObject>[] chipSourceStacks;
    private List<GameObject>[] chipBankStacks;
    private int maxChipSourceStackCount = 0;
    private GameBoardController gameBoardController;
    private float chipPrefabRadius;
    private Vector2[,] gameBoardPositionMap;
    private GameObject[,] gameBoardStackShells;
    private Coroutine hoverCoroutine;
    public bool AnyChipMoving => chipMoveRoutines.Count > 0;

    [Header("Prefabs")]
    [Description("Direct Prefab References for player chips.")]
    public Chip chipPrefabPlayer0;
    public Chip chipPrefabPlayer1;
    [Description("Height and Diameter of the chip prefabs.")]
    public float chipPrefabHeight = 0.24f;
    public float chipPrefabDiameter = 1.2f;
    [Description("Side Length of one board tile.")]
    public float boardSquareSize = 2.0f;
    [Description("Height of board.")]
    public float boardHeightOffset = 0.3f;

    [Header("Game View Preferences")]
    [Description("The number of chip stacks that will be show in a player's off-board pile.")]
    public int numChipSourceStacks = 3;
    [Description("The x and z position jitter applied when stacking chips (percentage of chip diameter).")]
    public float chipStackHorizontalMaxJitter = 0.1f;
    [Description("Amount of time in seconds it takes for a chip to snap back to the board after hovering.")]
    public float chipLerpDuration = 0.2f;
    [Description("The springiness of the chip hover movement (1 = instant, 0 = still)")]
    public float hoverLerpStrength = 0.8f;
    public float hoverHeight = 3f;
    public float teleportTimeDelay = 0.2f;

    [Header("Transform References")]
    [Description("The board, and player chip piles references by game logic.")]
    public Transform player0ChipSource;
    public Transform player1ChipSource;
    public Transform player0ChipBank;
    public Transform player1ChipBank;
    public Transform gameBoard;


    void Awake()
    {
        gameBoardController = GetComponent<GameBoardController>();

        chipPrefabRadius = chipPrefabDiameter / 2;

        chipSourceStacks = new List<GameObject>[2];
        chipBankStacks = new List<GameObject>[2];
        for (int p = 0; p < 2; p++)
        {
            chipSourceStacks[p] = new List<GameObject>();
            chipBankStacks[p] = new List<GameObject>();
        }

        // Calculate the gameBoardPositionMap
        int x = gameBoardController.GetBoardWidth();
        int y = gameBoardController.GetBoardHeight();

        gameBoardPositionMap = new Vector2[x,y];
        gameBoardStackShells = new GameObject[x,y];
        for (int xx = 0; xx < x; xx++)
        {
            for (int yy = 0; yy < y; yy++)
            {
                float xPos = boardSquareSize * xx;
                float yPos = boardSquareSize * yy * -1; //-1 because origin is top left, must come back in z direction
                gameBoardPositionMap[xx,yy] = new Vector2(xPos, yPos);

                Vector3 stackShellLocalPos = new Vector3(xPos, boardHeightOffset, yPos);
                GameObject stackShell = new GameObject($"StackShell_{xx}_{yy}");
                stackShell.transform.SetParent(transform,false);
                stackShell.transform.localPosition = stackShellLocalPos;
                gameBoardStackShells[xx,yy] = stackShell;
            }
        }

        var chipSources = gameBoardController.GetChipSources();
        SpawnSourceChips(chipSources);
    }

    public GameObject[,] GetShells()
    {
        return gameBoardStackShells;
    }

    public Transform GetSourceTransform(int playerIndex)
    {
        return playerIndex == 0 ? (player0ChipSource ?? transform) : (player1ChipSource ?? transform);
    }

    public Transform GetBankTransforms(int playerIndex)
    {
        return playerIndex == 0 ? (player0ChipBank ?? transform) : (player1ChipBank ?? transform);
    }

    public void SpawnSourceChips(List<ChipData>[] chipSources)
    {
        for (int p = 0; p <= 1; p++)
        {
            // create the chip source stack empty game objects (we create a variable number around the chip source transform origin)
            float chipSourceStackPositionRadius = 1.5f * chipPrefabRadius;
            Transform parentChipSource = GetSourceTransform(p);
            Transform parentChipBank = GetBankTransforms(p);
            for (int s = 0; s < numChipSourceStacks; s++)
            {
                // calc the angle at which the stack should be placed
                float chipSourceStackPositionAngle = s * Mathf.PI * 2f / numChipSourceStacks;
                // calc the position for the stack local to the parent chip source object
                Vector3 chipSourceStackLocalPos = new Vector3(Mathf.Cos(chipSourceStackPositionAngle) * chipSourceStackPositionRadius, 0f, Mathf.Sin(chipSourceStackPositionAngle) * chipSourceStackPositionRadius);

                // create the chip source stack empty game object
                GameObject chipSourceStack = new GameObject($"ChipSourceStack_{s}");
                chipSourceStack.transform.parent = parentChipSource;
                chipSourceStack.transform.localPosition = chipSourceStackLocalPos;

                // Do the same for the chip banks just with the Bank Parent
                GameObject chipBankStack = new GameObject($"ChipBankStack_{s}");
                chipBankStack.transform.parent = parentChipBank;
                chipBankStack.transform.localPosition = chipSourceStackLocalPos;

                // add it to chip source stack list for the player
                chipSourceStacks[p].Add(chipSourceStack);
                chipBankStacks[p].Add(chipBankStack);
            }

            // Instantiate Chip objects 
            for (int cd = 0; cd < chipSources[p].Count; cd++)
            {
                maxChipSourceStackCount = Mathf.CeilToInt((float)chipSources[p].Count / numChipSourceStacks);
                int stackIndex = GetFirstAvailableSourceStack(chipSourceStacks, maxChipSourceStackCount, p);

                var chipData = chipSources[p][cd];
                var prefab = chipData.playerIndex == 0 ? chipPrefabPlayer0 : chipPrefabPlayer1;
                if (prefab == null) return;

                // assign the parent as the next available chip source stack and get stack count
                var parentChipSourceStackTransform = chipSourceStacks[p][stackIndex].transform;
                int chipSourceStackCount = chipSourceStacks[p][stackIndex].transform.childCount;
                var chip = Instantiate(prefab, parentChipSourceStackTransform, false);
                chip.transform.localPosition = GetChipPositionOnStack(chipSourceStackCount);
                chip.transform.localRotation = Quaternion.identity;
                viewMap[chipData] = chip;
            }
        }
    }

    public int GetFirstAvailableSourceStack(List<GameObject>[] chipSourceStacks, int maxStackCount, int playerIndex) 
    {
        for (int si = 0; si < chipSourceStacks[playerIndex].Count; si++)
        {
            if (chipSourceStacks[playerIndex][si].transform.childCount < maxStackCount) return si;
        }

        return 0;
    }

    public Vector3 GetChipPositionOnStack(int stackCount)
    {
        float y = chipPrefabHeight * stackCount;

        float xJitterMultiplier = UnityEngine.Random.Range(-1, 1);
        float zJitterMultiplier = UnityEngine.Random.Range(-1, 1);

        float x = chipStackHorizontalMaxJitter * xJitterMultiplier;
        float z = chipStackHorizontalMaxJitter * zJitterMultiplier;

        Vector3 localPos = new Vector3(x, y, z);

        return localPos;
    }

    // Place a chip somewhere on the board, reparent it to the stack shell, and calculate its new local position
    public void ShowPlaceChip(ChipData chipData, Vector2Int pos)
    {
        Chip chip = viewMap[chipData];
        if (chip == null) return;

        // get the stack shell parent and then reparent the chip - keep world pos because later I'll want to move this over time
        GameObject parentStackShell = gameBoardStackShells[pos.x,pos.y];
        Transform parentStackShellTransform = parentStackShell.transform;
        // Get count of chips within the stack shell
        int stackShellCount = parentStackShell.transform.childCount;
        chip.transform.SetParent(parentStackShellTransform, true);

        // Get new local target pos for chip on new stack shell
        Vector3 newLocalPos = GetChipPositionOnStack(stackShellCount);

        if (chipMoveRoutines.TryGetValue(chip, out var running))
            StopCoroutine(running);

        chip.StopWobbling();

        Coroutine co = StartCoroutine(MoveChipCoroutine(chip, newLocalPos, chipLerpDuration));
        chipMoveRoutines[chip] = co;
    }

    // Place a stack somewhere on the board, reparent all its children chips to the new stack shell, and calculate their local positions
    public void ShowPlaceStack(Vector2Int startPos, Vector2Int endPos)
    {
        List<ChipData> chipDataList = gameBoardController.GetBoardChipStack(startPos);

        for (int c = 0; c < chipDataList.Count; c++)
        {
            ChipData chipData = chipDataList[c];
            ShowPlaceChip(chipData, endPos);
        }
    }

    // Place a chip into it's source pile, calculate the available source stack and calculate its local position
    // The chip poofs back into place
    public void ShowPlaceSourceChip(ChipData chipData)
    {
        int playerIndex = chipData.playerIndex;
        Chip chip = viewMap[chipData];
        if (chip == null) return;

        chip.transform.SetParent(transform, true);

        int stackIndex = GetFirstAvailableSourceStack(chipSourceStacks, maxChipSourceStackCount, playerIndex);
        GameObject parentSourceStack = chipSourceStacks[playerIndex][stackIndex];
        Transform parentChipSourceStackTransform = parentSourceStack.transform;

        int chipSourceStackCount = chipSourceStacks[playerIndex][stackIndex].transform.childCount;        
        Vector3 newLocalPos = GetChipPositionOnStack(chipSourceStackCount);

        chip.transform.SetParent(parentChipSourceStackTransform, true);

        if (chipMoveRoutines.TryGetValue(chip, out var running))
            StopCoroutine(running);

        chip.StopWobbling();

        Coroutine co = StartCoroutine(MoveChipCoroutine(chip, newLocalPos, chipLerpDuration));
        chipMoveRoutines[chip] = co;
    }

    public void ShowPlaceBankChip(ChipData chipData)
    {
        int playerIndex = chipData.playerIndex;
        Chip chip = viewMap[chipData];
        if (chip == null) return;

        chip.transform.SetParent(transform, true);

        int stackIndex = GetFirstAvailableSourceStack(chipBankStacks, maxChipSourceStackCount, playerIndex);
        GameObject parentBankStack = chipBankStacks[playerIndex][stackIndex];
        Transform parentBankStackTransform = parentBankStack.transform;
        
        int chipBankStackCount = chipBankStacks[playerIndex][stackIndex].transform.childCount;
        Vector3 newLocalPos = GetChipPositionOnStack(chipBankStackCount);

        chip.transform.SetParent(parentBankStackTransform, true);

        if (chipMoveRoutines.TryGetValue(chip, out var running))
            StopCoroutine(running);

        chip.StopWobbling();

        Coroutine co = StartCoroutine(MoveChipCoroutine(chip, newLocalPos, chipLerpDuration));
        chipMoveRoutines[chip] = co;
    }

    public void ShowBankStack(Vector2Int startPos, int playerIndex)
    {
        GameObject stackShell = gameBoardStackShells[startPos.x, startPos.y];
        var chips = new List<Chip>(stackShell.GetComponentsInChildren<Chip>());
        int xMiddleIndex = (int)Mathf.Floor((float)gameBoardController.GetBoardWidth() / 2f);
        int playerOrientation = gameBoardController.GetPlayerOrientation(playerIndex);
        int playerSideIndex = (playerOrientation == -1) ? 0 : gameBoardController.GetBoardHeight() - 1;
        Vector2 pos = gameBoardPositionMap[xMiddleIndex, playerSideIndex];

        // position to hover first
        Vector3 hoverPos = new Vector3(xMiddleIndex, hoverHeight, playerSideIndex + (boardSquareSize * playerOrientation));

        // Tilt up camera for this action

        // trigger coroutine to move stack to the hover location
        // then move friendly chips above bank,
        // then 1 by 1 with a 0.2 second delay, lep them down onto their targe spot
        // then freeze for a time delay
        // then move the enemy chips back to their source
        // then stack them all down at once



    }

    // resets a chip stack shell parent to its designated board position
    public void ShowResetStack(Vector2Int startPos)
    {
        GameObject stackShell = gameBoardStackShells[startPos.x, startPos.y];
        Vector2 pos = gameBoardPositionMap[startPos.x, startPos.y];
        Vector3 newLocalPos = new Vector3(pos.x, boardHeightOffset, pos.y);

        var chips = new List<Chip>(stackShell.GetComponentsInChildren<Chip>());
        for (int c = 0; c < chips.Count; c++)
        {
            Chip chip = chips[c];
            chip.StopWobbling();
        }
        //stackShell.transform.localPosition = newLocalPos;

        if (stackShellMoveRoutines.TryGetValue(stackShell, out var running))
             StopCoroutine(running);

        Coroutine co = StartCoroutine(TeleportStackShellCouroutine(stackShell, newLocalPos, teleportTimeDelay));
        stackShellMoveRoutines[stackShell] = co;
    }

    public void ShowResetChipToSource(ChipData chipData)
    {
        int playerIndex = chipData.playerIndex;
        Chip chip = viewMap[chipData];
        if (chip == null) return;

        chip.transform.SetParent(transform, true);

        int stackIndex = GetFirstAvailableSourceStack(chipSourceStacks, maxChipSourceStackCount, playerIndex);
        GameObject parentSourceStack = chipSourceStacks[playerIndex][stackIndex];
        Transform parentChipSourceStackTransform = parentSourceStack.transform;

        int chipSourceStackCount = chipSourceStacks[playerIndex][stackIndex].transform.childCount;        
        Vector3 newLocalPos = GetChipPositionOnStack(chipSourceStackCount);

        chip.transform.SetParent(parentChipSourceStackTransform, true);

        if (chipMoveRoutines.TryGetValue(chip, out var running))
            StopCoroutine(running);

        chip.StopWobbling();

        Coroutine co = StartCoroutine(TeleportChipCoroutine(chip, newLocalPos, teleportTimeDelay));
        chipMoveRoutines[chip] = co;
    }

    // IDEAS!!!!!
    // for when a chip is hovering and released and it needs to go back to source , or when a stack is hovering and it is reset and needs to go back to the board spot, there should be a poof particle and they should instantly telepport back.
        // For this same idea, maybe we add a coroutine with a half second delay and then reappear it in its original location with another particle effect (animation staging principal)
    
    // when a chip or stack are placed on the board , we can leave the above place functions as so, where they lerp to their target. It will always be close enough to the target and look uniformly good

    // when chips are banked or return to the opponent's source pile, we can have a longer animation where the chips glide horizontally to the target first and hover, but then there's an animation sequence where, 
        // 1. your banked chips hover over to above the bank, paue, then one by one move down to it, with a satisfying pop noise for each
        // 2. then opponent's chips hover back over to their source pile and all go down vertically together. This is less satisfying as you are just returning them

    public void StartHoverChip(ChipData chipData)
    {
        if (hoverCoroutine != null) return;

        Chip chip = viewMap[chipData];
        if (chip == null) return;

        chip.StartWobbling();

        if (hoverCoroutine == null) hoverCoroutine = StartCoroutine(HoverChipCoroutine(chip));
    }

    public void StartHoverStackShell(Vector2Int srcPos)
    {
        if (hoverCoroutine != null) return;

        GameObject stackShell = gameBoardStackShells[srcPos.x, srcPos.y];

        var chips = new List<Chip>(stackShell.GetComponentsInChildren<Chip>());
        for (int c = 0; c < chips.Count; c++)
        {
            Chip chip = chips[c];
            chip.StartWobbling();
            float x = chip.transform.localPosition.x;
            float z = chip.transform.localPosition.z;
            float y = chip.transform.localPosition.y;
            Vector3 newLocalChipPosition = new Vector3(x, y + (chipPrefabHeight * c * 0.6f), z);
            chip.transform.localPosition = newLocalChipPosition;
        }

        if (hoverCoroutine == null) hoverCoroutine = StartCoroutine(HoverStackShellCoroutine(stackShell));
    }

    public void StopHover()
    {
        if (hoverCoroutine == null) return;
        StopCoroutine(hoverCoroutine);
        hoverCoroutine = null;
    }

    public Vector3 MouseScreenToWorldXZ()
    {
        Camera cam = Camera.main;
        float y = transform.position.y + boardHeightOffset;
        if (cam == null) return new Vector3(0f, y, 0f);

        Plane plane = new Plane(Vector3.up, new Vector3(0f, y, 0f));
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 worldPoint = ray.GetPoint(enter);
            return new Vector3(worldPoint.x, y, worldPoint.z);
        }

        return new Vector3(0f, y, 0f);
    }


    // COROs =====================================================================================================
    private IEnumerator MoveChipCoroutine(Chip chip, Vector3 targetPos, float duration = 0.2f)
    {
        float timeElapsed = 0f;
        Vector3 startPos = chip.transform.localPosition;

        while (timeElapsed < duration)
        {
            float terpFactor = timeElapsed / duration;
            terpFactor = Mathf.SmoothStep(0f, 1f, terpFactor);

            chip.transform.localPosition = Vector3.Lerp(startPos, targetPos, terpFactor);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        chip.transform.localPosition = targetPos;
        chipMoveRoutines.Remove(chip);
    }

    private IEnumerator MoveStackShellCoroutine(GameObject stackShell, Vector3 targetPos, float duration = 0.2f)
    {
        float timeElapsed = 0f;
        Vector3 startPos = stackShell.transform.localPosition;

        while (timeElapsed < duration)
        {
            float terpFactor = timeElapsed / duration;
            terpFactor = Mathf.SmoothStep(0f, 1f, terpFactor);

            stackShell.transform.localPosition = Vector3.Lerp(startPos, targetPos, terpFactor);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        stackShell.transform.localPosition = targetPos;
        stackShellMoveRoutines.Remove(stackShell);
    }

    private IEnumerator TeleportChipCoroutine(Chip chip, Vector3 targetPos, float timeDelay)
    {
        Renderer renderer = chip.GetComponent<Renderer>();
        if (renderer != null) renderer.enabled = false;

        yield return new WaitForSeconds(timeDelay);

        chip.transform.localPosition = targetPos;
        if(renderer != null) renderer.enabled = true;

        chipMoveRoutines.Remove(chip);
    }

    private IEnumerator TeleportStackShellCouroutine(GameObject stackShell, Vector3 targetPos, float timeDelay)
    {
        var chips = new List<Chip>(stackShell.GetComponentsInChildren<Chip>());
        for (int c = 0; c < chips.Count; c++)
        {
            Chip chip = chips[c];
            Renderer renderer = chip.GetComponent<Renderer>();
            if (renderer != null) renderer.enabled = false;
        }

        yield return new WaitForSeconds(timeDelay);

        stackShell.transform.localPosition = targetPos;
        for (int c = 0; c < chips.Count; c++)
        {
            Chip chip = chips[c];
            Renderer renderer = chip.GetComponent<Renderer>();
            if (renderer != null) renderer.enabled = true;
        }

        stackShellMoveRoutines.Remove(stackShell);
    }

    private IEnumerator HoverChipCoroutine(Chip chip)
    {
        const float epsilon = 0.001f;
        float hoverY = hoverHeight;

        while (true)
        {
            Vector3 mousePos = MouseScreenToWorldXZ();
            Vector3 targetPos = new Vector3(mousePos.x, hoverY, mousePos.z);
            Vector3 current = chip.transform.position;

            float dist = Vector3.Distance(current, targetPos);

            if (dist > epsilon)
            {
                float lerpFactor = Mathf.SmoothStep(0f, 1f, hoverLerpStrength);
                chip.transform.position = Vector3.Lerp(current, targetPos, lerpFactor);
            }

            yield return null;
        }
    }

    private IEnumerator HoverStackShellCoroutine(GameObject stackShell)
    {
        const float epsilon = 0.001f;
        float hoverY = hoverHeight;

        while (true)
        {
            Vector3 mousePos = MouseScreenToWorldXZ();
            Vector3 targetPos = new Vector3(mousePos.x, hoverY, mousePos.z);
            Vector3 current = stackShell.transform.position;

            float dist = Vector3.Distance(current, targetPos);

            if (dist > epsilon)
            {
                float lerpFactor = Mathf.SmoothStep(0f, 1f, hoverLerpStrength);
                stackShell.transform.position = Vector3.Lerp(current, targetPos, lerpFactor);
            }

            yield return null;
        }
    }

    // yield until all chip move coroutines have finished relating to chip movement - 
    // callback-style waiter (no need to be called from a coroutine)
    public void WaitForAllChipMoves(System.Action onComplete)
    {
        if (!AnyChipMoving)
        {
            onComplete?.Invoke();
            return;
        }
        StartCoroutine(WaitForAllChipMoves_Coro(onComplete));
    }

    private IEnumerator WaitForAllChipMoves_Coro(System.Action onComplete)
    {
        yield return new WaitUntil(() => chipMoveRoutines.Count == 0 && stackShellMoveRoutines.Count == 0);
        onComplete?.Invoke();
    }
}