using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class RotateComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private UnityEvent rotateDone;
    public UnityEvent RotateDone => rotateDone;

    private Camera mainCamera;
    private bool isTouch;
    private bool isRotate;
    private float angleRotate;
    private Vector2 touchZeroPrevPos, touchOnePrevPos, lastRotate, currentRotate;

#if UNITY_EDITOR
    private Vector2 lastMousePosition;
#endif

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isTouch = true;
#if UNITY_EDITOR
        lastMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
#endif
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isTouch = false;

        if (isRotate)
        {
            rotateDone?.Invoke();
            isRotate = false;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!isTouch || !Input.GetMouseButton(0) || !Input.GetMouseButton(1)) return;

        DragDropComponent.IsLockDrag = true;
        isRotate = true;

        lastRotate = lastMousePosition - (Vector2)transform.position;
        currentRotate = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        angleRotate = Vector2.Angle(lastRotate, currentRotate);
        if (angleRotate >= 1)
        {
            angleRotate *= DirectionRotate(Vector2.zero, lastRotate, currentRotate);
            transform.Rotate(Vector3.forward, angleRotate);
        }
        lastMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
#else
        if (!isTouch || Input.touchCount != 2) return;

        isRotate = true;
        DragDropComponent.IsLockDrag = true;

        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        lastRotate = touchZeroPrevPos - touchOnePrevPos;
        currentRotate = touchZero.position - touchOne.position;

        angleRotate = Vector2.Angle(currentRotate, lastRotate);
        if (angleRotate >= 1)
        {
            angleRotate *= DirectionRotate(Vector2.zero, lastRotate, currentRotate);
            transform.Rotate(Vector3.forward, angleRotate);
        }
#endif
    }

    private int DirectionRotate(Vector2 origin, Vector2 from, Vector2 to)
    {
        float a = from.y - to.y;
        float b = to.x - from.x;
        float c = -(a * from.x + b * from.y);
        float x = -(b * origin.y + c) / a;
        return ((to - from).y > 0 ? -1 : 1) * (x < origin.x ? 1 : -1);
    }
}