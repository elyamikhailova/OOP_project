using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class textArchitectTest : MonoBehaviour
{

    public TextArchitect architect;
    public Text text;
    public TextMeshProUGUI tmprotext;

    [TextArea(5, 10)]
    public string say;
    public int charactersPerFrame = 1;
    public float speed = 0.5f;
    public bool useEncap = true;
    public bool useTMPRo = true;
    
    void Start()
    {
        architect = new TextArchitect(say, "", charactersPerFrame, speed,useEncap, useTMPRo);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            architect = new TextArchitect(say, "", charactersPerFrame, speed, useEncap, useTMPRo);
        }

        if (useTMPRo)
            tmprotext.text = architect.currentText;
    }
}