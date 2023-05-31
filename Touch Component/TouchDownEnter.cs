using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TouchDownEnter : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
    [SerializeField] private UnityEvent ClickEventDown;
    [SerializeField] private UnityEvent ClickEventUp;
   
    public void OnPointerDown(PointerEventData eventData)
    {
        ClickEventDown?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ClickEventUp?.Invoke();
    }
}
