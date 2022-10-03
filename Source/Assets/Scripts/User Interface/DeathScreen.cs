using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class DeathScreen : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        Player.OnDie += ShowDeathScreen;
    }

    private void OnDestroy()
    {
        Player.OnDie -= ShowDeathScreen;
    }

    private void ShowDeathScreen()
    {
        PlayerManager.CurrentState = PlayerStates.Wait;
        canvasGroup.DOFade(1f, 3f);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    public void Retry()
    {
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
