using UnityEngine;

public class Level2Load : MonoBehaviour
{
    public SaveLoadSystem SaveLoadSystem;
    void Start()
    {
        SaveLoadSystem.loadGame();
    }

}
