using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI References")]
    public TextMeshProUGUI termText;
    public Image backgroundImage;

    // Propriedade para guardar os dados da palavra
    public WordData MyWordData { get; private set; }

    // Variáveis para controle do arrasto
    private Vector3 _startPosition;
    private Transform _startParent;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;

    void Awake()
    {
        // Pega as referências necessárias no início para otimização
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Configura o item com os dados de uma palavra específica.
    /// Chamado pelo GameManager ao criar os itens.
    /// </summary>
    public void Setup(WordData wordData)
    {
        MyWordData = wordData;
        termText.text = wordData.termo;
    }

    /// <summary>
    /// Chamado quando o arrasto começa.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Salva a posição e o pai original para poder retornar em caso de erro
        _startPosition = _rectTransform.position;
        _startParent = transform.parent;

        // Move o item para a raiz do canvas para que fique por cima de todos os outros UIs
        transform.SetParent(transform.root);
        
        // Desativa o 'blocksRaycasts' para que o evento de 'drop' seja detectado no slot que está abaixo do item
        _canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Chamado continuamente enquanto o item é arrastado.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        // Move o item seguindo a posição do mouse/dedo
        _rectTransform.position = eventData.position;
    }

    /// <summary>
    /// Chamado quando o arrasto termina (o botão do mouse é solto).
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        // Reativa o 'blocksRaycasts' para que o item possa ser arrastado novamente se necessário
        _canvasGroup.blocksRaycasts = true;

        // Se o pai ainda for a raiz do canvas, significa que não foi solto em um slot válido.
        if (transform.parent == transform.root)
        {
            ResetPosition();
        }
    }

    /// <summary>
    /// Trava o item no lugar correto após um acerto.
    /// Este método foi atualizado para se esticar e preencher a área do novo pai.
    /// </summary>
    /// <param name="newParentRect">A área (RectTransform) onde o item deve se encaixar.</param>
    public void LockInPlace(RectTransform newParentRect)
    {
        // Define o novo pai
        transform.SetParent(newParentRect);

        // Garante que o item não possa mais ser arrastado
        _canvasGroup.blocksRaycasts = false;
        
        // A mágica do encaixe perfeito: força o item a preencher o novo pai.
        _rectTransform.anchorMin = Vector2.zero; // Âncora inferior esquerda
        _rectTransform.anchorMax = Vector2.one;  // Âncora superior direita
        _rectTransform.offsetMin = Vector2.zero; // Distância zero das bordas
        _rectTransform.offsetMax = Vector2.zero; // Distância zero das bordas
    }

    /// <summary>
    /// Retorna o item à sua posição original em caso de erro.
    /// </summary>
    public void ResetPosition()
    {
        transform.SetParent(_startParent);
        _rectTransform.position = _startPosition;
    }
}