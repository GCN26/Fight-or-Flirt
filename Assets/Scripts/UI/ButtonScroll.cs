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

    public void hoverDebug()
    {
        text.color = targetColor;
        //text.transform.localPosition = initPos - new Vector3(0, 15, 0);
        text.transform.localScale = new Vector3(initSize.x*1.5f, initSize.y * 1.5f, initSize.z * 1.5f);
    }
    public void leaveHover()
    {
        text.color = startColor;
        //text.transform.localPosition = initPos;
        text.transform.localScale = initSize;
        //selfButton.transform.localScale = buttonInitSize;
    }
}
