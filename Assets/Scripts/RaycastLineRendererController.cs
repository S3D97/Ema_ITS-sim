using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using JetBrains.Annotations;
using UnityEngine.InputSystem;
using UnityEditor;

public class RaycastController : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Camera mainCamera;
    public GameObject imageIcon;
    public Vector3 lineOffset;
    public float lineRendererDistance;
    public Color excludeColor;
    
    private int _interaction;
    private bool _doneTask = false;

    public TextMeshProUGUI timerUi;
    public bool timerConfirm;
    public float remainingTime;
    public float timerTime;
    public float newTimerTime;
    public bool newTimerStart = false;
    public GameObject doneButton;
    public GameObject exhibitionPanel;
    public GameObject exhibitionPanel1;
    public GameObject exhibitionPanel2;
    public GameObject[] solutionsButtons;

    
    

    public Color[] targetColors;
    public Dictionary<Color, GameObject> colorToIconMap = new Dictionary<Color, GameObject>();
    private Dictionary<Color, int> colorCounts = new Dictionary<Color, int>();

    private bool isRaycastActive = false;
    private List<ImageIconData> imageIconsData = new List<ImageIconData>();

    public Dictionary<Color, GameObject> SolutionObjectsDictionary = new Dictionary<Color, GameObject>();
    public SolutionObjectsDictionary solutionObjectsDictionary;
    
    

    void Start()
    {
        lineRenderer.enabled = false;

        foreach (var color in targetColors)
        {
            colorCounts[color] = 0;
        }
        
        exhibitionPanel.SetActive(true);
        _interaction = LayerMask.GetMask("Interaction");
        remainingTime = timerTime; 
        doneButton.GetComponent<Button>().onClick.AddListener(DeactivateRaycast);
        
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

                    if (_doneTask == true || timerConfirm == false)
                    {
                        DeactivateRaycast();
                    }
                    
                }
            }
        }
    }

    private void ActivateRaycast()
    {
        isRaycastActive = true;
        lineRenderer.enabled = true;
    }

    private void DeactivateRaycast()
    {
        isRaycastActive = false;
        lineRenderer.enabled = false;
        remainingTime = 0f; 
        timerUi.text = ""; 
        timerUi.enabled = false; 
    }

    private void UpdateLineRenderer()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        lineRenderer.SetPosition(0, ray.origin + lineOffset);
        lineRenderer.SetPosition(1, ray.origin + ray.direction * lineRendererDistance);
    }

    private bool ColorsAreEqual(Color color1, Color color2, float tolerance = 0.1f) //metodo che confronta i colori target con quelli presi dal raycast
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
                doneButton.SetActive(false);
                UpdateTimerText();
                SaveImageIconsData();
                Debug.Log("Il tempo è scaduto!");
                DeactivateRaycast();
                TimerStopped();
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
        Debug.Log("Dati jason salvati correttamente");
    }

    // public void LoadImageIconsData()
    // {
    //     string path = Application.persistentDataPath + "/ImageIconsData.json";
    //     if (File.Exists(path))
    //     {
    //         string json = File.ReadAllText(path);
    //         ImageIconsDataList loadedData = JsonUtility.FromJson<ImageIconsDataList>(json);
    //         foreach (var data in loadedData.Icons)
    //         {
    //             GameObject iconInstance = Instantiate(imageIcon, data.Position, Quaternion.identity);
    //             iconInstance.transform.position = data.Position;
    //             // Aggiungere logica per mappare colori a icone specifiche sui pointer (tipo rampa, pedana, etc.)
    //         }
    //     }
    // }

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
    
    
    private void TimerStopped() 
    {
        exhibitionPanel1.SetActive(true);
        foreach (var ObjectButton in solutionsButtons)
        {
            if (ObjectButton != null)
            {
                ObjectButton.SetActive(true);
            } 
        }
    
        // Nuovo Timer
        remainingTime = newTimerTime;
        timerConfirm = true;
        timerUi.enabled = true; 
        NewTimerPointerPhase();
}
    
private void NewTimerPointerPhase()
{
    newTimerStart = true;
    if (timerConfirm)
    {
        if (remainingTime > 0)
        {
            //PlaceObjectsSolution();
            remainingTime -= Time.deltaTime;
            UpdateTimerText();
        }
        else
        {
            timerConfirm = false;
            remainingTime = 0;
            exhibitionPanel2.SetActive(true);
            UpdateTimerText();
            DeactivateRaycast();
        }
    }
}



// private void PlaceObjectsSolution()
// {
//     string path = Application.persistentDataPath + "/ImageIconsData.json";
//     if (File.Exists(path))
//     {
//         string json = File.ReadAllText(path);
//         ImageIconsDataList loadedData = JsonUtility.FromJson<ImageIconsDataList>(json);
//
//         foreach (var data in loadedData.Icons)
//         {
//             Vector3 position = data.Position;
//             Color jsonColor = data.Color;
//
//             
//             foreach (var entry in solutionObjectsDictionary.SolutionObjectsNames)
//             {
//                 if (ColorsAreEqual(jsonColor, entry.Key))
//                 {
//                     GameObject solutionObject = entry.Value;
//                     Ray ray = new Ray(mainCamera.transform.position, position - mainCamera.transform.position);
//                     if (Physics.Raycast(ray, out RaycastHit hit, lineRendererDistance, _interaction))
//                     {
//                         solutionObject.transform.position = hit.point;
//                     }
//                     else
//                     { 
//                         Debug.LogError("Se so rubati i COLORI, chi si è rubato i COLORI");
//                     }
//                     break;
//                 }
//             }
//         }
//     }
// }


public void PlaceObjectsSolution()
{
    if (newTimerStart == true)
    { 
        string path = Application.persistentDataPath + "/ImageIconsData.json";
    if (File.Exists(path))
    {
        string json = File.ReadAllText(path);
        ImageIconsDataList loadedData = JsonUtility.FromJson<ImageIconsDataList>(json);

        foreach (var data in loadedData.Icons)
        {
            Vector3 position = data.Position;
            Color jsonColor = data.Color;

            foreach (var entry in solutionObjectsDictionary.SolutionObjectsNames)
            {
                if (ColorsAreEqual(jsonColor, entry.Key))
                {
                    
                    GameObject solutionObject = entry.Value;
                    Ray ray = new Ray(mainCamera.transform.position, position - mainCamera.transform.position);
                    
                    

                    ActivateRaycast();
                    // lineRenderer.enabled = true;
                    // lineRenderer.SetPosition(0, ray.origin + lineOffset);
                    // lineRenderer.SetPosition(1, ray.origin + ray.direction * lineRendererDistance);

                    if (Physics.Raycast(ray, out RaycastHit hit, lineRendererDistance, _interaction))
                    {
                        // Posiziona l'oggetto solo se il colore corrisponde
                        Renderer renderer = hit.collider.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            Texture2D texture = renderer.material.mainTexture as Texture2D;
                            Vector2 pixelUV = hit.textureCoord;
                            pixelUV.x *= texture.width;
                            pixelUV.y *= texture.height;
                            Color hitColor = texture.GetPixel((int)pixelUV.x, (int)pixelUV.y);
                            if (ColorsAreEqual(hitColor, jsonColor))
                            {
                                GameObject solutionObjectInstance = Instantiate(solutionObject, hit.point, Quaternion.identity); //posizionamento oggetto su hitpoint del raycast
                                solutionObjectInstance.transform.position = hit.point;
                                Debug.Log("Sei un cecchino");
                            }
                            else
                            {
                                Debug.LogError("Hai sbagliato colore, sempre e solo giallo rosso");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Missed me");
                    }

                    // Disabilita il LineRenderer dopo aver posizionato l'oggetto per poi premere di nuovo il bottone?
                    DeactivateRaycast();
                    break;
                }
            }
        }
    }
    }
}
}



