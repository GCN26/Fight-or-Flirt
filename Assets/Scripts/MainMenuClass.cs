using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuClass : MonoBehaviour
{
    public TMP_InputField input;
    public GameObject namePanel, classPanel, optionsPanel, buttonsObj,creditsPanel;
    public Button newGameButton, loadGameButton, optionsButton, quitButton;
    public Slider masterS, musicS, sfxS;

    private void OnEnable()
    {
        SoundSliders.loadVolPrefs();
        masterS.value = SoundSliders.masterVol;
        musicS.value = SoundSliders.musicVol;
        sfxS.value = SoundSliders.sfxVol;
        string loadString = "";
        string path = Path.Combine(Application.persistentDataPath, "save.dat");
        using (StreamReader saveFile = new StreamReader(path))
        {
            loadString = saveFile.ReadToEnd();
            saveFile.Close();
        }

        if (loadString != "") loadGameButton.interactable = true;
    }

    public void selectClass(int inputInt)
    {
        SceneIndependentClass.charName = input.text;
        SceneIndependentClass.classInt = inputInt;
        SceneManager.LoadScene("Level1");
    }

    public void openNamePanel()
    {
        namePanel.SetActive(true);
        newGameButton.gameObject.SetActive(false);
        loadGameButton.gameObject.SetActive(false);
        optionsButton.gameObject.SetActive(false);
    }

    public void openClassPanel()
    {
        if (input.text != "")
        {
            namePanel.SetActive(false);
            classPanel.SetActive(true);
        }
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void tryLoadGame()
    {
        saveData loadObject = new();
        string loadString;
        string path = Path.Combine(Application.persistentDataPath, "save.dat");
        using (StreamReader saveFile = new StreamReader(path))
        {
            loadString = saveFile.ReadToEnd();
            saveFile.Close();
        }
        JsonUtility.FromJsonOverwrite(loadString, loadObject);

        if (loadObject.currentScene == "Level1" || loadObject.currentScene == "Level2")
        {
            LoadedGame.loadGame = true;
            SceneManager.LoadScene(loadObject.currentScene);
        }
    }
    public void changeVol()
    {
        SoundSliders.changeVolSFX(sfxS.value);
        SoundSliders.changeVolMusic(musicS.value);
        SoundSliders.changeVolMaster(masterS.value);
    }

    public void optionsMenu()
    {
        if (optionsPanel.activeSelf)
        {
            optionsPanel.SetActive(false);
            buttonsObj.SetActive(true);
        }
        else
        {
            masterS.value = SoundSliders.masterVol;
            musicS.value = SoundSliders.musicVol;
            sfxS.value = SoundSliders.sfxVol;
            optionsPanel.SetActive(true);
            buttonsObj.SetActive(false);
        }
    }
    public void creditsMenu()
    {
        if (creditsPanel.activeSelf)
        {
            creditsPanel.SetActive(false);
            buttonsObj.SetActive(true);
        }
        else
        {
            creditsPanel.SetActive(true);
            buttonsObj.SetActive(false);
        }
    }
}

public static class LoadedGame
{
    public static bool loadGame = false;
}