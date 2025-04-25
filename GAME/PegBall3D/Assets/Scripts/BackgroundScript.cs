using System;
using UnityEngine;

public class BackgroundScript : MonoBehaviour
{
    private SpriteRenderer bg;

    private void Awake()
    {
        bg = GetComponent<SpriteRenderer>();
    }

    public void SetBG(Sprite image)
    {
        bg.sprite = image;
    }
}
