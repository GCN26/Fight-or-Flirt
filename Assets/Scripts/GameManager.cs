using System.Collections;
using TMPro;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
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
    public int Five = 5;

    public BattleManager battleManager;
    public TextEventManager textEventManager;
    public Inventory inventoryman;

    public int money;
    public bool BossRecruit;

    public int monstersKilled;
    public int reputation;

    public AudioSource audioSource;
    public AudioClip overworldMusic;

    public bool flirtButtonLabelSaysFlirt = false;
    public TextMeshProUGUI flirtButtonLabel;

    public bool knowsRockyName = false;

    public string currentScene;
    public GameObject[] triggerBoxes;
    public bool[] triggerBoxesActiveStates;

    public bool pauseMenuOpen;
    public MainMenuClass menu;


    public Sprite[] wrenWSprites, wrenBSprites, wrenRSprites, wrenMSprites, rockySprites;
    //All
    //0 - Happy
    //1 - Sad
    //2 - Flustered/Blushy
    //Warrior
    //3 - Suprised
    //4 - Confused
    //5 - Serious
    //Bard
    //3 - Suprised
    //4 - Confused
    //5 - Flirty
    //Rogue
    //3 - Suprised
    //4 - Shy
    //5 - Serious
    //Mage
    //3 - Annoyed
    //4 - Distain
    //5 - Smug
    //Rocky
    //3 - Angry
    //4 - Shining
    //5 - Shy
    public Image wrenImage, rockyImage;
    public RectTransform wrenLocHidden, wrenLocShown, rockyLocHidden, rockyLocShown;
    float timerWren,timerRocky;
    bool wrenTimerLock = true;
    bool rockyTimerLock = true;
    bool wrenTimerDir, rockyTimerDir;
    //True = Show, False = Hide;

    private void Start()
    {
        changePlayerName(SceneIndependentClass.charName);
        changeClass(SceneIndependentClass.classInt);
        audioSource.clip = overworldMusic;
        audioSource.loop = true;
        audioSource.Play();
        updateLabel();
    }
    private void Update()
    {
        if (!wrenTimerLock)
        {
            if (wrenTimerDir && timerWren < 1) timerWren += Time.deltaTime*2;
            else if (!wrenTimerDir && timerWren > 0) timerWren -= Time.deltaTime*2;

            if (timerWren > 1)
            {
                timerWren = 1;
                wrenTimerLock = true;
            }
            if (timerWren < 0)
            {
                timerWren = 0;
                wrenTimerLock = true;
            }
            wrenImage.rectTransform.position = Vector3.Lerp(wrenLocHidden.position, wrenLocShown.position, timerWren);

        }
        if (!rockyTimerLock)
        {
            if (rockyTimerDir && timerRocky < 1) timerRocky += Time.deltaTime * 2;
            else if (!rockyTimerDir && timerRocky > 0) timerRocky -= Time.deltaTime * 2;

            if (timerRocky > 1)
            {
                timerRocky = 1;
                rockyTimerLock = true;
            }
            if (timerRocky < 0)
            {
                timerRocky = 0;
                rockyTimerLock = true;
            }
            rockyImage.rectTransform.position = Vector3.Lerp(rockyLocHidden.position, rockyLocShown.position, timerRocky);

        }
        audioSource.volume = SoundSliders.musicVol * SoundSliders.masterVol * .75f;
    }
    public void updateTriggerBoxes()
    {
        for (int i = 0; i < triggerBoxes.Length; i++)
        {
            triggerBoxesActiveStates[i] = triggerBoxes[i].activeSelf;
        }
    }
    public void loadTriggerBoxes()
    {
        for (int i = 0; i < triggerBoxes.Length; i++)
        {
            triggerBoxes[i].SetActive(triggerBoxesActiveStates[i]);
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
                power = 3; speed = 6; charisma = 1; defense = 2;
                break;
            case 1: //Bard
                power = 1; speed = 3; charisma = 4; defense = 2;
                break;
            case 2: //Rogue
                power = 1; speed = 12; charisma = 1; defense = 1;
                break;
            case 3: //Mage
                power = 4; speed = 3; charisma = 2; defense = 1;
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

    public void addToMonsterKillCount()
    {
        monstersKilled++;
    }
    public void gainRep()
    {
        reputation++;
    }
    public void loseRep()
    {
        reputation--;
    }
    public void updateLabel()
    {
        if (flirtButtonLabelSaysFlirt)
        {
            flirtButtonLabel.text = "Flirt";
        }
        else
        {
            flirtButtonLabel.text = "Talk";
        }
    }
    public void learnRockyName()
    {
        knowsRockyName = true;
    }

    public void openPauseMenu()
    {
        if (!battleManager.battleOpen && !textEventManager.textOpen)
        {
            pauseMenuOpen = true;
            menu.buttonsObj.SetActive(true);
            menu.optionsPanel.SetActive(false);
        }
    }
    public void closePauseMenu()
    {
        pauseMenuOpen = false;
        menu.buttonsObj.SetActive(false);
        menu.optionsPanel.SetActive(false);
    }

    public void changeWrenExpression(int index)
    {
        switch (pcClass)
        {
            case playerClass.Warrior: wrenImage.sprite = wrenWSprites[index];break;
            case playerClass.Bard: wrenImage.sprite = wrenBSprites[index]; break;
            case playerClass.Rogue: wrenImage.sprite = wrenRSprites[index]; break;
            case playerClass.Mage: wrenImage.sprite = wrenMSprites[index]; break;
        }
    }
    public void changeRockyExpression(int index)
    {
        rockyImage.sprite = rockySprites[index];
    }
    public void showWrenImage()
    {
        wrenTimerLock = false;
        wrenTimerDir = true;
    }
    public void hideWrenImage()
    {
        wrenTimerLock = false;
        wrenTimerDir = false;
    }
    public void showRockyImage()
    {
        rockyTimerLock = false;
        rockyTimerDir = true;
    }
    public void hideRockyImage()
    {
        rockyTimerLock = false;
        rockyTimerDir = false;
    }
}
