using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Character;
using UnityEngine.UI;
using Unity.VisualScripting;

[System.Serializable]
public class Character 
{
    public string characterName;

    [HideInInspector] public RectTransform root;

    //public bool enable;

    DialogueSystem dialogue;

    public bool enabled{get{ return root.gameObject.activeInHierarchy;} set{root.gameObject.SetActive(value);}}

    public Vector2 anchorPadding { get { return root.anchorMax - root.anchorMin; } }
     
    public void Say(string speech, bool add = false)
    {
        if (!enabled)
            enabled = true;

        dialogue.Say(speech, characterName, add);
    }

    Vector2 targetPosition;
    Coroutine moving;
    bool IsMoving{get{ return moving != null;}}

    public void MoveTo(Vector2 target, float speed, bool smooth = true)
    {
        StopMoving();
        moving = CharacterManager.instance.StartCoroutine(Moving(target, speed, smooth));
    }

    public void StopMoving(bool arriveTargetPositionImmeediately = false)
    {
        if (IsMoving)
        {
            CharacterManager.instance.StopCoroutine(moving);
            if (arriveTargetPositionImmeediately)
                SetPosition(targetPosition);
        }

        moving = null;

    }

    public void SetPosition(Vector2 target)
    {
        targetPosition = target;
        Vector2 padding = anchorPadding;
        // get Anchors of the Character
        float maxX = 1f - padding.x;
        float maxY = 1f - padding.y;


        //now get the minimum anchors of the target position ( i.e: how much we are actually moving )

        Vector2 minAnchorTarget = new Vector2(maxX * targetPosition.x, maxY * targetPosition.y);
        root.anchorMin = minAnchorTarget;
        root.anchorMax = root.anchorMin + padding;

    }

    IEnumerator Moving(Vector2 target, float speed, bool smooth)
    {
        targetPosition = target;
        Vector2 padding = anchorPadding;
        // get Anchors of the Character
        float maxX = 1f - padding.x;
        float maxY = 1f - padding.y;

        //now get the minimum anchors of the target position ( i.e: how much we are actually moving )

        Vector2 minAnchorTarget = new Vector2(maxX * targetPosition.x, maxY * targetPosition.y);

        speed *= Time.deltaTime;

        while (root.anchorMin != minAnchorTarget)
        {
            if (!smooth)
            {
                root.anchorMin = Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed);
            }
            else
            {
                root.anchorMin = Vector2.Lerp(root.anchorMin, minAnchorTarget, speed);
            }

            root.anchorMax = root.anchorMin + padding;
            yield return new WaitForEndOfFrame();
        }
        StopMoving();
    }

    public Sprite GetSprie(int index = 0)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Characters/" + characterName);
        return sprites[index];
    }

    public void SetBody(int index)
    {
        renderers.bodyRenderer.sprite = GetSprie(index);
    }
    public void SetBody(Sprite sprite)
    {
        renderers.bodyRenderer.sprite = sprite;
    }

    bool isTransitioningBody { get { return transitioningBody != null; } }
    Coroutine transitioningBody = null;

    public void TransitionBody(Sprite sprite, float speed, bool smooth)
    {
        if (renderers.bodyRenderer.sprite == sprite)
            return;

        StopTransitioningBody();
        transitioningBody = CharacterManager.instance.StartCoroutine(TransitioningBody(sprite, speed, smooth));
    }

    void StopTransitioningBody()
    {
        if (isTransitioningBody)
            CharacterManager.instance.StopCoroutine(transitioningBody);
        transitioningBody = null;
    }

    public IEnumerator TransitioningBody(Sprite sprite, float speed, bool smooth)
    {
        for (int i = 0; i < renderers.allBodyRenderers.Count; i++)
        {
            Image image = renderers.allBodyRenderers[i];
            if (image.sprite == sprite)
            {
                renderers.bodyRenderer = image;
                break;
            }
        }

        if (renderers.bodyRenderer.sprite != sprite)
        {
            Image image = GameObject.Instantiate(renderers.bodyRenderer.gameObject, renderers.bodyRenderer.transform.parent).GetComponent<Image>();
            renderers.allBodyRenderers.Add(image);
            renderers.bodyRenderer = image;
            image.color = GlobalF.SetAlpha(image.color, 0f);
            image.sprite = sprite;
        }

        while (GlobalF.TransitionImages(ref renderers.bodyRenderer, ref renderers.allBodyRenderers, speed, smooth))
        {
            yield return new WaitForEndOfFrame();
        }

        StopTransitioningBody();
    }

    /// <summary>
    /// Create a new character
    /// </summary>
    /// <param name="_name"></param>
    public Character(string _name, bool enableOnStart = true)
    {
        CharacterManager cm = CharacterManager.instance;
        //locate the character prefab
        GameObject prefab = Resources.Load("Characters/Character[" + _name + "]") as GameObject;
        //spawn an instance of the prefab directly on the character panel.
        GameObject ob = GameObject.Instantiate(prefab, cm.characterPanel);

        root = ob.GetComponent<RectTransform>();
        characterName = _name;

        renderers.bodyRenderer = ob.transform.Find("BodyLayer").GetComponentInChildren<Image>();
        renderers.allBodyRenderers.Add(renderers.bodyRenderer);

        dialogue = DialogueSystem.instance;
        enabled = enableOnStart;

        //get the render(s)
    }

    [System.Serializable]
    public class Renderers
    {
        public Image bodyRenderer;

        public List<Image> allBodyRenderers = new List<Image>();
    }

    public Renderers renderers = new Renderers();

}
