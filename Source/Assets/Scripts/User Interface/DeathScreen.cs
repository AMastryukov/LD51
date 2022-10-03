using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class DeathScreen : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private PauseMenu pauseMenu;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        pauseMenu = FindObjectOfType<PauseMenu>();
    }

    public void ShowDeathScreen()
    {
        // We don't want to show the pause menu if we died ever again
        Destroy(pauseMenu.gameObject);

        canvasGroup.DOFade(1f, 3f);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
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
