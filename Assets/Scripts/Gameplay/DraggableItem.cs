using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TextMeshProUGUI termText;
    public Image backgroundImage;
    
    public WordData MyWordData { get; private set; }
    
    private Vector3 _startPosition;
    private Transform _startParent;
    private CanvasGroup _canvasGroup;

    void Awake()
    {
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Setup(WordData wordData)
    {
        MyWordData = wordData;
        termText.text = wordData.termo;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _startPosition = transform.position;
        _startParent = transform.parent;
        transform.SetParent(transform.root); // Para ficar por cima de tudo
        
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Se não foi solto em um slot válido, retorna à posição original
        if (transform.parent == transform.root)
        {
           ResetPosition();
        }
        _canvasGroup.blocksRaycasts = true;
    }

    public void LockInPlace(Transform newParent)
    {
        transform.SetParent(newParent);
        transform.localPosition = Vector3.zero;
        // Pode desabilitar este script ou o CanvasGroup para impedir novo drag
        _canvasGroup.blocksRaycasts = false;
    }

    public void ResetPosition()
    {
        // Adicionar uma animação aqui (ex: LeanTween, DoTween) seria ótimo
        transform.SetParent(_startParent);
        transform.position = _startPosition;
    }
}