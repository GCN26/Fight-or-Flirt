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

    public int money;

    private void Start()
    {
        changePlayerName("Stink");
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
