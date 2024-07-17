using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;

public class RaycastController : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Camera mainCamera;
    public GameObject imageIcon;
    public Vector3 lineOffset;
    public float lineRendererDistance;
    public Color excludeColor;
    
    private int _interaction;
    private bool _doneTask;

    public TextMeshProUGUI timerUi;
    public bool timerConfirm;
    public float remainingTime;
    public float timerTime;
    public GameObject doneButton;
    

    public Color[] targetColors;
    public Dictionary<Color, GameObject> colorToIconMap = new Dictionary<Color, GameObject>();
    private Dictionary<Color, int> colorCounts = new Dictionary<Color, int>();

    private bool isRaycastActive = false;
    private List<ImageIconData> imageIconsData = new List<ImageIconData>();
    
    

    void Start()
    {
        lineRenderer.enabled = false;

        foreach (var color in targetColors)
        {
            colorCounts[color] = 0;
        }

        _interaction = LayerMask.GetMask("Interaction");
        remainingTime = timerTime; 
    }

    void Update()
    {
        if (isRaycastActive)
        {
            UpdateLineRenderer();
            StartTimer();
            TimerPointerPhase();

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, lineRendererDistance, _interaction))
                {
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

                        foreach (var targetColor in targetColors) //controllo sui target color della scena
                        {
                            if (ColorsAreEqual(hitColor, targetColor) && timerConfirm) //metodo che confronta i colori target con quelli presi dal raycast
                            {
                                doneButton.SetActive(true); //per fermare prima il posizionamento dei pointer se si finisce prima del tempo
                                GameObject iconInstance = Instantiate(imageIcon, hit.point, Quaternion.identity); //posizionamento pointer su hitpoint del raycast
                                iconInstance.transform.position = hit.point;
                                colorCounts[targetColor]++;
                                imageIconsData.Add(new ImageIconData //json del punto cliccato con il colore
                                {
                                    Position = hit.point,
                                    Color = new SerializableColor(targetColor)
                                });
                                Debug.Log($"Colore {targetColor} selezionato. Conteggio: {colorCounts[targetColor]}");
                                break;
                            }
                        }
                    }

                    if (timerConfirm == false || _doneTask == true)
                    {
                        DeactivateRaycast();
                        remainingTime = 0;
                    }
                    
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
        lineRenderer.SetPosition(1, ray.origin + ray.direction * lineRendererDistance);
    }

    private bool ColorsAreEqual(Color color1, Color color2, float tolerance = 0.1f)
    {
        return Mathf.Abs(color1.r - color2.r) < tolerance &&
               Mathf.Abs(color1.g - color2.g) < tolerance &&
               Mathf.Abs(color1.b - color2.b) < tolerance;
    }

    private void TimerPointerPhase()
    {
        if (timerConfirm)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                UpdateTimerText();
            }
            else
            {
                timerConfirm = false;
                remainingTime = 0;
                UpdateTimerText();
                SaveImageIconsData();
                Debug.Log("Il tempo è scaduto!");
                DeactivateRaycast();
            }
        }
    }

    void StartTimer()
    {
        timerConfirm = true;
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerUi.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void SaveImageIconsData()
    {
        string json = JsonUtility.ToJson(new ImageIconsDataList { Icons = imageIconsData });
        File.WriteAllText(Application.persistentDataPath + "/ImageIconsData.json", json);
    }

    public void LoadImageIconsData()
    {
        string path = Application.persistentDataPath + "/ImageIconsData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            ImageIconsDataList loadedData = JsonUtility.FromJson<ImageIconsDataList>(json);
            foreach (var data in loadedData.Icons)
            {
                GameObject iconInstance = Instantiate(imageIcon, data.Position, Quaternion.identity);
                iconInstance.transform.position = data.Position;
                // Aggiungere logica per mappare colori a icone specifiche sui pointer (tipo rampa, pedana, etc.)
            }
        }
    }

    [System.Serializable]
    public class ImageIconData
    {
        // dati da salvare per i pointer
        public Vector3 Position;
        public SerializableColor Color;
    }

    [System.Serializable]
    public class SerializableColor
    {
        public float r, g, b, a;

        public SerializableColor(Color color)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
        }

        public static implicit operator Color(SerializableColor c) => new Color(c.r, c.g, c.b, c.a);
    }

    [System.Serializable]
    public class ImageIconsDataList
    {
        public List<ImageIconData> Icons;
    }

    public void DoneTask()
    {
        _doneTask = true;
    }
}
