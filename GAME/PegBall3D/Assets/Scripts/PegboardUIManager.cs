using System.Collections.Generic;
using UnityEngine;

public enum PegboardScreen
{
    None,
    Pegboard,
    Scores,
    Loading,
    Lose
}
public class PegboardUIManager : MonoBehaviour
{
    [System.Serializable]
    public struct UIScreen
    {
        public PegboardScreen type;
        public GameObject screenObject;
    }

    [SerializeField] private List<UIScreen> screens;
    
    private Dictionary<PegboardScreen, GameObject> screenDict = new Dictionary<PegboardScreen, GameObject>();

    private void Awake()
    {
        foreach (var screen in screens)
        {
            screenDict[screen.type] = screen.screenObject;
            screen.screenObject.SetActive(false);
        }
    }

    public void Show(PegboardScreen type)
    {
        foreach (var kvp in screenDict)
        {
            kvp.Value.SetActive(kvp.Key == type);
            if (kvp.Key == type)
            {
                kvp.Value?.GetComponent<PegboardHUDBase>().UpdateHUD();
            }
        }
    }

    public void HideAll()
    {
        foreach (var kvp in screenDict)
        {
            kvp.Value.SetActive(false);
        }
    }
}
