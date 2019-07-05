using UnityEngine;

public class Spin : MonoBehaviour
{
    public float speed = 90f;
    public Vector3 rotation = new Vector3(0, 1, 0);

    void Update()
    {
        transform.Rotate(rotation * speed * Time.deltaTime);
    }
}