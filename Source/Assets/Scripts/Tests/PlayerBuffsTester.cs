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

    private void OnNewBuff(BuffData newBuff, BuffData nextBuff)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnNewBuff) + " ( " + nameof(newBuff.Buff) + ": " + newBuff.Buff + " , " + nameof(nextBuff.Buff) + ": " + nextBuff.Buff + " )", this);
        }

        activeBuffText.text = "active: " + newBuff.Buff;
        nextBuffText.text = "next: " + nextBuff.Buff;
    }
}
