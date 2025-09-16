using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public TextMeshProUGUI definitionText;
    public Image backgroundImage; // Para mudar a cor em acerto/erro
    public GameObject correctIcon; // Ícone de "check"
    
    public string CorrectTerm { get; private set; }
    private bool _isOccupied = false;

    public void Setup(string definition, string correctTerm)
    {
        definitionText.text = definition;
        CorrectTerm = correctTerm;
        correctIcon.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_isOccupied) return;

        DraggableItem droppedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        if (droppedItem != null)
        {
            // Verifica se o termo arrastado é o correto para esta definição
            if (droppedItem.MyWordData.termo == CorrectTerm)
            {
                // ACERTOU
                HandleCorrectDrop(droppedItem);
            }
            else
            {
                // ERROU
                HandleIncorrectDrop(droppedItem);
            }
        }
    }

    private void HandleCorrectDrop(DraggableItem item)
    {
        _isOccupied = true;
        item.LockInPlace(transform); // O item se encaixa visualmente
        backgroundImage.color = Color.green;
        correctIcon.SetActive(true);

        EventManager.OnCorrectMatch?.Invoke(item, this);
    }
    
    private void HandleIncorrectDrop(DraggableItem item)
    {
        // O item volta sozinho através do OnEndDrag
        // Sinalizar o erro visualmente de forma temporária
        StartCoroutine(FlashColor(Color.red));
        
        EventManager.OnIncorrectMatch?.Invoke(item);
    }

    private System.Collections.IEnumerator FlashColor(Color flashColor)
    {
        Color originalColor = backgroundImage.color;
        backgroundImage.color = flashColor;
        yield return new WaitForSeconds(0.5f);
        backgroundImage.color = originalColor;
    }
}