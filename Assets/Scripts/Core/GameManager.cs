using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Referências para os containers de UI
    public Transform termsContainer;
    public Transform definitionsContainer;
    
    // Prefabs
    public GameObject termItemPrefab;
    public GameObject definitionSlotPrefab;

    private int _currentScore;
    private int _negativeScore;
    private float _gameTimeElapsed;
    private bool _isGameRunning;
    
    private int _currentRound = 0;
    private int _correctMatchesThisRound;
    private bool _errorMadeThisRound;

    private List<WordData> _fullWordList;
    private List<WordData> _wordsInPreviousRound = new List<WordData>();

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        EventManager.OnCorrectMatch += HandleCorrectMatch;
        EventManager.OnIncorrectMatch += HandleIncorrectMatch;
    }

    void OnDisable()
    {
        EventManager.OnCorrectMatch -= HandleCorrectMatch;
        EventManager.OnIncorrectMatch -= HandleIncorrectMatch;
    }
    
    void Update()
    {
        if (_isGameRunning)
        {
            _gameTimeElapsed += Time.deltaTime;
            EventManager.OnTimeUpdated?.Invoke(_gameTimeElapsed);

            if (_gameTimeElapsed * 1000 >= DataManager.Instance.Config.tempoDoJogoMs)
            {
                EndGame();
            }
        }
    }

    public void StartGame()
    {
        _currentScore = 0;
        _negativeScore = 0;
        _gameTimeElapsed = 0;
        _currentRound = 0;
        
        // Agora a chamada é síncrona (imediata)
        if (DataManager.Instance.LoadData())
        {
            _fullWordList = new List<WordData>(DataManager.Instance.Words);
            _isGameRunning = true;
            SetupNewRound();
        }
        else
        {
            // Se os dados não puderem ser carregados, o jogo não pode começar.
            // Aqui você poderia mostrar uma mensagem de erro na UI.
            Debug.LogError("Falha ao carregar os dados do jogo. O jogo não pode iniciar.");
        }
    }

    void SetupNewRound()
    {
        _currentRound++;
        _correctMatchesThisRound = 0;
        _errorMadeThisRound = false;

        // Limpar containers
        foreach (Transform child in termsContainer) Destroy(child.gameObject);
        foreach (Transform child in definitionsContainer) Destroy(child.gameObject);

        // 1. Selecionar 5 palavras aleatórias
        List<WordData> availableWords = _fullWordList.Except(_wordsInPreviousRound).ToList();
        List<WordData> roundWords = availableWords.OrderBy(x => Random.value).Take(5).ToList();
        _wordsInPreviousRound = new List<WordData>(roundWords);

        // 2. Preparar as 6 definições (5 corretas + 1 "confundir")
        List<string> definitions = roundWords.Select(w => w.descricao).ToList();
        
        // Pega uma palavra aleatória que NÃO está na rodada atual para a definição de "confundir"
        WordData confuseWord = _fullWordList.Except(roundWords).OrderBy(x => Random.value).First();
        definitions.Add(confuseWord.confundir);

        // Embaralhar as definições
        definitions = definitions.OrderBy(x => Random.value).ToList();

        // 3. Instanciar os objetos na UI
        foreach (var word in roundWords)
        {
            GameObject termObj = Instantiate(termItemPrefab, termsContainer);
            termObj.GetComponent<DraggableItem>().Setup(word);
        }

        foreach (var def in definitions)
        {
            GameObject slotObj = Instantiate(definitionSlotPrefab, definitionsContainer);
            // Encontrar a qual palavra esta definição pertence
            WordData correctWord = roundWords.FirstOrDefault(w => w.descricao == def);
            slotObj.GetComponent<DropSlot>().Setup(def, correctWord?.termo);
        }
    }

    private void HandleCorrectMatch(DraggableItem item, DropSlot slot)
    {
        _currentScore += DataManager.Instance.Config.pontosPorAcerto;
        EventManager.OnScoreUpdated?.Invoke(_currentScore);
        
        _correctMatchesThisRound++;
        if (_correctMatchesThisRound >= 5)
        {
            // Bônus por cena sem erros
            if (!_errorMadeThisRound)
            {
                _currentScore += DataManager.Instance.Config.bonusCenaSemErros;
                EventManager.OnScoreUpdated?.Invoke(_currentScore);
            }

            if (_currentRound >= DataManager.Instance.Config.quantidadeDeCenas)
            {
                EndGame();
            }
            else
            {
                // Atraso para o jogador ver o resultado antes de carregar a próxima
                Invoke(nameof(SetupNewRound), 1.5f);
            }
        }
    }

    private void HandleIncorrectMatch(DraggableItem item)
    {
        _errorMadeThisRound = true;
        _negativeScore += DataManager.Instance.Config.pontosPorErro; // pontosPorErro já deve ser negativo
        _gameTimeElapsed += DataManager.Instance.Config.penalidadeTempoPorErroMs / 1000f;
        
        // O score na tela não precisa mostrar o negativo, só o total.
        // Se quiser mostrar, crie outro evento.
    }
    
    private void EndGame()
    {
        if (!_isGameRunning) return;
        _isGameRunning = false;

        // Cálculo da pontuação final
        int timeBonus = 0;
        if (_gameTimeElapsed * 1000 < DataManager.Instance.Config.tempoDoJogoMs)
        {
            float timeLeft = (DataManager.Instance.Config.tempoDoJogoMs / 1000f) - _gameTimeElapsed;
            // Exemplo: 10 pontos por segundo restante. Ajuste conforme necessário.
            timeBonus = Mathf.FloorToInt(timeLeft * 10); 
        }

        int finalScore = _currentScore + _negativeScore + timeBonus;

        // Salvar pontuação no PlayerPrefs
        string username = PlayerPrefs.GetString("Username", "Jogador");
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (finalScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", finalScore);
        }

        EventManager.OnGameEnd?.Invoke();
        Debug.Log($"Fim de Jogo! Pontuação Final de {username}: {finalScore}");
        // UIManager irá mostrar a tela final com os dados
    }
}