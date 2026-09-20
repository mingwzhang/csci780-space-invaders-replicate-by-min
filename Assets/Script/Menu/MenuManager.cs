using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    void Update()
    {
        // Checks if ANY key on the keyboard was pressed down this frame
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            StartGame();
        }
    }

    void StartGame()
    {
        // Can be checked in FIle > Build Profiles > Scene List
        SceneManager.LoadScene(1);

        // Or 
        // SceneManager.LoadScene("Main");

        Debug.Log("Game Started!");
    }
}