using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum playerClass
    {
        Warrior,
        Bard,
        Rogue,
        Mage
    }
    public playerClass pcClass;

    public playerClass classW = playerClass.Warrior;
    public playerClass classB = playerClass.Bard;
    public playerClass classR = playerClass.Rogue;
    public playerClass classM = playerClass.Mage;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void changeClass(int index)
    {
        pcClass = (playerClass)index;
    }
}
