using System.Collections;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TextEventManager : MonoBehaviour
{
    //Comment this
    public float textSpeed = 3;

    int normalTextSpeed = 5;
    int fastTextSpeed = 15;

    public int nextIndex;
    public TextObject currentTextObject;

    public GameObject textboxPanel;
    public TextMeshProUGUI textBox;
    public int bar;

    public TextAsset jsonFile;
    public string jsonStr;
    
    int numberOfChar = 0;
    public bool textOpen;
    Coroutine textCo, buttonCo;
    [SerializeField] Button[] buttonArr;
    [SerializeField] TextMeshProUGUI[] buttonTextArr;

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
            else if(textSpeed == normalTextSpeed) textSpeed = fastTextSpeed;
            else if(textSpeed == fastTextSpeed) textBox.maxVisibleCharacters = numberOfChar;
        }
        foreach(TransformEvent trans in currentTextObject.transforms)
        {
            if(!trans.reached) trans.transformProcess();
        }
    }

    IEnumerator typewriterFunc()
    {
        currentTextObject.transforms.Clear();
        currentTextObject.functions.Clear();
        if (currentTextObject.choices.strings != null)
        {
            currentTextObject.choices.strings.Clear();
            currentTextObject.choices.ids.Clear();
        }
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
        while (currentTextObject.checkReaches())
        {
            yield return null;
        }
        if(currentTextObject.choices != null && currentTextObject.choices.strings.Count > 0)
        {
            for(int i = 0; i <  currentTextObject.choices.strings.Count; i++)
            {
                buttonTextArr[i].text = currentTextObject.choices.strings[i];
                buttonArr[i].enabled = true;
                buttonArr[i].gameObject.SetActive(true);
            }
        }
        else nextIndex = currentTextObject.next_id;
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
            currentTextObject.functions.Clear();
            currentTextObject.transforms.Clear();
            currentTextObject.choices = null;

            textOpen = false;
            textboxPanel.SetActive(false);
            if(textCo != null) StopCoroutine(textCo);
            nextIndex = 0;
        }
    }

    public void choiceButtonPress(int buttonIndex)
    {
        if(buttonCo != null) StopCoroutine(buttonCo);

        for (int i = 0; i < buttonArr.Length; i++)
        {
            buttonArr[i].interactable = false;
            if (i == buttonIndex) { 
                //highlight
            }
            else buttonArr[i].gameObject.SetActive(false);
        }
        buttonCo = StartCoroutine(buttonPressCoroutine(buttonIndex));
    }

    IEnumerator buttonPressCoroutine(int buttonIndex)
    {
        currentTextObject.next_id = currentTextObject.choices.ids[buttonIndex];
        yield return new WaitForSeconds(.45f);
        buttonArr[buttonIndex].gameObject.SetActive(false);
        advanceText();
        StopCoroutine(buttonCo);
    }
}
