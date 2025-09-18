using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum CardFinish
{
    Matte,
    Holographic,
    Foil,
    Inverse,
    Void,
}

public class Card : MonoBehaviour,
    IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler,
    IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] public CardID ID;
    [SerializeField] public CardFinish finish;

    [Header("References")]
    [SerializeField] public Selectable cardLogic;
    [SerializeField] public SpriteRenderer cardFace;
    [SerializeField] public GameManager gameManager;
    [HideInInspector] public Transform parentSlot;
    private Canvas cardCanvas;

    [Header("Visual")]
    [SerializeField] public Sprite cardSprite;

    [Header("Movement")]
    public float moveSpeed;
    [HideInInspector] public Vector3 dragTargetPos;
    private Vector3 offset;

    [Header("States")]
    public bool isHovering;
    public bool isSelected;
    public bool isDragging;

    [Header("Events")]
    [HideInInspector] public UnityEvent<Card, bool> PointerEnterEvent;
    [HideInInspector] public UnityEvent<Card> PointerExitEvent;
    [HideInInspector] public UnityEvent<Card> PointerUpEvent;
    [HideInInspector] public UnityEvent<Card> PointerDownEvent;
    [HideInInspector] public UnityEvent<Card> BeginDragEvent;
    [HideInInspector] public UnityEvent<Card> EndDragEvent;
    [HideInInspector] public UnityEvent<Card, bool> SelectEvent;

    [Header("FinishMaterials")]
    public Material normalMat;
    public Material holoMat;
    public Material foilMat;
    public Material inverseMat;
    public Material voidMat;

    public void SetupCard(CardID cardID, CardFinish cardFinish)
    {
        // Assign the ID and finish
        ID = cardID;
        finish = cardFinish;

        // Apply artwork and material
        ApplyFinish(cardID);
    }

    private void Start()
    {
        moveSpeed = gameManager.cardSpeed;
        cardCanvas = GetComponentInParent<Canvas>();
        dragTargetPos = transform.position; // target start at card's relative starting position
        ApplyFinish(ID);
    }

    public void ApplyFinish(CardID cardID)
    {
        switch (finish)
        {
            case CardFinish.Matte:
                cardFace.material = normalMat;
                break;
            case CardFinish.Holographic:
                cardFace.material = holoMat;
                break;
            case CardFinish.Foil:
                cardFace.material = foilMat;
                break;
            case CardFinish.Inverse:
                cardFace.material = inverseMat;
                break;
            case CardFinish.Void:
                cardFace.material = voidMat;
                break;
        }

        cardFace.sprite = cardID.artwork;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, dragTargetPos, Time.deltaTime * moveSpeed);
    }

    private void FixedUpdate()
    {
        if (isDragging)
        {
            Vector3 dir = dragTargetPos - transform.position;

            float tiltZ = Mathf.Clamp(dir.x * 5f, -15f, 15f);
            transform.rotation = Quaternion.Euler(0, 0, tiltZ);
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        }
    }

    #region DRAG_EVENTS

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        // Calculate offset to avoid snapping center to mouse
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            cardCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out worldPos
        );
        offset = transform.position - worldPos;

        BeginDragEvent?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Update target position to follow mouse when dragged
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            cardCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out worldPos
        );

        dragTargetPos = worldPos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        EndDragEvent?.Invoke(this);

        // Placeholder for snapping back / reordering logic
        // dragTargetPos = grid/array thing idk
    }

    #endregion

    #region POINTER_EVENTS

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        PointerEnterEvent?.Invoke(this, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        PointerExitEvent?.Invoke(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerUpEvent?.Invoke(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDownEvent?.Invoke(this);
    }

    #endregion
}
