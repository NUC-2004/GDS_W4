using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    public int player1Health = 100;
    public int player2Health = 100;

    public float arenaLimitX = 5f;
    public float arenaLimitY = 5f;

    public GameObject gameOverPanel;

    private bool gameEnded = false;

    void Update()
    {
        if (gameEnded) return;

        CheckRingOut();
        CheckHealthWin();
    }

    void CheckRingOut()
    {
        if (Mathf.Abs(player1.position.x) > arenaLimitX || Mathf.Abs(player1.position.y) > arenaLimitY)
        {
            EndGame("Player 2 Wins! (Ring-out)");
        }

        if (Mathf.Abs(player2.position.x) > arenaLimitX || Mathf.Abs(player2.position.y) > arenaLimitY)
        {
            EndGame("Player 1 Wins! (Ring-out)");
        }
    }

    void CheckHealthWin()
    {
        if (player1Health <= 0)
        {
            EndGame("Player 2 Wins! (KO)");
        }

        if (player2Health <= 0)
        {
            EndGame("Player 1 Wins! (KO)");
        }
    }

    public void EndGame(string result)
    {
        if (gameEnded) return;

        gameEnded = true;
        Debug.Log(result);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            TMPro.TextMeshProUGUI resultText = gameOverPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (resultText != null)
            {
                resultText.text = result;
            }
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
