using System;
using System.Collections;
using UnityEngine;

public class SpecialEventManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip jeraldNoise;
    public GameObject jerald;
    public Rigidbody jeraldRB;
    public GameObject jeraldBagDropped;
    public SpriteRenderer jeraldRenderer;

    public bool jeraldAlive = true;
    public bool hasJeraldFlowers = false;
    public bool knowJeraldName = false;
    public bool jeraldLetterOpen = false;

    public Sprite jeraldBag, jeraldNoBag;

    public GameObject mrRat;
    public bool knowsAboutTony = false;

    public GameObject player;
    public CharacterMovementScript playerMove;
    Coroutine spinCo;

    public BattleManager battleManager;
    public Inventory inventory;
    public TextEventManager textEventManager;

    public GameObject jeraldLetter;
    public int afterBattleIndex = -1;
    public bool mrRatFight = false;
    public bool willowFight = false;

    public int willowBattleState;
    public int firstWillowChoice;
    public int secondWillowChoice;

    public bool isWillowDead;
    public bool attackedWillow;
    public bool willowAttacksPlayer;

    public GameObject willow;

    public bool TrueA = true;

    public bool rockyFight = false;
    public bool hasRockyAttackedYet = false;
    public bool flirtedWithRockyYet = false;
    public bool betrayAttack = false;
    public enum rockyBattleState
    {
        needToHPCheck,
        calmedDown,
        postApology,
        postApologyNoFlirt
    }
    public rockyBattleState rockyState;

    private void Update()
    {
        battleManager.textMan.charMove.specialAllowMove = !jeraldLetterOpen;
    }
    public void playJeraldNoise()
    {
        audioSource.PlayOneShot(jeraldNoise);
    }
    public void jeraldTurn()
    {
        jerald.transform.localScale = new Vector3(jerald.transform.localScale.x*-1, jerald.transform.localScale.y, jerald.transform.localScale.z);
    }
    public void jeraldDropBagAndRunAway()
    {
        jeraldTurn();
        jerald.GetComponent<SpriteRenderer>().sprite = jeraldNoBag;
        jeraldRB.AddForce(new Vector3(35, 0, 0));
        jeraldBagDropped.SetActive(true);
    }
    public void jeraldDie()
    {
        jerald.SetActive(false);
        jeraldBagDropped.SetActive(true);
        jeraldAlive = false;
    }
    public void openJeraldLetter()
    {
        Debug.Log("Jerald Letter Opened");
        inventory.closeMenu();
        jeraldLetter.SetActive(true);
        jeraldLetterOpen = true;
    }
    public void closeJeraldLetter()
    {
        jeraldLetterOpen = false;
        jeraldLetter.SetActive(false);
        if (!knowJeraldName)
        {
            knowJeraldName = true;
            if (jeraldAlive)
            {
                textEventManager.callText(53);
            }
            else
            {
                textEventManager.callText(54);
            }
        }
    }
    public void playerDoASpin()
    {
        spinCo = StartCoroutine(playerSpin());
    }
    IEnumerator playerSpin()
    {
        playerMove.spriteRenderer.flipX = !playerMove.spriteRenderer.flipX;
        yield return new WaitForSeconds(.75f);
        spinCo = StartCoroutine(playerSpin());
    }
    public void resetPlayerTransformAfterSpin()
    {
        StopCoroutine(spinCo);
        playerMove.spriteRenderer.flipX = false;
    }
    public void startMrRatFight()
    {
        mrRatFight = true;
        battleManager.enemyTableIndex = 3;
        battleManager.startBattle();
        Debug.Log("Mr Rat Fight");
        sendMrRatToTheVoid();
    }
    public void startWillowFight()
    {
        mrRatFight = false;
        willowFight = true;
        battleManager.enemyTableIndex = 11;
        battleManager.startBattle();
        Debug.Log("Willow Fight");
        sendMrRatToTheVoid();
    }
    public void summonMrRat()
    {
        afterBattleIndex = -1;
        mrRat.transform.position = new Vector3(mrRat.transform.position.x, mrRat.transform.position.y, player.transform.position.z);
        mrRat.SetActive(true);
    }
    public void sendMrRatToTheVoid()
    {
        mrRat.SetActive(false);
    }
    public void setMrRatInfatToZero()
    {
        battleManager.enemies[0].infatuation = 0;
        afterBattleIndex = 92;
        battleManager.showBattleUI();
        knowsAboutTony = true;
        mrRatFight = false;
        battleManager.holdForText = false;
    }
    public void endBattleText()
    {
        //afterBattleIndex = -1;
        battleManager.showBattleUI();
    }
    public void hideJerald()
    {
        jerald.SetActive(false);
    }
    public void startRocky()
    {
        rockyFight = true;
        battleManager.enemyTableIndex = 1;
        battleManager.startBattle();
    }
    public void endMrRat()
    {
        mrRatFight = false;
    }
    public void setWillowInfatToZero()
    {
        battleManager.enemies[0].infatuation = 0;
        afterBattleIndex = 178;
        battleManager.showBattleUI();
        willowFight = false;
        battleManager.holdForText = false;
    }
    public void attackWillow()
    {
        attackedWillow = true;
    }
    public void willowAttacksYou()
    {
        willowAttacksPlayer = true;
    }
    public void killowWillow()
    {
        battleManager.enemies[0].hp = 0;
        afterBattleIndex = 188;
        battleManager.showBattleUI();
        willowFight = false;
        battleManager.holdForText = false;
        hideWillow();
        Debug.Log("Shes dead");
    }
    public void showWillow()
    {
        willow.transform.position = new Vector3(willow.transform.position.x, 9, player.transform.position.z);
        willow.SetActive(true);
    }
    public void hideWillow()
    {
        willow.SetActive(false);
    }
    public void endRocky()
    {
        rockyFight = false;
    }
    public void replaceRockyMoves()
    {
        battleManager.gameMan.flirtButtonLabelSaysFlirt = true;
        battleManager.gameMan.updateLabel();
        battleManager.enemies[0].attackList.Clear();
        battleManager.enemies[0].attackList.Add(Attacks.attackList[29]);
    }
    public void fightRockyAgain()
    {
        battleManager.enemies[0].attackList.Clear();
        battleManager.enemies[0].attackList.Add(Attacks.attackList[27]);
        battleManager.enemies[0].attackList.Add(Attacks.attackList[28]);
    }

    public void apology()
    {
        rockyState = rockyBattleState.postApology;
    }
}
