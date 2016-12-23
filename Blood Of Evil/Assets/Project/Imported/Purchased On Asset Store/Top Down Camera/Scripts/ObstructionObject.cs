using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public sealed class ObstructionObj
{
    public GameObject obj;  
    public List<ObstructionMat> materials;   
    public bool activateFade;
    public float tValue;
}