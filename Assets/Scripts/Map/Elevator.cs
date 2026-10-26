
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private Transform cabin;
    [SerializeField] private List<Transform> stops = new();
    [SerializeField] private float speed = 2f;

    private Transform _target;

    [Button]
    public void GoToFloor(int floor)
    {
        if (floor < 1 || floor > stops.Count)
            return;

        _target = stops[floor - 1];
    }

    private void Update()
    {
        if (_target == null)
            return;

        cabin.position = Vector3.MoveTowards(
            cabin.position,
            _target.position,
            speed * Time.deltaTime
        );

        if (cabin.position == _target.position)
            _target = null;
    }
}