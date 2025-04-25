using UnityEngine;

public class FanRotate : MonoBehaviour
{
    private float rotationSpeed = 120f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,0,1), rotationSpeed * Time.deltaTime);
    }
}
