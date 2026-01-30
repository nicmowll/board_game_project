using NUnit.Framework.Constraints;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Dice : MonoBehaviour
{
    [SerializeField] private float tolerance = 0.80f;

    private Rigidbody rb;
    private bool hasStoppedRolling = false;
    private bool hasThrowDelayFinished = false;
    private int diceIndex = -1;

    public static UnityAction<int,int> OnDiceResult;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasThrowDelayFinished) { return; }

        if (!hasStoppedRolling && rb.linearVelocity.sqrMagnitude == 0f)
        {
            hasStoppedRolling = true;
            GetSideUp();
        }   
    }

    private void GetSideUp()
    {
        Vector3[] sides = new Vector3[] {
            transform.forward.normalized, //1
            transform.up.normalized,      //2
            -transform.right.normalized,  //3
            transform.right.normalized,   //4
            -transform.up.normalized,     //5
            -transform.forward.normalized //6
        };

        for (int i = 0; i < sides.Length; i++)
        {
            if(Vector3.Dot(sides[i], Vector3.up) > tolerance)
            {
                OnDiceResult?.Invoke(diceIndex, i+1);
            }
        }

        Debug.Log("No sides within tolerance for Dice Index: " + diceIndex);

    }

    internal void RollDice(float _throwForce, float _throwSideForce, float _rollForce, int _diceIndex)
    {
        diceIndex = _diceIndex;

        float randomVariance = UnityEngine.Random.Range(-1f, 1f);

        //forward throw force
        rb.AddForce(transform.forward * (randomVariance + _throwForce), ForceMode.Impulse);
        //side force variance
        rb.AddForce(transform.right * (randomVariance * _throwSideForce), ForceMode.Impulse);


        float rollX = UnityEngine.Random.Range(-1f, 1f);
        float rollY = UnityEngine.Random.Range(-1f, 1f);
        float rollZ = UnityEngine.Random.Range(-1f, 1f);

        rb.AddTorque(new Vector3 (rollX, rollY, rollZ) * (_rollForce + randomVariance));

        StartCoroutine(ThrowDelay());
    }

    private IEnumerator ThrowDelay()
    {
        yield return new WaitForSeconds(1);
        hasThrowDelayFinished = true;
    }
}
