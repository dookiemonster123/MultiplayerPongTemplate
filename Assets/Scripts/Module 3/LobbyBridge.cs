using Netcode.Transports.Facepunch;
using Steamworks;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyBridge : MonoBehaviour
{
    [Header("UI Objets")]
    [SerializeField] Button hostButton;
    [SerializeField] Button joinByIdButton;
    [SerializeField] Button pasteIdButton;
    [SerializeField] TMP_InputField steamIdInput;
    [SerializeField] Transform friendListContentParent;

    [Header("Prefabs")]
    [SerializeField] GameObject friendJoinButtonPrefab;

    [Header("External References")]
    // Does not need to be assigned
    [SerializeField] FacepunchTransport fpTransport;
    [SerializeField] NetworkUI networkUI;

    void Start()
    {
        StartCoroutine(InitializeLobbyNetwork());
    }

    IEnumerator InitializeLobbyNetwork()
    {
        // Hide host and join buttons
        hostButton.gameObject.SetActive(false);
        joinByIdButton.gameObject.SetActive(false);
        // Wait for Network Manager and Steam Client
        while (NetworkManager.Singleton == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        while (!SteamClient.IsValid)
        {
            yield return new WaitForSeconds(0.1f);
        }
        // Both are now initialized
        GenerateFriendJoinList();
        hostButton.gameObject.SetActive(true);
        joinByIdButton.gameObject.SetActive(true);
    }

    // Column #1
    public void HostGame()
    {
        // Just host the game, Steam transport will figure out what to do when host begins 
        networkUI.HostGame();
    }

    // Column #2 
    public void GenerateFriendJoinList()
    {
        foreach (Friend friend in SteamFriends.GetFriends())
        {
            // Skip this friend if they are not playing this game
            if (!friend.IsPlayingThisGame) continue;

            // Create friend join button and initialize it
            GameObject friendJoinObj = Instantiate(friendJoinButtonPrefab, friendListContentParent);
            if (friendJoinObj.GetComponent<SteamFriendJoin>() is SteamFriendJoin steamFriend)
            {
                steamFriend.InitializeJoinButton(friend);
            }
        }
    }

    // Column #3 
    public void JoinGameById()
    {
        if (!fpTransport)
        {
            Debug.LogError("Steam Transport not assigned!");
            return;
        }

        var originalTarget = fpTransport.targetSteamId;
        if (!ulong.TryParse(steamIdInput.text, out fpTransport.targetSteamId))
        {
            fpTransport.targetSteamId = originalTarget;
            Debug.LogError("Could not parse Steam ID from input!");
            return;
        }
        networkUI.JoinGame();
    }


    public void PasteSteamIdFromClipboard()
    {
        if (!steamIdInput)
        {
            Debug.LogError("No Input Field for inputting server ID found!");
            return;
        }
        TextEditor textEditor = new()
        {
            multiline = true
        };
        textEditor.Paste();
        steamIdInput.text = textEditor.text;
    }

    private void OnEnable()
    {
        hostButton.onClick.AddListener(HostGame);
        joinByIdButton.onClick.AddListener(JoinGameById);
        pasteIdButton.onClick.AddListener(PasteSteamIdFromClipboard);
    }

    private void OnDisable()
    {
        hostButton.onClick.RemoveListener(HostGame);
        joinByIdButton.onClick.RemoveListener(JoinGameById);
        pasteIdButton.onClick.RemoveListener(PasteSteamIdFromClipboard);
    }
}
