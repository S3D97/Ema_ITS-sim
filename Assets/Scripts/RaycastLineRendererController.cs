using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastController : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Camera mainCamera;
    public GameObject imageIcon;
    public Vector3 lineOffset;
    public float lineRendereDistance;
    public int interaction;
    public Color excludeColor;

    // Array di colori da riconoscere
    public Color[] targetColors;
    // Dizionario per contare le occorrenze di ogni colore
    private Dictionary<Color, int> colorCounts = new Dictionary<Color, int>();

    private bool isRaycastActive = false;

    void Start()
    {
        lineRenderer.enabled = false;

        // Inizializza il dizionario dei contatori dei colori
        foreach (var color in targetColors)
        {
            colorCounts[color] = 0;
        }

        interaction = LayerMask.GetMask("Interaction");
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

                if (Physics.Raycast(ray, out hit, lineRendereDistance, interaction))
                {
                    // Riconoscimento del colore del pixel
                    Renderer renderer = hit.collider.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        Texture2D texture = renderer.material.mainTexture as Texture2D;
                        Vector2 pixelUV = hit.textureCoord;
                        pixelUV.x *= texture.width;
                        pixelUV.y *= texture.height;
                        Color hitColor = texture.GetPixel((int)pixelUV.x, (int)pixelUV.y);
                        if (hitColor == excludeColor)
                        {
                           return; 
                        }

                        // Controlla se il colore colpito è uno dei colori target
                        foreach (var targetColor in targetColors)
                        {
                            if (ColorsAreEqual(hitColor, targetColor))
                            {
                                Instantiate(imageIcon, hit.point, Quaternion.identity);
                                imageIcon.transform.position = hit.point;
                                colorCounts[targetColor]++;
                                Debug.Log($"Colore {targetColor} selezionato. Conteggio: {colorCounts[targetColor]}");
                                break;
                            }
                        }
                    }

                    
                    DeactivateRaycast();
                }
            }
        }
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
        lineRenderer.SetPosition(1, ray.origin + ray.direction * lineRendereDistance); 
    }

    // Funzione per confrontare due colori con una tolleranza
    private bool ColorsAreEqual(Color color1, Color color2, float tolerance = 0.1f)
    {
        return Mathf.Abs(color1.r - color2.r) < tolerance &&
               Mathf.Abs(color1.g - color2.g) < tolerance &&
               Mathf.Abs(color1.b - color2.b) < tolerance;
    }
}
