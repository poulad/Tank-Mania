using UnityEngine;

// ReSharper disable once CheckNamespace
public class CameraMovement : MonoBehaviour
{
    public GameObject Tank;

    private Vector3 offset;

    public void Start()
    {
        offset = transform.position - Tank.transform.position;
    }

    public void LateUpdate()
    {
        transform.position = Tank.transform.position + offset;
    }
}
