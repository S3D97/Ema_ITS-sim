using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;
using UnityEngine.InputSystem;

public class RaycastController : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Camera mainCamera;
    public GameObject imageIcon;
    public Vector3 lineOffset;
    public float lineRendererDistance;
    public Color excludeColor;

    private int _interaction;
    private int _pointerLayer;
    private bool _doneTask = false;

    private bool objectInstanciated = false;

    public TextMeshProUGUI timerUi;
    public bool timerConfirm;
    public float remainingTime;
    public float timerTime;
    public float newTimerTime;
    public bool newTimerStart = false;
    
    public float placingObjectDistance = 1f;
    public GameObject doneButton;
    public GameObject exhibitionPanel;
    public GameObject exhibitionPanel1;
    public GameObject exhibitionPanel2;
    public GameObject[] solutionsObjects;
    public GameObject[] itemsButtons;
    
    private GameObject iconInstance;
    //public Collider iconCollider;
    private Vector3 iconhitpoint;


    
    private bool firstInstance = false;

    public bool objectPositioning = false;
    private GameObject currentObjectToPlace = null;

    public Vector3 myJsonPosition;

    public Color[] targetColors;
    public Dictionary<Color, GameObject> colorToIconMap = new Dictionary<Color, GameObject>();
    private Dictionary<Color, int> colorCounts = new Dictionary<Color, int>();

    private bool isRaycastActive = false;
    private List<ImageIconData> imageIconsData = new List<ImageIconData>();

    public SolutionObjectsDictionary solutionObjectsDictionary;

    void Start()
    {
        lineRenderer.enabled = false;
        firstInstance = true;
        _pointerLayer = LayerMask.GetMask("Pointer");

        foreach (var color in targetColors)
        {
            colorCounts[color] = 0;
        }

        exhibitionPanel.SetActive(true);
        _interaction = LayerMask.GetMask("Interaction");
        _pointerLayer = LayerMask.GetMask("Pointer");
        remainingTime = timerTime;
        doneButton.GetComponent<Button>().onClick.AddListener(DeactivateRaycast);

        // Bottoni UI solutionObjects
        for (int i = 0; i < itemsButtons.Length; i++)
        {
            int index = i; 
            itemsButtons[i].GetComponent<Button>().onClick.AddListener(() => SelectObjectToPlace(index));
        }
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
                HandleRaycastClick();
            }
        }

        if (newTimerStart)
        {
            NewTimerPointerPhase();
        }
    }

    private void HandleRaycastClick()
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
                if (renderer != null)
                {
                 foreach (var targetColor in targetColors)
                {
                    if (ColorsAreEqual(hitColor, targetColor) && firstInstance == true)
                    {
                        doneButton.SetActive(true);
                        iconInstance = Instantiate(imageIcon, hit.point, Quaternion.identity);
                        //iconInstance.layer = LayerMask.NameToLayer("Pointer");
                        //iconInstance.tag = "Pointer";
                        iconInstance.transform.position = hit.point;
                        iconhitpoint = hit.point;
                        colorCounts[targetColor]++;
                        imageIconsData.Add(new ImageIconData
                        {
                            Position = hit.point,
                            Color = new SerializableColor(targetColor)
                        });
                        Debug.Log($"Colore {targetColor} selezionato. Conteggio: {colorCounts[targetColor]}");
                        break;
                    }
                }   
                }
                
            }

            if (_doneTask || !timerConfirm)
            {
                DeactivateRaycast();
            }

            if (objectPositioning && currentObjectToPlace != null)
            {
                InstantiateSolutionObject(currentObjectToPlace, hit.point);
                currentObjectToPlace = null;
                objectPositioning = false;
                //DeactivateRaycast();
                
            }
        }
        
            Collider iconCollider = iconInstance.GetComponent<BoxCollider>();
            if (Physics.Raycast(ray, out hit, lineRendererDistance, _pointerLayer)) //&& iconhitpoint == hitPoint
            {
               /*  if (hit.collider.tag == "Pointer")
                {
                    Destroy(iconInstance);
                    Debug.Log("pointer Distrutto"); 
                } */
                
                
                Debug.Log("Hit object: " + hit.collider.gameObject.name);
                if (hit.collider == iconCollider && firstInstance == false)
                {
                    if (objectInstanciated == true)
                    {
                        Destroy(iconInstance);
                        Debug.Log("pointer Distrutto"); 
                        objectInstanciated = false;   
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
        if (lineRenderer.enabled)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            lineRenderer.SetPosition(0, ray.origin + lineOffset);
            lineRenderer.SetPosition(1, ray.origin + ray.direction * lineRendererDistance);
        }
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
        Debug.Log("Dati json salvati correttamente");
    }

    public void DoneTask()
    {
        _doneTask = true;
    }

    public void NewTimerStart()
    {
        newTimerStart = true;
    }

    private void TimerStopped()
    {
        exhibitionPanel1.SetActive(true);
        foreach (var ObjectButton in solutionsObjects)
        {
            if (ObjectButton != null)
            {
                ObjectButton.SetActive(true);
            }
        }

        remainingTime = newTimerTime;
        timerConfirm = true;
        timerUi.enabled = true;
       
        
    }

    private void NewTimerPointerPhase()
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
                DeactivateRaycast();
                newTimerStart = false;
                
            }
        }
    }

    
    public void SelectObjectToPlace(int index)
    {
        firstInstance = false;
        if (index >= 0 && index < solutionsObjects.Length)
        {
            string path = Application.persistentDataPath + "/ImageIconsData.json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                ImageIconsDataList loadedData = JsonUtility.FromJson<ImageIconsDataList>(json);
                GameObject solutionObject = solutionsObjects[index];

                foreach (var data in loadedData.Icons)
                {
                    Vector3 jsonPosition = data.Position;
                    myJsonPosition = data.Position;
                    Color jsonColor = data.Color;
                    foreach (var entry in solutionObjectsDictionary.SolutionObjectsNames)
                    {
                        if (ColorsAreEqual(jsonColor, entry.Key) && firstInstance == false)
                        {
                            currentObjectToPlace = solutionObject;
                            myJsonPosition = data.Position;
                            ActivateRaycast();
                            objectPositioning = true;
                            return;
                        }   
                    }
                    
                }

                Debug.LogError("Nessun colore corrispondente trovato nel JSON.");
            }
        }
    }

    public void InstantiateSolutionObject(GameObject solutionObject, Vector3 hitPoint)
    {
        if (objectPositioning == true && Vector3.Distance(myJsonPosition, hitPoint) < placingObjectDistance) // confronto distanza vector3 con tolleranza
        {
            /* if (iconInstance) //&& iconhitpoint == hitPoint
            {
                Destroy(iconInstance);
                Debug.Log("pointer Distrutto");
            } */
            GameObject solutionObjectInstance = Instantiate(solutionObject, hitPoint, Quaternion.identity);
            solutionObjectInstance.SetActive(true);
            solutionObjectInstance.transform.position = hitPoint;
            objectInstanciated = true;
            
            Debug.Log("Oggetto posizionato correttamente");
        }
        else
        {
            Debug.LogError("Posizionamento oggetto fallito. Colore o posizione non corrispondono.");
        }
        if (newTimerStart == false && remainingTime == 0)
        {
            exhibitionPanel2.SetActive(true);
            DeactivateRaycast();
        }
    }
}

[System.Serializable]
public class ImageIconData
{
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

    public static implicit operator Color(SerializableColor serializableColor)
    {
        return new Color(serializableColor.r, serializableColor.g, serializableColor.b, serializableColor.a);
    }

    public static implicit operator SerializableColor(Color color)
    {
        return new SerializableColor(color);
    }
}

[System.Serializable]
public class ImageIconsDataList
{
    public List<ImageIconData> Icons;
}





