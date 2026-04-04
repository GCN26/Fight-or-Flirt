using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuClass : MonoBehaviour
{
    public TMP_InputField input;
    public GameObject namePanel, classPanel;
    public Button newGameButton, loadGameButton, optionsButton, quitButton;

    public void selectClass(int inputInt)
    {
        SceneIndependentClass.charName = input.text;
        SceneIndependentClass.classInt = inputInt;
        SceneManager.LoadScene("LTUXFINAL");
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
}
