using UnityEngine;

public class LockRotation : MonoBehaviour
{
    [SerializeField] private Vector3 rotation;

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(rotation);
    }
}