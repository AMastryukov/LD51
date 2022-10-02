using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthStaminaDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image staminaFill;

    private void Awake()
    {
        Player.OnPlayerHealthChanged += UpdateHealth;
        Player.OnPlayerStaminaChanged += UpdateStamina;
    }

    private void OnDestroy()
    {
        Player.OnPlayerHealthChanged -= UpdateHealth;
        Player.OnPlayerStaminaChanged -= UpdateStamina;
    }

    private void UpdateHealth(int health)
    {
        healthText.text = health.ToString();
    }

    private void UpdateStamina(int stamina)
    {
        var fillValue = ((float)stamina) / 100f;
        staminaFill.fillAmount = fillValue;
    }
}
