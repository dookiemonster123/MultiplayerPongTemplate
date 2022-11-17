using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    public static void PauseGameUpdate()
    {
        Time.timeScale = 0;
    }

    public static void ResumeGameUpdate()
    {
        Time.timeScale = 1;
    }

    public static void ReloadCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public static void LoadLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public static void QuitGame()
    {
        // If you are running this script from the Unity Editor,
        // stop the editor to simulate quitting
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
