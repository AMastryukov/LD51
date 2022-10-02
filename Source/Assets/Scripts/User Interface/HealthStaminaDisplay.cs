using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthStaminaDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image staminaFill;

    private void Awake()
    {
        // TODO: Subscribe to Player.OnHealthChanged to UpdateHealth
        // TODO: Subscribe to Player.OnStaminaChanged to UpdateStamina
    }

    private void OnDestroy()
    {
        // TODO: Unsubscribe from Player.OnHealthChanged to UpdateHealth
        // TODO: Unsubscribe from Player.OnStaminaChanged to UpdateStamina
    }

    private void UpdateHealth(int health)
    {
        healthText.text = health.ToString();
    }

    private void UpdateStamina(float stamina)
    {
        var fillValue = stamina / 100f;
        staminaFill.fillAmount = fillValue;
    }
}
