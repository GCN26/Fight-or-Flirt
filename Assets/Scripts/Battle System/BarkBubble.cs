using System.Collections;
using TMPro;
using UnityEngine;

public class BarkBubble : MonoBehaviour
{
    public GameObject bg;
    public TextMeshProUGUI text;
    
    public void setStringAndAppearForABit(string str)
    {
        text.text = str;
        text.ForceMeshUpdate();
        StartCoroutine(Corout());
    }
    IEnumerator Corout()
    {
        bg.SetActive(true);
        text.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        bg.SetActive(false);
        text.gameObject.SetActive(false);
    }

    public void closeCoroutine()
    {
        bg.SetActive(false);
        text.gameObject.SetActive(false);
    }
}
