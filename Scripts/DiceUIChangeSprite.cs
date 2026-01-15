using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.Profiling;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Legacy;
using UnityEngine;
using UnityEngine.UI;

public class DiceUIChangeSprite : MonoBehaviour
{
    private UnityEngine.UI.Image uiPanelImage;

    public int diceUIIndex;

    public Sprite n1;
    public Sprite n2;
    public Sprite n3;
    public Sprite n4;
    public Sprite n5;
    public Sprite n6;

    private List<Sprite> spriteList = new List<Sprite>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiPanelImage = GetComponent<UnityEngine.UI.Image>();

        if (uiPanelImage == null)
        {
            Debug.LogError("Image component cannot be found on panel when trying to project dice sprite UI");
        }

        spriteList.Add(n1);
        spriteList.Add(n2);
        spriteList.Add(n3);
        spriteList.Add(n4);
        spriteList.Add(n5);
        spriteList.Add(n6);
    }

    private void OnEnable()
    {
        DiceRoller.OnDiceRoll += SetRollImage;
        Dice.OnDiceResult += ChangeDiceUISprite;      
    }

    private void OnDisable()
    {
        DiceRoller.OnDiceRoll -= SetRollImage;
        Dice.OnDiceResult -= ChangeDiceUISprite;
    }

    private void ChangeDiceUISprite(int _diceIndex, int _diceResult)
    {
        if (diceUIIndex != _diceIndex) { return; }
        ChangeImage(_diceResult);
    }

    private void ChangeImage(int _diceResult)
    {
        Color newColor = new Color(255,255,255,255);
        uiPanelImage.color = newColor;
        uiPanelImage.sprite = spriteList[_diceResult - 1];
    }

    private void SetRollImage(int _amountOfDice)
    {
        Color newColor = new Color(255,255,255,0.2f);
        uiPanelImage.color = newColor;
    }
}
