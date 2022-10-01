using TMPro;
using UnityEngine;

public class GameManagerTester : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameStateText = null;

    [SerializeField] private TextMeshProUGUI secondText = null;
    [SerializeField] private GameObject tenSecondsPassedText = null;

    [SerializeField] private TextMeshProUGUI weaponText = null;
    [SerializeField] private TextMeshProUGUI trapText = null;
    [SerializeField] private TextMeshProUGUI bonusText = null;

    [SerializeField] private GameObject startGameButton = null;
    [SerializeField] private GameObject endGameButton = null;

    [SerializeField] private bool verboseLogging = false;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;

        GameManager.Instance.OnSecondPassed += OnSecond;
        GameManager.Instance.OnTenSecondsPassed += OnTenSecondsPassed;
        GameManager.Instance.OnTimerStopped += OnTimerStopped;

        GameManager.Instance.OnNewWeapon += OnNewWeapon;
        GameManager.Instance.OnNewTrap += OnNewTrap;
        GameManager.Instance.OnNewBonus += OnNewBonus;
    }

    private void OnGameStateChanged(GameStates gameState)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnGameStateChanged) + " ( " + nameof(gameState) + ": " + gameState + " )", this);
        }

        gameStateText.text = "Game State: " + gameState.ToString();

        weaponText.gameObject.SetActive(gameState == GameStates.Play);
        trapText.gameObject.SetActive(gameState == GameStates.Play);
        bonusText.gameObject.SetActive(gameState == GameStates.Play);

        startGameButton.SetActive(gameState != GameStates.Play);
        endGameButton.SetActive(gameState == GameStates.Play);

        if (gameState != GameStates.Play)
        {
            tenSecondsPassedText.SetActive(false);
        }
    }

    private void OnSecond(int second)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnSecond) + " ( " + nameof(second) + ": " + second + " )", this);
        }

        secondText.text = second.ToString();

        if (second <= 9)
        {
            tenSecondsPassedText.SetActive(false);
        }
    }

    private void OnTenSecondsPassed()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnTenSecondsPassed), this);
        }

        tenSecondsPassedText.SetActive(true);
    }

    private void OnTimerStopped()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnTimerStopped), this);
        }

        secondText.text = string.Empty;
    }

    private void OnNewWeapon(string weapon)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnNewWeapon) + " ( " + nameof(weapon) + ": " + weapon + " )", this);
        }

        weaponText.text = "Weapon: " + weapon;
    }

    private void OnNewTrap(string trap)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnNewTrap) + " ( " + nameof(trap) + ": " + trap + " )", this);
        }

        trapText.text = "Trap: " + trap;
    }

    private void OnNewBonus(Bonuses bonus)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnNewBonus) + " ( " + nameof(bonus) + ": " + bonus + " )", this);
        }

        bonusText.text = "Bonus: " + bonus;
    }
}
