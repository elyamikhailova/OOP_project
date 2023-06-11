using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class textArchitectTest : MonoBehaviour
{

    public TextArchitect architect; 
    public TextMeshProUGUI tmprotext;

    [TextArea(5, 10)]
    public string say;
    public int charactersPerFrame = 1;
    public float speed = 0.5f;
    
    void Start()
    {
        architect = new TextArchitect(tmprotext, say, "", charactersPerFrame, speed);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            architect = new TextArchitect(tmprotext, say, "", charactersPerFrame, speed);
        }
    }
}