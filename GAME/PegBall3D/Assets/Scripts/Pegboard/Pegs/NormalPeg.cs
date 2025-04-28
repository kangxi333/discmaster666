using UnityEngine;

public class NormalPeg : BasePeg
{
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PegType = PegType.NormalPeg;
        
    }

    protected override void Hit()
    {
        base.Hit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
