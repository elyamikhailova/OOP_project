using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSavePanel : MonoBehaviour
{
    public static GameSavePanel instance;

    public List<BUTTON> buttons = new List<BUTTON>();

    [HideInInspector]
    public int currentSaveLoadPage = 1;

    public CanvasGroup canvasGroup;

    public Button loadButton;
    public Button saveButton;
    public Button deleteButton;

    public enum TASK
    {
        saveToSlot,
        loadFromSlot,
        deleteSlot
    }
    public TASK slotTask = TASK.loadFromSlot;

    void Awake()
    {
        instance = this;
    }

    public void LoadFilesOntoScreen(int page = 1)
    {
        currentSaveLoadPage = page;

        string directory = FileManager.savPath + "savData/gameFiles/" + page.ToString() + "/";

        if (System.IO.Directory.Exists(directory))
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                BUTTON b = buttons[i];
                string expectedFile = directory + (i + 1).ToString() + ".txt";
                if (System.IO.File.Exists(expectedFile))
                {
                    GAMEFILE file = FileManager.LoadEncryptedJSON<GAMEFILE>(expectedFile, FileManager.keys);

                    b.button.interactable = true;
                    byte[] previewImageData = FileManager.LoadComposingBytes(directory + (i + 1).ToString() + ".png");
                    Texture2D previewImage = new Texture2D(2, 2);
                    ImageConversion.LoadImage(previewImage, previewImageData);
                    file.previewImage = previewImage;
                    b.previewDisplay.texture = file.previewImage;

                    //need to read date and time information from file.
                    b.dateTimeText.text = /*page.ToString() + "\n" + */file.modificationDate;
                }
                else
                {
                    b.button.interactable = allowSavingFromThisScreen;
                    b.previewDisplay.texture = Resources.Load<Texture2D>("Images/UI/EmptyGameFile");
                    b.dateTimeText.text = /*page.ToString() + "\n" + */"empty file...";
                }
            }
        }
        else
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                BUTTON b = buttons[i];
                b.button.interactable = allowSavingFromThisScreen;
                b.previewDisplay.texture = Resources.Load<Texture2D>("Images/UI/EmptyGameFile");
                b.dateTimeText.text = /*page.ToString() + "\n" + */"empty file...";
            }
        }
    }

    [HideInInspector]
    public BUTTON selectedButton = null;
    string selectedGameFile = "";
    string selectedFilePath = "";
    public bool allowLoadingFromThisScreen = true;
    public bool allowSavingFromThisScreen = true;
    public bool allowDeletingFromThisScreen = true;

    public void ClickOnSaveSlot(Button button)
    {
        foreach(BUTTON B in buttons)
        {
            if (B.button == button)
                selectedButton = B;
        }

        selectedGameFile = currentSaveLoadPage.ToString() + "/" + (buttons.IndexOf(selectedButton) + 1).ToString();
        selectedFilePath = FileManager.savPath + "savData/gameFiles/" + selectedGameFile + ".txt";

        //Run an error chech just to be sure the file has not been removed since load.
        if (System.IO.File.Exists(selectedFilePath))
        {
            loadButton.interactable = allowLoadingFromThisScreen;
            saveButton.interactable = allowSavingFromThisScreen;
            deleteButton.interactable = allowDeletingFromThisScreen;
        }
        else
        {
            selectedButton.dateTimeText.text = "<color=red>FILE NOT FOUND";
            loadButton.interactable = false;
            saveButton.interactable = allowSavingFromThisScreen;
            deleteButton.interactable = true;
        }
    }

    public void LoadFromSelectedSlot()
    {
        GAMEFILE file = FileManager.LoadEncryptedJSON<GAMEFILE>(selectedFilePath, FileManager.keys);

        FileManager.SaveFile(FileManager.savPath + "savData/file", selectedGameFile);

        UnityEngine.SceneManagement.SceneManager.LoadScene("Novel");

        gameObject.SetActive(false); //deactivate the panel after loading
    }

    public void ClosePanel()
    {
        if (gameObject.activeInHierarchy)
            GetComponent<Animator>().SetTrigger("deactivate");
    }

    public void SaveToSelectedSlot()
    {
        if (NovelController.instance != null)
        {
            savingFile = StartCoroutine(SavingFile());
        }
    }

    Coroutine savingFile = null;
    IEnumerator SavingFile()
    {
        NovelController.instance.activeGameFileName = selectedGameFile;

        GetComponent<Animator>().SetTrigger("instantInvisible");
        yield return new WaitForEndOfFrame();

        //a screenshot is made during this point
        NovelController.instance.SaveGameFile();

        selectedButton.dateTimeText.text = /*currentSaveLoadPage.ToString() + "\n" + */GAMEFILE.activeFile.modificationDate;

        yield return new WaitForEndOfFrame();
        
        GetComponent<Animator>().SetTrigger("instantVisible");

        savingFile = null;
    }

    public void DeleteSlot()
    {
        print("We'll do this later");
    }

    [System.Serializable]
    public class BUTTON
    {
        public Button button;
        public RawImage previewDisplay;
        public TextMeshProUGUI dateTimeText;
    }
}
