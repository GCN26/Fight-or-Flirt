using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class BattleManager : MonoBehaviour
{
    public Combatant[] party;
    public List<Combatant> enemies;

    int currentPartyIndex;

    public List<Combatant> battleList;
    public Coroutine battleCo;

    public bool battleOpen;
    public GameObject battleUI;
    bool pausedForInput;

    public GameObject partyHPBox;
    public GameObject enemyHPBox;
    public HPBars[] partyHPSliders;
    public HPBars[] enemyHPSliders;

    public GameObject attackTargetPanel;
    public GameObject rizzTargetPanel;
    //Add item/party target panel
    //Change type of array to buttons?
    public GameObject[] attackTargetButtons;
    public GameObject[] rizzTargetButtons;
    public TextMeshProUGUI[] attackTargetLabels, rizzTargetLabels;
    //get these into a class
    public GameObject battleBG;

    public GameObject attackMovePanel;
    public GameObject rizzMovePanel;

    public MoveButton[] attackMoveButtons;
    public MoveButton[] rizzMoveButtons;

    public TextEventManager textMan;
    public GameManager gameMan;
    public AudioSource musicSource, sfxSource;
    public AudioClip[] hurtSounds;

    public string battleOrder;
    public TextMeshProUGUI battleOrderDisplay;
    public Sprite[] spriteTable;

    public Image[] BattleSpritesParty, BattleSpritesEnemy;
    public bool holdForText;
    public bool attackType;

    string additionalString;
    public int enemyTableIndex;

    public int currentBossIndex;
    public GameObject bossRecruitPanel;
    public bool waitForPlayerToRecruit = false;
    public characterBattleText[] characterBattleTexts =
    {
        //REPLACE THE INDEXES WITH RESPECTIVE ONES
        new characterBattleText("Rocky",new int[] {26},new int[] {27}, new int[] {28}, new int[]{29},new int[]{0}),
        new characterBattleText("Mandi",new int[] {26},new int[] {27}, new int[] {28}, new int[]{29},new int[]{0}),
        new characterBattleText("Slimon",new int[] {26},new int[] {27}, new int[] {28}, new int[]{29},new int[]{0}),
        new characterBattleText("Dot",new int[] {26},new int[] {27}, new int[] {28}, new int[]{29},new int[]{0})
    };
    public ItemInstance[] BossWeapons;
    public ItemInstance[] BossArmor;

    public GameObject transitionPanel;
    public bool progressTransition;

    public BarkBubble[] barkBubbleParty, barkBubbleEnemy;
    public int specialIndex;
    public SpecialEventManager specialEventManager;
    public Reactions[] enemyFlirtReacts;
    public PartyMenuTest partyMenu;
    void Start()
    {
        //party[0].armor = itemTables.armorTable[2];
        //battleCo = StartCoroutine(battleProcess());

        //May need to remove this at some point in the future
        foreach(Combatant comb in party)
        {
            comb.party = true;
            comb.equipStatChange();
        }
    }
    void Update()
    {
        textMan.charMove.battleAllowMove = !battleOpen;
    }

    public void fightButton(int index)
    {
        party[currentPartyIndex].target = enemies[index];
        pausedForInput = false;
    }
    public void flirtButton(int index)
    {
        party[currentPartyIndex].target = enemies[index];
        pausedForInput = false;
    }

    public void startBattle()
    {
        if (!battleOpen)
        {
            holdForText = false;
            party[0].hp = party[0].maxHp;
            battleOpen = true;
            progressTransition = false;
            textMan.callText(31);
            musicSource.enabled = true;
            musicSource.Play();
            battleBG.SetActive(true);
            gameMan.audioSource.Pause();

            foreach (int index in encounterTables.combatantIndexes[enemyTableIndex])
            {
                enemies.Add(new(enemyList.enemyTable[index]));
                enemies[enemies.Count - 1].type = enemyList.enemyTable[index].type;
                Debug.Log(enemies[enemies.Count-1].type);
            }

            int pL = 0;
            foreach (Combatant partyMember in party)
            {
                partyMember.party = true;
                partyMember.attackList.Clear();
                partyMember.rizzAttackList.Clear();
                getPartyAttackIndexes(partyMember);
                partyMember.getAttacksInList();
                partyMember.partyIndex = pL;
                partyMember.battleSprite = spriteTable[partyMember.battleSpriteIndex];

                partyMember.hp = partyMember.maxHp;

                BattleSpritesParty[pL].gameObject.SetActive(true);
                BattleSpritesParty[pL].sprite = partyMember.battleSprite;
                BattleSpritesParty[pL].SetNativeSize();

                battleList.Add(partyMember);
                if(gameMan.pcClass == GameManager.playerClass.Rogue && specialEventManager.willowFight == false && specialEventManager.mrRatFight == false) battleList.Add(partyMember);
                pL++;
            }
            int eL = 0;
            foreach (Combatant enemy in enemies)
            {
                enemy.attackList.Clear();
                enemy.rizzAttackList.Clear();
                enemy.getAttacksInList();
                enemy.battleSprite = spriteTable[enemy.battleSpriteIndex];
                enemy.partyIndex = eL;

                BattleSpritesEnemy[eL].gameObject.SetActive(true);
                BattleSpritesEnemy[eL].sprite = enemy.battleSprite;
                BattleSpritesEnemy[eL].SetNativeSize();

                battleList.Add(enemy);
                eL++;
            }
            for (int i = 0; i < 4; i++)
            {
                attackTargetButtons[i].GetComponent<Button>().interactable = true;
                rizzTargetButtons[i].GetComponent<Button>().interactable = true;
            }

            battleList = battleList.OrderBy(x => x.speed).ToList();
            battleList.Reverse();

            //testImg.sprite = party[1].battleSprite;

            battleOrder = "<b>Turn Order</b>\n> ";
            foreach (Combatant comb in battleList)
            {
                if (comb.isPlayer)
                {
                    switch (gameMan.pcClass)
                    {
                        case GameManager.playerClass.Warrior: battleOrder += "<sprite=0>"; break;
                        case GameManager.playerClass.Bard: battleOrder += "<sprite=1>"; break;
                        case GameManager.playerClass.Rogue: battleOrder += "<sprite=2>"; break;
                        case GameManager.playerClass.Mage: battleOrder += "<sprite=3>"; break;
                    }
                }
                battleOrder += comb.charName + "\n";
            }
            battleOrderDisplay.text = battleOrder;

            Debug.Log(enemyTableIndex);
            if (enemyTableIndex == 1)
            {
                startBattleBoss("Rocky");
            }
            battleCo = StartCoroutine(battleProcess());
        }
    }
    public void startBattle2()
    {
        StartCoroutine(startBattleTransitionCoroutine2());
        
    }

    public void endBattle()
    {
        foreach (Attack atk in Attacks.attackList)
        {
            atk.secondaryEffect2 = "";
        }
        foreach (Attack flrt in Attacks.rizzList)
        {
            flrt.secondaryEffect2 = "";
        }
        for (int i = 0; i < 4; i++)
        {
            BattleSpritesParty[i].gameObject.SetActive(false);
            BattleSpritesEnemy[i].gameObject.SetActive(false);
        }
        battleOrder = "";
        battleOrderDisplay.text = battleOrder;
        disableHealthBars();
        battleList.Clear();
        enemies.Clear();
        battleOpen = false;
        battleUI.SetActive(false);
        musicSource.enabled = false;
        battleBG.SetActive(false);
        gameMan.audioSource.UnPause();
    }
    
    IEnumerator battleProcess()
    {
        disableHealthBars();
        enableHealthBars();
        updateHealthBars();

        if (specialEventManager.rockyFight && specialEventManager.rockyState == SpecialEventManager.rockyBattleState.needToHPCheck && checkRockyHP())
        {
            specialEventManager.rockyState = SpecialEventManager.rockyBattleState.calmedDown;
            party[0].attackList.Clear();
            party[0].rizzAttackList.Clear();
            getPartyAttackIndexes(party[0]);
            party[0].getAttacksInList();
            holdForText = true;
            specialIndex = 226;
        }
        else if (specialEventManager.rockyFight && specialEventManager.rockyState == SpecialEventManager.rockyBattleState.postApology)
        {
            party[0].attackList.Clear();
            party[0].rizzAttackList.Clear();
            getPartyAttackIndexes(party[0]);
            party[0].getAttacksInList();
        }

        if (holdForText)
        {
            battleUI.SetActive(false);
            textMan.callText(specialIndex);
        }
        while (holdForText) yield return null;
        specialIndex = -1;
        int partyDeadInt = 0;
        foreach (Combatant comb in party)
        {
            if (comb.hp > 0)
            {

            }
            else
            {
                partyDeadInt++;
            }
        }
        if (party.Length == partyDeadInt)
        {
            battleUI.SetActive(false);
            textMan.battleText = true;
            if (party.Length > 1) textMan.battleTextString = "Your team lost the battle!";
            else textMan.battleTextString = "You lost the battle!";
            textMan.startBattleText();
            yield return new WaitForSeconds(.5f);
            while (!textMan.progressable) yield return null;
            while (!Input.GetKeyDown(KeyCode.Space) && !Input.GetMouseButtonDown(0)) yield return null;
            textMan.progressable = false;
            textMan.endBattleText();
            endBattle();
            holdForText = false;
            //End in Loss
            //StopCoroutine(battleCo);
            SceneManager.LoadScene("MainMenu");
            yield break;
        }

        int enemyDeadInt = 0;
        foreach (Combatant comb in enemies)
        {
            if (comb.hp > 0 && comb.infatuation > 0)
            {
                
            }
            else if(comb.infatuation <= 0 && waitForPlayerToRecruit)
            {
                //Check stat requirements
                //Prompt Recruit
                recruitBoss();
                enemyDeadInt = enemies.Count;
            }
            else
            {
                enemyDeadInt++;
            }
        }
        if (enemies.Count == enemyDeadInt)
        {
            Debug.Log(specialEventManager.afterBattleIndex);
            battleUI.SetActive(false);
            textMan.battleText = true;
            if(party.Length > 1) textMan.battleTextString = "Your team won the battle! Your team gained experience!";
            else textMan.battleTextString = "You won the battle! You gained experience!";
            foreach (Combatant p in party)
            {
                p.experience += (int)((float)20 / (float)party.Length);
                p.getLevel();
            }
            gameMan.monstersKilled++;
            textMan.startBattleText();
            yield return new WaitForSeconds(.5f);
            while (!textMan.progressable) yield return null;
            while (!Input.GetKeyDown(KeyCode.Space) && !Input.GetMouseButtonDown(0)) yield return null;
            textMan.progressable = false;
            textMan.endBattleText();
            endBattle();
            if(specialEventManager.afterBattleIndex != -1)
            {
                textMan.callText(specialEventManager.afterBattleIndex);
                specialEventManager.afterBattleIndex = -1;
            }
            specialEventManager.mrRatFight = false;
            specialEventManager.willowFight = false;
            specialEventManager.rockyFight = false;
            holdForText = false;
            //End in win
            //StopCoroutine(battleCo);
            yield break;
        }

        while (textMan.textOpen)
        {
            yield return null;
        }

        if (battleList[0].party == true)
        {
            if (battleList[0].hp > 0)
            {
                battleUI.SetActive(true);
                currentPartyIndex = battleList[0].partyIndex;
                pausedForInput = true;
                battleList[0].target = null;
                battleUI.SetActive(true);
                clearMoveList();
                setMoveList(Array.IndexOf(party, battleList[0]));
                while (pausedForInput) yield return null;

                closeAttackTargetPanel();
                closeRizzTargetPanel();
                closeAttackMovePanel();
                closeRizzMovePanel();
                pausedForInput = true;
            }
        }
        else
        {
            if (battleList[0].hp > 0 && battleList[0].infatuation > 0)
            {
                battleList[0].target = party[checkSlots(UnityEngine.Random.Range(0, party.Length))];
            }
        }
        battleUI.SetActive(false);

        textMan.battleText = true;
        if (battleList[0].hp > 0 && battleList[0].infatuation > 0)
        {
            switch (battleList[0].attackType)
            {
                case Combatant.type_of_attack.fight:
                    int damage = battleList[0].attackEnemy();
                    if (battleList[0].selectedAttack.barkListIndexes != -1)
                    {
                        if (battleList[0].party)
                        {
                            barkBubbleParty[battleList[0].partyIndex].setStringAndAppearForABit(Attacks.barkListList[battleList[0].selectedAttack.barkListIndexes][UnityEngine.Random.Range(0, Attacks.barkListList[battleList[0].selectedAttack.barkListIndexes].Length - 1)]);
                        }
                        else
                        {
                            barkBubbleEnemy[battleList[0].partyIndex].setStringAndAppearForABit(Attacks.barkListList[battleList[0].selectedAttack.barkListIndexes][UnityEngine.Random.Range(0, Attacks.barkListList[battleList[0].selectedAttack.barkListIndexes].Length - 1)]);
                        }
                    }
                    if (damage != -1 && damage != -2)
                    {
                        sfxSource.PlayOneShot(hurtSounds[UnityEngine.Random.Range(0, hurtSounds.Length)]);
                        if (damage > 0) textMan.battleTextString = battleList[0].charName + " hits " + battleList[0].target.charName + " for " + damage.ToString() + " with " + battleList[0].selectedAttack.name + "." + additionalString;
                        else { 
                            textMan.battleTextString = "";
                        }
                    }
                    else if(damage == -2)
                    {
                        textMan.battleTextString = battleList[0].charName + "'s attack is blocked by " + battleList[0].target.charName + "!" + additionalString;
                    }
                    else if(damage == -1)
                    {
                        textMan.battleTextString = battleList[0].charName + " protects!" + additionalString;
                    }
                        break;
                case Combatant.type_of_attack.flirt:
                    additionalString = battleList[0].rizzEnemy();
                    textMan.battleTextString = battleList[0].charName + " uses " + battleList[0].selectedAttack.name + " on " + battleList[0].target.charName + ". " + additionalString;
                    if (additionalString == "a") textMan.battleTextString = battleList[0].charName + " tries talking to " + battleList[0].target.charName + ".";
                    if (additionalString == "b") textMan.battleTextString = "";
                    break;
            }
        }
        else textMan.battleTextString = battleList[0].charName + " is unable to fight!";
        additionalString = "";
        updateHealthBars();
        textMan.startBattleText();
        if(textMan.battleTextString != "")yield return new WaitForSeconds(.5f);
        while (!textMan.progressable && textMan.battleTextString != "") yield return null;
        while ((!Input.GetKeyDown(KeyCode.Space)&& !Input.GetMouseButtonDown(0)) && textMan.battleTextString != "") yield return null;
        textMan.progressable = false;

        textMan.endBattleText();
        if (battleList[0].currentStatus == Combatant.status.Burned && battleList[0].hp > 0 && battleList[0].infatuation > 0)
        {
            battleList[0].hp -= (int)((float)battleList[0].maxHp / 16);
            sfxSource.PlayOneShot(hurtSounds[UnityEngine.Random.Range(0, hurtSounds.Length)]);
            updateHealthBars();

            textMan.battleText = true;
            textMan.battleTextString = battleList[0].charName + " took "+ (int)((float)battleList[0].maxHp / 16) + " damage due to their burn.";
            textMan.startBattleText();
            yield return new WaitForSeconds(.5f);
            while (!textMan.progressable) yield return null;
            while (!Input.GetKeyDown(KeyCode.Space) && !Input.GetMouseButtonDown(0)) yield return null;
            textMan.progressable = false;
            textMan.endBattleText();
            
        }

        foreach (Combatant comb in battleList)
        {
            if (comb.hp <= 0 || comb.infatuation <= 0)
            {
                if (comb.party) BattleSpritesParty[comb.partyIndex].gameObject.SetActive(false);
                else BattleSpritesEnemy[comb.partyIndex].gameObject.SetActive(false);
                if (specialEventManager.mrRatFight && !comb.party && comb.hp <= 0)
                {
                    specialIndex = 200;
                    battleUI.SetActive(false);
                    holdForText = true;
                }
                if(specialEventManager.rockyFight && !comb.party && comb.hp <= 0)
                {
                    specialIndex = 272;
                    battleUI.SetActive(false);
                    holdForText = true;

                }
            }
        }

        Combatant temp = battleList[0];
        battleList.RemoveAt(0);
        if(temp.hp >0 && temp.infatuation > 0) battleList.Add(temp);

        battleList.RemoveAll(x => x.hp == 0);
        battleList.RemoveAll(x => x.infatuation == 0);

        battleOrder = "<b>Turn Order</b>\n";
        foreach (Combatant comb in battleList)
        {
            if (comb.isPlayer)
            {
                switch (gameMan.pcClass)
                {
                    case GameManager.playerClass.Warrior: battleOrder += "<sprite=0>"; break;
                    case GameManager.playerClass.Bard: battleOrder += "<sprite=1>"; break;
                    case GameManager.playerClass.Rogue: battleOrder += "<sprite=2>"; break;
                    case GameManager.playerClass.Mage: battleOrder += "<sprite=3>"; break;
                }
            }
            battleOrder += comb.charName + "\n";
        }
        battleOrderDisplay.text = battleOrder;

        foreach(BarkBubble bub in barkBubbleParty)
        {
            bub.closeCoroutine();
        }
        foreach (BarkBubble bub in barkBubbleEnemy)
        {
            bub.closeCoroutine();
        }

        Debug.Log("End of Turn");
        battleCo = StartCoroutine(battleProcess());
        yield break;
    }

    int checkSlots(int num)
    {
        if (party[num].hp > 0) return num;
        else
        {
            if (num > 0) return checkSlots(num - 1);
            else return checkSlots(party.Count() - 1);
        }
    }

    public void updateHealthBars() {
        for (int i = 0; i < party.Length; i++)
        {
            partyHPSliders[i].hpSlider.value = (float)party[i].hp / (float)party[i].maxHp;
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemyHPSliders[i].hpSlider.value = (float)enemies[i].hp / (float)enemies[i].maxHp;
            enemyHPSliders[i].infatSlider.value = (float)enemies[i].infatuation / (float)enemies[i].maxInfatuation;
            if (enemies[i].hp <= 0 || enemies[i].infatuation <= 0)
            {
                attackTargetButtons[i].GetComponent<Button>().interactable = false;
                rizzTargetButtons[i].GetComponent<Button>().interactable = false;
            }
        }

    }
    public void enableHealthBars()
    {
        partyHPBox.SetActive(true);
        enemyHPBox.SetActive(true);

        for(int i = 0; i < party.Length; i++)
        {
            partyHPSliders[i].gameObject.SetActive(true);
            for(int j = 0; j < party[i].attackList.Count; j++)
            {
                attackMoveButtons[j].button.gameObject.SetActive(true);
            }
            for (int j = 0; j < party[i].rizzAttackList.Count; j++)
            {
                rizzMoveButtons[j].button.gameObject.SetActive(true);
            }
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemyHPSliders[i].gameObject.SetActive(true);
            attackTargetButtons[i].SetActive(true);
            attackTargetLabels[i].text = enemies[i].charName;
            rizzTargetButtons[i].SetActive(true);
            rizzTargetLabels[i].text = enemies[i].charName;
        }
    }
    public void disableHealthBars()
    {
        partyHPBox.SetActive(false);
        enemyHPBox.SetActive(false);

        for (int i = 0; i < 4; i++)
        {
            partyHPSliders[i].gameObject.SetActive(false);
            for (int j = 0; j < 4; j++)
            {
                attackMoveButtons[j].button.gameObject.SetActive(false);
                rizzMoveButtons[j].button.gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < 4; i++)
        {
            enemyHPSliders[i].gameObject.SetActive(false);
            attackTargetButtons[i].SetActive(false);
            rizzTargetButtons[i].SetActive(false);
        }
    }
    public void openAttackMovePanel()
    {
        closeRizzTargetPanel();
        closeRizzMovePanel();
        attackMovePanel.SetActive(true);
    }
    public void closeAttackMovePanel()
    {
        attackMovePanel.SetActive(false);
    }
    public void openRizzMovePanel()
    {
        closeAttackTargetPanel();
        closeAttackMovePanel();
        rizzMovePanel.SetActive(true);
    }
    public void closeRizzMovePanel()
    {
        rizzMovePanel.SetActive(false);
    }

    public void openAttackTargetPanel()
    {
        closeRizzTargetPanel();
        attackTargetPanel.SetActive(true);
    }
    public void closeAttackTargetPanel()
    {
        attackTargetPanel.SetActive(false);
    }
    public void openRizzTargetPanel()
    {
        closeAttackTargetPanel();
        rizzTargetPanel.SetActive(true);
    }
    public void closeRizzTargetPanel()
    {
        rizzTargetPanel.SetActive(false);
    }
    public void moveSelectAttack(int index)
    {
        party[currentPartyIndex].attackListIndex = index;
        party[currentPartyIndex].attackType = Combatant.type_of_attack.fight;
        openAttackTargetPanel();
    }
    public void moveSelectRizz(int index)
    {
        party[currentPartyIndex].attackListIndex = index;
        party[currentPartyIndex].attackType = Combatant.type_of_attack.flirt;
        openRizzTargetPanel();
    }

    public void setMoveList(int partyIndex)
    {
        for (int i = 0; i < party[partyIndex].attackList.Count; i++)
        {
            attackMoveButtons[i].gameObject.SetActive(true);
            attackMoveButtons[i].setMoves(this, partyIndex, i, false);
        }
        for (int i = 0; i < party[partyIndex].rizzAttackList.Count; i++)
        {
            rizzMoveButtons[i].gameObject.SetActive(true);
            rizzMoveButtons[i].setMoves(this, partyIndex, i, true);
        }
    }
    public void clearMoveList()
    {
        foreach(MoveButton button in attackMoveButtons)
        {
            button.gameObject.SetActive(false);
            button.attack = null;
            button.label.text = "";
        }
        foreach (MoveButton button in rizzMoveButtons)
        {
            button.gameObject.SetActive(false);
            button.attack = null;
            button.label.text = "";
        }
    }

    public void setEnemyStatus(Combatant target,int index)
    {
        int random = UnityEngine.Random.Range(0, 2);
        if (random == 0) return;

        target.currentStatus = Combatant.status.Burned;
        additionalString = " " + target.charName + " was burned!";
    }
    public void showBattleUI()
    {
        battleUI.SetActive(true);
        holdForText = false;
    }
    public void endBattleText()
    {
        holdForText = false;
        textMan.textOpen = false;
    }

    public void startBattleBoss(string name)
    {
        waitForPlayerToRecruit = true;
        //Get name and index of boss
        //Add their index for recruiting and for in-battle text
        switch (name)
        {
            case "Rocky":
                currentBossIndex = 0;
                foreach(Combatant comb in party)
                {
                    foreach(Attack atk in comb.attackList)
                    {
                        //atk.secondaryEffect2 = "bossRockyAttackText";
                        atk.secondaryEffect2 = "rockyFightAfterCalm";
                    }
                    foreach(Attack flrt in comb.rizzAttackList)
                    {
                        //flrt.secondaryEffect2 = "bossRockyFlirtText";
                    }
                }
                enemies[0].isBoss = true;
                break;
        }
    }

    public void recruitBoss()
    {
        gameMan.BossRecruit = true;
        waitForPlayerToRecruit = false;
        bossRecruitPanel.SetActive(false);
        if(party.Length < 4)
        {
            int index = currentBossIndex;
            List<Combatant> newParty = party.ToList();
            newParty.Add(enemyList.bossRecruitedTable[index]);
            newParty[newParty.Count - 1].weapon = BossWeapons[index];
            newParty[newParty.Count - 1].armor = BossArmor[index];
            newParty[newParty.Count - 1].characterType = (Combatant.bossTypeChar)(index + 1);
            party = newParty.ToArray();
            partyMenu.childArray[newParty.Count - 1].gameObject.SetActive(true);
            partyMenu.updateOrder();
        }
    }
    public void dontRecruitBoss()
    {
        gameMan.BossRecruit = false;
        waitForPlayerToRecruit =false;
        bossRecruitPanel.SetActive(false);
    }

    public void getPartyAttackIndexes(Combatant comb)
    {
        //Move 0 and 2 Based on Class
        int newAtk0 = 0;
        int newAtk1 = 0;
        int newAtk2 = 0;
        int newAtk3 = 0;

        int newFlrt0 = 1;
        int newFlrt1 = 2;
        int newFlrt2 = 0;
        int newFlrt3 = 7;


        if (comb.partyIndex == 0)
        {
            switch (gameMan.pcClass)
            {
                case GameManager.playerClass.Warrior: newAtk0 = 0; newAtk2 = 18; newAtk3 = 1; break;
                case GameManager.playerClass.Bard: newAtk0 = 6; newAtk2 = 19; newAtk3 = 8; break;
                case GameManager.playerClass.Rogue: newAtk0 = 9; newAtk2 = 20; newAtk3 = 10; break;
                case GameManager.playerClass.Mage: newAtk0 = 3; newAtk2 = 21; newAtk3 = 4; break;
            }
        }
        else
        {
            switch (comb.characterType)
            {
                case Combatant.bossTypeChar.rocky: newAtk0 = 12; newAtk2 = 13; newAtk3 = 14; break;
                case Combatant.bossTypeChar.mandi: newAtk0 = 0; newAtk2 = 0; newAtk3 = 0; break;
                case Combatant.bossTypeChar.slimon: newAtk0 = 0; newAtk2 = 0; newAtk3 = 0; break;
                case Combatant.bossTypeChar.dot: newAtk0 = 0; newAtk2 = 0; newAtk3 = 0; break;
            }
        }

        //Move 1 Based on Weapon
        newAtk1 = comb.weapon.itemType.moveIndex;

        comb.attackListIndexes[0] = newAtk0;
        comb.attackListIndexes[1] = newAtk1;
        comb.rizzAttackListIndexes[0] = newFlrt0;
        comb.rizzAttackListIndexes[1] = newFlrt1;
        comb.rizzAttackListIndexes[2] = newFlrt2;
        comb.rizzAttackListIndexes[3] = newFlrt3;
        if (comb.level >= 3)
        {
            comb.attackListIndexes[2] = newAtk2;
            comb.attackListIndexes[3] = newAtk3;
        }
        else
        {
            comb.attackListIndexes[2] = -1;
            comb.attackListIndexes[3] = -1;
        }

        if (specialEventManager.mrRatFight)
        {
            comb.attackListIndexes[1] = -1;
            comb.attackListIndexes[2] = -1;
            comb.attackListIndexes[3] = -1;
            comb.rizzAttackListIndexes[0] = 4;
            comb.rizzAttackListIndexes[1] = -1;
            comb.rizzAttackListIndexes[2] = -1;
            comb.rizzAttackListIndexes[3] = -1;
        }
        else if (specialEventManager.willowFight)
        {
            int willowBasicAttackIndex = 0;
            switch (gameMan.pcClass)
            {
                case GameManager.playerClass.Warrior: willowBasicAttackIndex = 23; break;
                case GameManager.playerClass.Bard: willowBasicAttackIndex = 25; break;
                case GameManager.playerClass.Rogue: willowBasicAttackIndex = 26; break;
                case GameManager.playerClass.Mage: willowBasicAttackIndex = 24; break;
            }
            comb.attackListIndexes[0] = willowBasicAttackIndex;
            comb.attackListIndexes[1] = -1;
            comb.attackListIndexes[2] = -1;
            comb.attackListIndexes[3] = -1;
            comb.rizzAttackListIndexes[0] = 5;
            comb.rizzAttackListIndexes[1] = 6;
            comb.rizzAttackListIndexes[2] = -1;
            comb.rizzAttackListIndexes[3] = -1;
        }
        else if (specialEventManager.rockyFight && specialEventManager.rockyState == SpecialEventManager.rockyBattleState.needToHPCheck)
        {
            comb.rizzAttackListIndexes[0] = 8;
            comb.rizzAttackListIndexes[1] = -1;
            comb.rizzAttackListIndexes[2] = -1;
            comb.rizzAttackListIndexes[3] = -1;
        }
        else if (specialEventManager.rockyFight && specialEventManager.rockyState == SpecialEventManager.rockyBattleState.calmedDown)
        {
            comb.rizzAttackListIndexes[0] = 9;
            comb.rizzAttackListIndexes[1] = -1;
            comb.rizzAttackListIndexes[2] = -1;
            comb.rizzAttackListIndexes[3] = -1;
        }
        else if (specialEventManager.rockyFight && specialEventManager.rockyState == SpecialEventManager.rockyBattleState.postApology)
        {
            comb.rizzAttackListIndexes[0] = 10;
            comb.rizzAttackListIndexes[1] = 11;
            comb.rizzAttackListIndexes[2] = 12;
            comb.rizzAttackListIndexes[3] = 13;
        }
        else if (specialEventManager.rockyFight && specialEventManager.rockyState == SpecialEventManager.rockyBattleState.postApologyNoFlirt)
        {
            comb.rizzAttackListIndexes[0] = 14;
            comb.rizzAttackListIndexes[1] = -1;
            comb.rizzAttackListIndexes[2] = -1;
            comb.rizzAttackListIndexes[3] = -1;
        }
    }

    public void battleTransition()
    {
        StartCoroutine(startBattleTransitionCoroutine1());
    }
    IEnumerator startBattleTransitionCoroutine1()
    {
        while(transitionPanel.transform.position.x < 1700)
        {
            transitionPanel.transform.position += new Vector3(300, 0, 0);
            yield return null;
        }
        startBattle2();
    }
    IEnumerator startBattleTransitionCoroutine2()
    {
        while (transitionPanel.transform.position.x < 7000)
        {
            transitionPanel.transform.position += new Vector3(300, 0, 0);
            yield return null;
        }
        transitionPanel.transform.position = new Vector3(-5000, 0, 0);
    }

    public bool checkRockyHP()
    {
        return(enemies[0].hp <= enemies[0].maxHp / 2);
    }
}

public class characterBattleText
{
    public string charName; // Just for sorting
    public List<bool> attackHistory = new(); //True for Attack, False for Flirt/Talk
    public int[] indexAttack;
    public int[] indexFlirt;
    public int[] indexAttackAfterFlirt;
    public int[] indexFlirtAfterAttack;
    public int[] progressDialogue;
    public int progressIndex = 0;

    public int textIndex;

    public characterBattleText(string charName, int[] iA, int[] iF, int[] iAF, int[] iFA, int[] progessIndexes)
    {
        this.charName = charName;
        this.indexAttack = iA;
        this.indexFlirt = iF;
        this.indexAttackAfterFlirt = iAF;
        this.indexFlirtAfterAttack = iFA;
        this.progressDialogue = progessIndexes;
    }
    public int addAttackHistory(bool attackType)
    {
        attackHistory.Add(attackType);

        //If attack
        if (attackHistory[attackHistory.Count - 1] == true)
        {
            textIndex = indexAttack[UnityEngine.Random.Range(0, indexAttack.Count())];
        }
        //If flirt
        if (attackHistory[attackHistory.Count - 1] == false)
        {
            textIndex = indexFlirt[UnityEngine.Random.Range(0, indexFlirt.Count())];
        }
        if (attackHistory.Count > 1)
        {
            Debug.Log("More than 1 attack");
            //If attack after flirt
            if (attackHistory[attackHistory.Count-1] == true && attackHistory[attackHistory.Count-2] == false)
            {
                Debug.Log("Attack after Flirt");
                textIndex = indexAttackAfterFlirt[UnityEngine.Random.Range(0, indexAttackAfterFlirt.Count())];
            }
            //If flirt after attack
            if (attackHistory[attackHistory.Count-1] == false && attackHistory[attackHistory.Count - 2] == true)
            {
                Debug.Log("Flirt after Attack");
                textIndex = indexFlirtAfterAttack[UnityEngine.Random.Range(0, indexFlirtAfterAttack.Count())];
            }
        }
        return textIndex;
    }
}