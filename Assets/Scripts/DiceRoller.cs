using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiceRoller : MonoBehaviour
{
    public Dice dicePrefab;
    public int amountOfDice = 2;
    public float throwForce = 5.0f;
    public float throwSideForce = 1.0f;
    public float rollForce = 10.0f;

    private List<GameObject> spawnedDice = new List<GameObject>();

    public static UnityAction<int> OnDiceRoll;

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     StartCoroutine(RollDice());
        // }
    }

    public void TriggerRollDice()
    {
        StartCoroutine(RollDice());
    }

    private IEnumerator RollDice()
    {
        OnDiceRoll?.Invoke(amountOfDice);

        if (dicePrefab == null)
        {
            yield break;
        }

        foreach (var die in spawnedDice)
        {
            Destroy(die);
        }

        for (int i = 0; i < amountOfDice; i++)
        {
            Dice dice = Instantiate(dicePrefab, transform.position + new Vector3(i, 0f, 0f), transform.rotation);
            spawnedDice.Add(dice.gameObject);
            dice.RollDice(throwForce, throwSideForce, rollForce, i);
            yield return null;
        }
    }
}
