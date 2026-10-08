using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hints : MonoBehaviour
{
    // Lists kept as-is so your Inspector setup survives.
    // Entry N of each list is one hint, so both lists must be the same length.
    [SerializeField] private List<Sprite> hintImages = new List<Sprite>();
    [SerializeField] private List<string> hintText = new List<string>();

    [SerializeField] private Image hintImageBox;
    [SerializeField] private TMP_Text hintTextBox;

    [SerializeField] private float hintResetTimer = 5f; // seconds between hints

    private float hintTimer;
    private int currentIndex = -1;

    private void Start()
    {
        if (hintText.Count == 0)
        {
            Debug.LogWarning("Hints: no hint text assigned.", this);
            enabled = false;
            return;
        }

        if (hintImages.Count != hintText.Count)
            Debug.LogWarning("Hints: image and text lists differ in length. Extra entries are ignored.", this);

        ShowRandomHint(); // show one immediately instead of an empty box
    }

    private void Update()
    {
        hintTimer += Time.unscaledDeltaTime; // keeps rotating even if timeScale is 0
        if (hintTimer >= hintResetTimer)
        {
            hintTimer = 0f;
            ShowRandomHint();
        }
    }

    private void ShowRandomHint()
    {
        int count = Mathf.Min(hintText.Count, hintImages.Count > 0 ? hintImages.Count : hintText.Count);
        int next = Random.Range(0, count);

        // Avoid showing the same hint twice in a row.
        if (count > 1)
            while (next == currentIndex)
                next = Random.Range(0, count);

        currentIndex = next;
        hintTextBox.text = hintText[next];

        if (hintImageBox != null && next < hintImages.Count)
            hintImageBox.sprite = hintImages[next];
    }
}
