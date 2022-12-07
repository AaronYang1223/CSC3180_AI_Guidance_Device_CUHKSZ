using UnityEngine;

public class Collectible : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.up, 60f * Time.deltaTime, Space.World);
    }
}
