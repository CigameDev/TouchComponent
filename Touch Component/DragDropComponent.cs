using Skymare.KickTheBuddy;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragDropComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public static bool IsLockDrag = false;

    [SerializeField] private Vector2 minMaxY = new Vector2(float.MinValue, float.MaxValue);
    [SerializeField] private Vector2 minMaxX = new Vector2(float.MinValue, float.MaxValue);

    [SerializeField] private UnityEvent pointerDownEvent = new UnityEvent();
    public UnityEvent PointerDownEvent => pointerDownEvent;

    [SerializeField] private UnityEvent startDragEvent = new UnityEvent();
    public UnityEvent StartDragEvent => startDragEvent;

    [SerializeField] private UnityEvent endDragEvent = new UnityEvent();
    public UnityEvent EndDragEvent => endDragEvent;

    [SerializeField] private UnityEvent shakeEvent = new UnityEvent();
    public UnityEvent ShakeEvent => shakeEvent;

    private Transform endBottom;

    private Transform bottomBound;
    private Transform topBound;
    private Transform rightBound;
    private Transform leftBound;

    private Camera mainCamera;
    private Vector3 mousePosition;
    private Vector3 mouseTouchOffset;
    private bool isDragged = false;
    private int maxTouchCount = 0;
    private int touchId = -1;
    private int fingerId = -1;
    private float mouseDeltaMagnitude;

    // for shake
    private float firstShakeTime;
    private int shakeCount = 0;
    private int shakeCountMax = 8;

    private void Start()
    {
        mainCamera = Camera.main;
        //endBottom = UiManager.Instance.EndBottom;

        bottomBound = GameController.Instance.bottomBound;
        topBound = GameController.Instance.topBound;
        rightBound = GameController.Instance.rightBound;
        leftBound = GameController.Instance.leftBound;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragged = true;
        startDragEvent?.Invoke();
    }

    private void Update()
    {
        if (GameController.Instance == null || GameController.Instance.IsWinGame) return;

#if UNITY_EDITOR
        if (IsLockDrag)
        {
            IsLockDrag &= Input.GetMouseButton(0) || Input.GetMouseButton(1);
        }
#else
        if (IsLockDrag)
        {
            if (Input.touchCount != 0)
            {
                return;
            }

            IsLockDrag = false;
        }
#endif
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameController.Instance == null || GameController.Instance.IsWinGame || IsLockDrag) return;

#if UNITY_EDITOR
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition) + mouseTouchOffset;
        //mousePosition.x = Mathf.Clamp(mousePosition.x, minMaxX.x, minMaxX.y);
        //mousePosition.y = Mathf.Clamp(mousePosition.y, Mathf.Max(minMaxY.x, endBottom.position.y), minMaxY.y);

        mousePosition.x = Mathf.Clamp(mousePosition.x, leftBound.position.x, rightBound.position.x);
        mousePosition.y = Mathf.Clamp(mousePosition.y, bottomBound.position.y, topBound.position.y);

        mousePosition.z = transform.position.z;
        transform.position = mousePosition;

        //Vector3 localPOsition = transform.InverseTransformPoint(mainCamera.ScreenToWorldPoint(Input.mousePosition));
        //localPOsition.z = 0f;
        //transform.localPosition = localPOsition;
#else
        if (maxTouchCount != Input.touchCount)
        {
            touchId = -1;
            maxTouchCount = Input.touchCount;
            for (int i = 0; i < maxTouchCount; i++)
            {
                if (Input.touches[i].fingerId == fingerId)
                {
                    touchId = i;
                    break;
                }
            }
        }

        if (touchId >= 0 && touchId < Input.touchCount)
        {
            mousePosition = mainCamera.ScreenToWorldPoint(Input.touches[touchId].position) + mouseTouchOffset;
            //mousePosition.x = Mathf.Clamp(mousePosition.x, minMaxX.x, minMaxX.y);
            //mousePosition.y = Mathf.Clamp(mousePosition.y, Mathf.Max(minMaxY.x, endBottom.position.y), minMaxY.y);
            mousePosition.x = Mathf.Clamp(mousePosition.x, leftBound.position.x, rightBound.position.x);
            mousePosition.y = Mathf.Clamp(mousePosition.y, bottomBound.position.y, topBound.position.y);
            mousePosition.z = transform.position.z;
            transform.position = mousePosition;
        }
        else
        {
            OnPointerUp(null);
        }
#endif
        if (shakeCount < shakeCountMax)
        {
            mouseDeltaMagnitude = eventData.delta.magnitude;
            //Debug.Log($"OnDrag eventData.scrollDelta.sqrMagnitude: {mouseDeltaMagnitude}");
            if (mouseDeltaMagnitude >= 25 && mouseDeltaMagnitude <= 60)
            {
                if (shakeCount == 0) firstShakeTime = Time.time;

                shakeCount++;
                if (shakeCount >= shakeCountMax)
                {
                    if (Time.time - firstShakeTime <= 1)
                    {
                        //Debug.Log("Shake event detected at time " + Time.time);
                        shakeEvent?.Invoke();
                    }
                    else
                    {
                        shakeCount = 1;
                        firstShakeTime = Time.time;
                    }
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ZoomCameraComponent.IsLockZoom = true;

#if UNITY_EDITOR
        mouseTouchOffset = transform.position - mainCamera.ScreenToWorldPoint(Input.mousePosition);
#else
        fingerId = eventData.pointerId;
        maxTouchCount = Input.touchCount;
        for (int i = 0; i < maxTouchCount; i++)
        {
            if (Input.touches[i].fingerId == fingerId)
            {
                touchId = i;
                break;
            }
        }
        mouseTouchOffset = transform.position - mainCamera.ScreenToWorldPoint(Input.touches[touchId].position);
#endif

        pointerDownEvent?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragged = false;
        touchId = -1;
        fingerId = -1;
        shakeCount = 0;

        endDragEvent?.Invoke();
    }

    public void ChangeMinMaxValue(Vector2 minMaxNewX, Vector2 minMaxNewY)
    {
        minMaxX = minMaxNewX;
        minMaxY = minMaxNewY;
    }
}
