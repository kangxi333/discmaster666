using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MultHUDState
{
    UNACTIVE, // turned off
    ACTIVE, // turned on but not the active multiplier (e.g. if you were at mult x3 and this was x2) 
    MAIN // main multiplier (current highest mult)
}
public class MultComponent : MonoBehaviour
{
    // private Image _multBG;
    private TMP_Text _text;

    private void Awake()
    {
        // _multBG = GetComponent<Image>();
        _text = GetComponentInChildren<TMP_Text>();
    }

    // Sets the HUD icon for current multiplier.
    public void SetState(MultHUDState state)
    {
        switch (state)
        {
            case MultHUDState.UNACTIVE:
                gameObject.SetActive(false);
                break;
            case MultHUDState.ACTIVE:
                gameObject.SetActive(true);
                _text.color = Color.black;
                break;
            case MultHUDState.MAIN:
                gameObject.SetActive(true);
                _text.color = Color.yellow;
                break;
        }
    }
}
