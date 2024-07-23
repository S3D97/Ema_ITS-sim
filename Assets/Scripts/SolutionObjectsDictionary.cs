using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class SolutionObjectsDictionary : MonoBehaviour
{
   
    [SerializeField]
   SolutionDictItem[] solutionDict;
   public Dictionary<Color, GameObject> SolutionObjectsNames { get; private set; }

   //Dictionary<Color, GameObject> SolutionObjectsNames;

   private void Start()
   {
    SolutionObjectsNames = new Dictionary<Color, GameObject>();
        foreach (var item in solutionDict)
        {
            SolutionObjectsNames.Add(item.referenceColor, item.referenceObj);
        }

    }


}

[Serializable]
public class SolutionDictItem
{
    [SerializeField]
    public GameObject referenceObj;
    
    [SerializeField]
    public Color referenceColor;
}


[Serializable]
public class NewDict
{
   [SerializeField]
   SolutionDictItem[] solutionDict; 

   

   public Dictionary<Color, GameObject> ToDictionary()
   {
    
    Dictionary<Color, GameObject> solutionDict = new Dictionary<Color, GameObject>();
    

    foreach (var item in solutionDict)
    {
       solutionDict.Add(item.Key, item.Value); 
    }

    return solutionDict;
   }
}




