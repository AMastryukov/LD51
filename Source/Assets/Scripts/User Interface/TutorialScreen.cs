using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScreen : MonoBehaviour
{
    [SerializeField] private Canvas[] tutorialPages;
    [SerializeField] private bool showTutorial = true;

    private CanvasGroup _canvasGroup;
    private PlayerInputHandler _playerInputHandler;
    private AudioSource _mainMenuTheme; // don't ask why it's here, no time

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _mainMenuTheme = GetComponent<AudioSource>();
        _playerInputHandler = FindObjectOfType<PlayerInputHandler>();

        if (showTutorial)
        {
            OpenTutorial();
            return;
        }

        _mainMenuTheme.Play();
    }

    public void OpenTutorial()
    {
        Cursor.visible = true;
        _playerInputHandler.enabled = false;

        Time.timeScale = 0f;

        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

        OpenPage(0);
    }

    public void CloseTutorial()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        Time.timeScale = 1f;

        _mainMenuTheme.Play();

        Cursor.visible = false;

        _playerInputHandler.enabled = true;
    }

    public void OpenPage(int page)
    {
        if (page >= tutorialPages.Length) return;

        CloseAll();
        tutorialPages[page].enabled = true;
    }

    private void CloseAll()
    {
        foreach (var page in tutorialPages)
        {
            page.enabled = false;
        }
    }
}
