using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectMenu : MonoBehaviour
{
    //Level select squares you can click to 
    public List<Button> levelButtons;

    void Start()
    {
        //The most recent level a player hasn't beat should be selectable, but not the levels after it
        bool revealedNextUncleared = false;
        for (int i = 0; i < RecordKeeper.LEVEL_COUNT; i++)
        {
            if (RecordKeeper.instance.getRecord(i) == RecordKeeper.NO_RECORD)
            {
                if (revealedNextUncleared)
                {
                    levelButtons[i].gameObject.SetActive(false);
                }
                else revealedNextUncleared = true;
            }
        }
    }

}
