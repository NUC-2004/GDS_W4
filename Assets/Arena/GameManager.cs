using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Players")]
    public Transform player1;
    public Transform player2;

    [Header("Player Health")]
    public int player1Health = 100;
    public int player2Health = 100;

    [Header("Arena Bounds")]
    public float arenaLimitX = 5f;
    public float arenaLimitY = 5f;

    [Header("Timer")]
    public float matchTime = 60f;
    private float currentTime;

    [Header("UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI timerText;

    private bool gameEnded = false;

    void Start()
    {
        currentTime = matchTime;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        UpdateTimerUI();
    }

    void Update()
    {
        if (gameEnded) return;

        CheckRingOut();
        CheckHealthWin();
        UpdateTimer();
    }

    void CheckRingOut()
    {
        if (player1 == null || player2 == null) return;

        if (Mathf.Abs(player1.position.x) > arenaLimitX || Mathf.Abs(player1.position.y) > arenaLimitY)
        {
            EndGame("Player 2 Wins!\n(Ring-out)");
            return;
        }

        if (Mathf.Abs(player2.position.x) > arenaLimitX || Mathf.Abs(player2.position.y) > arenaLimitY)
        {
            EndGame("Player 1 Wins!\n(Ring-out)");
            return;
        }
    }

    void CheckHealthWin()
    {
        if (player1Health <= 0)
        {
            EndGame("Player 2 Wins!\n(KO)");
            return;
        }

        if (player2Health <= 0)
        {
            EndGame("Player 1 Wins!\n(KO)");
            return;
        }
    }

    void UpdateTimer()
    {
        currentTime -= Time.deltaTime;

        if (currentTime < 0f)
        {
            currentTime = 0f;
        }

        UpdateTimerUI();

        if (currentTime <= 0f)
        {
            CheckTimeWin();
        }
    }

    void CheckTimeWin()
    {
        if (player1Health > player2Health)
        {
            EndGame("Player 1 Wins!\n(Time Up)");
        }
        else if (player2Health > player1Health)
        {
            EndGame("Player 2 Wins!\n(Time Up)");
        }
        else
        {
            EndGame("Draw!\n(Time Up)");
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(currentTime).ToString();
        }
    }

    public void EndGame(string message)
    {
        if (gameEnded) return;

        gameEnded = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (resultText != null)
        {
            resultText.text = message;
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void SetPlayer1Health(int value)
    {
        player1Health = value;
    }

    public void SetPlayer2Health(int value)
    {
        player2Health = value;
    }
}