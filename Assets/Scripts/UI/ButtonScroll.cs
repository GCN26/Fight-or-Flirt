using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScroll : MonoBehaviour
{
    Button selfButton;
    Vector3 initSize, initPos;
    Vector3 buttonInitSize;
    bool hover;
    float timer;
    //AudioManager audioManager;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;
    [SerializeField] private Color startColor, targetColor;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Sprite buttonNoHover, buttonHover;
    void Start()
    {
        selfButton = GetComponent<Button>();
        initSize = text.transform.localScale;
        initPos = text.transform.localPosition;
        buttonInitSize = selfButton.transform.localScale;
     }
    private void OnEnable()
    {
        if (!selfButton.interactable)
        {
            text.color = new Color(0, 0, 0, 0);
        }
        else
        {
            text.color = startColor;

        }
    }

    public void hoverDebug()
    {
        if (selfButton.interactable)
        {
            text.color = targetColor;
            //text.transform.localPosition = initPos - new Vector3(0, 15, 0);
            //text.transform.localScale = new Vector3(initSize.x * 1.5f, initSize.y * 1.5f, initSize.z * 1.5f);
        }
        else
        {
            leaveHover();
        }
    }
    public void leaveHover()
    {
        if (selfButton.interactable)
        {
            text.color = startColor;
            //text.transform.localPosition = initPos;
            //selfButton.transform.localScale = buttonInitSize;
        }
        text.transform.localScale = initSize;
    }
}
