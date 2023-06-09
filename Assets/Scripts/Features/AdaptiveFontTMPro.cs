using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdaptiveFontTMPro : MonoBehaviour
{

    TextMeshProUGUI txt;        //Text component we will be controlling
    public bool continualUpdate = true;
    // Start is called before the first frame update

    public int fontSizeAtDefaultResolution = 40;
    public static float defaultResolution = 2525f;

    void Start()
    {
        txt = GetComponent<TextMeshProUGUI >();

        if (continualUpdate)
        {
            InvokeRepeating("Adjust", 0f, Random.Range(0.2f, 1f));
        }
        else
        {
            Adjust();
            enabled = false;
        }
    }

    void Adjust()
    {
        if (!enabled || !gameObject.activeInHierarchy)
        {
            return;
        }

        float totalCurrentRes = Screen.height + Screen.width;

        float perc = totalCurrentRes / defaultResolution;
        int fontsize = Mathf.RoundToInt((float)fontSizeAtDefaultResolution * perc);

        txt.fontSize = fontsize;

    }
}