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

    private void Start()
    {
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
    }
    public void closeJeraldLetter()
    {
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
        battleManager.enemyTableIndex = 3;
        battleManager.startBattle();
        mrRatFight = true;
        Debug.Log("Mr Rat Fight");
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
        mrRatFight = false;
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
        afterBattleIndex = -1;
        battleManager.showBattleUI();
    }
    public void hideJerald()
    {
        jerald.SetActive(false);
    }
    public void startRocky()
    {
        battleManager.enemyTableIndex = 1;
        battleManager.startBattle();
    }
}
