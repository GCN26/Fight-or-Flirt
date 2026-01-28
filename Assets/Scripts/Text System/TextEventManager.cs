using System.Collections;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class TextEventManager : MonoBehaviour
{
    public float textSpeed = 3;

    int normalTextSpeed = 3;
    int fastTextSpeed = 9;

    public int nextIndex;
    public TextObject currentTextObject;

    public GameObject textboxPanel;
    public TextMeshProUGUI textBox;
    public int bar;

    public TextAsset jsonFile;
    public string jsonStr;
    
    int numberOfChar = 0;
    public bool textOpen;
    Coroutine textCo;

    public CharacterMovementScript charMove;

    void Start()
    {
        readJSON();
    }

    void Update()
    {
        charMove.allowMove = !textOpen;
        if (Input.GetKeyDown(KeyCode.T) && !textOpen)
        {
            textOpen = true;
            nextIndex = 1;
            textCo = StartCoroutine(typewriterFunc());
        }
        if (Input.GetKeyDown(KeyCode.Space) && textOpen)
        {
            if(textBox.maxVisibleCharacters == numberOfChar) advanceText();
            else textSpeed = fastTextSpeed;
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
        textBox.maxVisibleCharacters = 0;
        numberOfChar = textBox.text.Length;

        foreach (TransformEvent trans in currentTextObject.transforms)
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
            char curChar = textBox.text[textBox.maxVisibleCharacters-1];
            if (curChar == '.' || curChar == '!' || curChar == '?' || curChar == ',' || curChar == ':' || curChar == ';') yield return new WaitForSeconds(.25f * (1 / textSpeed));
            else yield return new WaitForSeconds(.1125f*(1/textSpeed));
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
        textSpeed = normalTextSpeed;

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
