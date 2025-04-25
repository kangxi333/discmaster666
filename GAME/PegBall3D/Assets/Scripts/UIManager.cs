using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public ScrollingTextScript scrollingPipeText { get; private set; }
    
    
    
    private void Awake()
    {
        scrollingPipeText = GetComponentInChildren<ScrollingTextScript>();
    }

    public void ClickMoveButton(bool direction)
    {
        // gross way of handling if left or right button selected but it is late and i am lazy
        
        // false for left, true for right
        GameMaster.Instance.MoveCameraCycle(
            direction ? ButtonDirection.Right : ButtonDirection.Left
            );
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
