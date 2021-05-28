using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //[SerializeField] private string Tag
    private GameObject[] orbitLines;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPostRender()
    {
        //orbitLines = GameObject.FindGameObjectsWithTag("OrbitLine");

        //foreach (GameObject item in orbitLines)
        //{
        //    item.GetComponent<OrbitLineRenderer>().DrawLines();
        //}
    }
}
