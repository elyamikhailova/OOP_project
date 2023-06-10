using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Respon
/// </summary>
public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;

    //Panel References
    public RectTransform characterPanel;

    // List of All Characters in the scene.
    public List<Character> characters = new List<Character>();

    /// <summary>
    /// Easy lookup for our characters.
    /// </summary>
    public Dictionary<string, int> characterDictionary = new Dictionary<string, int>();

    void Awake()
    {
        instance = this;
    }

    //Used to search charcterDictionary and return the specified character from characters
    public Character GetCharacter(string characterName, bool createCharacterIfDoesNotExist = true, bool enableCreatedCharacterOnStart = true)
    {
        //search our dictionary to find out the characther quickly if it is already in our scene
        int index = -1;
        if (characterDictionary.TryGetValue(characterName, out index))
        {
            return characters[index];
        }

        else if (createCharacterIfDoesNotExist)
        {
            return CreateCharacter(characterName, enableCreatedCharacterOnStart);
        }

        return null;
    }

    public Character CreateCharacter(string characterName, bool enabledOnStart = true)
    {
        Character newCharacter = new Character(characterName, enabledOnStart);
        characterDictionary.Add(characterName, characters.Count);
        characters.Add(newCharacter);

        return newCharacter;
    }

    public class CHARACTERPOSITIONS
    {
        public Vector2 bottomLeft = new Vector2(0, 0);
        public Vector2 topLeft = new Vector2(0, 1f);
        public Vector2 center = new Vector2(0.5f, 0.5f);
        public Vector2 bottomRight = new Vector2(1f, 0);
        public Vector2 topRight = new Vector2(1f, 1f);
    }
    public static CHARACTERPOSITIONS characterPositions = new CHARACTERPOSITIONS();

    public class CHARACTERBODY
    {
        public int usual = 0;
        public int sword = 1;
        public int helm = 2;
        public int attack = 3;
        public int side = 4;
        public int sad = 5;
    }
    public static CHARACTERBODY characterBody = new CHARACTERBODY();
}
