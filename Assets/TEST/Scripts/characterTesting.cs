using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterTesting : MonoBehaviour
{
    public Character Chugdaan;
    // Start is called before the first frame update
    void Start()
    {
        Chugdaan = CharacterManager.instance.GetCharacter("Chugdaan", enableCreatedCharacterOnStart: false );
    }

    public string[] speech;
    int i = 0;

    public Vector2 moveTarget;
    public float moveSpeed;
    public bool smooth;

    public int bodyIndex = 0;
    public float speed = 5f;
    public bool smoothTransition = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (i < speech.Length)
                Chugdaan.Say(speech[i]);
            else
                DialogueSystem.instance.Close();
            i++;
        }

        if (Input.GetKey(KeyCode.M))
        {
            Chugdaan.MoveTo(moveTarget, moveSpeed / 2, smooth);
        }

        if (Input.GetKey(KeyCode.S))
        {
            Chugdaan.StopMoving(true);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (Input.GetKey(KeyCode.T))
                Chugdaan.TransitionBody(Chugdaan.GetSprite(CharacterManager.characterBody.sad), speed, smoothTransition);
            else
                Chugdaan.SetBody(bodyIndex);
        }
    }
}
