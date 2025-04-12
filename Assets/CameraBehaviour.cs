using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomSpeed = 10f;
    public float minZoomOrtho = 5f;
    public float maxZoomOrtho = 50f;
    public float minZoomPerspective = 10f;
    public float maxZoomPerspective = 100f;

    private Vector3 dragOrigin;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    void HandleMovement()
    {
        if (Input.GetMouseButtonDown(2)) // Middle mouse pressed
        {
            dragOrigin = cam.ScreenToWorldPoint(GetMouseWorldPosition());
        }

        if (Input.GetMouseButton(2)) // Middle mouse held
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(GetMouseWorldPosition());
            transform.position += new Vector3(difference.x, 0, difference.z);
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            if (cam.orthographic)
            {
                cam.orthographicSize -= scroll * zoomSpeed;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoomOrtho, maxZoomOrtho);
            }
            else
            {
                // Move along local Y axis (for top-down)
                transform.position += transform.forward * scroll * zoomSpeed;
                float height = transform.position.y;
                height = Mathf.Clamp(height, minZoomPerspective, maxZoomPerspective);
                transform.position = new Vector3(transform.position.x, height, transform.position.z);
            }
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam.orthographic ? cam.nearClipPlane : Mathf.Abs(transform.position.y);
        return mousePos;
    }
}