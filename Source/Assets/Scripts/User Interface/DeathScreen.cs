using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private int currentScore = 0;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        Enemy.OnEnemyDied += IncreaseScore;
        Player.OnDie += ShowDeathScreen;
    }

    private void OnDestroy()
    {
        Player.OnDie -= ShowDeathScreen;
        Enemy.OnEnemyDied -= IncreaseScore;
    }

    private void ShowDeathScreen()
    {
        PlayerManager.CurrentState = PlayerStates.Wait;
        canvasGroup.DOFade(1f, 3f);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (scoreText != null)
        {
            scoreText.text ="Score: " + currentScore;
            currentScore = 0;
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void IncreaseScore(Enemy enemy)
    {
        currentScore += 10;
    }
}
