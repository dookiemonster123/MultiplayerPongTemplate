using Netcode.Transports.Facepunch;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SteamFriendJoin : MonoBehaviour
{
    [SerializeField] Button joinButton; // Button clicked to join
    [SerializeField] ulong friendSteamId = 0; // The ID of your friend
    [SerializeField] string friendName = ""; // The name of your friend
    [Header("External References")]
    [SerializeField] FacepunchTransport transport; // The Steam transport
    [SerializeField] NetworkUI networkUI; // The network UI that handles joining

    // Initialize some items at the start of the game object's life
    private void Start()
    {
        if (joinButton == null)
        {
            joinButton = GetComponent<Button>();
        }
        if (transport == null
        && FindObjectOfType<FacepunchTransport>() is FacepunchTransport facepunch)
        {
            transport = facepunch;
        }
        if (networkUI == null
        && FindObjectOfType<NetworkUI>() is NetworkUI netUI)
        {
            networkUI = netUI;
        }
    }

    // Another public initializer to provide Steam information 
    public void InitializeJoinButton(Friend friend)
    {
        friendSteamId = friend.Id.Value;
        friendName = friend.Name;
        if (GetComponentInChildren<TMP_Text>() is TMP_Text joinText)
        {
            joinText.text = $"Try joining {friendName}'s game";
        }
    }

    public void TryJoin()
    {
        // Error handling code
        if (friendSteamId == 0)
        {
            Debug.LogError("Steam Id is invalid!");
            return;
        }
        if (networkUI == null)
        {
            Debug.LogError("Network UI is not valid!");
            return;
        }
        if (transport == null)
        {
            Debug.LogError("Transport not provided!");
            return;
        }

        // Track original server target in case of failure
        var originalTarget = transport.targetSteamId;
        transport.targetSteamId = friendSteamId;

        networkUI.JoinGame();
    }

    private void OnEnable()
    {
        // Start listening on activation if the button is available
        if (joinButton) joinButton.onClick.AddListener(TryJoin);
    }

    private void OnDisable()
    {
        // Stop listening on deactivation if the button is available
        if (joinButton) joinButton.onClick.RemoveListener(TryJoin);
    }
}
