using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class textArchitectTest : MonoBehaviour
{

    public TextArchitect architect;
    public Text text;
    public TextMeshProUGUI tmproText;

    public float speed = 0.5f;

    public bool useEncap = true;

    public int charactersPerFrame = 1;
    public bool useTmPRo = true;



    [TextArea(5, 10)]
    public string say;
    void Start()
    {
        architect = new TextArchitect(say);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            architect = new TextArchitect(say, "", charactersPerFrame, speed);
        }
    }
}