using UnityEngine;
using UnityEngine.UI;

public class Reactions : MonoBehaviour
{
    public Image display;
    public Sprite[] loveReaction;
    public Sprite[] hateReaction;
    public int fps;
    public int currentFrame;
    float timer;
    public bool ifGoodTrue;

    public void OnEnable()
    {
        currentFrame = 0;
        timer = 0;
    }
    private void Update()
    {
        timer += Time.deltaTime;
        timer += Time.deltaTime;
        timer += Time.deltaTime;
        if (timer > loveReaction.Length)
        {
            timer = 0;
            this.gameObject.SetActive(false);
        }
        else
        {
            currentFrame = (int)timer;
            if (ifGoodTrue) display.sprite = loveReaction[currentFrame];
            else display.sprite = hateReaction[currentFrame];
        }
    }
}
