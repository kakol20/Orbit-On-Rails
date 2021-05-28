using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    public static GlobalVariables Instance { get; private set; }

    public float timeScale = 5000;

    private void Awake()
    {
        Instance = this;
    }
}
