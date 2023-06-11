using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    class representing the higlight game object (ui component)
*/
public class Highlight : MonoBehaviour
{
    [SerializeField]
    Color highlightColor;
    Material material;

    private void Start() 
    {
        material = GetComponent<MeshRenderer>().material;
        material.color=highlightColor;        
    }
    private void OnDestroy() 
    {
        Destroy(material);
    }
}
