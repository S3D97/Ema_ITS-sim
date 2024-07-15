using UnityEngine;
using UnityEngine.UI;

public class RaycastController : MonoBehaviour
{
    public Button myButton;
    public LineRenderer lineRenderer;
    public Camera mainCamera;
    public Sprite imagePrefab;

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

    void ActivateRaycast()
    {
        isRaycastActive = true;
        lineRenderer.enabled = true;
    }

    void DeactivateRaycast()
    {
        isRaycastActive = false;
        lineRenderer.enabled = false;
    }

    void UpdateLineRenderer()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        lineRenderer.SetPosition(0, ray.origin);
        lineRenderer.SetPosition(1, ray.origin + ray.direction * 100); // Adjust the length as needed
    }
}

