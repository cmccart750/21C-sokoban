using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//A class that just checks every frame if the goal has been satisfied (every marked square contains a box)
public class GoalChecker : MonoBehaviour
{

    public List<Transform> goals;
    public List<Transform> boxes;

    public PlayerMovement playerMovement; // a reference to the player movement script, 
                                          // which we need to forward the move count to the move record keeper

    void Update()
    {
        int goodBoxes = 0; //counter for the number of boxes that are in goals

        foreach (Transform b in boxes)
        {
            foreach (Transform g in goals)
            {
                if (b.position.Equals(g.position)) goodBoxes++;
            }
        }

        if (goodBoxes == goals.Count)
        {
            RecordKeeper.instance.recordLevel(playerMovement.getMoves());
        }
    }

}
