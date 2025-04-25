using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public ScrollingTextScript scrollingPipeText { get; private set; }

    private void Awake()
    {
        scrollingPipeText = GetComponentInChildren<ScrollingTextScript>();
    }

    public void DebugString(string str)
    {
        Debug.Log(str);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
