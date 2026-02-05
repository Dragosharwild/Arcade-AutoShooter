using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void LateUpdate()
    {
        if (!target) return;
        Vector3 p = target.position;
        p.z = transform.position.z;
        transform.position = p;
    }
}
