using System.Collections;
using UnityEngine;

public class Chip : MonoBehaviour
{
    // The data object this visual represents
    public ChipData chipData { get; private set; }
    private Coroutine wobbleCoroutine;

    // Called by game logic when creating or reusing a visual
    public void Bind(ChipData data)
    {
        chipData = data;
        UpdateVisual();
    }

        void UpdateVisual()
    {
        // Simple example: tint sprite by playerIndex if a SpriteRenderer exists.
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && chipData != null)
        {
            if (chipData.playerIndex == 0) sr.color = Color.red;
            else sr.color = Color.blue;
        }

        // Additional visual setup (text/type icons, etc.) goes here.
    }

    public void StartWobbling()
    {
        if (wobbleCoroutine == null) wobbleCoroutine = StartCoroutine(WobbleCoroutine());
    }

    public void StopWobbling()
    {
        if (wobbleCoroutine != null)
        {
            StopCoroutine(wobbleCoroutine);
            wobbleCoroutine = null;
            transform.localRotation = Quaternion.identity;
        }
    }

    private IEnumerator WobbleCoroutine()
    {
        float elapsed = 0f;
        float yawSpeed = Random.Range(25f, 35f);
        float leanAngle = Random.Range(6f, 10f);
        float leanFreq = Random.Range(0.4f, 0.6f);
        Quaternion startRotation = transform.localRotation;

        while (true)
        {
            elapsed += Time.deltaTime;
            float yaw = (elapsed * yawSpeed) % 360f;
            float roll = Mathf.Sin(elapsed * Mathf.PI * 2f * leanFreq) * leanAngle;
            transform.localRotation = startRotation * Quaternion.Euler(0f, yaw, roll);
            yield return null;
        }
    }
}
