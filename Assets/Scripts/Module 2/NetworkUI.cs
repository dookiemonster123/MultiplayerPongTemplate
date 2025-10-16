using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkUI : MonoBehaviour
{
    [Header("User Interface")]
    [SerializeField] GameObject[] outOfGameUI = new GameObject[0];
    [SerializeField] GameObject[] inGameUI = new GameObject[0];

    public void SetActiveInGameUI(bool isInGameActive)
{
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

    void Start()
    {
        SetActiveInGameUI(false);
    }

    void Update()
    {
        
    }

    public void HostGame(){
        bool hostSuccess = NetworkManager.Singleton.StartHost();
        if (hostSuccess){
            SetActiveInGameUI(true);
        }
        else {
            SetActiveInGameUI(false);
        }
    }

    public void JoinGame(){
        bool joinSuccess = NetworkManager.Singleton.StartClient();
        if (joinSuccess){
            SetActiveInGameUI(true);
        }
        else {
            SetActiveInGameUI(false);
        }
    }

    public void StopGame(){
        NetworkManager.Singleton.Shutdown();
        SetActiveInGameUI(false);
    }

    public void LeaveToMenu(){
        MenuActions.LoadLevel(0);
        Destroy(NetworkManager.Singleton.gameObject);
    }
}
