using UnityEngine;
using UnityEngine.UI;

public class DateMenu : MonoBehaviour
{
    GameManager gameMan;
    public GameObject dateMenuPanel;
    public DateButton[] buttons;

    private void Start()
    {
        gameMan = GetComponent<GameManager>();
    }
    public void openPanel()
    {
        getButtons();
        dateMenuPanel.SetActive(true);
    }
    public void getButtons()
    {
        clearButtons();
        for (int i = 1; i < gameMan.battleManager.party.Length; i++)
        {
            buttons[i - 1].gameObject.SetActive(true);
            buttons[i - 1].selfText.text = gameMan.battleManager.party[i].charName;
            switch (gameMan.battleManager.party[i].characterType)
            {
                case Combatant.bossTypeChar.none: break;
                case Combatant.bossTypeChar.rocky:
                    buttons[i-1].selfButton.onClick.AddListener(() => {dateRocky();}); break;
                default: break;
            }
        }
    }
    public void clearButtons()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].selfText.text = "";
            //buttons[i].selfImage.sprite = null;
            buttons[i].gameObject.SetActive(false);
            buttons[i].selfButton.onClick.RemoveAllListeners();
            buttons[i].selfButton.onClick.AddListener(() => { closePanel(); });
        }
    }
    public void closePanel()
    {
        clearButtons();
        dateMenuPanel.SetActive(false);
    }
    public void dateRocky()
    {
        gameMan.textEventManager.callText(17);
    }
}
