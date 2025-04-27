using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public ScrollingTextScript scrollingPipeText { get; private set; }

    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button playButton;
    
    
    private void Awake()
    {
        scrollingPipeText = GetComponentInChildren<ScrollingTextScript>();
        
        leftButton.onClick.AddListener(() =>ClickMoveButton(ButtonDirection.Left));
        rightButton.onClick.AddListener(() =>ClickMoveButton(ButtonDirection.Right));
        playButton.onClick.AddListener(ClickPlayButton);
        
    }
    
    private void ClickMoveButton(ButtonDirection dir)
    {
        GameMaster.Instance.MoveCameraCycle(dir);
    }

    private void ClickPlayButton()
    {
        GameMaster.Instance.PegboardMaster.StartGame();
        playButton.gameObject.SetActive(false);
    }

    public void ResetPlayButton()
    {
        playButton.gameObject.SetActive(true);
    }

    public void SetButtonState(bool enabled)
    {
        leftButton.interactable = enabled;
        rightButton.interactable = enabled;
        playButton.interactable = !enabled;
    }
    
}
