using UnityEngine;

public class Missile : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * 30 * Time.deltaTime);
    }
}
