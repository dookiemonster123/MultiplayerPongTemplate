using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkGameManager : NetworkBehaviour
{
    public const int MIN_PLAYERS_REQUIRED = 2;
    [SerializeField] NetworkVariable<int> playerOneScore = new NetworkVariable<int>(0);
    [SerializeField] NetworkVariable<int> playerTwoScore = new NetworkVariable<int>(0);
    [SerializeField] TextUpdater playerOneText;
    [SerializeField] TextUpdater playerTwoText;

    [SerializeField] NetworkBall networkedBallPrefab;
    [SerializeField] NetworkPlayer networkedPaddlePrefab;

    [SerializeField] Vector3[] playerStartPositions = new Vector3[MIN_PLAYERS_REQUIRED];
    [SerializeField] Transform BallPosition;

    private int currentNumberOfPlayers = 0;
    private NetworkBall ball = null;
    private NetworkPlayer[] playerPaddles = new NetworkPlayer[MIN_PLAYERS_REQUIRED];

    public void Reset(){
        if (!IsServer) return;

        if (ball) ball.Reset();
        foreach (var paddle in playerPaddles) {
            if (paddle) paddle.Reset();
        }
    }

    public void ResetAndLaunchBall(bool launchRight){
        Reset();
        if (ball) ball.LaunchBall(launchRight);
    }

    public void PlayerScored(int playerNumber) {
        if (playerNumber <= 1) {
            playerOneScore.Value++;
        }
        else if (playerNumber >= 2) {
            playerTwoScore.Value++;
        }
    }

       public void NewPlayerConnected(ulong playerID)
   {
       currentNumberOfPlayers++;
        Debug.Log("New player connected" + currentNumberOfPlayers);
       if (IsServer)
       {
           if (currentNumberOfPlayers == 1)
           {
               playerOneScore.Value = 0;
               playerTwoScore.Value = 0;

               var newPaddle = Instantiate(networkedPaddlePrefab);
               newPaddle.UpdateStartPosition(playerStartPositions[0]);
               newPaddle.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerID);
               playerPaddles[0] = newPaddle;
           }
           else if (currentNumberOfPlayers == MIN_PLAYERS_REQUIRED)
           {
               int playerIndex = MIN_PLAYERS_REQUIRED - 1;

               ball = Instantiate(networkedBallPrefab, BallPosition);
               var newPaddle = Instantiate(networkedPaddlePrefab);
               newPaddle.UpdateStartPosition(playerStartPositions[playerIndex]);

               ball.GetComponent<NetworkObject>().Spawn();
               newPaddle.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerID);
               playerPaddles[playerIndex] = newPaddle;

               ResetAndLaunchBall(true);
           }
       }
   }

    public void PlayerDisconnected(ulong playerID) {
        currentNumberOfPlayers--;

        if (IsServer) {
            if (currentNumberOfPlayers < MIN_PLAYERS_REQUIRED) {
                playerOneScore.Value = 0;
                playerTwoScore.Value = 0;
                if (ball) ball.GetComponent<NetworkObject>().Despawn(true);
            }
        }
   }

   [ClientRpc]
   public void UpdatePlayerOneTextClientRpc(int oldScore, int currentScore) {
        if (playerOneText) playerOneText.ShowInt(currentScore);
        ResetAndLaunchBall(false);
   }

   [ClientRpc]
   public void UpdatePlayerTwoTextClientRpc(int oldScore, int currentScore) {
        if (playerTwoText) playerTwoText.ShowInt(currentScore);
        ResetAndLaunchBall(true);
   }

   public override void OnNetworkSpawn() {
        if (!IsServer) return;

        playerOneScore.OnValueChanged += UpdatePlayerOneTextClientRpc;
        playerTwoScore.OnValueChanged += UpdatePlayerTwoTextClientRpc;

        if (NetworkManager.Singleton) {
            NetworkManager.Singleton.OnClientConnectedCallback += NewPlayerConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += NewPlayerConnected;
        }
        NewPlayerConnected(OwnerClientId);
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn() {
        if (!IsServer) return;

        playerOneScore.OnValueChanged -= UpdatePlayerOneTextClientRpc;
        playerTwoScore.OnValueChanged -= UpdatePlayerOneTextClientRpc;

        if (NetworkManager.Singleton) {
            NetworkManager.Singleton.OnClientConnectedCallback -= NewPlayerConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= PlayerDisconnected;
            }
        base.OnNetworkDespawn();
        }
    }