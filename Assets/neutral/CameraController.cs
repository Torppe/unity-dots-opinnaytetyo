using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private Camera cam;

    [SerializeField]
    private float zoomAmount = 1;
    [SerializeField]
    private float smoothSpeed = 2.0f;
    [SerializeField]
    private float minSize = 10.0f;
    [SerializeField]
    private float maxSize = 50.0f;
    private float targetSize;

    private Vector3 panStartPosition;
    private Vector3 panDirection;
    private bool panning;

    void Start() {
        cam = GetComponent<Camera>();
        targetSize = cam.orthographicSize;
    }

    void Update() {
        HandleZoom();
        HandlePanning();
    }

    private void HandleZoom() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f) {
            targetSize -= scroll * zoomAmount;
            targetSize = Mathf.Clamp(targetSize, minSize, maxSize);
        }

        float difference = Mathf.Abs(targetSize - cam.orthographicSize);
        if (difference > 0.1f)
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, smoothSpeed * Time.deltaTime);
    }

    private void HandlePanning() {
        if (Input.GetMouseButtonDown(0)) {
            panStartPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0)) {
            panDirection = panStartPosition - cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position += panDirection * Time.deltaTime * 15f;
        }
    }
}
