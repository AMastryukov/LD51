using TMPro;
using UnityEngine;

public class PlayerBuffsTester : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI activeBuffText = null;
    [SerializeField] private TextMeshProUGUI nextBuffText = null;
    [SerializeField] private bool verboseLogging = false;

    private void Start()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Start), this);
        }

        GameManager.OnNewBuff += OnNewBuff;
    }

    private void OnNewBuff(Buffs newBuff, Buffs nextBuff)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnNewBuff) + " ( " + nameof(newBuff) + ": " + newBuff + " , " + nameof(nextBuff) + ": " + nextBuff + " )", this);
        }

        activeBuffText.text = "active: " + newBuff;
        nextBuffText.text = "next: " + nextBuff;
    }
}
