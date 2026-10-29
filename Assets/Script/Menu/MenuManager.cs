
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.UI;

// Controls main menu buttons and keyboard selection.

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string standardSceneName;
    [SerializeField] private string unlimitedSceneName;

    [Header("Menu Navigation")]
    [SerializeField] private Button startButton;

    private void Start()
    {
        InputSystemUIInputModule inputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();

        if (inputModule != null)
            inputModule.deselectOnBackgroundClick = false;

        EventSystem.current.SetSelectedGameObject(startButton.gameObject);
    }

    public void StartGame()
    {
        GameManager.ResetData();
        Time.timeScale = 1f;
        SceneManager.LoadScene(standardSceneName);
    }

    public void StartUnlimited()
    {
        GameManager.ResetData();
        Time.timeScale = 1f;
        SceneManager.LoadScene(unlimitedSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
