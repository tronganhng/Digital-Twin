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
    [SerializeField] private GameObject polePrefab;

    [Button(ButtonSizes.Medium)]
    public void SpawnPoles()
    {
#if UNITY_EDITOR
        for (int i = poleContainer.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(poleContainer.GetChild(i).gameObject);
        }

        var positions = grid.GetAllPositions();
        if (positions.Count == 0)
            return;

        for (int i = 0; i < positions.Count; i++)
        {
            var pole = (GameObject)PrefabUtility.InstantiatePrefab(polePrefab, poleContainer);
            pole.transform.position = positions[i] + new Vector3(0, 0, grid.size/2 + 0.5f);
        }
#endif
    }
}