using UnityEngine;

public class Camer : MonoBehaviour
{
    public Transform target;

    //offset =>보정값을 의미
    public Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;
    }
}
