using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    [SerializeField] public GameObject LevelGameObject { get; private set; }
    [SerializeField] public Texture2D LevelBackground { get; private set; }
}
