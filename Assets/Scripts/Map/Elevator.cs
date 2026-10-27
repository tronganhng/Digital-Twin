
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private Transform cabin;
    [SerializeField] private List<Transform> stops = new();
    [SerializeField] private float speed = 2f;

    [SerializeField, ReadOnly] private ElevatorDto _elevatorDto;
    private Transform _target;

    public ElevatorDto StateDto => _elevatorDto;

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
        {
            _target = null;
            // Arrived
        }
    }

    public async Task Init()
    {
        _elevatorDto = new ElevatorDto
        {
            CurrentFloor = 1,
            Status = ElevatorStatus.Idle
        };

        await TryRegister();
    }

    private async Task TryRegister()
    {
        var res = await SimulationManager.Instance.WebSocket.SendRequestAsync<ElevatorDto, ElevatorDto>(SocketMessageType.RegisterElevator, _elevatorDto);
        _elevatorDto.ElevatorId = res.ElevatorId;
    }
}