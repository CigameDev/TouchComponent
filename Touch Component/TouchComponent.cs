using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TouchComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private UnityEvent onPointerDownEvent;
    [SerializeField] private UnityEvent onPointerUpEvent;

    private void OnDestroy()
    {
        RemoveEvents();
    }

    public void InitComponentEvent(UnityAction pointerDown, UnityAction pointerUp)
    {
        onPointerDownEvent.AddListener(pointerDown);
        onPointerUpEvent.AddListener(pointerUp);
    }

    private void RemoveEvents()
    {
        onPointerDownEvent.RemoveAllListeners();
        onPointerUpEvent.RemoveAllListeners();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDownEvent?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUpEvent?.Invoke();
    }

}
