using UnityEngine;

public class ChipData
{
    public int playerIndex;
    public string type;
    public bool isOnBoard = false;
    public bool isBanked = false;

    public void Initialize(int playerIndex, string type)
    {
        this.playerIndex = playerIndex;
        this.type = type;
        this.isOnBoard = false;
        this.isBanked = false;
    }
}
