using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    public float depthPower = -0.02f;
    
    void Start()
    {
        
    }

    
    void Update()
    {
      //var mpos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
      var mpos = Input.mousePosition;
      mpos.x = (mpos.x / Screen.width*2-1)* -depthPower;
      mpos.y = (mpos.y / Screen.height*2-1)* -depthPower;
      Shader.SetGlobalVector("_MousePos", new Vector4(mpos.x, mpos.y, 0, 0));
    }
}
