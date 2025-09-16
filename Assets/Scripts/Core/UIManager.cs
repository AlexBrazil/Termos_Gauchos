using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject endPanel;

    [Header("Start Panel")]
    public TMP_InputField nameInput;

    [Header("Game Panel")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;

    [Header("End Panel")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI endMessageText;

    void OnEnable()
    {
        EventManager.OnScoreUpdated += UpdateScore;
        EventManager.OnTimeUpdated += UpdateTime;
        EventManager.OnGameEnd += ShowEndPanel;
    }

    void OnDisable()
    {
        EventManager.OnScoreUpdated -= UpdateScore;
        EventManager.OnTimeUpdated -= UpdateTime;
        EventManager.OnGameEnd -= ShowEndPanel;
    }

    void Start()
    {
        startPanel.SetActive(true);
        gamePanel.SetActive(false);
        endPanel.SetActive(false);
    }
    
    public void OnClickStart()
    {
        string username = nameInput.text;
        if (string.IsNullOrEmpty(username))
        {
            username = "Jogador Anônimo";
        }
        PlayerPrefs.SetString("Username", username);
        
        startPanel.SetActive(false);
        gamePanel.SetActive(true);

        GameManager.Instance.StartGame();
    }

    private void UpdateScore(int newScore)
    {
        scoreText.text = "Pontos: " + newScore;
    }

    private void UpdateTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void ShowEndPanel()
    {
        gamePanel.SetActive(false);
        endPanel.SetActive(true);

        string username = PlayerPrefs.GetString("Username");
        int finalScore = PlayerPrefs.GetInt("HighScore"); // Mostra o recorde
        
        endMessageText.text = $"Parabéns, {username}!";
        finalScoreText.text = $"Sua melhor pontuação: {finalScore}";
    }

    public void OnClickRestart()
    {
        // Recarrega a cena atual
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}