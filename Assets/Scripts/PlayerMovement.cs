using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//all the player's controls (movement, interaction with buttons, pausing the game)
public class PlayerMovement : MonoBehaviour
{
    public ConveyorManager conveyorManager;

    //LayerMasks to quickly check if a grid square contains a specific object
    public LayerMask wallLayer;
    public LayerMask boxLayer;
    public LayerMask powerSwitches;
    public LayerMask quarterTurnSwitches;
    public LayerMask uTurnSwitches;

    //text displaying current number of moves, and record number of moves for a level
    public Text moveCountUI;
    public Text recordMovesUI;

    //player sprites for each direction
    public SpriteRenderer spriteRenderer;
    public Sprite up;
    public Sprite down;
    public Sprite left;
    public Sprite right;

    public GameObject pauseCanvas;
    bool paused = false;

    //total number of squares moved, interactions with switches, and "waits" taken in a level
    int moves = 0;

    //the facing direction of the player (to determine if they are pressing a switch)
    Vector3 facing;

    void Start()
    {
        //display the record for the level
        recordMovesUI.text = RecordKeeper.instance.getRecordText();
        facing = Vector3.down;
    }

    void Update()
    {
        //disable all controls while in pause menu
        if (!paused)
        {
            //bools for if you are currently standing on a conveyor, and for trying to take a step on a conveyor
            bool onConveyor = conveyorManager.isSquareConveyor((int)transform.position.x, (int)transform.position.y);
            bool movedOnConveyor = false;

            //record where the player was before/after all the conveyors resolve
            Vector3 preConveyorPosition = transform.position;
            Vector3 postConveyorPosition = transform.position;

            //record inputs to figure out which direction to move in
            bool inputUp = (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") == 1);
            bool inputLeft = (Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") == -1);
            bool inputDown = (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") == -1);
            bool inputRight = (Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") == 1);

            
            if (inputUp || inputLeft || inputDown || inputRight)
            {
                //if on a conveyor, you lose movement control until you hit something solid (you can only spin around)
                if (onConveyor)
                {
                    moves++;
                    conveyorManager.doConveyorStep();
                    movedOnConveyor = true;
                    if (inputUp)
                    {
                        spriteRenderer.sprite = up;
                    }
                    else if (inputLeft)
                    {
                        spriteRenderer.sprite = left;
                    }
                    else if (inputDown)
                    {
                        spriteRenderer.sprite = down;
                    }
                    else if (inputRight)
                    {
                        spriteRenderer.sprite = right;
                    }
                }
                //record the position of the player after conveyor movements
                postConveyorPosition = transform.position;

                //if the conveyor can't push the player further, they should be able to step off it.
                if (!onConveyor || preConveyorPosition == postConveyorPosition)
                {
                    if (movedOnConveyor) moves--; //correct the move count if the player is walking off the current conveyor chain
                    //move and change your facing sprite based on direction input
                    if (inputUp)
                    {
                        MoveIfNoWallAhead(Vector2.up);
                        spriteRenderer.sprite = up;
                    }
                    else if (inputLeft)
                    {
                        MoveIfNoWallAhead(Vector2.left);
                        spriteRenderer.sprite = left;
                    }
                    else if (inputDown)
                    {
                        MoveIfNoWallAhead(Vector2.down);
                        spriteRenderer.sprite = down;
                    }
                    else if (inputRight)
                    {
                        MoveIfNoWallAhead(Vector2.right);
                        spriteRenderer.sprite = right;
                    }
                }
                //delay the moves text update for different conveyor movement cases
                moveCountUI.text = "MOVES: " + moves.ToString("d2");

            }
            else if (Input.GetButtonDown("Jump")) //space
            {
                Wait(); //spend an action not moving
            }
            else if (Input.GetButtonDown("Cancel")) //esc
            {
                Pause(); //pause the game
            }
            else if (Input.GetButtonDown("Enable Debug Button 2")) //backspace
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //reset the level
            }
            
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            Pause();
        }

    }

    //pause the game
    private void Pause()
    {
        pauseCanvas.SetActive(!pauseCanvas.activeInHierarchy);
        paused = !paused;
    }

    //spend an action staying in the same place. if you're looking at a switch, toggle it
    void Wait()
    {
        TakeMove();
        Vector3 point = transform.position + facing;
        if (Physics2D.OverlapCircle(point, .45f, powerSwitches)) conveyorManager.invertPower();
        if (Physics2D.OverlapCircle(point, .45f, quarterTurnSwitches)) conveyorManager.rotateConveyors(90);
        if (Physics2D.OverlapCircle(point, .45f, uTurnSwitches)) conveyorManager.rotateConveyors(180);
        conveyorManager.doConveyorStep();
    }

    public int getMoves()
    {
        return moves;
    }

    void MoveIfNoWallAhead(Vector3 dir)
    {
        //desired location to move to
        Vector3 point = transform.position + dir;
        facing = dir; //change your facing direction

        //check if a box is in front of you
        Collider2D boxInFront = Physics2D.OverlapCircle(point, .45f, boxLayer);
        if (boxInFront != null)
        {
            //if there is a box, check for free space behind it. if there is space, push the box.
            if (Physics2D.OverlapCircle(point + dir, .45f, wallLayer) == null && Physics2D.OverlapCircle(point + dir, .45f, boxLayer) == null)
            {
                TakeMove();
                boxInFront.transform.position += dir;
                transform.position = point;
                conveyorManager.doConveyorStep();
            }
        }
        //otherwise just walk into the space as long as it's not a wall
        else if (Physics2D.OverlapCircle(point, .45f, wallLayer) == null)
        {
            TakeMove();
            transform.position = point;
            conveyorManager.doConveyorStep();
        }
    }

    //update the moves count and text display
    void TakeMove()
    {
        moves++;
        moveCountUI.text = "MOVES: " +moves.ToString("d2");
    }

}
