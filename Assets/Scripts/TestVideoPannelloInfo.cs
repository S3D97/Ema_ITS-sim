using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVideoPannelloInfo : MonoBehaviour
{
    public GameObject panelInfo;
    private bool isPanelVisible = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if (Input.GetKeyDown(KeyCode.J))
        {
            isPanelVisible = !isPanelVisible;
            panelInfo.SetActive(isPanelVisible);
        }  
    }
}
