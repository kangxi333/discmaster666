using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScrollingTextScript : MonoBehaviour
{
    private string _fullString = "";
    private TMP_Text _tmpText;
    [SerializeField] private float _scrollDelay = .1f;
    [SerializeField] public bool shouldDeleteText = true;
    
    private char _tempChar;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _tmpText = GetComponent<TMP_Text>();
        _fullString = _tmpText.text;
        StartCoroutine(ScrollText());
    }

    public void AddText(string text)
    {
        _fullString += text;
    }
    
    IEnumerator ScrollText()
    {
        while (true)
        {
            if (shouldDeleteText)
            {
                if (_fullString.Length > 0)
                    _fullString = _fullString.Substring(1);
                _fullString += "–";
            }
            else
            {
                _tempChar = _fullString[0];
                _fullString = _fullString.Substring(1);
                _fullString += _tempChar;
            }

            _tmpText.text = _fullString;
            yield return new WaitForSeconds(_scrollDelay);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
