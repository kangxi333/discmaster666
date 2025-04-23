using UnityEngine;

public enum PegType
{
    NormalPeg = 0,
    ScorePeg = 1,
    SpecialPeg = 2
}

[CreateAssetMenu(fileName = "PegData", menuName = "Scriptable Objects/PegData")]
public class PegData : ScriptableObject
{
    [field: SerializeField] public PegType pegType { get; private set; }
    [field: SerializeField] public int pegValue { get; private set; }
    [field: SerializeField] public Sprite pegSpriteNormal { get; private set; }
    [field: SerializeField] public Sprite pegSpriteHit { get; private set; }
    
}
