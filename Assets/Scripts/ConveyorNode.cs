using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Some methods attached to each conveyor tile to adjust their sprite and rotation (ConveyorManager broadcasts to all of them).
public class ConveyorNode : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    public Sprite onSprite;
    public Sprite offSprite;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void TurnOn()
    {
        spriteRenderer.sprite = onSprite;
    }

    void TurnOff()
    {
        spriteRenderer.sprite = offSprite;
    }

    //a quarter turn clockwise
    void Rotate90()
    {
        transform.Rotate(0, 0, 90);
    }

}
