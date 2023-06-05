using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance;

    public ELEMENTS elements;

    void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Say(string speech, string speaker)
    {
        if (isSpeaking)
        {
            StopSpeaking();
            speaking = StartCoroutine(Speaking(speech, speaker));
        }
    }

    public void StopSpeaking()
    {
        if (isSpeaking)
        {
            StopCoroutine(speaking);
        }
        speaking = null;
    }

    public bool isSpeaking { get { return speaking != null; } }
    [HideInInspector] public bool isWaitingForUserInput = false;

    Coroutine speaking = null;
    IEnumerator Speaking(string targetSpeesch, string speaker)
    {
        speechPanel.SetActive(true);
        speechText.text = "";
        speakerNameText.text = speaker;
        isWaitingForUserInput = false;

        while (speechText.text != targetSpeesch)
        {
            speechText.text += targetSpeesch[speechText.text.Length];
            yield return new WaitForEndOfFrame();
        }

        isWaitingForUserInput = true;
        while (isWaitingForUserInput)
        {
            yield return new WaitForEndOfFrame();
        }

        StopSpeaking();
    }

    [System.Serializable]
    public class ELEMENTS
    {
        public GameObject speechPanel;
        public TMP_Text speakerNameText;
        public TMP_Text speechText;
    }

    public GameObject speechPanel { get { return elements.speechPanel; } }
    public TMP_Text speakerNameText { get { return elements.speakerNameText; } }
    public TMP_Text speechText { get { return elements.speechText; } }
}