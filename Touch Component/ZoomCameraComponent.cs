using Skymare.KickTheBuddy;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ZoomCameraComponent : MonoBehaviour
{
    public static bool IsLockZoom = false;

    //[SerializeField] private float perspectiveZoomSpeed = 0.1f;        // The rate of change of the field of view in perspective mode.
    [SerializeField] private float orthoZoomSpeed = 0.01f;        // The rate of change of the orthographic size in orthographic mode.

    [Tooltip("Min and max orthographic size")]
    [SerializeField] private Vector2 orthographicSizeRange = new Vector2(5, 10);

    private Camera mainCamera;
    private Touch touchZero, touchOne;
    private Vector2 touchZeroPrevPos, touchOnePrevPos;
    private float prevTouchDeltaMag, touchDeltaMag, deltaMagnitudeDiff;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (GameController.Instance.IsWinGame) return;

        if (IsLockZoom)
        {
            if (Input.touchCount != 0)
            {
                return;
            }

            IsLockZoom = false;
        }

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

            // If the camera is orthographic...
            if (mainCamera.orthographic)
            {
                // Make sure the orthographic size never drops below zero.
                mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize + deltaMagnitudeDiff * orthoZoomSpeed, orthographicSizeRange.x, orthographicSizeRange.y);
            }
            //else
            //{
            //    // Clamp the field of view to make sure it's between 0 and 180.
            //    mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView + deltaMagnitudeDiff * perspectiveZoomSpeed, 0.1f, 179.9f);
            //}
        }
    }
}
