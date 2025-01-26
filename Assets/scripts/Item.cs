using UnityEngine;

public class Item : MonoBehaviour
{

    public enum Type {Ammo, Coin, Grenade, Heart, Weapon};
    public Type type;

    public int value;
     
    void Update()
    {
        transform.Rotate(Vector3.up * 10 * Time.deltaTime);
    }
}
