using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

//Level move record manager (saves, loads, and writes the UI text for record scores)
public class RecordKeeper : MonoBehaviour
{

    public const int LEVEL_COUNT = 6;
    public const int NO_RECORD = 20000; //placeholder value in save data for uncleared levels

    public static RecordKeeper instance;

    int[] records;

    List<string> jsonLines;

    string path;

    //current level number, starting at 0 on main menu, 1 for level 1, etc.
    int current_level;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        current_level = 0;

        records = new int[LEVEL_COUNT];
        
        path = Path.Combine(Application.persistentDataPath, "level_progress.json");

        //If the save file doesn't exist yet, make one and put some data into it.
        if (!File.Exists(path))
        {
            FileStream fs = File.Create(path);
            string blank_data = "";
            for (int i = 0; i < LEVEL_COUNT; i++) blank_data += NO_RECORD.ToString() + "\n";
            fs.Write(Encoding.UTF8.GetBytes(blank_data), 0, Encoding.UTF8.GetByteCount(blank_data));
            fs.Close();
        }

        //read and load the save data into the game
        jsonLines = System.IO.File.ReadLines(path).ToList<string>();

        int index = 0;
        foreach (string line in jsonLines)
        {
            records[index] = System.Int32.Parse(line);
            index++;
        }

    }

    public void advanceToLevel(int level)
    {
        current_level = level;
    }

    //find the level number of the first uncleared level
    public int nextNewLevel()
    {
        for (int i = 0; i < LEVEL_COUNT; i++)
        {
            if (records[i] == NO_RECORD) return i + 1;
        }
        return LEVEL_COUNT;
    }

    //if a record exists for the current level, supply a string to that level's text box to display
    public string getRecordText()
    {
        int recordMoves = records[current_level];
        return recordMoves < NO_RECORD ? "RECORD: " + recordMoves.ToString() : "";
    }

    public int getRecord(int level)
    {
        return records[level];
    }

    //submit a new score to the record keeper, and then advance to the next level
    public void recordLevel(int moves)
    {
        //if the new score is actually better than the old score, save it
        if (moves < records[current_level])
        {
            records[current_level] = moves;
            string recordString = "";
            FileStream fs = File.Open(path, FileMode.Truncate);
            for (int i = 0; i < LEVEL_COUNT; i++) recordString += records[i].ToString() + "\n";
            fs.Write(Encoding.UTF8.GetBytes(recordString), 0, Encoding.UTF8.GetByteCount(recordString));
            fs.Close();
        }

        //advance to the next level, unless there are no more levels left.
        current_level++;
        if (current_level < LEVEL_COUNT)
        {
            SceneManager.LoadScene("L" + (current_level + 1).ToString());
        }
        else
        {
            SceneManager.LoadScene("Main Menu");
        }

    }

}
