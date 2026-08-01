using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChargingStation : MonoBehaviour
{
    [SerializeField] private GridDrawer grid;
    [SerializeField] private Transform poleContainer;
    [SerializeField] private ChargingPole polePrefab;

    [SerializeField, ReadOnly] private List<ChargingPole> _poles = new();

    [Button(ButtonSizes.Medium)]
    public void SpawnPoles()
    {
#if UNITY_EDITOR
        for (int i = poleContainer.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(poleContainer.GetChild(i).gameObject);
        }
        _poles.Clear();

        var positions = grid.GetAllPositions();
        if (positions.Count == 0)
            return;

        for (int i = 0; i < positions.Count; i++)
        {
            var pole = (ChargingPole)PrefabUtility.InstantiatePrefab(polePrefab, poleContainer);
            pole.transform.position = positions[i] + new Vector3(0, 0, grid.size / 2 + 0.5f);
            _poles.Add(pole);
        }
#endif
    }

    public ChargingPole GetFreePole(Robot owner)
    {
        foreach (var pole in _poles)
        {
            if (!pole.IsOccupied)
            {
                pole.Occupy(owner);
                return pole;
            }
        }
        return null;
    }
}