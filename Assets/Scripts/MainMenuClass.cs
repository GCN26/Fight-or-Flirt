using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuClass : MonoBehaviour
{
    public TMP_InputField input;
    public void selectClass(int inputInt)
    {
        SceneIndependentClass.charName = input.text;
        SceneIndependentClass.classInt = inputInt;
        SceneManager.LoadScene("Playtest4Scene");
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
