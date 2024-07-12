using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public LayerMask interactionLayer; // LayerMask per il layer "Interaction"
    public Camera mainCamera; // Riferimento alla main camera
    public Color targetColor;
    public Color targetColor2;
    public Color targetColor3;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Esegui l'azione quando il mouse Ã¨ cliccato
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Debug.Log("Raycasting");
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactionLayer))
            {
                Debug.Log("Hit!");
                Renderer renderer = hit.collider.GetComponent<Renderer>();
                MeshCollider meshCollider = hit.collider as MeshCollider;

                if (renderer != null && renderer.material.mainTexture != null && meshCollider != null)
                {
                    Texture2D texture = renderer.material.mainTexture as Texture2D;
                    Vector2 pixelUV = hit.textureCoord;
                    pixelUV.x *= texture.width;
                    pixelUV.y *= texture.height;

                    Color color = texture.GetPixel((int)pixelUV.x, (int)pixelUV.y);
                    Debug.Log("Colore del pixel: " + color);
                    targetColor = color;
                    targetColor2 = color;
                    targetColor3 = color;
                }
            }
        }
    }
}
