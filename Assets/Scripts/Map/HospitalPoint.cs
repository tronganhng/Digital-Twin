using UnityEngine;

public class HospitalPoint : MonoBehaviour
{
    public string id;
    public PointType type;

    [Header("Display")]
    public string displayName;

    public Transform Anchor => transform;
}