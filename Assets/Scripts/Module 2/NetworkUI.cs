using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkUI : MonoBehaviour
{
    [Header("User Interface")]
    [SerializeField] GameObject[] outOfGameUI = new GameObject[0]; // used for UI outside of game (button holder)
    [SerializeField] GameObject[] inGameUI = new GameObject[0]; // used for UI in game (score and leave game)

    public void SetActiveInGameUI(bool isInGameActive)
    {
        // Out of Game UI follows the opposite of In Game UI
        bool isOutGameActive = !isInGameActive;

        foreach (GameObject inUI in inGameUI)
        {
            inUI.SetActive(isInGameActive);
        }
        foreach (GameObject outUI in outOfGameUI)
        {
            outUI.SetActive(isOutGameActive);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetActiveInGameUI(false);
    }

    public void HostGame()
    {
        // We will try to start hosing a game
        bool hostSuccess = NetworkManager.Singleton.StartHost();
        // If we succeed, show the in-game UI
        if (hostSuccess)
        {
            SetActiveInGameUI(true);
        }
        else
        // Otherwise, show out of game UI
        {
            SetActiveInGameUI(false);
        }
    }

    public void JoinGame()
    {
        // We will try to join a game
        bool joinSuccess = NetworkManager.Singleton.StartClient();
        // If we succeed, show the in-game UI
        if (joinSuccess)
        {
            SetActiveInGameUI(true);
        }
        else
        // Otherwise, show out of game UI
        {
            SetActiveInGameUI(false);
        }
    }

    public void StopGame()
    {
        // Shutdown game and show out of game UI
        NetworkManager.Singleton.Shutdown();
        SetActiveInGameUI(false);
    }

    public void LeaveToMenu()
    {
        // Switch to menu scene (level 0)
        MenuActions.LoadLevel(0);
        // Destory network manager gameObject
        Destroy(NetworkManager.Singleton.gameObject);
    }
}
