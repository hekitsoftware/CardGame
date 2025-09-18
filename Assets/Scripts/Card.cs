using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public enum CardFinish
{
    Matte,
    Chroma,
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

    // Animation Stuff
    private Tween idleTween;

    private float rotationAmplitude = 5f;
    private float rotationSpeed = 1f;
    private float time;
    private float timeOffset;
    private Vector3 originalScale;

    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float tweenDuration = 0.2f;

    public void SetupCard(CardID cardID, CardFinish cardFinish)
    {
        ID = cardID;
        finish = cardFinish;
        ApplyFinish(cardID);
    }

    private void Start()
    {
        moveSpeed = gameManager.cardSpeed;
        cardCanvas = GetComponentInParent<Canvas>();
        dragTargetPos = transform.position;
        ApplyFinish(ID);
        timeOffset = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
        originalScale = transform.localScale;
    }

    private void Awake()
    {
        StartIdleRotation();
    }

    public void ApplyFinish(CardID cardID)
    {
        switch (finish)
        {
            case CardFinish.Matte: cardFace.material = normalMat; break;
            case CardFinish.Chroma: cardFace.material = holoMat; break;
            case CardFinish.Foil: cardFace.material = foilMat; break;
            case CardFinish.Inverse: cardFace.material = inverseMat; break;
            case CardFinish.Void: cardFace.material = voidMat; break;
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

    #region ANIMATIONS
    private void StartIdleRotation()
    {
        // Kill any existing tween just in case
        idleTween?.Kill();

        idleTween = DOVirtual.Float(0, 1, 1f / rotationSpeed, t =>
        {
            time += Time.deltaTime * rotationSpeed;
            float rotX = Mathf.Cos(time + timeOffset) * rotationAmplitude;
            float rotY = Mathf.Sin(time + timeOffset) * rotationAmplitude;
            transform.rotation = Quaternion.Euler(rotX, rotY, 0f);
        })
        .SetLoops(-1, LoopType.Incremental)
        .SetEase(Ease.Linear);
    }

    private void StopIdleRotation()
    {
        if (idleTween != null && idleTween.IsActive())
        {
            idleTween.Kill();
            idleTween = null;
        }
    }

    private void Shrink() => transform.DOScale(originalScale, tweenDuration).SetEase(Ease.OutBack);
    private void Grow() => transform.DOScale(originalScale * hoverScale, tweenDuration).SetEase(Ease.OutBack);
    #endregion

    #region DRAG_EVENTS
    public void OnBeginDrag(PointerEventData eventData)
    {
        StopIdleRotation();
        isDragging = true;

        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            cardCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out worldPos
        );
        offset = transform.position - worldPos;

        Shrink();
        BeginDragEvent?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            cardCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out worldPos
        );
        dragTargetPos = worldPos + offset;
        Shrink();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        EndDragEvent?.Invoke(this);

        // No swapping – just return to original slot
        dragTargetPos = parentSlot.position;

        Shrink();
        StartIdleRotation();
    }
    #endregion

    #region POINTER_EVENTS
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        Grow();
        PointerEnterEvent?.Invoke(this, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        Shrink();
        PointerExitEvent?.Invoke(this);
    }

    public void OnPointerUp(PointerEventData eventData) => PointerUpEvent?.Invoke(this);
    public void OnPointerDown(PointerEventData eventData) => PointerDownEvent?.Invoke(this);
    #endregion
}
