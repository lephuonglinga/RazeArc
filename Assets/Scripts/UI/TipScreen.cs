using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TipScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject tipCanvas;
    [SerializeField] private GameObject inGameHud;

    [Header("Auto Show")]
    [SerializeField] private bool autoShowOnStart = true;
    [SerializeField] private string autoShowSceneName = "Lv2";

    void Start()
    {
        if (!autoShowOnStart)
        {
            return;
        }

        if (SceneManager.GetActiveScene().name == autoShowSceneName)
        {
            DisplayTip();
        }
    }

    public void DisplayTip()
    {
        if (tipCanvas != null)
        {
            tipCanvas.SetActive(true);
        }

        if (inGameHud != null)
        {
            inGameHud.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideTip()
    {
        if (tipCanvas != null)
        {
            tipCanvas.SetActive(false);
        }

        if (inGameHud != null)
        {
            inGameHud.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Optional wrappers if UI events are hooked with lowercase names.
    public void displayTip()
    {
        DisplayTip();
    }

    public void hideTip()
    {
        HideTip();
    }
}
