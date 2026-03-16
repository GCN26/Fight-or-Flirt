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

    public int rockyRP = 0;
    public int b2RP,b3RP,b4RP,b5RP;

    //These may seem redundant but its required for the relaitonship system
    public int negTwo = -2;
    public int negOne = -1;
    public int Zero = 0;
    public int One = 1;
    public int Two = 2;
    public int Three = 3;

    public BattleManager battleManager;
    public TextEventManager textEventManager;
    public Inventory inventoryman;

    public int money;

    private void Start()
    {
        changePlayerName(SceneIndependentClass.charName);
        changeClass(SceneIndependentClass.classInt);
    }
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
        battleManager.party[0].battleSpriteIndex = index;

        int power = 1, defense = 1, speed = 1, charisma = 1;

        switch (index)
        {
            case 0: //Warrior
                power = 3; speed = 2; charisma = 1; defense = 2;
                break;
            case 1: //Bard
                power = 1; speed = 1; charisma = 4; defense = 2;
                break;
            case 2: //Rogue
                power = 2; speed = 4; charisma = 1; defense = 1;
                break;
            case 3: //Mage
                power = 4; speed = 1; charisma = 2; defense = 1;
                break;
        }

        battleManager.party[0].attack = power;
        battleManager.party[0].baseAttack = power;
        battleManager.party[0].defense = defense;
        battleManager.party[0].baseDefense = defense;
        battleManager.party[0].charisma = charisma;
        battleManager.party[0].baseCharisma = charisma;
        battleManager.party[0].speed = speed;
        battleManager.party[0].baseSpeed = speed;
    }

    public void addPoints(int index)
    {
        battleManager.party[index].relationshipPoints += 1;
        switch (battleManager.party[index].characterType)
        {
            case Combatant.bossTypeChar.none: break;
            case Combatant.bossTypeChar.rocky: rockyRP = battleManager.party[index].relationshipPoints; break;
            default: break;
        }
        Debug.Log("Party Member " + index.ToString() + ": " + battleManager.party[index].relationshipPoints.ToString());
    }

    public void removePoints(int index)
    {
        battleManager.party[index].relationshipPoints -= 1;
        switch (battleManager.party[index].characterType)
        {
            case Combatant.bossTypeChar.none: break;
            case Combatant.bossTypeChar.rocky: rockyRP = battleManager.party[index].relationshipPoints; break;
            default: break;
        }
        Debug.Log("Party Member " + index.ToString() + ": " + battleManager.party[index].relationshipPoints.ToString());
    }

    public string changePlayerName(string playerName)
    {
        textEventManager.characterName = playerName;
        battleManager.party[0].charName = playerName;
        return playerName;
    }
}
