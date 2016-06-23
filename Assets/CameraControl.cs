using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    private Vector3 ResetCamera;
    private Vector3 Origin;
    private Vector3 Diference;
    private bool Drag = false;

    public float zoomAmount = 2;
    public float minZoomAmount = 5;
    public float maxZoomAmount = 30;

    void LateUpdate()
    {
        // Move Camera
        if (Input.GetMouseButton(1))
        {
            Diference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (Drag == false)
            {
                Drag = true;
                Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else {
            Drag = false;
        }
        if (Drag == true)
        {
            Camera.main.transform.position = Origin - Diference;
        }

        // Zoom camera
        Camera cam = gameObject.GetComponent<Camera>();
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize+ Input.GetAxisRaw("Mouse ScrollWheel")* zoomAmount,minZoomAmount,maxZoomAmount);

    }
}
