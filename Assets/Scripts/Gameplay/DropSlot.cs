using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections; // Necessário para usar Coroutines (FlashColor)

public class DropSlot : MonoBehaviour, IDropHandler
{
    [Header("UI References")]
    public TextMeshProUGUI definitionText;
    public Image backgroundImage;
    public GameObject correctIcon;

    [Header("Drop Configuration")]
    [Tooltip("A área exata (RectTransform) onde o item arrastado deve se encaixar. Geralmente, é a própria imagem de fundo.")]
    public RectTransform dropArea;

    // Propriedades internas
    public string CorrectTerm { get; private set; }
    private bool _isOccupied = false;

    /// <summary>
    /// Configura o slot com uma definição e o termo correto correspondente.
    /// </summary>
    public void Setup(string definition, string correctTerm)
    {
        definitionText.text = definition;
        CorrectTerm = correctTerm;
        correctIcon.SetActive(false);
        _isOccupied = false;
    }
    
    /// <summary>
    /// Chamado pela Unity quando um DraggableItem é solto sobre este slot.
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        if (_isOccupied) return; // Se já estiver ocupado, não faz nada

        DraggableItem droppedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        if (droppedItem != null)
        {
            // Verifica se o termo do item solto corresponde ao termo correto para este slot
            if (droppedItem.MyWordData.termo == CorrectTerm)
            {
                HandleCorrectDrop(droppedItem);
            }
            else
            {
                HandleIncorrectDrop(droppedItem);
            }
        }
    }

    /// <summary>
    /// Lida com a lógica de um acerto.
    /// </summary>
    private void HandleCorrectDrop(DraggableItem item)
    {
        _isOccupied = true;

        // Passa a área de encaixe específica para o item saber onde se alinhar.
        // Se dropArea não foi definida, usa o próprio transform do slot como fallback.
        RectTransform targetArea = dropArea != null ? dropArea : this.GetComponent<RectTransform>();
        item.LockInPlace(targetArea);

        backgroundImage.color = Color.green;
        correctIcon.SetActive(true);

        EventManager.OnCorrectMatch?.Invoke(item, this);
    }

    /// <summary>
    /// Lida com a lógica de um erro.
    /// </summary>
    private void HandleIncorrectDrop(DraggableItem item)
    {
        // O item voltará para a posição original por conta própria (em seu OnEndDrag).
        // Apenas sinalizamos o erro visualmente e disparamos o evento.
        StartCoroutine(FlashColor(Color.red));
        EventManager.OnIncorrectMatch?.Invoke(item);
    }

    /// <summary>
    /// Pisca uma cor para dar feedback visual de erro.
    /// </summary>
    private IEnumerator FlashColor(Color flashColor)
    {
        Color originalColor = backgroundImage.color;
        backgroundImage.color = flashColor;
        yield return new WaitForSeconds(0.5f);
        backgroundImage.color = originalColor;
    }
}