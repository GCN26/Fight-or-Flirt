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

    public GameObject textboxPanel;
    public TextMeshProUGUI textBox;
    public int bar;

    public TextAsset jsonFile;
    public string jsonStr;

    //Written Variables
    int numberOfChar = 0;

    public bool textOpen;

    Coroutine textCo;

    void Start()
    {
        readJSON();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !textOpen)
        {
            textOpen = true;
            nextIndex = 0;
            textCo = StartCoroutine(typewriterFunc());
        }
        if (Input.GetKeyDown(KeyCode.Space) && textOpen)
        {
            advanceText();
        }
        foreach(TransformEvent trans in currentTextObject.transforms)
        {
            if(!trans.reached) trans.transformProcess();
        }
    }

    IEnumerator typewriterFunc()
    {
        endText();
        readID(nextIndex);

        textBox.text = currentTextObject.dialogue;
        textboxPanel.SetActive(true);


        numberOfChar = textBox.text.Length;
        textBox.maxVisibleCharacters = 0;
        foreach(TransformEvent trans in currentTextObject.transforms)
        {
            trans.clearReaches();
        }
        foreach (FuncEvent func in currentTextObject.functions)
        {
            func.callFunc();
        }

        while (textBox.maxVisibleCharacters < numberOfChar)
        {
            textBox.maxVisibleCharacters++;
            yield return new WaitForSeconds(.1125f);
        }
        nextIndex = currentTextObject.next_id;
    }

    public void aa(int i)
    {
        Debug.Log(i);
    }
    public void advanceText()
    {
        //account for text skipping
        if (textCo != null) StopCoroutine(textCo);

        nextIndex = currentTextObject.next_id;
        if (nextIndex != -1) textCo = StartCoroutine(typewriterFunc());
        else endText();
    }

    public void readJSON()
    {
        jsonStr = jsonFile.text;
        DialogueArrayManager.objArr = JsonUtility.FromJson<TextObjectArr>(jsonStr);
        //readID(nextIndex);
    }
    public void readID(int id)
    {
        currentTextObject.id = id;
        currentTextObject.setVars();
    }
    public void endText()
    {
        if (nextIndex == -1)
        {
            textOpen = false;
            textboxPanel.SetActive(false);
            if(textCo != null) StopCoroutine(textCo);
        }
    }
}
