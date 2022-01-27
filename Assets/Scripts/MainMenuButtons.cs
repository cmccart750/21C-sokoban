using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Code for buttons on the main menu and/or pause menu.
public class MainMenuButtons : MonoBehaviour
{
    public Text startButtonText;
    public GameObject levelSelectButton;

    //level number, starting at 1
    int level;

    private void Start()
    {
        if (startButtonText != null)
        {
            level = RecordKeeper.instance.nextNewLevel();
            if (level > 1)
            {
                startButtonText.text = "Continue";
                levelSelectButton.SetActive(true);
            }
        }
    }

    public void Play()
    {
        string levelString = "L" + level.ToString();
        RecordKeeper.instance.advanceToLevel(level - 1);
        SceneManager.LoadScene(levelString);
    }

    public void LevelSelect(GameObject levelSelectCanvas)
    {
        levelSelectCanvas.SetActive(!levelSelectCanvas.activeInHierarchy);
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        RecordKeeper.instance.advanceToLevel(0); //move the record keeper back to the main menu
        SceneManager.LoadScene("Main Menu");
    }

    public void Exit()
    {
        Application.Quit();
    }

}
