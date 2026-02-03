using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TextEventManager : MonoBehaviour
{
    //Text speed does not change in game, instead it uses a bool to determine how many chars to add.
    public float textSpeed = 5;
    bool fastText = false;

    //Next index of JSON array to call
    public int nextIndex;
    //Text object from the array
    public TextObject currentTextObject;

    //Game Objects
    public GameObject textboxPanel;
    public TextMeshProUGUI textBox;
    public Image[] portraits;

    //Test Value
    public int bar;

    //JSON Vars
    public TextAsset jsonFile;
    public string jsonStr;
    
    //Text Values
    int numberOfChar = 0;
    bool progressable = false;
    public bool textOpen;
    Coroutine textCo, buttonCo;
    [SerializeField] Button[] buttonArr;
    [SerializeField] TextMeshProUGUI[] buttonTextArr;

    //Character to pause movement on
    public CharacterMovementScript charMove;

    Vector3 textBoxPos;
    Vector3 textBoxDisablePos;

    void Start()
    {
        //Get JSON data
        readJSON();
        textBoxPos = textboxPanel.transform.position;
        textBoxDisablePos = textBoxPos - new Vector3(0, 1100, 0);
    }

    void Update()
    {
        //Set allowMove to the opposite of textOpen
        charMove.allowMove = !textOpen;
        if (Input.GetKeyDown(KeyCode.T) && !textOpen)
        {
            //Stop Coroutine if there is one
            if(textCo != null) StopCoroutine(textCo);
            //Enable textOpen, set index to test, and start coroutine
            textOpen = true;
            nextIndex = 1;
            textCo = StartCoroutine(typewriterFunc());
        }
        if (Input.GetKeyDown(KeyCode.Space) && textOpen)
        {
            //Code to run depending on state

            //If no choices are present and the entire string is revealed, advance text.
            if(progressable && currentTextObject.choices.strings.Count <= 0) advanceText();
            //If text is not entirely revealed and space is pressed, speed up text.
            else if(textBox.maxVisibleCharacters != numberOfChar && fastText == false) fastText = true;
            //If text is not entirely revealed and text is already sped up, skip to end.
            else if(textBox.maxVisibleCharacters != numberOfChar && fastText == true) textBox.maxVisibleCharacters = numberOfChar;
        }
    }
    private void FixedUpdate()
    {
        //Run transform events
        foreach (TransformEvent trans in currentTextObject.transforms)
        {
            if (!trans.reached) trans.transformProcess();
        }
    }

    IEnumerator typewriterFunc()
    {
        //Clear vars for next object to come in
        currentTextObject.transforms.Clear();
        currentTextObject.functions.Clear();
        if (currentTextObject.choices.strings.Count > 0)
        {
            currentTextObject.choices.strings.Clear();
            currentTextObject.choices.ids.Clear();
        }
        //Check if text has ended
        endText();
        //Get object
        readID(nextIndex);

        //Set variables for text
        textBox.text = currentTextObject.dialogue;
        if (currentTextObject.dialogue != "") enableTextbox();
        else disableTextbox();
            textBox.maxVisibleCharacters = 0;
        numberOfChar = textBox.text.Length;

        //Reset events and start calling them
        foreach (TransformEvent trans in currentTextObject.transforms)
        {
            trans.clearReaches();
        }
        foreach (FuncEvent func in currentTextObject.functions)
        {
            func.callFunc();
        }

        //While text is not entirely revealed
        while (textBox.maxVisibleCharacters < numberOfChar)
        {
            //Add a character every loop
            textBox.maxVisibleCharacters++;
            if (fastText)
            {
                //if text is sped up, add up to 2 characters more in each loop
                if(textBox.maxVisibleCharacters < numberOfChar) textBox.maxVisibleCharacters += 1;
                if (textBox.maxVisibleCharacters < numberOfChar) textBox.maxVisibleCharacters += 1;
            }
            //Get current character and check if it is punctuation. If it is, stop for an extended period of time.
            char curChar = textBox.text[textBox.maxVisibleCharacters-1];
            if (!fastText && (curChar == '.' || curChar == '!' || curChar == '?' || curChar == ',' || curChar == ':' || curChar == ';')) yield return new WaitForSeconds(.25f / textSpeed);
            else yield return new WaitForSeconds(.1125f/textSpeed);
        }

        //Check for all requirements and resert fastText
        fastText = false;
        while (!currentTextObject.checkReaches())
        {
            yield return null;
        }
        //Check for choices
        if (currentTextObject.choices.strings.Count > 0)
        {
            for (int i = 0; i < currentTextObject.choices.strings.Count; i++)
            {
                buttonTextArr[i].text = currentTextObject.choices.strings[i];
                buttonArr[i].enabled = true;
                buttonArr[i].interactable = true;
                buttonArr[i].gameObject.SetActive(true);
            }
        }
        //Get next object index
        else
        {
            nextIndex = currentTextObject.next_id;
            progressable = true;
        }

        if(currentTextObject.dialogue == "")
        {
            advanceText();
        }
    }

    public void aa(int i)
    {
        Debug.Log(i);
    }
    public void advanceText()
    {
        //End textCo if needed
        if (textCo != null) StopCoroutine(textCo);
        fastText = false;

        //Get next index and check if text is ending
        progressable = false;
        nextIndex = currentTextObject.next_id;
        if (nextIndex != -1) textCo = StartCoroutine(typewriterFunc());
        else endText();
    }

    public void readJSON()
    {
        //Get JSON data
        jsonStr = jsonFile.text;
        DialogueArrayManager.objArr = JsonUtility.FromJson<TextObjectArr>(jsonStr);
        //readID(nextIndex);
    }
    public void readID(int id)
    {
        //Read JSON data for object
        currentTextObject.id = id;
        currentTextObject.setVars();
    }
    public void endText()
    {
        if (nextIndex == -1)
        {
            //currentTextObject.functions.Clear();
            //currentTextObject.transforms.Clear();
            //currentTextObject.choices = null;

            textOpen = false;
            disableTextbox();
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
        //Get index from button, wait, and disable. Then progress.
        currentTextObject.next_id = currentTextObject.choices.ids[buttonIndex];
        yield return new WaitForSeconds(.45f);
        buttonArr[buttonIndex].gameObject.SetActive(false);
        advanceText();
        readJSON();
        StopCoroutine(buttonCo);
    }

    public void highlightPortrait(int index)
    {
        if(index == -1)
        {
            foreach(Image img in portraits)
            {
                img.color = Color.white;
            }
            return;
        }

        for (int i = 0; i < portraits.Length; i++)
        {
            if (i == index) portraits[i].color = Color.white;
            else portraits[i].color = new Color(.635f, .635f, .635f);
        }
    }
    public void enableTextbox()
    {
        StartCoroutine(enableTextCo());
    }
    IEnumerator enableTextCo()
    {
        textboxPanel.SetActive(true);
        yield return 1;
    }

    public void disableTextbox()
    {
        StartCoroutine(disableTextCo());
    }
    IEnumerator disableTextCo()
    {
        textboxPanel.SetActive(false);
        yield return new WaitForSeconds(1);
    }
}
