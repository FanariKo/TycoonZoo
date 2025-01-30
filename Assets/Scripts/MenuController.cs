using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private AudioSource click;
    private void Start()
    {
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.LogError("Play Button не назначен в MenuController");
        }
    }

    public void StartGame()
    {
        click.Play();
        SceneManager.LoadScene("Game");
    }
}
