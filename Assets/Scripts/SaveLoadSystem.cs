using NUnit.Framework;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    public GameManager gameManager;
    public SpecialEventManager specManager;
    public BattleManager battleManager;
    public TextEventManager textEventManager;
    public Inventory inventoryManager;
    public GameObject player;

    public string jsonString;
    public TextAsset jsonFile;
    saveData objectSave,objectLoad;

    private void Start()
    {
        objectSave = new();
        objectLoad = new();
        if (LoadedGame.loadGame)
        {
            LoadedGame.loadGame = false;  
            loadGame();
        }
    }
    public void saveGame()
    {
        BinaryFormatter binary = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, "save.dat");
        gameManager.updateTriggerBoxes();

        objectSave.party = battleManager.party;

        objectSave.inventoryItems = inventoryManager.items.ToArray();

        objectSave.playerClassInt = (int)gameManager.pcClass;
        objectSave.money = gameManager.money;
        objectSave.reputation = gameManager.reputation;
        objectSave.monstersKilled = gameManager.monstersKilled;
        objectSave.flirtLabel = gameManager.flirtButtonLabelSaysFlirt;
        objectSave.knowsRockyName = gameManager.knowsRockyName;
        objectSave.currentScene = gameManager.currentScene;
        objectSave.triggerBoxesActiveStates = gameManager.triggerBoxesActiveStates;

        objectSave.jeraldAlive = specManager.jeraldAlive;
        objectSave.hasJeraldFlowers = specManager.hasJeraldFlowers;
        objectSave.knowJeraldName = specManager.knowJeraldName;
        objectSave.knowsAboutTony = specManager.knowsAboutTony;
        objectSave.isWillowDead = specManager.isWillowDead;

        objectSave.playerName = textEventManager.characterName;

        objectSave.playerPos = player.transform.position;

        jsonString = JsonUtility.ToJson(objectSave);

        using (StreamWriter outputFile = new StreamWriter(path))
        {
            outputFile.Write(jsonString);
            outputFile.Close();
        }
    }
    public void loadGame()
    {
        string loadString;
        string path = Path.Combine(Application.persistentDataPath, "save.dat");
        using (StreamReader saveFile = new StreamReader(path))
        {
            loadString = saveFile.ReadToEnd();
            saveFile.Close();
        }
        JsonUtility.FromJsonOverwrite(loadString, objectLoad);
        battleManager.party = objectLoad.party;

        inventoryManager.items = objectLoad.inventoryItems.ToList();

        gameManager.pcClass = (GameManager.playerClass)objectLoad.playerClassInt;
        gameManager.money = objectLoad.money;
        gameManager.reputation = objectLoad.reputation;
        gameManager.monstersKilled = objectLoad.monstersKilled;
        gameManager.flirtButtonLabelSaysFlirt = objectLoad.flirtLabel;
        gameManager.knowsRockyName = objectLoad.knowsRockyName;

        if (gameManager.currentScene == objectLoad.currentScene)
        {
            player.transform.position = objectLoad.playerPos;
            gameManager.triggerBoxesActiveStates = objectLoad.triggerBoxesActiveStates;
            gameManager.loadTriggerBoxes();
        }

        specManager.jeraldAlive = objectLoad.jeraldAlive;
        specManager.hasJeraldFlowers = objectLoad.hasJeraldFlowers;
        specManager.knowJeraldName = objectLoad.knowJeraldName;
        specManager.knowsAboutTony = objectLoad.knowsAboutTony;
        specManager.isWillowDead = objectLoad.isWillowDead;

        textEventManager.characterName = objectLoad.playerName;

    }
}
[Serializable]
public class saveData
{
    //Battle Manager
    public Combatant[] party;

    //Inventory Manager
    public ItemInstance[] inventoryItems;

    //Game Manager
    public int playerClassInt;
    public int money;
    public int reputation;
    public int monstersKilled;
    public bool flirtLabel;
    public bool knowsRockyName;

    public string currentScene;
    public bool[] triggerBoxesActiveStates;

    //Special Event Manager
    public bool jeraldAlive;
    public bool hasJeraldFlowers;
    public bool knowJeraldName;
    public bool knowsAboutTony;
    public bool isWillowDead;

    //Text Event Manager
    public string playerName;

    //Player
    public Vector3 playerPos;
}