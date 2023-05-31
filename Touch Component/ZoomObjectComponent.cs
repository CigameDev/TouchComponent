using Skymare.KickTheBuddy;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class ZoomObjectComponent : MonoBehaviour
{
    [SerializeField] private float SpeedZoom = 5f;
    private Vector3 vecNormal = Vector3.one;        // The rate of change of the orthographic size in orthographic mode.

    [Tooltip("Min and max orthographic size")]
    [SerializeField] private float PrentMinScale;
    [SerializeField] private float PrentMaxScale;

    private Touch touchZero, touchOne;
    private Vector2 touchZeroPrevPos, touchOnePrevPos;
    private float prevTouchDeltaMag, touchDeltaMag, deltaMagnitudeDiff;

    public UnityEvent pointerDownEvent;
    public UnityEvent endZoomSmallEvent;
    public UnityEvent endZoomBigEvent;

    public bool NotZoomSmall;
    public bool NotZoomBig;

    private void Start()
    {
        vecNormal = transform.localScale;
    }

    private void OnMouseDrag()
    {
        if (GameController.Instance.IsWinGame) return;

        if (Input.touchCount == 2)
        {
            // Store both touches.
            touchZero = Input.GetTouch(0);
            touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            if (deltaMagnitudeDiff > 0 && transform.localScale.x <= vecNormal.x * PrentMinScale)
                deltaMagnitudeDiff = 0;
            else if (deltaMagnitudeDiff < 0 && transform.localScale.x >= vecNormal.x * PrentMaxScale)
                deltaMagnitudeDiff = 0;

            if(NotZoomSmall && deltaMagnitudeDiff > 0) return;
            if(NotZoomBig && deltaMagnitudeDiff < 0) return;

            transform.localScale = Vector3.MoveTowards(transform.localScale, transform.localScale + new Vector3(-deltaMagnitudeDiff, -deltaMagnitudeDiff, -deltaMagnitudeDiff), SpeedZoom * Time.deltaTime);
        }
    }

    private void OnMouseDown()
    {
        pointerDownEvent?.Invoke();
        _scaleOld = transform.localScale.x;
    }

    private float _scaleOld;
    private void OnMouseUp()
    {
        if (_scaleOld > transform.localScale.x)
            endZoomSmallEvent?.Invoke();
        else if (_scaleOld < transform.localScale.x)
            endZoomBigEvent?.Invoke();
    }
}
