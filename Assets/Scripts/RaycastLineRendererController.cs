using UnityEngine;
using UnityEngine.UI;

public class RaycastController : MonoBehaviour
{
    public Button myButton;
    public LineRenderer lineRenderer;
    public Camera mainCamera;
    public Sprite imagePrefab;
    public Vector3 lineOffset;

    private bool isRaycastActive = false;

    void Start()
    {
        myButton.onClick.AddListener(OnButtonClick);
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (isRaycastActive)
        {
            UpdateLineRenderer();

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Instantiate(imagePrefab, hit.point, Quaternion.identity);
                    DeactivateRaycast();
                }
            }
        }
    }

    void OnButtonClick()
    {
        ActivateRaycast();
    }

    public void ActivateRaycast()
    {
        isRaycastActive = true;
        lineRenderer.enabled = true;
    }

    public void DeactivateRaycast()
    {
        isRaycastActive = false;
        lineRenderer.enabled = false;
    }

    public void UpdateLineRenderer()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        lineRenderer.SetPosition(0, ray.origin + lineOffset);
        lineRenderer.SetPosition(1, ray.origin + ray.direction * 100); // Adjust the length as needed
    }
}

