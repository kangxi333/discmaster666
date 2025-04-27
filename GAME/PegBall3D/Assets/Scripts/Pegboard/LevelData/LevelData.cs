using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    [SerializeField] public GameObject LevelGameObject;
    [SerializeField] public Texture2D LevelBackground;
    [SerializeField] public string LevelName;
}
