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
    }
}
