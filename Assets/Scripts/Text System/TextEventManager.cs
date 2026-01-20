using System.Collections;
using TMPro;
using UnityEngine;

public class TextEventManager : MonoBehaviour
{
    public int textSpeed;

    int normalTextSpeed = 16;
    int fastTextSpeed = 48;

    public int nextIndex;
    public TextObject currentTextObject;

    public TextMeshProUGUI textBox;
    public int bar;

    //Written Variables
    int numberOfChar = 0;

    void Start()
    {
        StartCoroutine(typewriterFunc());
    }

    void Update()
    {
    }

    IEnumerator typewriterFunc()
    {
        numberOfChar = textBox.text.Length;
        textBox.maxVisibleCharacters = 0;

        foreach(FuncEvent func in currentTextObject.funcList)
        {
            func.callFunc();
        }
        foreach(TransformEvent transform in currentTextObject.transformList)
        {
            //transform.startTransform
        }

        while (textBox.maxVisibleCharacters < numberOfChar)
        {
            textBox.maxVisibleCharacters++;
            yield return new WaitForSeconds(.1125f);
        }
    }

    public void aa(int i)
    {
        Debug.Log(i);
    }
    public void advanceText()
    {
    }
}
