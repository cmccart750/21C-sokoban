using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorManager : MonoBehaviour
{
    //LayerMasks to check if an object exists nearby using Physics2D
    public LayerMask boxLayer;
    public LayerMask wallLayer;
    public LayerMask playerLayer;

    //Mapping integers to different directions to store inside our conveyor grids
    public enum ConveyorDirection {
        Right,
        Up,
        Left,
        Down,
        None
    }

    //height and width of the level in tiles (including all walls, for simplicity)
    public int levelHeight;
    public int levelWidth;

    //GameObjects that will parent all of the conveyor tiles, and objects that can be moved by conveyors
    public GameObject conveyors1;
    public GameObject conveyors2;
    public GameObject boxes;
    List<Transform> boxTransforms;

    //conveyorGrid1 starts turned on, conveyorGrid2 starts turned off
    //only one conveyor can be on at a time
    int[,] conveyorGrid1;
    int[,] conveyorGrid2;
    bool activeConveyor1 = true;

    void Start()
    {
        //initialize empty conveyor grids
        conveyorGrid1 = new int[levelHeight, levelWidth];
        conveyorGrid2 = new int[levelHeight, levelWidth];

        for (int i = 0; i < levelHeight; i++)
        {
            for (int j = 0; j < levelWidth; j++)
            {
                conveyorGrid1[i, j] = (int)ConveyorDirection.None;
                conveyorGrid2[i, j] = (int)ConveyorDirection.None;
            }
        }

        //read in conveyor tiles and record their directions in the grids
        foreach (Transform child in conveyors1.transform)
        {
            ConveyorDirection cd = (ConveyorDirection)((int)child.rotation.eulerAngles.z / 90); //ConveyorDirection enum has all direction vectors in order of clockwise rotation
            conveyorGrid1[(int)(child.position.y - 0.5f), (int)(child.position.x - 0.5f)] = (int)cd;
        }
        foreach (Transform child in conveyors2.transform)
        {
            ConveyorDirection cd = (ConveyorDirection)((int)child.rotation.eulerAngles.z / 90);
            conveyorGrid2[(int)(child.position.y - 0.5f), (int)(child.position.x - 0.5f)] = (int)cd;
        }

        //build a list of all boxes in the level (including the player, because the conveyors also push them)
        boxTransforms = new List<Transform>();

        foreach (Transform child in boxes.transform)
        {
            boxTransforms.Add(child);
        }
    }

    //executed after every move the player makes
    //pushes every box or player on a conveyor tile 1 square in the direction shown on the given tile
    public void doConveyorStep()
    {
        //refer only to the conveyor that's currently powered on
        int[,] currentConveyor = activeConveyor1 ? conveyorGrid1 : conveyorGrid2;

        //for each object that could be moved
        foreach (Transform box in boxTransforms)
        {
            //translate its position to a grid square
            int x = (int)(box.position.x - 0.5f);
            int y = (int)(box.position.y - 0.5f);

            //check the conveyor tile's direction at that grid location
            int cd = currentConveyor[y, x];

            //if the box is on a conveyor
            if (cd != (int)ConveyorDirection.None)
            {
                //look ahead to the location the conveyor tile would send you, if there are no obstructions
                Vector3 current_point = box.position +  (Quaternion.Euler(0, 0, cd * 90) * Vector3.right);
                
                //save the destination for later
                Vector3 destination = current_point;

                //if you're a box on a conveyor and there's more boxes on conveyors ahead of you, check to see if the box chain will actually move this round
                int loop_threshold = 0; //to avoid getting stuck looping over an infinite cycle of conveyors

                int current_cd = currentConveyor[(int)current_point.y, (int)current_point.x];
                while (Physics2D.OverlapCircle(current_point, .45f, boxLayer) && current_cd != (int)ConveyorDirection.None && loop_threshold < levelWidth * levelHeight)
                {
                    //look ahead at the next tile in the belt until you fall off the belt, or there is no box in the way
                    current_point += (Quaternion.Euler(0, 0, current_cd * 90) * Vector3.right);
                    current_cd = currentConveyor[(int)current_point.y, (int)current_point.x];
                }

                //boxes don't move if there's a wall
                if (Physics2D.OverlapCircle(current_point, .45f, wallLayer)) continue;

                //boxes don't move if the player is in the way
                if (Physics2D.OverlapCircle(current_point, .45f, playerLayer)) continue;

                //boxes don't move if a box on normal ground blocks them
                if (Physics2D.OverlapCircle(current_point, .45f, boxLayer)) continue;

                //if the box has passed all the conditionals above, it should be able to slide 1 tile forward in the direction of the conveyor under it.
                box.transform.position = destination;

            }

        }

    }

    //check if a grid square is a conveyor belt
    public bool isSquareConveyor(int x, int y)
    {
        int[,] currentConveyor = activeConveyor1 ? conveyorGrid1 : conveyorGrid2;
        return currentConveyor[y, x] != (int)ConveyorDirection.None;
    }

    //turn all the conveyors (regardless of active status) by either 90 or 180 degrees using the appropriate wall switches
    public void rotateConveyors(int angle)
    {
        //the enum values are already increasing with clockwise rotation, so we just need to increment the values of every
        //grid square that is a conveyor square, and then mod by 4 to keep them in the 4-direction cycle.
        for (int i = 0; i < levelWidth; i++)
        {
            for (int j = 0; j < levelHeight; j++)
            {
                if (conveyorGrid1[i, j] != (int)ConveyorDirection.None)
                {
                    conveyorGrid1[i, j] = (conveyorGrid1[i, j] + angle / 90) % 4;
                }
                if (conveyorGrid2[i, j] != (int)ConveyorDirection.None)
                {
                    conveyorGrid2[i, j] = (conveyorGrid2[i, j] + angle / 90) % 4;
                }
            }
        }

        //Tell all conveyor squares under this object (i.e. the two conveyor grid parents) to rotate their sprites
        for (int i = 0; i < angle / 90; ++i)
        {
            BroadcastMessage("Rotate90");
        }

    }

    //Switch the power off on the active conveyor and turn on the previously inactive conveyor
    public void invertPower()
    {
        //toggle functionality in ConveyorManager
        activeConveyor1 = !activeConveyor1;
        
        //toggle functionality in all the conveyor tiles (i.e. tell them to switch sprites)
        GameObject onConveyor = activeConveyor1 ? conveyors1 : conveyors2;
        GameObject offConveyor = activeConveyor1 ? conveyors2 : conveyors1;
        onConveyor.BroadcastMessage("TurnOn");
        offConveyor.BroadcastMessage("TurnOff");
    }

}
