using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        if (GameMaster.Instance.PegboardMaster.GetPegboardState() == PegboardState.ResultsScreen)
        {
            GameMaster.Instance.PegboardMaster.SetPegboardState(PegboardState.LoadingLevel);
            ResetPlayButton();
        }
        else if (GameMaster.Instance.PegboardMaster.GetPegboardState() == PegboardState.LoadingLevel)
        {
            GameMaster.Instance.PegboardMaster.StartGame();
        }
        else if (GameMaster.Instance.PegboardMaster.GetPegboardState() == PegboardState.LoseScreen)
        {
            GameMaster.Instance.DestroyPlayerInput();
            Scene screen = SceneManager.GetActiveScene();
            SceneManager.LoadScene(screen.name);
        }
        // playButton.gameObject.SetActive(false);
    }

    public void ResetPlayButton()
    {
        playButton.gameObject.SetActive(true);
    }

    public void SetButtonState(bool enabled)
    {
        leftButton.interactable = enabled;
        rightButton.interactable = enabled;
        playButton.interactable = GameMaster.Instance.IsInGame;
    }
    
}
