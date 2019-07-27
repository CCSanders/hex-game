using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPrefabDatabase : MonoBehaviour
{
    public Unit[] unitPrefabs;

    public Unit GetPrefab(int id)
    {
        if(id < 0 || id >= unitPrefabs.Length)
        {
            return null;
        }

        return unitPrefabs[id];
    }
}
