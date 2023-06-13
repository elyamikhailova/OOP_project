using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovelController : MonoBehaviour
{
    public static NovelController instance;

    List<string> data = new List<string>();

    int chapterProgress = 0;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadChapterFile("Chapter1.txt");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Next();
        }
    }

    void LoadChapterFile(string filename)
    {
        data = FileManager.LoadFile(FileManager.savPath + "Resources/Story/Chapters/" + filename);
        cachedLastSpeaker = "";

        if (handlingChapterFile != null)
        {
            StopCoroutine(handlingChapterFile);
        }
        handlingChapterFile = StartCoroutine(HandlingChapterFile());

        Next(); //autostart the chapter
    }


    Coroutine handlingChapterFile = null;

    public bool isHandlingChapterFile
    {
        get
        {
            return handlingChapterFile != null;
        }
    }

    bool _next = false;

    public void Next()
    {
        _next = true;
    }

    IEnumerator HandlingChapterFile()
    {
        //the progress through the lines inside the chapter
        chapterProgress = 0;

        while (chapterProgress < data.Count)
        {
            //need a way of knowing when the next trigger will appear.
            //there should be multiple types of triggers
            if (_next)
            {
                HandleLine(data[chapterProgress]);
                chapterProgress++;
                while (isHandlingLine())
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    void HandleLine(string rawLine)
    {
        CLM.LINE line = CLM.Interpret(rawLine);
        //now we need to handle the line, which requires a loop that handles each segment individually
        StopHandlingLine();
        handlingLine = StartCoroutine(HandlingLine(line));
    }

    void StopHandlingLine()
    {
        if (isHandlingLine())
        {
            StopCoroutine(handlingLine);
        }

        handlingLine = null;
    }

    [HideInInspector]
    public string cachedLastSpeaker = "";

    Coroutine handlingLine = null;
    public bool isHandlingLine()
    {
        return handlingLine != null;
    }

    IEnumerator HandlingLine(CLM.LINE line)
    {
        //since the next trigger controls the flow of a chapter by moving through lines and controls
        //progression through a line by its segments, it must be reset

        _next = false;
        int lineProgress = 0; //progress through the segments of a line

        while (lineProgress < line.segments.Count)
        {
            _next = false; //reset _next
            CLM.LINE.SEGMENT segment = line.segments[lineProgress];

            //always run the first segment automatically. But wait for trigger on all proceeding segments
            if (lineProgress > 0)
            {
                if (segment.trigger == CLM.LINE.SEGMENT.TRIGGER.autoDelay)
                {
                    for (float timer = segment.autoDelay; timer >= 0; timer = timer - Time.deltaTime)
                    {
                        yield return new WaitForEndOfFrame();
                        if (_next)
                        {
                            break;  //termiante loop if player chooses to skip via trigger. Prevents unskippable wait timers.
                        }
                    }
                }

                else
                {
                    //wait until the player says to move to the next segment
                    while (!_next)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
            _next = false; //next could have been triggered during an event above
            //the segment now needs to build and run.
            segment.Run();

            while (segment.isRunning)
            {
                yield return new WaitForEndOfFrame();
                //allow for auto-completion for skipping purposes, if that is decided
                //so _next is TRUE if the user is skipping
                if (_next)
                {
                    //if the architect is not already set to skip, set it to skip first
                    if (!segment.architect.skip)
                    {
                        segment.architect.skip = true;
                    }
                    //on the next call to skip, force the segment to finish
                    else
                    {
                        segment.forceFinish();
                    }

                    _next = false;
                }

            }
            lineProgress++;

            yield return new WaitForEndOfFrame();
        }
        //add actions to Line
        for (int i = 0; i < line.actions.Count; i++)
        {
            HandleAction(line.actions[i]);
        }
        handlingLine = null;

    }

    

//ACTIONS
//\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

    public void HandleAction(string action)
    {
        string[] data = action.Split('(', ')');

        switch (data[0])
        {
            case ("setBackground"):
                Command_SetLayerImage(data[1], BCFC.instance.background);
                break;
            case ("setCinematic"):
                Command_SetLayerImage(data[1], BCFC.instance.cinematic);
                break;
            case ("setForeground"):
                Command_SetLayerImage(data[1], BCFC.instance.foreground);
                break;
            case ("playSound"):
                Command_PlaySound(data[1]);
                break;
            case ("playMusic"):
                Command_PlayMusic(data[1]);
                break;
            case ("move"):
                Command_MoveCharacter(data[1]);
                break;
            case ("setPosition"):
                Command_SetPosition(data[1]);
                break;
            case ("setBody"):
                Command_SetBody(data[1]);
                break;
            case ("flip"):
                Command_Flip(data[1]);
                break;
            case ("faceLeft"):
                Command_FaceLeft(data[1]);
                break;
            case ("faceRight"):
                Command_FaceRight(data[1]);
                break;
            case ("transBackground"):
                Command_TransLayer(BCFC.instance.background, data[1]);
                break;
            case ("transCinematic"):
                Command_TransLayer(BCFC.instance.cinematic, data[1]);
                break;
            case ("transForeground"):
                Command_TransLayer(BCFC.instance.foreground, data[1]);
                break;
            case ("showScene"):
                Command_ShowScene(data[1]);
                break;

        }
    }

    void Command_SetLayerImage(string data, BCFC.LAYER layer)
    {
        string texName = data.Contains(",") ? data.Split(',')[0] : data;
        Texture2D tex = texName == "null" ? null : Resources.Load("Images/UI/Backdrops/" + texName) as Texture2D;
        float spd = 2f;
        bool smooth = false;

        if (data.Contains(","))
        {
            string[] parameters = data.Split(',');
            foreach (string p in parameters)
            {
                float fVal = 0;
                bool bVal = false;

                if (float.TryParse(p, out fVal))
                {
                    spd = fVal; continue;
                }

                if (bool.TryParse(p, out bVal))
                {
                    smooth = bVal; continue;
                }
            }
        }

        layer.TransitionToTexture(tex, spd, smooth);
    }

    void Command_PlaySound(string data)
    {
        AudioClip clip = Resources.Load("Audio/SFX/" + data) as AudioClip;
        if (clip != null)
            AudioManager.instance.PlaySFX(clip);
        else
            Debug.LogError("Clip does not exist - " + data);
    }

    void Command_PlayMusic(string data)
    {
        AudioClip clip = Resources.Load("Audio/Music/" + data) as AudioClip;
        if (clip != null)
            AudioManager.instance.PlaySong(clip);
        else
            Debug.LogError("Clip does not exist - " + data);
    }

    void Command_MoveCharacter(string data)
    {
        string[] parameters = data.Split(',');
        string character = parameters[0];
        float locationX = float.Parse(parameters[1]);
        float locationY = parameters.Length >= 3 ? float.Parse(parameters[2]) : 0;
        float speed = parameters.Length >= 4 ? float.Parse(parameters[3]) : 5f;
        bool smooth = parameters.Length == 5 ? bool.Parse(parameters[4]) : true;

        Character c = CharacterManager.instance.GetCharacter(character);
        c.MoveTo(new Vector2(locationX, locationY), speed, smooth);
    }

    void Command_SetPosition(string data)
    {
        string[] parameters = data.Split(',');
        string character = parameters[0];
        float locationX = float.Parse(parameters[1]);
        float locationY = float.Parse(parameters[2]);

        Character c = CharacterManager.instance.GetCharacter(character);
        c.SetPosition(new Vector2(locationX, locationY));
    }

    void Command_SetBody(string data)
    {
        string[] parameters = data.Split(',');
        string character = parameters[0];
        string expression = parameters[1];
        float speed = parameters.Length == 3 ? float.Parse(parameters[2]) : 5f;

        Character c = CharacterManager.instance.GetCharacter(character);
        Sprite sprite = c.GetSprie(expression);
        c.TransitionBody(sprite, speed, false);
    }

    void Command_Flip(string data)
    {
        string[] characters = data.Split(',');

        foreach (string s in characters)
        {
            Character c = CharacterManager.instance.GetCharacter(s);
            c.Flip();
        }
    }

    void Command_FaceLeft(string data)
    {
        string[] characters = data.Split(',');

        foreach (string s in characters)
        {
            Character c = CharacterManager.instance.GetCharacter(s);
            c.FaceLeft();
        }
    }

    void Command_FaceRight(string data)
    {
        string[] characters = data.Split(',');

        foreach (string s in characters)
        {
            Character c = CharacterManager.instance.GetCharacter(s);
            c.FaceRight();
        }
    }

    void Command_TransLayer(BCFC.LAYER layer, string data)
    {
        string[] parameters = data.Split(',');

        string texName = parameters[0];
        string transTexName = parameters[1];
        Texture2D tex = texName == "null" ? null : Resources.Load("Images/UI/Backdrops/" + texName) as Texture2D;
        Texture2D transTex = Resources.Load("Images/TransitionEffects/" + transTexName) as Texture2D;

        float spd = 2f;
        bool smooth = false;

        for (int i = 2; i < parameters.Length; i++)
        {
            string p = parameters[i];
            float fVal = 0;
            bool bVal = false;
            if (float.TryParse(p, out fVal))
            { spd = fVal; continue; }
            if (bool.TryParse(p, out bVal))
            { smooth = bVal; continue; }
        }

        TransitionMaster.TransitionLayer(layer, tex, transTex, spd, smooth);
    }

    void Command_ShowScene(string data)
    {
        string[] parameters = data.Split(',');
        bool show = bool.Parse(parameters[0]);
        string texName = parameters[1];
        Texture2D transTex = Resources.Load("Images/TransitionEffects/" + texName) as Texture2D;

        float spd = 2f;
        bool smooth = false;

        for (int i = 2; i < parameters.Length; i++)
        {
            string p = parameters[i];
            float fVal = 0;
            bool bVal = false;
            if (float.TryParse(p, out fVal))
            { spd = fVal; continue; }
            if (bool.TryParse(p, out bVal))
            { smooth = bVal; continue; }
        }

        TransitionMaster.ShowScene(show, spd, smooth, transTex);
    }
    void Command_Exit(string data)
    {
        string[] parameters = data.Split(',');
        string[] characters = parameters[0].Split(';');
        float speed = 3;
        bool smooth = false;

        for (int i = 1; 1 < parameters.Length; i++)
        {
            float fVal = 0; bool bVal = false;
            if (float.TryParse(parameters[i], out fVal))
            {
                speed = fVal; continue;
            }
            if (bool.TryParse(parameters[i], out bVal))
            {
                smooth = bVal; continue;
            }
        }

        foreach (string s in characters)
        {
            Character c = CharacterManager.instance.GetCharacter(s);
            c.FadeOut(speed, smooth);
        }
    }

    void Command_Enter(string data)
    {
        string[] parameters = data.Split(',');
        string[] characters = parameters[0].Split(';');
        float speed = 3;
        bool smooth = false;

        for (int i = 1; 1 < parameters.Length; i++)
        {
            float fVal = 0; bool bVal = false;
            if (float.TryParse(parameters[i], out fVal))
            {
                speed = fVal; continue;
            }
            if (bool.TryParse(parameters[i], out bVal))
            {
                smooth = bVal; continue;
            }
        }

        foreach (string s in characters)
        {
            Character c = CharacterManager.instance.GetCharacter(s, true, false);
            if (!c.enabled)
            {
                c.renderers.bodyRenderer.color = new Color(1, 1, 1, 0);
                c.enabled = true;

                c.TransitionBody(c.renderers.bodyRenderer.sprite, speed, smooth);
            }
            else
            {
                c.FadeIn(speed, smooth);
            }
        }
    }
}