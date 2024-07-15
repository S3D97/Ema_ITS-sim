using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SinglePixelColorExtractor : MonoBehaviour
{
    public GameObject targetGameObject;
    
    public LayerMask interactionLayer;
    public Color targetColor;

    public Camera mainCamera;


    public Cubemap cubemap2;

 



    void Start()
    {
           if (targetGameObject != null)
        {
            // Ottieni il Renderer dal gameobject target
            Renderer renderer = targetGameObject.GetComponent<Renderer>();
            
            // Controlla se il gameobject ha un renderer
            if (renderer != null)
            {
                // Ottieni il materiale dal renderer
                Material material = renderer.material;
                
                // Controlla se il materiale ha la proprietà della cubemap
                if (material.HasProperty("_Cubemap"))
                {
                    // Ottieni la cubemap dal materiale
                    Cubemap cubemap2 = material.GetTexture("_Cubemap") as Cubemap;

                }
            }
        }
    }



    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray =  mainCamera.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionLayer))
            {
                Renderer renderer = hit.collider.GetComponent<Renderer>();
                MeshCollider meshCollider = hit.collider as MeshCollider;
                // Assuming the hit object has a cubemap texture applied
                Vector3 hitPoint = hit.point;
                Vector3 localPoint = hit.transform.InverseTransformPoint(hitPoint);

                // Get the face of the cubemap
                CubemapFace face = GetCubemapFace(localPoint);

                // Get the UV coordinates on the cubemap face
                Vector2 uv = GetCubemapFaceUV(face, localPoint);

                // Get the color of the pixel at the mouse pointer
                Color pixelColor = GetPixelColor(cubemap2, face, uv);

                // Do something with the extracted pixel color
                ProcessPixelColor(pixelColor);
            }
        }
    }

    private CubemapFace GetCubemapFace(Vector3 localPoint)
    {
        // Determine which face of the cubemap the point is on
        // Implement your logic here
        // Example:
        if (Mathf.Abs(localPoint.x) > Mathf.Abs(localPoint.y) && Mathf.Abs(localPoint.x) > Mathf.Abs(localPoint.z))
            return localPoint.x > 0 ? CubemapFace.PositiveX : CubemapFace.NegativeX;
        else if (Mathf.Abs(localPoint.y) > Mathf.Abs(localPoint.x) && Mathf.Abs(localPoint.y) > Mathf.Abs(localPoint.z))
            return localPoint.y > 0 ? CubemapFace.PositiveY : CubemapFace.NegativeY;
        else
            return localPoint.z > 0 ? CubemapFace.PositiveZ : CubemapFace.NegativeZ;
    }

    private Vector2 GetCubemapFaceUV(CubemapFace face, Vector3 localPoint)
    {
        // Convert the localPoint to UV coordinates on the cubemap face
        // Implement your logic here
        // Example:
        switch (face)
        {
            case CubemapFace.PositiveX: return new Vector2(localPoint.z, localPoint.y);
            case CubemapFace.NegativeX: return new Vector2(-localPoint.z, localPoint.y);
            case CubemapFace.PositiveY: return new Vector2(localPoint.x, localPoint.z);
            case CubemapFace.NegativeY: return new Vector2(localPoint.x, -localPoint.z);
            case CubemapFace.PositiveZ: return new Vector2(localPoint.x, localPoint.y);
            case CubemapFace.NegativeZ: return new Vector2(-localPoint.x, localPoint.y);
            default: return Vector2.zero;
        }
    }

    private Color GetPixelColor(Cubemap cubemap, CubemapFace face, Vector2 uv)
    {
        // Convert UV to pixel coordinates
        int cubemapSize = cubemap.width;
        int x = (int)(uv.x * cubemapSize);
        int y = (int)(uv.y * cubemapSize);

        // Get the color of the pixel at the given coordinates
        return cubemap.GetPixel(face, x, y);
    }

    private void ProcessPixelColor(Color pixelColor)
    {
        // Implement your logic to process the pixel color here
        // Example: Print the color
        Debug.Log("Pixel color: " + pixelColor);
    }
}
