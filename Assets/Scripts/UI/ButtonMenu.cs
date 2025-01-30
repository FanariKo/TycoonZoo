using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMenu : MonoBehaviour
{
    [Header("Menu References")]
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject builds;
    [SerializeField] private AudioSource audioSource;

    public void OpenMenu()
    {
        SetMenuState(false, true);
    }

    public void CloseMenu()
    {
        SetMenuState(true, false);
    }

    public void OpenBuilds()
    {
        audioSource.Play();
        menu.SetActive(false);
        builds.SetActive(true);
    }

    public void CloseBuilds()
    {
        audioSource.Play();
        builds.SetActive(false);
        menu.SetActive(true);
    }

    private void SetMenuState(bool buttonState, bool menuState)
    {
        audioSource.Play();
        gameObject.SetActive(buttonState);
        menu.SetActive(menuState);
    }
}
