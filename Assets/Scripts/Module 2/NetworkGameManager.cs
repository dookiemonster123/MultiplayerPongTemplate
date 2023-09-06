using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkGameManager : NetworkBehaviour
{
    // Part 1: Public and SerializeField

    // Part 1a: Fields that deal with the game
    public const int MIN_PLAYERS_REQUIRED = 2; // Players required to start the game
    [SerializeField] NetworkVariable<int> playerOneScore = new NetworkVariable<int>(0);
    [SerializeField] NetworkVariable<int> playerTwoScore = new NetworkVariable<int>(0);
    [SerializeField] TextUpdater playerOneText;
    [SerializeField] TextUpdater playerTwoText;

    // Part 1b: Fields for prefabs
    [SerializeField] NetworkBall networkedBallPrefab;
    [SerializeField] NetworkPlayer networkedPaddlePrefab;

    // Part 1c: Fields that configure the game
    [SerializeField] Vector3[] playerStartPositions = new Vector3[MIN_PLAYERS_REQUIRED];

    // Part 2: Private - Internal fields to keep track of the players and game pieces
    private int currentNumberOfPlayers = 0;
    private NetworkBall ball = null;
    private NetworkPlayer[] playerPaddles = new NetworkPlayer[MIN_PLAYERS_REQUIRED];

    // (Server-only) Reset the ball and all the paddle
    public void Reset()
    {
        if (!IsServer) return;

        // Read as if we have a ball (i.e. not null), reset the ball
        if (ball) ball.Reset();
        foreach (var paddle in playerPaddles)
        {
            if (paddle) paddle.Reset();
        }
    }

    // Reset and then launch the ball
    public void ResetAndLaunchBall(bool launchRight)
    {
        Reset();
        if (ball) ball.LaunchBall(launchRight);
    }

    // Change the Network Variable score for a specific player
    public void PlayerScored(int playerNumber)
    {
        if (playerNumber <= 1)
        {
            playerOneScore.Value++;
        }
        else if (playerNumber >= 2)
        {
            playerTwoScore.Value++;
        }
    }

    public void NewPlayerConnected(ulong playerID)
    {
        Debug.LogWarning($"{playerID} joined!");
        currentNumberOfPlayers++;

        if (IsServer)
        {
            // If we have one player
            if (currentNumberOfPlayers == 1)
            {
                // Reset score
                playerOneScore.Value = 0;
                playerTwoScore.Value = 0;

                // Create and spawn a paddle for player 1
                var newPaddle = Instantiate(networkedPaddlePrefab);
                newPaddle.UpdateStartPosition(playerStartPositions[0]);
                newPaddle.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerID);
                playerPaddles[0] = newPaddle;
            }
            // If we have two players
            else if (currentNumberOfPlayers == MIN_PLAYERS_REQUIRED)
            {
                int playerIndex = MIN_PLAYERS_REQUIRED - 1;

                // Create the ball and another paddle
                ball = Instantiate(networkedBallPrefab);
                var newPaddle = Instantiate(networkedPaddlePrefab);
                newPaddle.UpdateStartPosition(playerStartPositions[playerIndex]);

                // Spawn ball with server as owner, 
                // and paddle with other player as owner
                ball.GetComponent<NetworkObject>().Spawn();
                newPaddle.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerID);
                playerPaddles[playerIndex] = newPaddle;

                // Reset everything and launch the ball.
                ResetAndLaunchBall(true);
            }
            // NOTE: If you change MIN_PLAYERS_REQUIRED to a number not equal to 2,
            //       there may be issues playing the game. Consider carefully what the
            //       code is doing before changing MIN_PLAYERS_REQUIRED.
        }
    }

    public void PlayerDisconnected(ulong playerID)
    {
        Debug.LogWarning($"{playerID} left!");
        currentNumberOfPlayers--;

        if (IsServer)
        {
            // If number of players falls below the required number for the game
            if (currentNumberOfPlayers < MIN_PLAYERS_REQUIRED)
            {
                // Reset score
                playerOneScore.Value = 0;
                playerTwoScore.Value = 0;
                // Despawn (and destroy) the ball
                if (ball) ball.GetComponent<NetworkObject>().Despawn(true);

                // Paddles remain if another player joins back in.
            }
        }
    }

    [ClientRpc]
    public void UpdatePlayerOneTextClientRpc(int oldScore, int currentScore)
    {
        if (playerOneText) playerOneText.ShowInt(currentScore);
        ResetAndLaunchBall(false);
    }

    [ClientRpc]
    public void UpdatePlayerTwoTextClientRpc(int oldScore, int currentScore)
    {
        if (playerTwoText) playerTwoText.ShowInt(currentScore);
        ResetAndLaunchBall(true);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        // Start listening for score change
        playerOneScore.OnValueChanged += UpdatePlayerOneTextClientRpc;
        playerTwoScore.OnValueChanged += UpdatePlayerTwoTextClientRpc;
        // Start listening for connection/disconnections
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NewPlayerConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += PlayerDisconnected;
        }
        // Call connect for host
        // NewPlayerConnected(OwnerClientId);
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        // Stop listening for score change
        playerOneScore.OnValueChanged -= UpdatePlayerOneTextClientRpc;
        playerTwoScore.OnValueChanged -= UpdatePlayerTwoTextClientRpc;
        // Stop listening for connection/disconnections
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= NewPlayerConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= PlayerDisconnected;
        }
        // Call disconnect for host
        // PlayerDisconnected(OwnerClientId);
        base.OnNetworkDespawn();
    }
}
