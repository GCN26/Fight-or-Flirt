using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelTransFade : MonoBehaviour
{
    public Image fade;
    public bool dir = true;
    public bool locked;
    //True = in, false = out
    public float timer;
    public bool loadAfter;
    public SaveLoadSystem save;
    public bool goToEndOfDemo;

    private void Start()
    {
        timer = .99f;
        fadeIn();
    }
    public void fadeIn()
    {
        locked = false;
        dir = true;
    }
    public void fadeOut()
    {
        locked = false;
        dir = false;
    }
    private void Update()
    {
        if (!locked)
        {
            if (!dir && timer < 1) timer += Time.deltaTime * 2;
            else if (dir && timer > 0) timer -= Time.deltaTime * 2;

            if (timer> 1)
            {
                if (goToEndOfDemo)
                {
                    SceneManager.LoadScene("EndOfDemo");
                    goToEndOfDemo = false;
                }
                if (loadAfter)
                {
                    SceneManager.LoadScene("Level2");
                    loadAfter = false;
                }
                timer = 1;
                locked = true;
            }
                if (timer< 0)
            {
                timer = 0;
                locked = true;
            }
            fade.color = new Color(0, 0, 0, timer);

        }
    }
}
