using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Code for mouse interactions which each level square in the level select menu
public class LevelSelectNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int levelNum;
    public string levelName;
    public Text levelInfoText;

    //mouse click
    public void OnPointerClick(PointerEventData eventData)
    {
        string level = "L" + levelNum.ToString();
        RecordKeeper.instance.advanceToLevel(levelNum - 1);
        SceneManager.LoadScene(level);
    }

    //mouse over
    public void OnPointerEnter(PointerEventData eventData)
    {
        //show the level name and a record, if the level has one
        int moves = RecordKeeper.instance.getRecord(levelNum - 1);
        string recordString = moves < RecordKeeper.NO_RECORD ? "Record: " + moves.ToString() + " Moves" : "";
        levelInfoText.text = System.String.Format("Level {0} : {1}\n{2}", levelNum, levelName, recordString);
        levelInfoText.enabled = true;
    }

    //mouse off
    public void OnPointerExit(PointerEventData eventData)
    {
        //hide all info once you mouse off any given square
        levelInfoText.enabled = false;
    }

}
