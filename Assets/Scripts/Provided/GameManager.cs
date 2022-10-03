using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int player1Score = 0;
    private int player2Score = 0;

    [SerializeField] BallController[] balls;
    [SerializeField] PaddleController[] paddles;

    [SerializeField] bool useFirstGetToScoreGoal = false;
    [SerializeField] int scoreToReach = 10;
    [SerializeField] bool useTimerLimit = false;
    [SerializeField] float timeLimitInSec = 120;
    private float timeRemaining = -1;
    private float TimeRemaining => timeRemaining;

    public UnityEvent_Int OnPlayerOneGoalTouched = new();
    public UnityEvent_Int OnPlayerTwoGoalTouched = new();
    public UnityEvent_PlayerWin OnGameFinished = new();

    private void Start()
    {
        timeRemaining = timeLimitInSec;

        // Get references to ball and paddles if not already assigned
        if (balls == null || balls.Length == 0)
        {
            balls = FindObjectsOfType<BallController>();
        }
        if (paddles == null || paddles.Length == 0)
        {
            paddles = FindObjectsOfType<PaddleController>();
        }
    }

    public void Reset()
    {
        foreach (BallController ball in balls)
        {
            ball.Reset();
        }
        foreach (PaddleController paddle in paddles)
        {
            paddle.Reset();
        }
    }

    private void Update()
    {
        if (useTimerLimit)
        {
            if (timeRemaining <= 0)
            {
                DetermineWinner();
            }
            else
            {
                timeRemaining -= Time.deltaTime;
            }
        }
    }

    public void PlayerScorePoint(bool isPlayerOne)
    {
        if (isPlayerOne)
        {
            player1Score += 1;
            OnPlayerOneGoalTouched.Invoke(player1Score);
        }
        else
        {
            player2Score += 1;
            OnPlayerTwoGoalTouched.Invoke(player2Score);
        }

        if (useFirstGetToScoreGoal && (player1Score >= scoreToReach || player2Score >= scoreToReach))
        {
            DetermineWinner();
        }
        else
        {
            Reset();
        }

    }

    public void DetermineWinner()
    {
        if (player1Score > player2Score)
        {
            OnGameFinished.Invoke(PlayerWin.Player_1);
        }
        else if (player1Score < player2Score)
        {
            OnGameFinished.Invoke(PlayerWin.Player_2);
        }
        else if (player1Score == player2Score)
        {
            OnGameFinished.Invoke(PlayerWin.Tie);
        }

    }

}

public enum PlayerWin
{
    Player_1,
    Player_2,
    Tie,
}

