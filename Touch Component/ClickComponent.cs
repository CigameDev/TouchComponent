using Skymare.KickTheBuddy;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClickComponent : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private UnityEvent clickEvent = new UnityEvent();
    public UnityEvent ClickEvent => clickEvent;

    private float pointerDownTime;
    private bool isDrag;

    public void OnDrag(PointerEventData eventData)
    {
        isDrag = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDrag && GameController.Instance != null && !GameController.Instance.IsWinGame && Time.time - pointerDownTime <= 0.2f)
            clickEvent?.Invoke();
    }
}
