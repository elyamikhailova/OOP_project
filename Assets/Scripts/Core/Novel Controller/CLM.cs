using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLM : MonoBehaviour
{
    public static LINE Interpret(string rawLine)
    {
        return new LINE(rawLine);
    }
    public class LINE
    {
        public string speaker = ""; // who is speaking on this line

        public List<SEGMENT> segments = new List<SEGMENT>(); // rawline that is split into segments

        public List<string> actions = new List<string>(); // actions in the rawline

        public string LastSegmentWholeDialog = "";
        public LINE(string rawLine)
        {
            //Debug.Log(rawLine);
            string[] dialogueAndActions = rawLine.Split('"');
            char actionSpliiter = ' ';
            string[] actionArray = dialogueAndActions.Length == 3 ? dialogueAndActions[2].Split(actionSpliiter) : dialogueAndActions[0].Split(actionSpliiter);

            //Debug.Log(dialogueAndActions[0]);
            //Debug.Log(dialogueAndActions.Length);

            if (dialogueAndActions.Length == 3) //contains dialogue
            {
                speaker = dialogueAndActions[0] == "" ? NovelController.instance.cachedLastSpeaker : dialogueAndActions[0];
                if (speaker[speaker.Length - 1] == ' ')
                {
                    speaker = speaker.Remove(speaker.Length - 1);
                }
                NovelController.instance.cachedLastSpeaker = speaker;
                //segment dialog
                SegmentDialog(dialogueAndActions[1]);
            }
            //else
            {
                //for each action, place it inside the action list
                for (int i = 0; i < actionArray.Length; i++)
                {
                    actions.Add(actionArray[i]);
                }
            }
        }

        void SegmentDialog(string dialogue)
        {
            segments.Clear();
            string[] parts = dialogue.Split('{', '}');

            for (int i = 0; i < parts.Length; i++)
            {
                //segmenters/commands will always be odd indexted, dialogue will always be even
                SEGMENT segment = new SEGMENT();
                //segments.Add(segment);
                bool isOdd = i % 2 != 0;
                if (isOdd)
                {
                    //commands and data are split by spaces
                    string[] commandData = parts[i].Split(' ');
                    //by input we mean user input
                    switch (commandData[0])
                    {
                        case "c": //wait for input and clear
                            segment.trigger = SEGMENT.TRIGGER.WaitforClick;
                            break;
                        case "a": //wait for input and append(additive)
                            segment.trigger = SEGMENT.TRIGGER.WaitforClick;
                            //appending requires fetching the text of the previous segment for the preText
                            segment.preText = segments.Count > 0 ? segments[segments.Count-1].dialogue : "";
                            break;
                        case "w": //wait for set time and clear
                            segment.trigger = SEGMENT.TRIGGER.autoDelay;
                            segment.autoDelay = float.Parse(commandData[1]);
                            break;
                        case "wa"://wait for set time and append
                            segment.trigger = SEGMENT.TRIGGER.autoDelay;
                            segment.autoDelay = float.Parse(commandData[1]);
                            segment.preText = segments.Count > 0 ? segments[segments.Count - 1].dialogue : "";
                            break;
                    }
                    i++;
                    //increment so we move past the command and to the next bit of dialogue
                }
                segment.dialogue = parts[i];
                //lastDialogue = parts[i];
                segment.line = this;
                segments.Add(segment);
            }
        }

        public string printCurrentSegments()
        {
            string segText = "";

            foreach (SEGMENT s in segments)
            {
                segText = segText + s.dialogue + "\n";
            }

            return segText;
        }

        public class SEGMENT
        {
            public LINE line;
            public string dialogue = "";
            public string preText = "";

            //enumerations are like constants
            public enum TRIGGER{WaitforClick, waitClickClear, autoDelay}
            public TRIGGER trigger = TRIGGER.WaitforClick;

            public float autoDelay = 0;

            public void Run()
            {
                if (running != null)
                {
                    NovelController.instance.StopCoroutine(running);
                }
                running = NovelController.instance.StartCoroutine(Running());
            }

            public bool isRunning{get{return running != null;}}
            Coroutine running = null;
            public TextArchitect architect = null;

            IEnumerator Running()
            {
                if (line.speaker != "narrator")
                {
                    Character character = CharacterManager.instance.GetCharacter(line.speaker);
                    character.Say(dialogue, preText != "");
                }
                else
                {
                    DialogueSystem.instance.Say(dialogue, line.speaker, preText != "") ;
                }

                architect = DialogueSystem.instance.currentArchitect;

                while (architect.isConstructing)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                running = null;
            }

            //if we are runing the segment, force it to stop
            public void forceFinish()
            {
                if (running != null)
                {
                    NovelController.instance.StopCoroutine(running);
                    running = null;
                }

                if (architect != null)
                {
                    architect.forceFinish();
                }
            }

        }
    }
}