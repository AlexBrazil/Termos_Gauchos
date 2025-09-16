using UnityEngine;
// (remova o "using GauchoGame.Data;" se não estiver usando namespaces)

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public GameConfig Config { get; private set; }
    public WordData[] Words { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Carrega todos os dados do jogo dos arquivos locais na pasta Resources.
    /// Retorna 'true' se tudo foi carregado com sucesso, 'false' se houve algum erro.
    /// </summary>
    public bool LoadData()
    {
        bool configLoaded = LoadConfig();
        bool wordsLoaded = LoadWords();
        
        return configLoaded && wordsLoaded;
    }

    private bool LoadConfig()
    {
        TextAsset configFile = Resources.Load<TextAsset>("config");
        if (configFile != null)
        {
            Config = JsonUtility.FromJson<GameConfig>(configFile.text);
            Debug.Log("Configurações carregadas com sucesso.");
            return true;
        }
        else
        {
            Debug.LogError("ERRO: Arquivo 'config.json' não encontrado na pasta Assets/Resources!");
            return false;
        }
    }

    private bool LoadWords()
    {
        TextAsset wordsFile = Resources.Load<TextAsset>("palavras");
        if (wordsFile != null)
        {
            // O wrapper "{\"palavras\": ... }" ainda é necessário para o JsonUtility da Unity
            // ler um array que está na raiz do JSON.
            string jsonString = "{\"palavras\":" + wordsFile.text + "}";
            WordList wordList = JsonUtility.FromJson<WordList>(jsonString);
            Words = wordList.palavras.ToArray();
            Debug.Log($"Carregadas {Words.Length} palavras com sucesso.");
            return true;
        }
        else
        {
            Debug.LogError("ERRO: Arquivo 'palavras.json' não encontrado na pasta Assets/Resources!");
            return false;
        }
    }
}